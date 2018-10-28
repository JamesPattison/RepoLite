using NS.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace NS.Base
{
    public partial interface IBaseRepository<T>
        where T : IBaseModel
    {
        long RecordCount();
        IEnumerable<T> GetAll();
        bool Create(T item);
        bool BulkCreate(List<T> items);
        bool BulkCreate(params T[] items);

        Where<T> Where(string col, Comparison comparison, object val);
        Where<T> Where(string col, Comparison comparison, object val, Type valueType);
        IEnumerable<T> Where(string query);
    }

    public interface IPkRepository<T> : IBaseRepository<T>
        where T : IBaseModel
    {
        bool Update(T item);
        bool Delete(T item);
        bool Delete(IEnumerable<T> items);
        bool Merge(List<T> items);
    }

    #region Enums

    internal enum ClauseType
    {
        Initial,
        And,
        Or
    }

    public enum FindComparison
    {
        Equals,
        NotEquals,
        Like,
        NotLike,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
    }

    public enum Comparison
    {
        Equals,
        NotEquals,
        Like,
        NotLike,
        GreaterThan,
        GreaterThanOrEquals,
        LessThan,
        LessThanOrEquals,
        In,
        NotIn,
        IsNull,
        IsNotNull
    }

    #endregion

    #region Models

    public class DeleteColumn
    {
        public string ColumnName { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public object Data { get; set; }

        public DeleteColumn(string columnName, object data, SqlDbType dbType)
        {
            ColumnName = columnName;
            Data = data;
            SqlDbType = dbType;
        }
    }

    public class ColumnDefinition
    {
        public string ColumnName { get; set; }
        public Type ValueType { get; set; }
        public string SqlDataTypeText { get; set; }
        public SqlDbType SqlDbType { get; set; }
        public bool Identity { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Nullable { get; set; }

        internal ColumnDefinition(string columnName) : this(columnName, typeof(string), "NVARCHAR(MAX)", SqlDbType.NVarChar, false, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, SqlDbType dbType) : this(columnName, valueType, sqlDataTypeText, dbType, false, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, SqlDbType dbType, bool nullable) : this(columnName, valueType, sqlDataTypeText, dbType, nullable, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, SqlDbType dbType, bool nullable, bool primaryKey) : this(columnName, valueType, sqlDataTypeText, dbType, nullable, primaryKey, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, SqlDbType dbType, bool nullable, bool primaryKey, bool identity)
        {
            ColumnName = columnName;
            ValueType = valueType;
            SqlDataTypeText = sqlDataTypeText;
            SqlDbType = dbType;
            Nullable = nullable;
            PrimaryKey = primaryKey;
            Identity = identity;
        }
    }

    public class QueryItem
    {
        public string DbColumnName { get; set; }
        public object Value { get; set; }
        public Type DataType { get; set; }

        public QueryItem(string dbColName, object value) : this(dbColName, value, value.GetType()) { }

        public QueryItem(string dbColName, object value, Type dataType)
        {
            DbColumnName = dbColName;
            Value = value;
            DataType = dataType;
        }
    }

    public class ValidationException : Exception
    {
        public List<ValidationError> ValidationErrors { get; set; }

        public ValidationException(List<ValidationError> validationErrors)
        {
            ValidationErrors = validationErrors;
        }
    }

    #endregion

    #region Where

    public class Where<T>
        where T : IBaseModel
    {
        private readonly StringBuilder _query = new StringBuilder();
        private readonly BaseRepository<T> _repository;
        private int _activeGroups;

        public Where(BaseRepository<T> baseRepository, string col, Comparison comparison, object val) : this(
            baseRepository, col, comparison, val, val.GetType())
        { }

        public Where(BaseRepository<T> baseRepository, string col, Comparison comparison, object val, Type valueType)
        {
            _repository = baseRepository;

            _query.Append(MakeClause(col, comparison, val, ClauseType.Initial, valueType));
        }

        private string MakeClause(string col, Comparison comparison, ClauseType clauseType)
        {
            return MakeClause(col, comparison, null, clauseType, null);
        }

        private string MakeClause(string col, Comparison comparison, object val, ClauseType clauseType, Type valueType)
        {
            var query = new StringBuilder();

            switch (comparison)
            {
                case Comparison.In:
                case Comparison.NotIn:
                    if (val is IEnumerable list && !(val as object[] ?? (val as IEnumerable).Cast<object>().ToArray()).Any())
                    {
                        //No elements in list, append a clause which will return no values
                        query.Append("1=0");
                        return query.ToString();
                    }

                    break;
            }

            float floatVal;
            if (new[]
                {
                    Comparison.GreaterThan, Comparison.GreaterThanOrEquals, Comparison.LessThan,
                    Comparison.LessThanOrEquals
                }.Contains(comparison)
                && !float.TryParse(val.ToString(), out floatVal))
                throw new Exception("Numeric comparison used on a non numeric value.");

            switch (clauseType)
            {
                case ClauseType.Initial:
                    query.Append(valueType == typeof(XmlDocument)
                        ? $"CONVERT(NVARCHAR(MAX), [{col}])"
                        : $"[{col}]");
                    break;
                case ClauseType.And:
                    query.Append(valueType == typeof(XmlDocument)
                        ? $" AND CONVERT(NVARCHAR(MAX), [{col}])"
                        : $" AND [{col}]");
                    break;
                case ClauseType.Or:
                    query.Append(valueType == typeof(XmlDocument)
                        ? $" OR CONVERT(NVARCHAR(MAX), [{col}])"
                        : $" OR [{col}]");
                    break;
            }

            query.Append(GetComparison(comparison));
            if (comparison != Comparison.IsNull && comparison != Comparison.IsNotNull)
            {
                var typeVal = GetTypeVal(col, val);

                if (comparison == Comparison.Like || comparison == Comparison.NotLike)
                    typeVal = typeVal.TrimStart('\'').TrimEnd('\'');

                query.Append(typeVal);
            }

            switch (comparison)
            {
                case Comparison.In:
                case Comparison.NotIn:
                    query.Append(")");
                    break;
                case Comparison.Like:
                case Comparison.NotLike:
                    query.Append("%'");
                    break;
            }

            return query.ToString();
        }

        private static string GetComparison(Comparison comparison)
        {
            switch (comparison)
            {
                case Comparison.Equals:
                    return " = ";
                case Comparison.NotEquals:
                    return " <> ";
                case Comparison.Like:
                    return " LIKE '%";
                case Comparison.NotLike:
                    return " NOT LIKE '%";
                case Comparison.GreaterThan:
                    return " > ";
                case Comparison.GreaterThanOrEquals:
                    return " >= ";
                case Comparison.LessThan:
                    return " < ";
                case Comparison.LessThanOrEquals:
                    return " <= ";
                case Comparison.In:
                    return " IN (";
                case Comparison.NotIn:
                    return " NOT IN (";
                case Comparison.IsNull:
                    return " IS NULL";
                case Comparison.IsNotNull:
                    return " IS NOT NULL";
                default:
                    throw new NotSupportedException("???");
            }
        }

        private string GetTypeVal(string col, object val)
        {
            var typeName = val is IList ? "List" : val.GetType().Name;
            switch (typeName)
            {
                case "Boolean":
                    if ((bool)val)
                        return "1";
                    return "0";
                case "Int16":
                case "UInt16":
                case "Int32":
                case "UInt32":
                case "Int64":
                case "UInt64":
                case "Decimal":
                case "Double":
                    return val.ToString();
                case "DateTime":
                case "Char":
                case "String":
                case "Guid":
                case "TimeSpan":
                case "DateTimeOffset":
                    return "'" + val + "'";
                case "List":
                    var result = "";

                    var enumerable = val as object[] ?? (val as IEnumerable).Cast<object>().ToArray();

                    const int batchSize = 2000;
                    var batches = Math.Ceiling((decimal)enumerable.Length / batchSize);
                    for (var i = 0; i < batches; i++)
                    {
                        result = enumerable
                            .Skip(i * batchSize)
                            .Take(batchSize)
                            .Aggregate(result, (current, o) => current + GetTypeVal(col, o) + ", ")
                            .TrimEnd(' ')
                            .TrimEnd(',');

                        if (batches > i + 1)
                            result += ") OR [" + col + "] IN (";
                    }

                    return result;
                default:
                    throw new NotSupportedException("Not supported yet");
            }
        }

        public IEnumerable<T> Results()
        {
            if (_activeGroups > 0) throw new Exception("Please close all Query Groups before calling Results()");
            return _repository.Where(_query.ToString());
        }

        public Where<T> And(string col, Comparison comparison)
        {
            if (comparison != Comparison.IsNull && comparison != Comparison.IsNotNull)
                throw new Exception("And(" + col + ", " + comparison + ") can only be called with Comparison.IsNull or Comparison.IsNotNull");
            _query.Append(MakeClause(col, comparison, ClauseType.And));
            return this;
        }

        public Where<T> And(string col, Comparison comparison, object val)
        {
            return And(col, comparison, val, val.GetType());
        }

        public Where<T> And(string col, Comparison comparison, object val, Type valueType)
        {
            _query.Append(MakeClause(col, comparison, val, ClauseType.And, valueType));
            return this;
        }

        public Where<T> Or(string col, Comparison comparison)
        {
            if (comparison != Comparison.IsNull && comparison != Comparison.IsNotNull)
                throw new Exception("Or(" + col + ", " + comparison + ") can only be called with Comparison.IsNull or Comparison.IsNotNull");

            _query.Append(MakeClause(col, comparison, ClauseType.Or));
            return this;
        }

        public Where<T> Or(string col, Comparison comparison, object val)
        {
            return Or(col, comparison, val, val.GetType());
        }

        public Where<T> Or(string col, Comparison comparison, object val, Type valueType)
        {
            _query.Append(MakeClause(col, comparison, val, ClauseType.Or, valueType));
            return this;
        }

        public Where<T> AndBeginGroup(string col, Comparison comparison)
        {
            if (comparison != Comparison.IsNull && comparison != Comparison.IsNotNull)
                throw new Exception("AndBeginGroup(" + col + ", " + comparison + ") can only be called with Comparison.IsNull or Comparison.IsNotNull");

            _activeGroups++;
            _query.Append(" AND (" + MakeClause(col, comparison, ClauseType.Initial));
            return this;
        }

        public Where<T> AndBeginGroup(string col, Comparison comparison, object val)
        {
            return AndBeginGroup(col, comparison, val, val.GetType());
        }

        public Where<T> AndBeginGroup(string col, Comparison comparison, object val, Type valueType)
        {
            _activeGroups++;
            _query.Append(" AND (" + MakeClause(col, comparison, val, ClauseType.Initial, valueType));
            return this;
        }

        public Where<T> OrBeginGroup(string col, Comparison comparison)
        {
            if (comparison != Comparison.IsNull && comparison != Comparison.IsNotNull)
                throw new Exception("OrBeginGroup(" + col + ", " + comparison + ") can only be called with Comparison.IsNull or Comparison.IsNotNull");

            _activeGroups++;
            _query.Append(" OR (" + MakeClause(col, comparison, ClauseType.Initial));
            return this;
        }

        public Where<T> OrBeginGroup(string col, Comparison comparison, object val)
        {
            return OrBeginGroup(col, comparison, val, val.GetType());
        }

        public Where<T> OrBeginGroup(string col, Comparison comparison, object val, Type valueType)
        {
            _activeGroups++;
            _query.Append(" OR (" + MakeClause(col, comparison, val, ClauseType.Initial, valueType));
            return this;
        }

        public Where<T> EndGroup()
        {
            _activeGroups--;
            _query.Append(")");
            return this;
        }

        public string QueryString()
        {
            return _repository.WhereQuery() + " WHERE " + _query;
        }
    }

    #endregion
    
    #region [ExpressionParser]

    internal static class ExpressionParser
    {
        private static readonly Dictionary<ExpressionType, string> NodeStr = new Dictionary<ExpressionType, string>
        {
            {ExpressionType.Add, "+"},
            {ExpressionType.And, "&"},
            {ExpressionType.AndAlso, "AND"},
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.Negate, "-"},
            {ExpressionType.Not, "NOT"},
            {ExpressionType.NotEqual, "<>"},
            {ExpressionType.Or, "|"},
            {ExpressionType.OrElse, "OR"},
            {ExpressionType.Subtract, "-"}
        };
        
        internal static string ToSql(LambdaExpression expression)
        {
            return Parse(expression.Body, true).Sql;
        }       
        
        internal static string ToSql<T>(Expression<Func<T, bool>> expression)
            where T : IBaseModel
        {
            return ToSql((LambdaExpression)expression);
        }
        
        internal static string ToSql<T, TK>(Expression<Func<T, TK, bool>> expression)
            where T : IBaseModel
            where TK : IBaseModel
        {
            return ToSql((LambdaExpression)expression);
        }

        private static Clause Parse(Expression expression, bool isUnary = false, string prefix = null, string postfix = null, bool boolComparison = false)
        {
            while (true)
            {
                switch (expression)
                {
                    case UnaryExpression unary:
                        return Clause.Make(NodeStr[unary.NodeType], Parse(unary.Operand, true));
                    case BinaryExpression body:
                        
                        var left = body.Left.Type == typeof(bool) ? Parse(body.Left, boolComparison: true) : Parse(body.Left);
                        var right = body.Right.Type == typeof(bool) ? Parse(body.Right, boolComparison: true) : Parse(body.Right);
                        
                        return Clause.Make(left, NodeStr[body.NodeType], right);
                    case ConstantExpression constant:
                    {
                        var value = constant.Value;
                        switch (value)
                        {
                            case int _:
                                return Clause.Make(value.ToString());
                            case string _:
                                value = prefix + (string) value + postfix;
                                break;
                        }

                        if (value is bool && isUnary)
                        {
                            return boolComparison ? Clause.Make($"'{value}'"): Clause.Make(Clause.Make($"'{value}'"), "=", Clause.Make("1"));
                        }

                        return Clause.Make($"'{value}'");
                    }
                    case MemberExpression member:
                    {
                        switch (member.Member)
                        {
                            case PropertyInfo property:
                            {
                                var colName = property.Name;
                                if (!(Activator.CreateInstance(property.DeclaringType ?? throw new Exception()) is IBaseModel model)) throw new Exception();

                                if (member.Type == typeof(bool))
                                    if (isUnary)
                                    {
                                        isUnary = false;
                                        prefix = null;
                                        postfix = null;
                                        continue;
                                    }
                                    else
                                    {
                                        return boolComparison ? Clause.Make($"[{model.EntityName}].[{colName}]"): Clause.Make(Clause.Make($"[{model.EntityName}].[{colName}]"), "=",
                                            Clause.Make("1"));
                                    }
                                else
                                {
                                    var value = $"[{model.EntityName}].[{colName}]";
                                    if (!string.IsNullOrEmpty(prefix))
                                        value = $"'{prefix}'+{value}";
                                    if (!string.IsNullOrEmpty(postfix))
                                        value = $"{value}+'{postfix}'";

                                    return Clause.Make(value);
                                }
                            }
                            case FieldInfo _:
                            {
                                var value = GetValue(member);
                                if (value is string)
                                {
                                    value = prefix + (string) value + postfix;
                                }

                                return Clause.Make($"'{value}'");
                            }
                            default:
                                throw new Exception($"Expression does not refer to a property or field: {expression}");
                        }
                    }
                    case MethodCallExpression methodCall:
                    {
                        // LIKE queries:
                        if (methodCall.Method == typeof(string).GetMethod("Contains", new[] {typeof(string)}))
                        {
                            return Clause.Make(Parse(methodCall.Object), "LIKE", Parse(methodCall.Arguments[0], prefix: "%", postfix: "%"));
                        }

                        if (methodCall.Method == typeof(string).GetMethod("StartsWith", new[] {typeof(string)}))
                        {
                            return Clause.Make(Parse(methodCall.Object), "LIKE", Parse(methodCall.Arguments[0], postfix: "%"));
                        }

                        if (methodCall.Method == typeof(string).GetMethod("EndsWith", new[] {typeof(string)}))
                        {
                            return Clause.Make(Parse(methodCall.Object), "LIKE", Parse(methodCall.Arguments[0], prefix: "%"));
                        }

                        // IN queries:
                        if (methodCall.Method.Name == "Contains")
                        {
                            Expression collection;
                            Expression property;
                            if (methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 2)
                            {
                                collection = methodCall.Arguments[0];
                                property = methodCall.Arguments[1];
                            }
                            else if (!methodCall.Method.IsDefined(typeof(ExtensionAttribute)) && methodCall.Arguments.Count == 1)
                            {
                                collection = methodCall.Object;
                                property = methodCall.Arguments[0];
                            }
                            else
                            {
                                throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                            }

                            var sb = new StringBuilder();
                            foreach (var val in (IEnumerable) GetValue(collection))
                            {
                                sb.Append($"'{val}',");
                            }

                            var values = sb.ToString();
                            values = values.Substring(0, values.Length - 1);
                            return Clause.Make(Parse(property), "IN", Clause.Make($"({values})"));
                        }

                        throw new Exception("Unsupported method call: " + methodCall.Method.Name);
                    }
                    default:
                        throw new Exception("Unsupported expression: " + expression.GetType().Name);
                }

                break;
            }
        }

        private static object GetValue(Expression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }
        
        private class Clause
        {
            public string Sql { get; private set; }

            public static Clause Make(string sql)
            {
                return new Clause
                {
                    Sql = sql
                };
            }

            public static Clause Make(string @operator, Clause operand)
            {
                return new Clause
                {
                    Sql = $"({@operator} {operand.Sql})"
                };
            }

            public static Clause Make(Clause left, string @operator, Clause right)
            {
                return new Clause
                {
                    Sql = $"({left.Sql} {@operator} {right.Sql})"
                };
            }
        }
    }
    
    #endregion

    public abstract partial class BaseRepository<T> : IBaseRepository<T>
        where T : IBaseModel
    {
        protected Action<Exception> Logger;
        protected string ConnectionString;
        private readonly string _schema;
        private readonly string _tableName;
        private List<ColumnDefinition> Columns;

        protected BaseRepository(string connectionString, Action<Exception> logMethod, string schema, string table, List<ColumnDefinition> columns)
        {
            _schema = schema;
            _tableName = table;
            ConnectionString = connectionString;
            Logger = logMethod ?? (exception => { });
            Columns = columns;

            var sql = $@"SELECT COUNT(*)
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = '{table}' AND TABLE_SCHEMA = '{schema}'";

            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = CreateCommand(cn, sql))
                {
                    try
                    {
                        cn.Open();
                        var count = (int)cmd.ExecuteScalar();
                        if (count != columns.Count)
                            throw new Exception(
                                "Repository Definition does not match Database. Please re-run the code generator to get a new repository");
                    }
                    finally { cn.Close(); }
                }
            }
        }

        public long RecordCount()
        {
            var query = BuildWhereQuery(new[]{new ColumnDefinition("'x'") });
            var dt = Where(query, "1=1");
            return dt == null ? 0 : dt.Rows.Count;
        }

        public IEnumerable<T> GetAll()
        {
            return Where("1=1");
        }

        public abstract bool Create(T item);
        public abstract bool BulkCreate(List<T> items);
        public abstract bool BulkCreate(params T[] items);
        protected abstract T ToItem(DataRow row);

        protected SqlCommand CreateCommand(SqlConnection cn, string command)
        {
            var cmd = new SqlCommand
            {
                Connection = cn,
                CommandType = CommandType.Text,
                CommandText = command
            };
            return cmd;
        }

        protected internal string WhereQuery()
        {
            return BuildWhereQuery(Columns);
        }

        private string BuildWhereQuery(IEnumerable<ColumnDefinition> columns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT ");

            var columnArray = columns.ToArray();
            if (columnArray.Any())
            {
                foreach (var column in columnArray)
                {
                    sb.Append(column.ColumnName);
                    if (column != columnArray.Last())
                        sb.Append(", ");
                }
            }

            sb.Append($" FROM [{_schema}].[{_tableName}]");

            return sb.ToString();
        }

        public Where<T> Where(string col, Comparison comparison, object val)
        {
            return Where(col, comparison, val, val.GetType());
        }

        public Where<T> Where(string col, Comparison comparison, object val, Type valueType)
        {
            return new Where<T>(this, col, comparison, val, valueType);
        }

        public IEnumerable<T> Where(string query)
        {
            var dt = Where(WhereQuery(), query);
            return dt == null ? new T[0] : ToItems(dt);
        }

        private DataTable Where(string columnPart, string filterPart)
        {
            if (HasInjection(columnPart) || HasInjection(filterPart))
                throw new Exception("Sql Injection attempted. Aborted");

            //Get
            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = CreateCommand(cn, $"{columnPart} WHERE {filterPart}"))
                {
                    if (HasInjection(cmd.CommandText))
                        throw new Exception("Sql Injection attempted. Aborted");

                    //Execute
                    cn.Open();
                    var dt = ToDataTable(cmd);
                    if (dt == null)
                        return null;

                    cn.Close();

                    return dt;
                }
            }
        }

        protected bool HasInjection(string query)
        {
            var isSqlInjection = false;

            string[] sqlCheckList =
            {
                "--", ";--", "/*", "*/"
            };

            var checkString = query.Replace("'", "''");

            for (var i = 0; i <= sqlCheckList.Length - 1; i++)
            {
                if ((checkString.IndexOf(sqlCheckList[i], StringComparison.OrdinalIgnoreCase) < 0))
                    continue;
                isSqlInjection = true;
                break;
            }

            return isSqlInjection;
        }

        protected IEnumerable<T> BaseSearch(List<QueryItem> queries)
        {
            if (!queries.Any())
                return new List<T>();

            var first = queries.First();
            var whereQuery = Where(first.DbColumnName, Comparison.Equals, first.Value, first.DataType);

            if (queries.Count > 1)
            {
                whereQuery = queries.Skip(1).Aggregate(whereQuery,
                    (current, query) => current.And(query.DbColumnName, Comparison.Equals, query.Value, first.DataType));
            }

            return whereQuery.Results();
        }

        protected Dictionary<string, object> BaseCreate(params object[] values)
        {
            var returnIds = new Dictionary<string, object>();

            //Creation
            using (var cn = new SqlConnection(ConnectionString))
            {
                var sb = new StringBuilder();
                var pkCols = Columns.Where(x => x.PrimaryKey).ToList();

                if (Columns.Any(x => x.PrimaryKey))
                {
                    sb.AppendLine("DECLARE @tempo TABLE (");
                    foreach (var pk in pkCols)
                    {
                        sb.Append($"[{pk.ColumnName}]  {pk.SqlDataTypeText}");
                        sb.AppendLine(pk != pkCols[pkCols.Count - 1] ? "," : string.Empty);
                    }
                    sb.AppendLine(")");
                }
                sb.AppendLine($"INSERT [{_schema}].[{_tableName}] (");

                var toCreate = Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity).ToList();
                foreach (var createColumn in toCreate)
                {
                    sb.Append($"[{createColumn.ColumnName}]");

                    sb.AppendLine(createColumn != Columns.Last() ? "," : ")");
                }

                if (Columns.Any(x => x.PrimaryKey))
                {
                    sb.Append("OUTPUT ");

                    foreach (var pk in pkCols)
                    {
                        sb.Append($"[Inserted].[{pk.ColumnName}] ");
                        sb.AppendLine(pk != pkCols[pkCols.Count - 1] ? "," : string.Empty);
                    }

                    sb.AppendLine("INTO @tempo ");
                }

                sb.AppendLine("VALUES (");

                var valueCols = Columns.Where(x => !x.PrimaryKey || (x.PrimaryKey && !x.Identity)).ToList();
                foreach (var createColumn in valueCols)
                {
                    sb.Append("@" + createColumn.ColumnName);
                    sb.AppendLine(createColumn != valueCols.Last() ? "," : ")");

                }

                if (Columns.Any(x => x.PrimaryKey))
                {
                    sb.AppendLine("SELECT * FROM @tempo");
                }

                var sql = sb.ToString();

                if (HasInjection(sql))
                    throw new Exception("Sql Injection attempted. Aborted");

                using (var cmd = CreateCommand(cn, sql))
                {
                    for (var i = 0; i < Columns.Count; i++)
                    {
                        var createColumn = Columns[i];
                        if (createColumn.PrimaryKey && (!createColumn.PrimaryKey || createColumn.Identity))
                            continue;

                        var parameter = cmd.Parameters.Add(createColumn.ColumnName, createColumn.SqlDbType);
                                                                   parameter.Value = values[i] != null
                                                                       ? (values[i].GetType() == typeof(XmlDocument)
                                                                           ? ((XmlDocument) values[i]).InnerXml
                                                                           : values[i])
                                                                       : DBNull.Value;
                    }

                    DataTable dt;
                    var isSuccess = ToDataTable(cmd, cn, out dt);
                    //Extract the primary keys

                    if (!isSuccess) return returnIds;

                    if (dt.Rows.Count > 0)
                    {
                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            var dataColumn = dt.Columns[i];
                            returnIds.Add(dataColumn.ColumnName, dt.Rows[0][i]);
                        }
                    }
                }
            }

            return returnIds;
        }

        protected bool BulkInsert(DataTable dt, string tableName)
        {
            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                {
                    cn.Open();

                    //Copy to staging table
                    using (var bulkCopy =
                        new SqlBulkCopy(cn,
                                SqlBulkCopyOptions.TableLock |
                                SqlBulkCopyOptions.FireTriggers |
                                SqlBulkCopyOptions.UseInternalTransaction, null)
                        { DestinationTableName = tableName })
                    {
                        //Needed if there is an identity column on the table
                        foreach (DataColumn dataColumn in dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName);
                        }

                        bulkCopy.WriteToServer(dt);
                    }

                    cn.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger(ex);
                return false;
            }
        }

        protected bool BulkInsert(DataTable dt)
        {
            return BulkInsert(dt, $"[{_schema}].[{_tableName}]");
        }

        protected bool BaseUpdate(List<string> dirtyColumns, params object[] values)
        {
            bool isSuccess;

            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE [{_schema}].[{_tableName}] SET");

            var nonpkCols = Columns.Where(x => !x.PrimaryKey).ToArray();
            foreach (var col in nonpkCols.Where(x => dirtyColumns.Contains(x.ColumnName)))
            {
                sb.Append($"[{col.ColumnName}] = @{col.ColumnName}");
                sb.AppendLine(col != nonpkCols.Last(x => dirtyColumns.Contains(x.ColumnName)) ? "," : "");
            }
            sb.AppendLine("WHERE");

            var pkCols = Columns.Where(x => x.PrimaryKey).ToArray();
            foreach (var pk in pkCols)
            {
                sb.AppendLine(pk == pkCols.First()
                    ? $"[{pk.ColumnName}] = @{pk.ColumnName}"
                        : $"AND [{pk.ColumnName}] = @{pk.ColumnName}");
            }

            var sql = sb.ToString();

            if (HasInjection(sql))
                throw new Exception("Sql Injection attempted. Aborted");

            //Creation
            using (var cn = new SqlConnection(ConnectionString))
            {
                using (var cmd = CreateCommand(cn, sql))
                {
                    for (var i = 0; i < Columns.Count; i++)
                    {
                        var updateColumn = Columns[i];
                        if (!dirtyColumns.Contains(updateColumn.ColumnName) && !updateColumn.PrimaryKey) continue;

                        var parameter = cmd.Parameters.Add(updateColumn.ColumnName, updateColumn.SqlDbType);
                        if (updateColumn.PrimaryKey)
                        {
                            parameter.Value = values[i];
                        }
                        else
                        {
                            parameter.Value = dirtyColumns.Contains(updateColumn.ColumnName) ? values[i] ?? DBNull.Value : values[i];
                        }
                    }

                    //Execute
                    isSuccess = NoneQuery(cn, cmd);
                }
            }

            return isSuccess;
        }

        protected bool BaseDelete(DeleteColumn deleteColumn)
        {
            bool isSuccess;

            //Creation
            using (var cn = new SqlConnection(ConnectionString))
            {
                var sb = new StringBuilder();
                sb.Append($"DELETE [{_schema}].[{_tableName}] WHERE ");
                sb.Append($"[{ deleteColumn.ColumnName}] = @{deleteColumn.ColumnName}");

                var sql = sb.ToString();
                if (HasInjection(sql))
                    throw new Exception("Sql Injection attempted. Aborted");

                using (var cmd = CreateCommand(cn, sql))
                {
                    var parameter = cmd.Parameters.Add(deleteColumn.ColumnName, deleteColumn.SqlDbType);
                    parameter.Value = deleteColumn.Data;

                    //Execute
                    isSuccess = NoneQuery(cn, cmd);
                }
            }

            return isSuccess;
        }

        protected bool BaseDelete(string columnName, List<object> dataValues)
        {
            bool isSuccess;


            //Creation
            using (var cn = new SqlConnection(ConnectionString))
            {
                var sb = new StringBuilder();

                sb.Append($"DELETE [{_schema}].[{_tableName}] WHERE [{columnName}] IN (");

                foreach (var dataValue in dataValues)
                {
                    sb.Append(dataValue);
                    if (dataValue != dataValues.Last())
                        sb.Append(", ");
                }

                sb.Append(")");

                var sql = sb.ToString();
                if (HasInjection(sql))
                    throw new Exception("Sql Injection attempted. Aborted");

                using (var cmd = CreateCommand(cn, sql))
                {
                    isSuccess = NoneQuery(cn, cmd);
                }
            }

            return isSuccess;
        }

        protected bool BaseMerge(List<object[]> mergeData)
        {
            var tempTableName = "staging" + DateTime.Now.Ticks;

            try
            {
                var dt = new DataTable();
                foreach (var mergeColumn in Columns)
                {
                    dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);
                    if (!mergeColumn.PrimaryKey)
                        dt.Columns.Add(mergeColumn.ColumnName + "Changed", typeof(bool));
                }

                foreach (var data in mergeData)
                {
                    dt.Rows.Add(data);
                }

                CreateStagingTable(tempTableName);
                BulkInsert(dt, tempTableName);

                using (var cn = new SqlConnection(ConnectionString))
                {
                    var mergeSql = new StringBuilder();
                    mergeSql.AppendLine($"MERGE INTO [{_schema}].[{_tableName}] AS [Target]");
                    mergeSql.AppendLine($"USING {tempTableName} AS Source");
                    mergeSql.AppendLine("ON");

                    var pks = Columns.Where(x => x.PrimaryKey).ToArray();

                    foreach (var pk in pks)
                    {
                        if (pk != pks.First())
                            mergeSql.Append("AND ");
                        mergeSql.AppendLine($"[Target].[{pk.ColumnName}] = [Source].[{pk.ColumnName}]");
                    }


                    mergeSql.AppendLine("WHEN MATCHED THEN UPDATE SET");

                    var nonpks = Columns.Where(x => !x.PrimaryKey).ToArray();

                    foreach (var mergeColumn in nonpks)
                    {
                        mergeSql.Append(
                            $"[Target].[{mergeColumn.ColumnName}] = CASE WHEN [Source].[{mergeColumn.ColumnName}Changed] = 1 THEN [Source].[{mergeColumn.ColumnName}] ELSE [Target].[{mergeColumn.ColumnName}] END");

                        mergeSql.AppendLine(mergeColumn != nonpks.Last() ? "," : Environment.NewLine);
                    }

                    mergeSql.AppendLine("WHEN NOT MATCHED THEN INSERT (");

                    mergeSql.AppendLine(string.Join(",", Columns.Where(x => !x.Identity).Select(x => $"[{x.ColumnName}]").ToArray()) + ")");
                    mergeSql.AppendLine("VALUES (");
                    mergeSql.AppendLine(string.Join(",", Columns.Where(x => !x.Identity).Select(x => $"[Source].[{x.ColumnName}]").ToArray()) + ");");
                    mergeSql.AppendLine("IF OBJECT_ID('dbo.DropTmpTable') IS NULL EXEC ('CREATE PROCEDURE dbo.DropTmpTable @table NVARCHAR(250) AS DECLARE @sql NVARCHAR(300) = N''DROP TABLE '' + @table; EXECUTE sp_executesql @sql')");

                    var sql = mergeSql.ToString();
                    if (HasInjection(sql))
                        throw new Exception("Sql Injection attempted. Aborted");

                    //Merge data
                    var cmd = new SqlCommand
                    {
                        Connection = cn,
                        CommandType = CommandType.Text,
                        CommandText = sql
                    };

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();

                    var dropCmd = new SqlCommand
                    {
                        Connection = cn,
                        CommandType = CommandType.StoredProcedure,
                        CommandText = "dbo.DropTmpTable"
                    };
                    
                    dropCmd.Parameters.AddWithValue("table", tempTableName);
                    dropCmd.ExecuteNonQuery();
                    dropCmd.Dispose();
                    cn.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger(ex);
                using (var cn = new SqlConnection(ConnectionString))
                {
                    var cmd = new SqlCommand
                    {
                        Connection = cn,
                        CommandType = CommandType.Text,
                        CommandText = $"DROP TABLE {tempTableName}"
                    };

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex2) { Logger(ex2); }
                }
                return false;
            }
        }

        protected IEnumerable<T> ToItems(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                var item = default(T);
                try
                {
                    item = ToItem(row);
                }
                catch (Exception ex)
                {
                    Logger(ex);
                }
                yield return item;
            }
        }

        protected bool ToDataTable(SqlCommand cmd, SqlConnection cn, out DataTable dt)
        {
            var isSuccess = true;
            if (HasInjection(cmd.CommandText))
                throw new Exception("Sql Injection attempted. Aborted");

            //Execute
            cn.Open();
            dt = ToDataTable(cmd);
            cn.Close();

            if (dt == null || dt.Rows.Count == 0)
                isSuccess = false;
            return isSuccess;
        }

        protected bool NoneQuery(SqlConnection cn, SqlCommand cmd)
        {
            if (HasInjection(cmd.CommandText))
                throw new Exception("Sql Injection attempted. Aborted");

            var isSuccess = true;
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger(ex);
                isSuccess = false;
            }
            cn.Close();
            return isSuccess;
        }

        protected bool GetBoolean(DataRow row, string fieldName)
        {
            return row.GetValue<bool>(fieldName) ?? false;
        }

        protected bool? GetNullableBoolean(DataRow row, string fieldName)
        {
            return row.GetValue<bool>(fieldName);
        }

        protected short GetInt16(DataRow row, string fieldName)
        {
            return row.GetValue<short>(fieldName) ?? default(Int16);
        }

        protected short? GetNullableInt16(DataRow row, string fieldName)
        {
            return row.GetValue<short>(fieldName);
        }

        protected int GetInt32(DataRow row, string fieldName)
        {
            return row.GetValue<int>(fieldName) ?? default(Int32);
        }

        protected int? GetNullableInt32(DataRow row, string fieldName)
        {
            return row.GetValue<int>(fieldName);
        }

        protected long GetInt64(DataRow row, string fieldName)
        {
            return row.GetValue<long>(fieldName) ?? default(Int64);
        }

        protected long? GetNullableInt64(DataRow row, string fieldName)
        {
            return row.GetValue<long>(fieldName);
        }

        protected decimal GetDecimal(DataRow row, string fieldName)
        {
            return row.GetValue<decimal>(fieldName) ?? default(decimal);
        }

        protected decimal? GetNullableDecimal(DataRow row, string fieldName)
        {
            return row.GetValue<decimal>(fieldName);
        }

        protected double GetDouble(DataRow row, string fieldName)
        {
            return row.GetValue<double>(fieldName) ?? default(double);
        }

        protected double? GetNullableDouble(DataRow row, string fieldName)
        {
            return row.GetValue<double>(fieldName);
        }

        protected DateTime GetDateTime(DataRow row, string fieldName)
        {
            return row.GetValue<DateTime>(fieldName) ?? default(DateTime);
        }

        protected DateTime? GetNullableDateTime(DataRow row, string fieldName)
        {
            return row.GetValue<DateTime>(fieldName);
        }

        protected byte GetByte(DataRow row, string fieldName)
        {
            return (byte)row[fieldName];
        }

        protected byte? GetNullableByte(DataRow row, string fieldName)
        {
            return (byte?)row[fieldName];
        }

        protected byte[] GetByteArray(DataRow row, string fieldName)
        {
            return (byte[])row[fieldName];
        }

        protected DateTimeOffset GetDateTimeOffset(DataRow row, string fieldName)
        {
            return row.GetValue<DateTimeOffset>(fieldName) ?? default(DateTimeOffset);
        }

        protected DateTimeOffset? GetNullableDateTimeOffset(DataRow row, string fieldName)
        {
            return row.GetValue<DateTimeOffset>(fieldName);
        }

        protected Guid GetGuid(DataRow row, string fieldName)
        {
            return row.GetValue<Guid>(fieldName) ?? Guid.Empty;
        }

        protected Guid? GetNullableGuid(DataRow row, string fieldName)
        {
            return row.GetValue<Guid>(fieldName);
        }

        protected TimeSpan GetTimeSpan(DataRow row, string fieldName)
        {
            return row.GetValue<TimeSpan>(fieldName) ?? default(TimeSpan);
        }

        protected TimeSpan? GetNullableTimeSpan(DataRow row, string fieldName)
        {
            return row.GetValue<TimeSpan>(fieldName);
        }

        protected XmlDocument GetXmlDocument(DataRow row, string fieldName)
        {
            return new XmlDocument
            {
                InnerXml = row.Table.Columns.Contains(fieldName) ? row.GetText(fieldName) : ""
            };
        }

        protected string GetString(DataRow row, string fieldName)
        {
            return row.Table.Columns.Contains(fieldName) ? row.GetText(fieldName) : default(string);
        }

        #region [Private]

        private DataTable ToDataTable(SqlCommand cmd)
        {
            try
            {
                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                Logger(ex);
                return null;
            }
        }

        protected void CreateStagingTable(string tempTableName, bool onlyPrimaryKeys = false)
        {
            var stagingSqlBuilder = new StringBuilder();
            stagingSqlBuilder.AppendLine(@"CREATE TABLE " + tempTableName + " (");
            foreach (var mergeColumn in Columns.Where(x => onlyPrimaryKeys && x.PrimaryKey || !onlyPrimaryKeys))
            {
                stagingSqlBuilder.Append($"[{mergeColumn.ColumnName}] {mergeColumn.SqlDataTypeText} NULL");

                if (!mergeColumn.PrimaryKey)
                {
                    stagingSqlBuilder.AppendLine(",");
                    stagingSqlBuilder.Append($"[{mergeColumn.ColumnName}Changed] [BIT] NOT NULL");
                }
                stagingSqlBuilder.AppendLine(mergeColumn != Columns[Columns.Count - 1] ? "," : ")");
            }

            var stagingSql = stagingSqlBuilder.ToString();
            if (HasInjection(stagingSql))
                throw new Exception("Sql Injection attempted. Aborted");

            using (var cn = new SqlConnection(ConnectionString))
            {
                //Create staging table
                var cmd = new SqlCommand
                {
                    Connection = cn,
                    CommandType = CommandType.Text,
                    CommandText = stagingSql
                };
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
    
    public static class Ext
    {
        public static T? GetValue<T>(this DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName) || !row.Table.Columns.Contains(columnName))
                return null;
    
            return row[columnName] as T?;
        }
    
        public static string GetText(this DataRow row, string columnName)
        {
            if (row.IsNull(columnName) || !row.Table.Columns.Contains(columnName))
                return null;
    
            return row[columnName] as string;
        }
    }
}
