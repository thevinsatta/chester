using System;
using System.Data;

namespace Chester.Support
{
    public static class DataParameterExtension
    {
        public static T OutputValue<T>(this IDbDataParameter param)
        {
            if (param.Direction == ParameterDirection.Input)
                throw new Exception($"Parameter {param.ParameterName} does not have Direction property set to Output, InputOutput or ReturnValue.");

            var ret = param.Value;

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
                throw new Exception($"{e.Message} Output value for parameter: {param.ParameterName}.", e);
            }
        }
    }
}
