using Microsoft.CSharp;

namespace RepoLite.Common
{
    public class Helpers
    {
        public static string Tab1 = "\t";
        public static string Tab2 = "\t\t";
        public static string Tab3 = "\t\t\t";
        public static string Tab4 = "\t\t\t\t";
        public static string Tab5 = "\t\t\t\t\t";
        public static string Tab6 = "\t\t\t\t\t\t";
        public static string Tab7 = "\t\t\t\t\t\t\t";
        public static string Tab8 = "\t\t\t\t\t\t\t\t";
        public static string Tab9 = "\t\t\t\t\t\t\t\t\t";

        public static bool IsCSharpNullable(string type)
        {
            return type != "byte[]" && type != "object" && type != "string" && type != "XmlDocument" &&
                   type != "Byte[]" && type != "Object" && type != "String";
        }

        public static bool ReservedWord(string value)
        {
            var prov = new CSharpCodeProvider();
            return !prov.IsValidIdentifier(value);

        }
    }
}
