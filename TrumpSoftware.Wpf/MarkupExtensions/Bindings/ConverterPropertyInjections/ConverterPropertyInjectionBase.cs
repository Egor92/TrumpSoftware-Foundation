using System.Collections.Generic;
using System.Windows.Data;
using TrumpSoftware.Wpf.Converters;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings.ConverterPropertyInjections
{
    public abstract class ConverterPropertyInjectionBase<TConverter> : IConverterPropertyInjection
        where TConverter : class, IChainConverter
    {
        public int Depth { get; set; }

        public IEnumerable<ValueInjector> GetInjectors(IValueConverter converter)
        {
            return GetInjectors(converter, 0);
        }

        protected virtual TConverter GetTargetConverter(IValueConverter converter)
        {
            return converter as TConverter;
        }

        protected abstract IEnumerable<ValueInjector> GetInjectors(TConverter converter);

        private IEnumerable<ValueInjector> GetInjectors(IValueConverter converter, int matchCount)
        {
            if (converter == null)
                return null;

            var targetConverter = GetTargetConverter(converter);
            if (targetConverter != null)
            {
                if (Depth == matchCount)
                {
                    return GetInjectors(targetConverter);
                }

                matchCount++;
            }

            var chainConverter = converter as IChainConverter;
            if (chainConverter == null)
                return null;

            var innerConverter = chainConverter.Converter;
            return GetInjectors(innerConverter, matchCount);
        }
    }
}
