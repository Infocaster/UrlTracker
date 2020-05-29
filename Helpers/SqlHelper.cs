using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web.Composing;

namespace InfoCaster.Umbraco.UrlTracker.Helpers
{
    public static class SqlHelper
    {

        public static T ExecuteScalar<T>(string query, List<SqlParameter> args = null)
        {
            using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
            {
                if (args == null)
                {
                    return scope.Database.ExecuteScalar<T>(query);
                }
                else
                {
                    var argsArray = args.ToArray();
                    return scope.Database.ExecuteScalar<T>(query, argsArray);
                }

            }; 
        }

        public static void ExecuteNonQuery(string query, List<SqlParameter> args = null)
        {
            using (var scope = Current.ScopeProvider.CreateScope(autoComplete: true))
            {
                if (args == null)
                {
                    scope.Database.Execute(query);
                }
                else
                {
                    var argsArray = args.ToArray();
                    scope.Database.Execute(query, argsArray);
                }

            };
        }



        public static SqlParameter CreateStringParameter(string parameterName, string value)
        {
            return new SqlParameter
            {
                Parameter = parameterName,
                Value = string.IsNullOrEmpty(value) ? DBNull.Value : (object)value
            };
        }

        public static SqlParameter CreateGenericParameter<T>(string parameterName, T value)
        {
            return new SqlParameter
            {
                Parameter = parameterName,
                Value = value == null ? DBNull.Value : (object)value
            };
        }


        public class SqlParameter
        {
            public string Parameter { get; set; }
            public object Value { get; set; }
        }
    }
}