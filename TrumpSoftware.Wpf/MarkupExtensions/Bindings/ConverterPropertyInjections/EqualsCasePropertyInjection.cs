using System.Collections.Generic;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters.Cases;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.ConverterPropertyInjections
{
    public class EqualsCasePropertyInjection : CasePropertyInjectionBase<EqualsCase>
    {
        public Binding KeyBinding { get; set; }

        public Binding ValueBinding { get; set; }

        protected override IEnumerable<ValueInjector> GetInjectors(EqualsCase @case)
        {
            var valueInjectors = new List<ValueInjector>();

            if (KeyBinding != null)
            {
                var valueInjector = new ValueInjector(KeyBinding, x => @case.Key = x);
                valueInjectors.Add(valueInjector);
            }

            if (ValueBinding!= null)
            {
                var valueInjector = new ValueInjector(ValueBinding, x => @case.Value = x);
                valueInjectors.Add(valueInjector);
            }

            return valueInjectors;
        }
    }
}