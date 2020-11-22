using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace Chester.Oracle.Support
{
    public static class OracleContextExtension
    {
        public static IDbDataParameter DbParamCursor(string name) =>
            !string.IsNullOrWhiteSpace(name)
                ? new OracleParameter(name, OracleDbType.RefCursor, ParameterDirection.ReturnValue)
                : throw new ArgumentException($"{nameof(name)} cannot be null, empty or consists only of white-space characters.");
    }
}
