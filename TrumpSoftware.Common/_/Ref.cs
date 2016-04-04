using System;

namespace TrumpSoftware.Common
{
    public sealed class Ref<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        public T Value
        {
            get { return _getter(); } 
            set { _setter(value); }
        }

        public Ref(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }
    }
}