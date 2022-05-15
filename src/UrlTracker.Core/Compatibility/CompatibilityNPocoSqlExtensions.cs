using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NPoco;
using Umbraco.Cms.Core.Persistence;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace UrlTracker.Core.Compatibility
{
    [ExcludeFromCodeCoverage]
    public static class CompatibilityNPocoSqlExtensions
    {
        // taken straight from umbraco source. This method is bugged since before 8.6.1, so I take over the code and add the fix myself
        public static object[] OrderByDescending<TDto>(Sql<ISqlContext> sql, params Expression<Func<TDto, object?>>[] fields)
        {
            var sqlSyntax = sql.SqlContext.SqlSyntax;
            var columns = fields.Length == 0
                ? sql.GetColumns<TDto>(withAlias: false)
                : fields.Select(x => sqlSyntax.GetFieldName(x)).ToArray();
            return columns.Select(x => x + " DESC").ToArray();
        }

        internal static string[] GetColumns<TDto>(this Sql<ISqlContext> sql, string? tableAlias = null, string? referenceName = null, Expression<Func<TDto, object>>[]? columnExpressions = null, bool withAlias = true)
        {
            var pd = sql.SqlContext.PocoDataFactory.ForType(typeof(TDto));
            var tableName = tableAlias ?? pd.TableInfo.TableName;
            var queryColumns = pd.QueryColumns.ToList();

            Dictionary<string, string>? aliases = null;

            if (columnExpressions is not null && columnExpressions.Length > 0)
            {
                var names = columnExpressions.Select(x =>
                {
                    (var member, var alias) = ExpressionHelper.FindProperty(x);
                    var field = (PropertyInfo)member;
                    var fieldName = field.GetColumnName();
                    if (alias is not null)
                    {
                        if (aliases == null)
                            aliases = new Dictionary<string, string>();
                        aliases[fieldName] = alias;
                    }
                    return fieldName;
                }).ToArray();

                //only get the columns that exist in the selected names
                queryColumns = queryColumns.Where(x => names.Contains(x.Key)).ToList();

                //ensure the order of the columns in the expressions is the order in the result
                queryColumns.Sort((a, b) => names.IndexOf(a.Key).CompareTo(names.IndexOf(b.Key)));
            }

            string? GetAlias(PocoColumn column)
            {
                if (aliases is not null && aliases.TryGetValue(column.ColumnName, out var alias))
                    return alias;

                return withAlias ? (string.IsNullOrEmpty(column.ColumnAlias) ? column.MemberInfoKey : column.ColumnAlias) : null;
            }

            return queryColumns
                .Select(x => GetColumn(sql.SqlContext.DatabaseType, tableName, x.Value.ColumnName, GetAlias(x.Value), referenceName))
                .ToArray();
        }

        private static string GetColumn(DatabaseType dbType, string tableName, string columnName, string? columnAlias, string? referenceName = null)
        {
            tableName = dbType.EscapeTableName(tableName);
            columnName = dbType.EscapeSqlIdentifier(columnName);
            var column = tableName + "." + columnName;
            if (columnAlias == null) return column;

            referenceName = referenceName == null ? string.Empty : referenceName + "__";
            columnAlias = dbType.EscapeSqlIdentifier(referenceName + columnAlias);
            column += " AS " + columnAlias;
            return column;
        }

        private static string GetColumnName(this PropertyInfo column)
        {
            var attr = column.FirstAttribute<ColumnAttribute>();
            return string.IsNullOrWhiteSpace(attr?.Name) ? column.Name : attr.Name;
        }

        private static class ExpressionHelper
        {
            public static (MemberInfo, string?) FindProperty(LambdaExpression lambda)
            {
                void Throw()
                {
                    throw new ArgumentException($"Expression '{lambda}' must resolve to top-level member and not any child object's properties. Use a custom resolver on the child type or the AfterMap option instead.", nameof(lambda));
                }

                Expression expr = lambda;
                var loop = true;
                string? alias = null;
                while (loop)
                {
                    switch (expr.NodeType)
                    {
                        case ExpressionType.Convert:
                            expr = ((UnaryExpression)expr).Operand;
                            break;
                        case ExpressionType.Lambda:
                            expr = ((LambdaExpression)expr).Body;
                            break;
                        case ExpressionType.Call:
                            var callExpr = (MethodCallExpression)expr;
                            var method = callExpr.Method;
                            if (method.DeclaringType != typeof(SqlExtensionsStatics) || method.Name != "Alias" || callExpr.Arguments[1] is not ConstantExpression aliasExpr)
                                Throw();
                            expr = callExpr.Arguments[0];
                            alias = aliasExpr.Value?.ToString();
                            break;
                        case ExpressionType.MemberAccess:
                            var memberExpr = (MemberExpression)expr;
                            if (memberExpr.Expression?.NodeType != ExpressionType.Parameter && memberExpr.Expression?.NodeType != ExpressionType.Convert)
                                Throw();
                            return (memberExpr.Member, alias);
                        default:
                            loop = false;
                            break;
                    }
                }

                throw new Exception("Configuration for members is only supported for top-level individual members on a type.");
            }
        }
    }
}
