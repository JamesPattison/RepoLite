using System;
using Microsoft.Extensions.DependencyInjection;

namespace RepoLite
{
    public class IOC
    {
        public static IServiceProvider CurrentProvider { get; internal set; }

        public static T Resolve<T>()
        {
            return CurrentProvider.GetService<T>();
        }
    }
}