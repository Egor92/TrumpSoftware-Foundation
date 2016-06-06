using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TrumpSoftware.Wpf.MarkupExtensions
{
    public class ConverterBindingAdapter : IMultiValueConverter
    {
        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var converterParameter = ConverterParameter;
            if (values.Length > 1)
            {
                converterParameter = values[2];
            }

            var resultValue = values[0];
            if (Converter != null)
            {
                resultValue= Converter.Convert(values[0], targetType, converterParameter, culture);
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

    public class ConverterBinding : MarkupExtension
    {
        public Binding Binding { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public Binding ConverterParameterBinding { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; } = UpdateSourceTrigger.Default;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Binding == null)
                throw new NullReferenceException("Binding must be not null");

            var multiBinding = new MultiBinding
            {
                UpdateSourceTrigger = UpdateSourceTrigger
            };

            multiBinding.Bindings.Add(Binding);
            if (ConverterParameterBinding != null)
                multiBinding.Bindings.Add(ConverterParameterBinding);

            var adapter = new ConverterBindingAdapter
            {
                Converter = Converter,
                ConverterParameter = ConverterParameter
            };

            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }
    }
}