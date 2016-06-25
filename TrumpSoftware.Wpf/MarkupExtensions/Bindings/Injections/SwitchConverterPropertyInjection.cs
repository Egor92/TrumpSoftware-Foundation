using System.Collections.Generic;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections
{
    public class SwitchConverterPropertyInjection : ConverterPropertyInjectionBase<SwitchConverter>
    {
        public Binding DefaultBinding { get; set; }

        protected override IEnumerable<ValueInjector> GetInjectors(SwitchConverter converter)
        {
            var valueInjectors = new List<ValueInjector>();

            if (DefaultBinding != null)
            {
                var valueInjector = new ValueInjector(DefaultBinding, x => converter.Default = x);
                valueInjectors.Add(valueInjector);
            }

            return valueInjectors;
        }
    }
}