using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters;
using TrumpSoftware.Wpf.Converters.Cases;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections
{
    public abstract class CasePropertyInjectionBase<TCase> : ConverterPropertyInjectionBase<IHaveCasesChainConverter>
        where TCase : class, ICase
    {
        public int CaseIndex { get; set; }

        protected override IHaveCasesChainConverter GetTargetConverter(IValueConverter converter)
        {
            var targetConverter = converter as IHaveCasesChainConverter;
            if (targetConverter == null)
                return null;

            var targetCase = targetConverter.Cases.Count(x => x is TCase) > CaseIndex;
            if (!targetCase)
                return null;

            return targetConverter;
        }

        protected override IEnumerable<ValueInjector> GetInjectors(IHaveCasesChainConverter converter)
        {
            var cases = converter.Cases;
            var @case = cases.Select(x => x as TCase)
                             .Where(x => x != null)
                             .Take(CaseIndex + 1)
                             .Last();
            return GetInjectors(@case);
        }

        protected abstract IEnumerable<ValueInjector> GetInjectors(TCase @case);
    }
}