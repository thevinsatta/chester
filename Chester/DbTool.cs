using Chester.Infrastructure;
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
        /// Command execution timeout in seconds
        /// </summary>
        protected int _cmdTimeout = 120;
        protected IDbConnection _conn;
        #endregion

        #region Properties
        public int CommandTimeout
        {
            get => _cmdTimeout;
            set => _cmdTimeout = value >= 0
                ? value
                : throw new Exception($"Cannot be less than 0.");
        }

        IDbConnection GetConnection
        {
            get
            {
                if (_conn == null)
                    throw new NullReferenceException($"{nameof(_conn)} is null.");

                if (_conn.State == ConnectionState.Broken)
                    _conn.Close();

                if (_conn.State == ConnectionState.Closed)
                {
                    _conn.Open();
                    AugmentConnection();
                }

                return _conn;
            }
        }
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
        protected abstract IDbConnection CreateConnection();

        protected IDbConnection CreateConnection(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr))
                throw ArgNullOrWhiteSpaceException(nameof(connStr));

            var conn = CreateConnection();
            conn.ConnectionString = connStr;

            return conn;
        }

        protected virtual void AugmentConnection() { }

        #region // CONNECTION *****************************************************
        public void CloseConnection()
        {
            if (_conn == null || _conn.State == ConnectionState.Closed)
                return;

            _conn.Close();
        }
        #endregion

        #region // COMMAND ********************************************************
        public IDbCommand CreateCommand(
            CommandType cmdType,
            string cmdText)
        {
            if (string.IsNullOrWhiteSpace(cmdText))
                throw ArgNullOrWhiteSpaceException(nameof(cmdText));

            var cmd = GetConnection.CreateCommand();

            cmd.CommandTimeout = _cmdTimeout;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.Parameters.Clear();

            return cmd;
        }

        public IDbCommand CreateCommand(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            var cmd = CreateCommand(cmdType, cmdText);

            if (!(@params?.Any() ?? false))
                return cmd;

            foreach (var param in @params)
            {
                param.Value ??= DBNull.Value;
                cmd.Parameters.Add(param);
            }

            return cmd;
        }
        #endregion

        #region // DATAREADER *****************************************************
        public DataReaderCommand DataReader(
            CommandType cmdType,
            string cmdText) =>
            DataReader(CommandBehavior.CloseConnection, cmdType, cmdText, null);

        public DataReaderCommand DataReader(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            DataReader(CommandBehavior.CloseConnection, cmdType, cmdText, @params);

        public DataReaderCommand DataReader(
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText) =>
            DataReader(cmdBehavior, cmdType, cmdText, null);

        public DataReaderCommand DataReader(
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            new DataReaderCommand(CreateCommand(cmdType, cmdText, @params), cmdBehavior);
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
            using var cmd = CreateCommand(cmdType, cmdText, @params);
            return cmd.ExecuteNonQuery();
        }
        #endregion

        #region // EXEC SCALAR ****************************************************
        public T ExecScalar<T>(
            CommandType cmdType,
            string cmdText) =>
            ExecScalar<T>(cmdType, cmdText, null);

        public T ExecScalar<T>(
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            using var cmd = CreateCommand(cmdType, cmdText, @params);
            var ret = cmd.ExecuteScalar();

            return ret == null || ret == DBNull.Value
                ? default
                : ret is T t
                    ? t
                    : (T)Convert.ChangeType(ret, typeof(T));
        }
        #endregion
        #endregion

        #region Helpers
        protected ArgumentException ArgNullOrWhiteSpaceException(string argName) =>
            new ArgumentException($"{argName} cannot be null, empty, or consists only of white-space characters.");
        #endregion
    }
}
