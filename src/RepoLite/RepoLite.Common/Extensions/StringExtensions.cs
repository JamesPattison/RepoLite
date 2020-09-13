using RepoLite.Common.Models;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RepoLite.Common.Extensions
{
    public static class StringExtensions
    {
        public static string CamelCase(this string value)
        {
            var knownAbbreviations = new[] { "IP", "USB", "DNS", "ID" };
            var sb = new StringBuilder();
            if (value.Contains("_"))
            {
                var segments = value.Split('_');
                if (segments[0].Length == 1 || knownAbbreviations.Contains(segments[0])) //Assume this is an abbreviation (I_P_ADDRESS)
                    sb.Append(segments[0]);
                else
                    sb.Append(segments[0].ToLower());

                foreach (var val in segments.Skip(1).Where(x => x.Length > 0))
                {
                    if (knownAbbreviations.Contains(val))
                    {
                        sb.Append(val);
                    }
                    else
                    {
                        sb.Append(val[0].ToString().ToUpper());
                        if (val.Length > 1)
                            sb.Append(val.Substring(1).ToLower());
                    }
                }
            }
            else
            {
                if (value.Length > 0)
                    sb.Append(value.ToLower());
            }

            return sb.ToString();
        }

        public static TableAndSchema GetTableAndSchema(this string tableName)
        {
            if (!tableName.Contains("."))
                return new TableAndSchema("dbo", tableName);

            var split = tableName.Split('.');
            var schema = split[0];
            if (string.IsNullOrEmpty(schema))
                schema = "dbo";
            var table = split[1];
            return new TableAndSchema(schema, table);
        }

        public static string LowerFirst(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            var str = input[0].ToString().ToLower();
            if (input.Length > 1)
                str += input.Substring(1);

            return str;
        }

        public static string ToModelName(this string input, string classNameFormat)
        {
            var name = Regex.Replace(
                classNameFormat,
                Regex.Escape("{Name}"),
                input,
                RegexOptions.IgnoreCase
            );

            if (Helpers.ReservedWord(name))
                name = "@" + name;

            return name;
        }

        public static string ToRepositoryName(this string input, string repositoryNameFormat)
        {
            var name = Regex.Replace(
                repositoryNameFormat,
                Regex.Escape("{Name}"),
                input,
                RegexOptions.IgnoreCase
            );

            if (Helpers.ReservedWord(name))
                name = "@" + name;

            return name;
        }
    }
}
