using System;
using System.Threading.Tasks;

namespace TrumpSoftware.Common.Interfaces
{
    public interface ILoader<T>
    {
        Task<T> Load(Uri uri);
    }
}
