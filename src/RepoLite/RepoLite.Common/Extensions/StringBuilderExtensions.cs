using System;
using System.Text;

namespace RepoLite.Common.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void Append(this StringBuilder builder, string text, params string[] additional)
        {
            builder.Append(text);

            foreach (var s in additional)
            {
                builder.Append(s);
            }
        }
        public static void AppendLine(this StringBuilder builder, string text, params string[] additional)
        {
            builder.Append(text);

            foreach (var s in additional)
            {
                builder.Append(s);
            }
            builder.Append(Environment.NewLine);
        }
    }
}
