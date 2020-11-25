using System;
using System.Collections.Generic;
using System.Data;

namespace Chester.Interfaces
{
    public interface IDbContext
    {
        #region Properties
        int? CommandTimeout { get; set; }
        CommandBehavior DefaultFetchCommandBehavior { get; }
        CommandBehavior DefaultFetchOneCommandBehavior { get; }
        CommandBehavior DefaultExecDataReaderCommandBehavior { get; }
        CommandType DefaultCommandType { get; }
        #endregion

        #region Fetch
        void Fetch(Action<IDataReader> action, string cmdText);
        void Fetch(Action<IDataReader> action, string cmdText, IEnumerable<IDbDataParameter> @params);
        void Fetch(Action<IDataReader> action, CommandType cmdType, string cmdText);
        void Fetch(Action<IDataReader> action, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        void Fetch(Action<IDataReader> action, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        T Fetch<T>(Action<IDataReader, T> action, string cmdText)
             where T : class, new();
        T Fetch<T>(Action<IDataReader, T> action, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, T> action, CommandType cmdType, string cmdText)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, T> action, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, T> action, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();

        T Fetch<T>(Action<IDataReader, IDictionary<string, int>, T> action, string cmdText)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, IDictionary<string, int>, T> action, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, IDictionary<string, int>, T> action, CommandType cmdType, string cmdText)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, IDictionary<string, int>, T> action, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();
        T Fetch<T>(Action<IDataReader, IDictionary<string, int>, T> action, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params)
            where T : class, new();

        IEnumerable<T> Fetch<T>(Func<IDataReader, T> func, string cmdText);
        IEnumerable<T> Fetch<T>(Func<IDataReader, T> func, string cmdText, IEnumerable<IDbDataParameter> @params);
        IEnumerable<T> Fetch<T>(Func<IDataReader, T> func, CommandType cmdType, string cmdText);
        IEnumerable<T> Fetch<T>(Func<IDataReader, T> func, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        IEnumerable<T> Fetch<T>(Func<IDataReader, T> func, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        IEnumerable<T> Fetch<T>(Func<IDataReader, IDictionary<string, int>, T> func, string cmdText);
        IEnumerable<T> Fetch<T>(Func<IDataReader, IDictionary<string, int>, T> func, string cmdText, IEnumerable<IDbDataParameter> @params);
        IEnumerable<T> Fetch<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandType cmdType, string cmdText);
        IEnumerable<T> Fetch<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        IEnumerable<T> Fetch<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        #endregion

        #region FetchOne
        T FetchOne<T>(Func<IDataReader, T> func, string cmdText);
        T FetchOne<T>(Func<IDataReader, T> func, string cmdText, IEnumerable<IDbDataParameter> @params);
        T FetchOne<T>(Func<IDataReader, T> func, CommandType cmdType, string cmdText);
        T FetchOne<T>(Func<IDataReader, T> func, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        T FetchOne<T>(Func<IDataReader, T> func, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);

        T FetchOne<T>(Func<IDataReader, IDictionary<string, int>, T> func, string cmdText);
        T FetchOne<T>(Func<IDataReader, IDictionary<string, int>, T> func, string cmdText, IEnumerable<IDbDataParameter> @params);
        T FetchOne<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandType cmdType, string cmdText);
        T FetchOne<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        T FetchOne<T>(Func<IDataReader, IDictionary<string, int>, T> func, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        #endregion

        #region ExecDataReader
        void ExecDataReader(Action<IDataReader> action, string cmdText);
        void ExecDataReader(Action<IDataReader> action, string cmdText, IEnumerable<IDbDataParameter> @params);
        void ExecDataReader(Action<IDataReader> action, CommandType cmdType, string cmdText);
        void ExecDataReader(Action<IDataReader> action, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        void ExecDataReader(Action<IDataReader> action, CommandBehavior cmdBehavior, CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        #endregion

        #region ExecNonQuery
        int ExecNonQuery(string cmdText);
        int ExecNonQuery(string cmdText, IEnumerable<IDbDataParameter> @params);
        int ExecNonQuery(CommandType cmdType, string cmdText);
        int ExecNonQuery(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        #endregion

        #region ExecScalar
        T ExecScalar<T>(string cmdText);
        T ExecScalar<T>(string cmdText, IEnumerable<IDbDataParameter> @params);
        T ExecScalar<T>(CommandType cmdType, string cmdText);
        T ExecScalar<T>(CommandType cmdType, string cmdText, IEnumerable<IDbDataParameter> @params);
        #endregion

        #region Exec
        void Exec(Action<IDbTool> action);
        #endregion

        #region Misc
        IDictionary<string, int> CreateOrdinalCache();
        #endregion
    }
}
