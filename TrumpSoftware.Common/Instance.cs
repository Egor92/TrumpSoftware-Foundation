using System;
using System.Collections.Generic;

namespace TrumpSoftware.Common
{
    public class Instance<T>
    {
        #region Fields

        private readonly object _syncRoot = new object();

        #endregion

        #region Ctor

        public Instance()
        {
            
        }

        public Instance(T value)
        {
            SetValue(value);
        }

        #endregion

        #region Properties

        #region IsInitialized

        public bool IsInitialized { get; private set; }

        #endregion

        #region Value

        private T _value;

        public T Value
        {
            get
            {
                if (!IsInitialized)
                    throw new InvalidOperationException("Instance doesn't initialized");
                return _value;
            }
        }

        #endregion

        #endregion

        #region Overridden members

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((Instance<T>) obj);
        }

        protected bool Equals(Instance<T> other)
        {
            return IsInitialized.Equals(other.IsInitialized) && EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(_value)*397) ^ IsInitialized.GetHashCode();
            }
        }

        #endregion

        #region Operators

        public static explicit operator T(Instance<T> instance)
        {
            return instance.Value;
        }

        public static explicit operator Instance<T>(T value)
        {
            return new Instance<T>(value);
        }

        #endregion

        public void SetValue(T value)
        {
            lock (_syncRoot)
            {
                IsInitialized = true;
                _value = value;
            }
        }

        public void Reset()
        {
            IsInitialized = false;
        }
    }
}
