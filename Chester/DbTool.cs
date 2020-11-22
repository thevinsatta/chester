﻿using Chester.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Chester
{
    public abstract class DbTool : IDbTool
    {
        #region Fields
        bool _isDisposed;

        protected int _cmdTimeout = 120; // default SQL execution timeout
        protected DbConnection _conn;
        protected DbCommand _cmd;
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
        public DbTool(DbConnection dbConn)
        {
            _conn = dbConn;
        }
        #endregion

        #region Destructor
        // no unmanaged resources, hence no finalizer here

        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
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
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Methods
        protected abstract DbConnection CreateConnection(string connStr);

        protected abstract DbCommand CreateCommand(DbConnection dbConn);

        protected abstract DbCommand CreateCommand(DbConnection dbConn, string cmdText);

        protected virtual void AugmentConnection() { }

        #region // CONNECTION *****************************************************
        public void OpenConnection()
        {
            if (_conn == null)
                throw new NullReferenceException("conn is null");

            switch (_conn.State)
            {
                case ConnectionState.Closed:
                    _conn.Open();
                    AugmentConnection();
                    break;

                case ConnectionState.Broken:
                    _conn.Close();
                    _conn.Open();
                    AugmentConnection();
                    break;
            }
        }

        public void CloseConnection()
        {
            if (_conn != null)
                while (_conn.State != ConnectionState.Closed)
                    if (_conn.State == ConnectionState.Open || _conn.State == ConnectionState.Broken)
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