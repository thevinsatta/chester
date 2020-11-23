using Chester.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Chester
{
    public abstract class DbTool : IDbTool
    {
        #region Fields
        bool _isDisposed;

        /// <summary>
        /// in seconds
        /// </summary>
        protected int _cmdTimeout = 120; // default SQL execution timeout
        protected IDbConnection _conn;
        protected IDbCommand _cmd;
        #endregion

        #region Constructors
        public DbTool(string connStr)
        {
            _conn = CreateConnection(connStr);
        }

        public DbTool(IDbConnection dbConn)
        {
            _conn = dbConn ?? throw new ArgumentNullException(nameof(dbConn));
        }
        #endregion

        #region Destructor
        // no unmanaged resources, hence no finalizer

        protected void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                // dispose the managed resources
                if (_cmd != null)
                {
                    _cmd.Dispose();
                    _cmd = null;
                }

                if (_conn != null)
                {
                    CloseConnection();
                    _conn.Dispose();
                    _conn = null;
                }
            }

            // dispose the un-managed resources

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        protected abstract IDbConnection CreateConnection(string connStr);

        protected abstract IDbCommand CreateCommand(IDbConnection dbConn);

        protected abstract IDbCommand CreateCommand(IDbConnection dbConn, string cmdText);

        protected virtual void AugmentConnection() { }

        #region // CONNECTION *****************************************************
        protected void OpenConnection()
        {
            if (_conn == null)
                throw new NullReferenceException($"{nameof(_conn)} is null.");

            if (_conn.State == ConnectionState.Broken)
                _conn.Close();

            if (_conn.State == ConnectionState.Closed)
                _conn.Open();

            AugmentConnection();
        }

        public void CloseConnection()
        {
            if (_conn == null || _conn.State == ConnectionState.Closed)
                return;

            _conn.Close();
        }
        #endregion

        #region // COMMAND ********************************************************
        protected void InitCommand()
        {
            _cmd ??= CreateCommand(_conn);
        }

        protected void SetCommand(
            CommandType cmdType,
            string cmdText)
        {
            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            InitCommand();
            
            _cmd.CommandText = cmdText;
            _cmd.CommandTimeout = _cmdTimeout;
            _cmd.CommandType = cmdType;
            _cmd.Parameters.Clear();
        }

        protected void SetCommand(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            SetCommand(cmdType, cmdText);

            if (!(@params?.Any() ?? false))
                return;

            foreach (var param in @params)
            {
                param.Value ??= DBNull.Value;

                _cmd.Parameters.Add(param);
            }
        }

        public void SetCommandTimeout(int timeout)
        {
            _cmdTimeout = timeout >= 0
                ? timeout
                : throw new ArgumentOutOfRangeException($"{nameof(timeout)} cannot be less than 0.");
        }
        #endregion

        #region // TRANSACTION ****************************************************
        public IDbTransaction BeginTransaction()
        {
            OpenConnection();
            InitCommand();

            _cmd.Transaction = _conn.BeginTransaction();

            return _cmd.Transaction;
        }

        public IDbTransaction BeginTransaction(IsolationLevel iso)
        {
            OpenConnection();
            InitCommand();

            _cmd.Transaction = _conn.BeginTransaction(iso);

            return _cmd.Transaction;
        }

        public void CommitTransaction() =>
            _cmd.Transaction?.Commit();

        public void RollbackTransaction() =>
            _cmd.Transaction?.Rollback();
        #endregion

        #region // DATAREADER *****************************************************
        public IDataReader DataReader(
            CommandType cmdType,
            string cmdText) =>
            DataReader(CommandBehavior.CloseConnection, cmdType, cmdText);

        public IDataReader DataReader(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            DataReader(CommandBehavior.CloseConnection, cmdType, cmdText, @params);

        public IDataReader DataReader(
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText) =>
            DataReader(cmdBehavior, cmdType, cmdText, null);

        public IDataReader DataReader(
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            OpenConnection();
            SetCommand(cmdType, cmdText, @params);

            return _cmd.ExecuteReader(cmdBehavior);
        }
        #endregion

        #region // EXEC NONQUERY **************************************************
        public int ExecNonQuery(
            CommandType cmdType,
            string cmdText) =>
            ExecNonQuery(cmdType, cmdText, null);

        public int ExecNonQuery(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            OpenConnection();
            SetCommand(cmdType, cmdText, @params);

            return _cmd.ExecuteNonQuery();
        }
        #endregion

        #region // EXEC SCALAR ****************************************************
        public object ExecScalar(
            CommandType cmdType,
            string cmdText) =>
            ExecScalar(cmdType, cmdText, null);

        public object ExecScalar(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            OpenConnection();
            SetCommand(cmdType, cmdText, @params);

            return _cmd.ExecuteScalar();
        }
        #endregion
        #endregion

        #region Helpers
        public ArgumentException ArgNullOrWhiteSpaceException(string argName) =>
            new ArgumentException($"{argName} cannot be null, empty, or consists only of white-space characters.");
        #endregion
    }
}
