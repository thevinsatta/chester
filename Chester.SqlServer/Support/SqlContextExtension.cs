﻿using Microsoft.Data.SqlClient;
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
        /// <param name="udtTypeName"></param>
        /// <param name="values"></param>
        /// <param name="createMeta"></param>
        /// <param name="populateRow"></param>
        /// <returns></returns>
        public static IDbDataParameter DbParamStructured<T>(
            string paramName,
            string udtTypeName,
            IEnumerable<T> values,
            Func<SqlMetaData[]> createMeta,
            Action<SqlDataRecord, T> populateRow)
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
                UdtTypeName = udtTypeName
            };
        }

        /// <summary>
        /// Creates a structure paramter that utilizes a populated data table.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="udtTypeName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDbDataParameter DbParamStructured(
            string paramName,
            string udtTypeName,
            DataTable value) =>
            new SqlParameter(paramName, value)
            {
                SqlDbType = SqlDbType.Structured,
                UdtTypeName = udtTypeName
            };
    }
}
