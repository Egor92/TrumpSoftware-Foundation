using System.Collections.Generic;
using System.Windows.Data;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections
{
    public interface IConverterPropertyInjection
    {
        IEnumerable<ValueInjector> GetInjectors(IValueConverter converter);
    }
}
