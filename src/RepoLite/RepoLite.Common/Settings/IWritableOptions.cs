using Microsoft.Extensions.Options;
using System;

namespace RepoLite.Common.Settings
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        void Update(Action<T> applyChanges);
    }
}
