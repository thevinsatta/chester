using Chester.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chester
{
    public abstract class DbTool : IDbTool
    {
        #region Fields
        bool _isDisposed;

        protected int _cmdTimeout = 120; // default SQL execution timeout
        protected IDbConnection _conn;
        protected IDbCommand _cmd;
        #endregion

        #region Constructors
        public DbTool(string connStr)
        {
            _conn = CreateConnection(connStr);
        }

        /// <summary>
        /// this will use a current connection "<paramref name="dbConn"/>", 
        /// used when connecting with a dif user,
        /// </summary>
        /// <param name="dbConn">a valid connection</param>
        public DbTool(IDbConnection dbConn)
        {
            _conn = dbConn;
        }
        #endregion

        #region Destructor
        // no unmanaged resources, hence no finalizer here

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
                throw new ArgumentException($"{nameof(cmdText)} is null or empty.");

            if (_cmd == null)
                _cmd = CreateCommand(_conn, cmdText);
            else
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

            if (@params != null)
                foreach (var param in @params)
                {
                    if (param.Value == null)
                        param.Value = DBNull.Value;

                    _cmd.Parameters.Add(param);
                }
        }

        public void SetCommandTimeout(int seconds)
        {
            _cmdTimeout = seconds;
        }
        #endregion

        #region // TRANSACTION ****************************************************
        /// <summary>
        /// Begin a transaction
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            OpenConnection();
            InitCommand();

            _cmd.Transaction = _conn.BeginTransaction();

            return _cmd.Transaction;
        }

        /// <summary>
        /// Begin a transaction
        /// </summary>
        /// <param name="iso"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel iso)
        {
            OpenConnection();
            InitCommand();

            _cmd.Transaction = _conn.BeginTransaction(iso);

            return _cmd.Transaction;
        }

        /// <summary>
        /// Commit a transaction
        /// </summary>
        public void CommitTransaction() =>
            _cmd.Transaction?.Commit();

        /// <summary>
        /// Rollback a transaction
        /// </summary>
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
    }
}
