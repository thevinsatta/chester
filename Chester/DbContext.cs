using Chester.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chester
{
    /// <summary>
    /// This class is not meant to be called from, it provides a basic framework for data access
    /// </summary>
    public abstract class DbContext : IDbContext
    {
        #region Fields
        readonly string _connStr;
        #endregion

        #region Properties
        /// <summary>
        /// If set, then use this command timeout value
        /// </summary>
        public int? CommandTimeout { get; set; }

        public virtual CommandBehavior DefaultFetchCommandBehavior =>
            CommandBehavior.CloseConnection | CommandBehavior.SingleResult;

        public virtual CommandBehavior DefaultFetchOneCommandBehavior =>
            CommandBehavior.CloseConnection | CommandBehavior.SingleRow;

        public virtual CommandBehavior DefaultExecDataReaderCommandBehavior =>
            CommandBehavior.CloseConnection;

        public virtual CommandType DefaultCommandType =>
            CommandType.StoredProcedure;
        #endregion

        #region Constructors
        // internal accessor to prevent external assemblies to inherit this class
        public DbContext(string connStr)
        {
            _connStr = connStr;
        }

        public DbContext(string connStr, int? commandTimeout) : this(connStr)
        {
            CommandTimeout = commandTimeout;
        }
        #endregion

        #region Methods
        #region Create DbTool
        /// <summary>
        /// Create a new object to work with a database
        /// </summary>
        /// <returns></returns>
        protected IDbTool CreateDbTool() =>
            CreateDbTool(_connStr);

        protected abstract IDbTool CreateDbTool(string connStr);

        protected abstract IDbTool CreateDbTool(IDbConnection dbConn);
        #endregion

        #region Fetch w/ while dr.Read() loop
        #region void Fetch()
        /// <summary>
        /// Allow access to an IDataReader within the generate action. The action is within
        /// implemented within a while (dr.Read()) loop.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual void Fetch(
            Action<IDataReader> action,
            string cmdText) =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Allow access to an IDataReader within the generate action. The action is within
        /// implemented within a while (dr.Read()) loop.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual void Fetch(
            Action<IDataReader> action,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Allow access to an IDataReader within the generate action. The action is within
        /// implemented within a while (dr.Read()) loop.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual void Fetch(
            Action<IDataReader> action,
            CommandType cmdType,
            string cmdText) =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Allow access to an IDataReader within the generate action. The action is within
        /// implemented within a while (dr.Read()) loop.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual void Fetch(
            Action<IDataReader> action,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Allow access to an IDataReader within the generate action. The action is within
        /// implemented within a while (dr.Read()) loop.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public void Fetch(
            Action<IDataReader> action,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            ExecDataReader(
                (IDataReader dr) => {
                    while (dr.Read())
                        action(dr);
                },
                cmdBehavior, cmdType, cmdText, @params);
        #endregion

        #region T Fetch<T>()
        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, T> action,
            string cmdText)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, T> action,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, T> action,
            CommandType cmdType,
            string cmdText)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, T> action,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T Fetch<T>(
            Action<IDataReader, T> action,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new()
        {
            var t = new T();

            Fetch(
                (IDataReader dr) => {
                    action(dr, t);
                },
                cmdBehavior, cmdType, cmdText, @params);

            return t;
        }

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, IDictionary<string, int>, T> action,
            string cmdText)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, IDictionary<string, int>, T> action,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, IDictionary<string, int>, T> action,
            CommandType cmdType,
            string cmdText)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T Fetch<T>(
            Action<IDataReader, IDictionary<string, int>, T> action,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new() =>
            Fetch(action, DefaultFetchCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Returns a collection T that is populated with the action parameter. The action is implemented
        /// inside a while(dr.Read()) loop, then the action uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache to produce an object that
        /// is added the collection T within the action.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T Fetch<T>(
            Action<IDataReader, IDictionary<string, int>, T> action,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
            where T : class, new()
        {
            var t = new T();
            var oc = CreateOrdinalCache();

            Fetch(
                (IDataReader dr) => {
                    action(dr, oc, t);
                },
                cmdBehavior, cmdType, cmdText, @params);

            return t;
        }
        #endregion

        #region IEnumerable<T> Fetch<T>()
        /// <summary>
        /// Return an IEnumerable&lt;T&gt;, using a generate function that takes an IDataReader
        /// and outputs object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, T> func,
            string cmdText) =>
            Fetch(func, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Return an IEnumerable&lt;T&gt;, using a generate function that takes an IDataReader
        /// and outputs object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, T> func,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(func, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Return an IEnumerable&lt;T&gt;, using a generate function that takes an IDataReader
        /// and outputs object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, T> func,
            CommandType cmdType,
            string cmdText) =>
            Fetch(func, DefaultFetchCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Return an IEnumerable&lt;T&gt;, using a generate function that takes an IDataReader
        /// and outputs object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, T> func,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(func, DefaultFetchCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Return an IEnumerable&lt;T&gt;, using a generate function that takes an IDataReader
        /// and outputs object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T>(
            Func<IDataReader, T> func,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            using var db = CreateDbTool();
            using var dr = db.DataReader(cmdBehavior, cmdType, cmdText, @params);

            while (dr.Read())
                yield return func(dr);

            dr.Close(); // explicit is always faster
            db.CloseConnection(); // explicit is always faster
        }

        /// <summary>
        /// Returns an IEnumerable&lt;T&gt;, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which produces
        /// object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            string cmdText) =>
            Fetch(func, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Returns an IEnumerable&lt;T&gt;, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which produces
        /// object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(func, DefaultFetchCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Returns an IEnumerable&lt;T&gt;, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which produces
        /// object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandType cmdType,
            string cmdText) =>
            Fetch(func, DefaultFetchCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Returns an IEnumerable&lt;T&gt;, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which produces
        /// object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> Fetch<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Fetch(func, DefaultFetchCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Returns an IEnumerable&lt;T&gt;, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which produces
        /// object T. This method will then yield the output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public IEnumerable<T> Fetch<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            using var db = CreateDbTool();
            using var dr = db.DataReader(cmdBehavior, cmdType, cmdText, @params);

            var oc = CreateOrdinalCache();

            while (dr.Read())
                yield return func(dr, oc);

            dr.Close(); // explicit is always faster
            db.CloseConnection(); // explicit is always faster
        }
        #endregion
        #endregion

        #region FetchOne w/ if dr.Read()
        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and outputs object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, T> func,
            string cmdText) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and outputs object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, T> func,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and outputs object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, T> func,
            CommandType cmdType,
            string cmdText) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and outputs object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, T> func,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and outputs object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T FetchOne<T>(
            Func<IDataReader, T> func,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            T t = default;

            ExecDataReader(
                (IDataReader dr) => {
                    if (dr.Read())
                        t = func(dr);
                },
                cmdBehavior, cmdType, cmdText, @params);

            return t;
        }

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which outputs object T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            string cmdText) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which outputs object T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which outputs object T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandType cmdType,
            string cmdText) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which outputs object T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T FetchOne<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            FetchOne(func, DefaultFetchOneCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Returns an object T, using a generate function that uses an IDataReader and
        /// IDictionary&lt;string, int&gt; column name to column index map cache, which outputs object T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T FetchOne<T>(
            Func<IDataReader, IDictionary<string, int>, T> func,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params)
        {
            T t = default;

            ExecDataReader(
                (IDataReader dr) => {
                    if (dr.Read())
                        t = func(dr, CreateOrdinalCache());
                },
                cmdBehavior, cmdType, cmdText, @params);

            return t;
        }
        #endregion

        #region Execute DataReader
        /// <summary>
        /// Template for executing a data reader
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        public virtual void ExecDataReader(
            Action<IDataReader> action,
            string cmdText) =>
            ExecDataReader(action, DefaultExecDataReaderCommandBehavior, DefaultCommandType, cmdText, null);

        /// <summary>
        /// Template for executing a data reader
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        public virtual void ExecDataReader(
            Action<IDataReader> action,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            ExecDataReader(action, DefaultExecDataReaderCommandBehavior, DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Template for executing a data reader
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        public virtual void ExecDataReader(
            Action<IDataReader> action,
            CommandType cmdType,
            string cmdText) =>
            ExecDataReader(action, DefaultExecDataReaderCommandBehavior, cmdType, cmdText, null);

        /// <summary>
        /// Template for executing a data reader
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        public virtual void ExecDataReader(
            Action<IDataReader> action,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            ExecDataReader(action, DefaultExecDataReaderCommandBehavior, cmdType, cmdText, @params);

        /// <summary>
        /// Template for executing a data reader
        /// </summary>
        /// <param name="action"></param>
        /// <param name="cmdBehavior"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        public void ExecDataReader(
            Action<IDataReader> action,
            CommandBehavior cmdBehavior,
            CommandType cmdType,
            string cmdText,
            IEnumerable<IDbDataParameter> @params) =>
            Exec(
                (IDbTool db) => {
                    using var dr = db.DataReader(cmdBehavior, cmdType, cmdText, @params);
                    action(dr);
                    dr.Close(); // explicit is always faster
                });
        #endregion

        #region Execute NonQuery
        /// <summary>
        /// Executes non-query
        /// </summary>
        /// <param name="cmdText">Name of the stored procedure to be ran.</param>
        /// <returns>Number of affected rows.</returns>
        public virtual int ExecNonQuery(string cmdText) =>
            ExecNonQuery(DefaultCommandType, cmdText, null);

        /// <summary>
        /// Executes non-query
        /// </summary>
        /// <param name="cmdText">Name of the stored procedure to be ran.</param>
        /// <param name="params">Parameters to send to the given stored procedure.</param>
        /// <returns>Number of affected rows.</returns>
        public virtual int ExecNonQuery(string cmdText, IEnumerable<IDbDataParameter> @params) =>
            ExecNonQuery(DefaultCommandType, cmdText.Trim(), @params);

        /// <summary>
        /// Executes non-query
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public int ExecNonQuery(CommandType cmdType, string cmdText) =>
            ExecNonQuery(cmdType, cmdText, null);

        /// <summary>
        /// Executes non-query
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public int ExecNonQuery(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
        {
            var rows = 0;

            Exec(
                (IDbTool db) => {
                    rows = db.ExecNonQuery(cmdType, cmdText.Trim(), @params);
                });

            return rows;
        }
        #endregion

        #region Execute Scalar
        /// <summary>
        /// Executes scalar and converts scalar value to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public virtual T ExecScalar<T>(string cmdText) =>
            ExecScalar<T>(DefaultCommandType, cmdText, null);

        /// <summary>
        /// Executes scalar and converts scalar value to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public virtual T ExecScalar<T>(string cmdText, IEnumerable<IDbDataParameter> @params) =>
            ExecScalar<T>(DefaultCommandType, cmdText, @params);

        /// <summary>
        /// Executes scalar and converts scalar value to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <returns></returns>
        public T ExecScalar<T>(CommandType cmdType, string cmdText) =>
            ExecScalar<T>(cmdType, cmdText, null);

        /// <summary>
        /// Executes scalar and converts scalar value to type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public T ExecScalar<T>(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
        {
            object ret = null;

            Exec(
                (IDbTool db) => {
                    ret = db.ExecScalar(cmdType, cmdText, @params);
                });

            if (ret == null || ret == DBNull.Value)
                return default;

            try
            {
                return ret is T t
                    ? t
                    : (T)Convert.ChangeType(ret, typeof(T));
            }
            catch (Exception e)
            {
                switch (cmdType)
                {
                    case CommandType.StoredProcedure:
                        throw new Exception($"{e.Message} Scalar value \"{ret}\" for stored procedure: {cmdText}.", e);
                    case CommandType.TableDirect:
                        throw new Exception($"{e.Message} Scalar value \"{ret}\" for table: {cmdText}.", e);
                    case CommandType.Text:
                        throw new Exception($"{e.Message} Scalar value \"{ret}\" for command text:\r\n{cmdText}.", e);
                    default:
                        throw; // re-throw original error
                }
            }
        }
        #endregion

        #region Execute
        /// <summary>
        /// Encapsulate execution that has access to the IDbTool object
        /// </summary>
        /// <param name="action"></param>
        public void Exec(Action<IDbTool> action)
        {
            using var db = CreateDbTool();
            action(db);
            db.CloseConnection(); // explicit is always faster
        }
        #endregion

        #region Create Parameter
        public abstract IDbDataParameter DbParam(string name, object value);

        public IDbDataParameter DbParam(string name, object value, ParameterDirection direction)
        {
            var p = DbParam(name, value);
            p.Direction = direction;

            return p;
        }

        public abstract IDbDataParameter DbParam(string name, DbType type);

        public abstract IDbDataParameter DbParam(string name, DbType type, int size);

        public IDbDataParameter DbParam(string name, DbType type, object value)
        {
            var p = DbParam(name, type);
            p.Value = value;

            return p;
        }

        public IDbDataParameter DbParam(string name, DbType type, ParameterDirection direction)
        {
            var p = DbParam(name, type);
            p.Direction = direction;

            return p;
        }

        public IDbDataParameter DbParam(string name, DbType type, object value, ParameterDirection direction)
        {
            var p = DbParam(name, type, direction);
            p.Value = value;

            return p;
        }

        public abstract IDbDataParameter DbParam(string name, DbType type, int size, object value, ParameterDirection direction);
        #endregion
        #endregion

        #region Misc
        public IDictionary<string, int> CreateOrdinalCache() =>
            new Dictionary<string, int>();

        public ArgumentException ArgNullOrWhiteSpaceException(string argName) =>
            new ArgumentException($"{argName} cannot be null, empty, or consists only of white-space characters.");

        public ArgumentException DbTypeNotSupportedException(DbType type, string dbName) =>
            new ArgumentException($"{Enum.GetName(typeof(DbType), type)} is not a supported or ambiguous DbType for {dbName}.");
        #endregion
    }
}
