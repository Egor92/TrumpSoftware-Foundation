using System.Collections.Generic;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters.Cases;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections
{
    public class RangeCasePropertyInjection : CasePropertyInjectionBase<RangeCase>
    {
        public Binding IsMaxStrictlyBinding { get; set; }
        public Binding IsMinStrictlyBinding { get; set; }
        public Binding MaxBinding { get; set; }
        public Binding MinBinding { get; set; }
        public Binding ValueBinding { get; set; }

        protected override IEnumerable<ValueInjector> GetInjectors(RangeCase @case)
        {
            var valueInjectors = new List<ValueInjector>();

            if (IsMaxStrictlyBinding != null)
            {
                var valueInjector = new ValueInjector(IsMaxStrictlyBinding, x => @case.IsMaxStrictly = (bool)x);
                valueInjectors.Add(valueInjector);
            }

            if (IsMinStrictlyBinding != null)
            {
                var valueInjector = new ValueInjector(IsMinStrictlyBinding, x => @case.IsMinStrictly = (bool)x);
                valueInjectors.Add(valueInjector);
            }

            if (MaxBinding != null)
            {
                var valueInjector = new ValueInjector(MaxBinding, x => @case.Max = (double)x);
                valueInjectors.Add(valueInjector);
            }

            if (MinBinding != null)
            {
                var valueInjector = new ValueInjector(MinBinding, x => @case.Min = (double)x);
                valueInjectors.Add(valueInjector);
            }

            if (ValueBinding != null)
            {
                var valueInjector = new ValueInjector(ValueBinding, x => @case.Value = x);
                valueInjectors.Add(valueInjector);
            }

            return valueInjectors;
        }
    }
}