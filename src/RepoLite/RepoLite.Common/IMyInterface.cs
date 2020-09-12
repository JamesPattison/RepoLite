namespace RepoLiteCore.Common
{
    public delegate IMyInterface MyInterfaceResolver(int myVal);
    public interface IMyInterface
    {
        string Hello();
    }
}
