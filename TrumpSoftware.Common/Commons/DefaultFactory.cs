using System;

namespace TrumpSoftware.Common.Commons
{
    public interface IDefaultFactory<out T>
    {
        T Create();
    }

    public class DefaultFactory<T> : IDefaultFactory<T>
    {
        #region Fields

        private readonly Func<T> _factoryMethod;

        #endregion

        #region Ctor

        public DefaultFactory(Func<T> factoryMethod)
        {
            if (factoryMethod == null)
                throw new ArgumentNullException("factoryMethod");
            _factoryMethod = factoryMethod;
        }

        #endregion

        #region Implementation of IDefaultFactory<T>

        public T Create()
        {
            return _factoryMethod();
        }

        #endregion
    }
}
