using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Chester.SqlServer.Support
{
    public static class SqlContextExtension
    {
        /// <summary>
        /// Creates a structured parameter that utilizes a createMeta function to construct the meta data and 
        /// a populateRow action action that takes an IEnumerable and populates the parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramName"></param>
        /// <param name="typeName"></param>
        /// <param name="values"></param>
        /// <param name="createMeta"></param>
        /// <param name="populateRow"></param>
        /// <returns></returns>
        public static IDbDataParameter DbParamStructured<T>(string paramName, string typeName, IEnumerable<T> values, Func<SqlMetaData[]> createMeta, Action<SqlDataRecord, T> populateRow)
        {
            var items = new List<SqlDataRecord>();

            if (values?.Any() ?? false)
            {
                var md = createMeta();

                foreach (T item in values)
                {
                    var row = new SqlDataRecord(md);
                    populateRow(row, item);
                    items.Add(row);
                }
            }

            return new SqlParameter(paramName, items)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = typeName
            };
        }

        /// <summary>
        /// Creates a structure paramter that utilizes a populated data table.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter DbParamStructured(string name, string type, DataTable value) =>
            new SqlParameter(name, value)
            {
                SqlDbType = SqlDbType.Structured,
                TypeName = type
            };
    }
}
