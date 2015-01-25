using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common
{
    public interface ILoader<T>
    {
        Task<T> Load(Uri uri);
    }
}
