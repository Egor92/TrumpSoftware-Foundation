using System;
using System.Collections.Generic;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters.Cases;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections
{
    public class TypeCasePropertyInjection : CasePropertyInjectionBase<TypeCase>
    {
        public Binding TypeBinding { get; set; }
        public Binding IsStrictlyBinding { get; set; }
        public Binding ValueBinding { get; set; }

        protected override IEnumerable<ValueInjector> GetInjectors(TypeCase @case)
        {
            var valueInjectors = new List<ValueInjector>();

            if (TypeBinding != null)
            {
                var valueInjector = new ValueInjector(TypeBinding, x => @case.Type = (Type)x);
                valueInjectors.Add(valueInjector);
            }

            if (IsStrictlyBinding != null)
            {
                var valueInjector = new ValueInjector(IsStrictlyBinding, x => @case.IsStrictly = (bool)x);
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