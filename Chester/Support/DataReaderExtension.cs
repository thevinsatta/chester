using System;
using System.Collections.Generic;
using System.Data;

namespace Chester.Support
{
    public static class DataReaderExtension
    {
        #region GetValue<T>
        /// <summary>
        /// Get value from datareader via column index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, int columnIndex)
        {
            try
            {
                return GetValue<T>(dr[columnIndex]);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()} for column at ordinal position: {columnIndex}.", e);
            }
        }

        /// <summary>
        /// Get value from datareader via named column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} cannot be null, empty or whitespace.");

            try
            {
                return GetValue<T>(dr[columnName]);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()} for column: {columnName}.", e);
            }
        }

        /// <summary>
        /// Get value from datareader via named column. More efficient lookup by using an index cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <param name="ordinalCache"></param>
        /// <returns></returns>
        public static T GetValue<T>(this IDataReader dr, string columnName, IDictionary<string, int> ordinalCache)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} cannot be null, empty or whitespace.");

            try
            {
                // check if column has been found prior and stored in the index cache
                // if not found in cache, then attempt to find it via TryGetColumnIndex
                if (!ordinalCache.TryGetValue(columnName, out int idx))
                {
                    if (!dr.TryGetColumnIndex(columnName, out idx))
                        throw new ArgumentOutOfRangeException(nameof(columnName), $"column name = {columnName} not found.");

                    ordinalCache[columnName] = idx;
                }

                return GetValue<T>(dr[idx]);
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()} for column: {columnName}.", e);
            }
        }
        #endregion

        #region GetValueOrDefault<T>
        /// <summary>
        /// Get value from datareader via named column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this IDataReader dr, string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} cannot be null, empty or whitespace.");

            try
            {
                return dr.TryGetColumnIndex(columnName, out int idx)
                    ? GetValue<T>(dr[idx])
                    : default;
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()} for column: {columnName}.", e);
            }
        }

        /// <summary>
        /// Get value from datareader via named column. More efficient lookup by using an index cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <param name="ordinalCache"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(this IDataReader dr, string columnName, IDictionary<string, int> ordinalCache)
        {
            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} cannot be null, empty or whitespace.");
            if (ordinalCache == null)
                throw new ArgumentNullException($"{nameof(ordinalCache)} cannot be null.");

            try
            {
                // check if column has been found prior and stored in the index cache
                // if not found in cache, then attempt to find it via TryGetColumnIndex
                if (!ordinalCache.TryGetValue(columnName, out int idx))
                {
                    dr.TryGetColumnIndex(columnName, out idx);
                    ordinalCache[columnName] = idx;
                }

                return idx >= 0
                    ? GetValue<T>(dr[idx])
                    : default;
            }
            catch (Exception e)
            {
                throw new Exception($"{e.GetType()} for column: {columnName}.", e);
            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// Tries to get the column index of the matching column name. Defaults to -1 if not found.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="columnName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetColumnIndex(this IDataReader dr, string columnName, out int index)
        {
            index = -1;

            if (string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException($"{nameof(columnName)} cannot be null, empty or whitespace.");

            for (var i = dr.FieldCount - 1; i >= 0; i--)
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    index = i;
                    return true;
                }

            return false;
        }
        #endregion

        #region Helper
        /// <summary>
        /// Returns value as the specified datatype or convert it to the specified datatype.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        static T GetValue<T>(object val)
        {
            if (val == null || val == DBNull.Value)
                return default;

            if (val is T t)
                return t;

            var type = typeof(T);
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type == typeof(Guid))
            {
                if (val is string)
                    val = new Guid(val as string);
                else if (val is byte[])
                    val = new Guid(val as byte[]);
            }

            return (T)Convert.ChangeType(val, type);
        }
        #endregion
    }
}
