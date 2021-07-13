namespace RepoLite.GeneratorEngine.Generators.CSharp
{
    public abstract class CSharpCodeGenerator : CodeGenerator
    {
        public override string FileExtension()
        {
            return "cs";
        }
    }
}