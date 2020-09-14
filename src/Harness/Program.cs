using RepoLite.Generator.DotNet;
using System;

namespace Harness
{
    class Program
    {
        static void Main(string[] args)
        {
            new CSharpSqlServerGenerator(null).FileExtension();
            Console.WriteLine("Hello World!");
        }
    }
}
