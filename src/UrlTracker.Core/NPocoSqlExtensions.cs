using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Persistence.Dtos;
using Umbraco.Cms.Infrastructure.Persistence.SqlSyntax;
using Umbraco.Extensions;
using UrlTracker.Core.Compatibility;

namespace UrlTracker.Core
{
    [ExcludeFromCodeCoverage]
    public static class NPocoSqlExtensions
    {
        public static string DaysDifference<TDto>(this ISqlSyntaxProvider syntaxProvider, Expression<Func<TDto, object?>> field, string? tableAlias = null)
        {
            var fieldName = syntaxProvider.GetFieldName(field, tableAlias);
            string template = syntaxProvider.ProviderName switch
            {
                "Microsoft.Data.Sqlite" => "julianday('now') - julianday({0})",
                _ => "DATEDIFF(day, {0}, GETDATE())",
            };
            return string.Format(template, fieldName);
        }

        public static string TimeFactorFunction(this ISqlSyntaxProvider syntaxProvider, params string[] arguments)
        {
            return syntaxProvider.ProviderName switch
            {
                "Microsoft.Data.Sqlite" => string.Format("1 / (({0} * {1}) + 1)", arguments.Take(2).ToArray()),
                _ => string.Format("POWER(CAST(0.5 as float), CAST({0} as float) / {1})", arguments),
            };
        }

        public static Sql<ISqlContext> From(this Sql<ISqlContext> sql, string alias, Action<Sql<ISqlContext>> subQuery)
        {
            sql.Append("FROM (");
            var sub = sql.SqlContext.Sql();
            subQuery(sub);
            sql.Append(sub);
            sql.Append(") AS " + sql.SqlContext.SqlSyntax.GetQuotedColumnName(alias));
            return sql;
        }

        public static Sql<ISqlContext> GenericOrderBy(this Sql<ISqlContext> sql, bool descending, params string[] fields)
        {
            return descending
                ? sql.OrderByDescending(fields)
                : sql.OrderBy(fields);
        }

        public static Sql<ISqlContext> OrderBy<TDto>(this Sql<ISqlContext> sql, bool descending, params Expression<Func<TDto, object?>>[] fields)
        {
            if (descending)
            {
                return sql.OrderBy(CompatibilityNPocoSqlExtensions.OrderByDescending(sql, fields));
            }

            return sql.OrderBy<TDto>(fields);
        }

        public static Sql<ISqlContext> SelectMax<TDto>(this Sql<ISqlContext> sql, string alias, string tableAlias, Expression<Func<TDto, object?>> field)
        {
            string text = CreateAggregateField(sql, alias, tableAlias, "MAX", field);
            return sql.Select(text);
        }

        public static Sql<ISqlContext> AndSelectMax<TDto>(this Sql<ISqlContext> sql, string? alias, string? tableAlias, Expression<Func<TDto, object?>> field)
        {
            string text = CreateAggregateField(sql, alias, tableAlias, "MAX", field);
            return sql.AndSelect(text);
        }

        public static Sql<ISqlContext> AndSelectMin<TDto>(this Sql<ISqlContext> sql, string? alias, string? tableAlias, Expression<Func<TDto, object?>> field)
        {
            string text = CreateAggregateField(sql, alias, tableAlias, "MIN", field);
            return sql.AndSelect(text);
        }

        private static string CreateAggregateField<TDto>(Sql<ISqlContext> sql, string? alias, string? tableAlias, string aggregator, Expression<Func<TDto, object?>> field)
        {
            if (sql is null)
            {
                throw new ArgumentNullException(nameof(sql));
            }

            if (field is null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            ISqlSyntaxProvider sqlSyntax = sql.SqlContext.SqlSyntax;
            string value = sqlSyntax.GetFieldName(field, tableAlias);
            string text = $"{aggregator} ({value})";
            if (alias is not null)
            {
                text = text + " AS " + sql.SqlContext.SqlSyntax.GetQuotedColumnName(alias);
            }

            return text;
        }
        public static Sql<ISqlContext> GroupBy<TDto>(this Sql<ISqlContext> sql, string tableAlias, params Expression<Func<TDto, object?>>[] fields)
        {
            ISqlSyntaxProvider sqlSyntax = sql.SqlContext.SqlSyntax;
            string[] array = (fields.Length == 0)
                ? sql.GetColumns<TDto>(tableAlias, null, null, withAlias: false)
                : fields.Select(x => sqlSyntax.GetFieldName(x, tableAlias)).ToArray();
            object[] columns = array;
            return sql.GroupBy(columns);
        }
    }
}
