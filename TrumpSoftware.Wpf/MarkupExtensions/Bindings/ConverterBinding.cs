using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections;

namespace TrumpSoftware.Wpf.MarkupExtensions.Bindings
{
    public class ConverterBindingAdapter : IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public bool HasConverterParameterBinding { get; set; }
        public IList<ValueInjector> ValueInjectors { get; set; }

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(x => x == DependencyProperty.UnsetValue))
                return DependencyProperty.UnsetValue;

            int valueInjectorsIndex = 1;

            var converterParameter = ConverterParameter;
            if (HasConverterParameterBinding)
            {
                valueInjectorsIndex++;
                converterParameter = values[1];
            }

            for (int i = 0; i < ValueInjectors.Count; i++)
            {
                var valueInjector = ValueInjectors[i];
                var value = values[valueInjectorsIndex + i];
                valueInjector.Inject(value);
            }

            var resultValue = values[0];
            if (Converter != null)
            {
                resultValue = Converter.Convert(values[0], targetType, converterParameter, culture);
            }

            return resultValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
           return new[]
            {
                Converter.ConvertBack(value, targetTypes[0], null, culture)
            };
        }

        #endregion
    }

    [ContentProperty("PropertyInjections")]
    public class ConverterBinding : MarkupExtension
    {
        public Binding Binding { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public Binding ConverterParameterBinding { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.Default;
        public IList<IConverterPropertyInjection> PropertyInjections { get; private set; } = new List<IConverterPropertyInjection>();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding == null)
                throw new NullReferenceException("Binding must be not null");

            var multiBinding = new MultiBinding
            {
                UpdateSourceTrigger = UpdateSourceTrigger
            };

            bool hasConverterParameterBinding = false;
            multiBinding.Bindings.Add(Binding);
            if (ConverterParameterBinding != null)
            {
                hasConverterParameterBinding = true;
                multiBinding.Bindings.Add(ConverterParameterBinding);
            }

            IList<ValueInjector> valueInjectors = new List<ValueInjector>();
            if (Converter != null)
            {
                valueInjectors = PropertyInjections.SelectMany(x => x.GetInjectors(Converter)).ToList();
                foreach (var valueInjector in valueInjectors)
                {
                    multiBinding.Bindings.Add(valueInjector.Binding);
                }
            }

            var adapter = new ConverterBindingAdapter
            {
                Converter = Converter,
                ConverterParameter = ConverterParameter,
                HasConverterParameterBinding = hasConverterParameterBinding,
                ValueInjectors = valueInjectors,
            };

            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}