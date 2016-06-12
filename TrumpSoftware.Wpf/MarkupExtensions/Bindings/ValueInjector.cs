using System;
using System.Windows.Data;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings
{
    public class ValueInjector
    {
        #region Fields

        private readonly Action<object> _inject;

        #endregion

        #region Ctor

        public ValueInjector(Binding binding, Action<object> inject)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            if (inject == null)
                throw new ArgumentNullException("inject");
            _inject = inject;
            Binding = binding;
        }

        #endregion

        public Binding Binding { get; private set; }

        public void Inject(object value)
        {
            _inject(value);
        }
    }
}
