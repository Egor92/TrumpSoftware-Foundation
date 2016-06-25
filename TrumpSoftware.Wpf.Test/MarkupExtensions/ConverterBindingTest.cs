using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Wpf.Converters;
using TrumpSoftware.Wpf.Converters.Cases;
using TrumpSoftware.Wpf.MarkupExtensions.Bindings;
using TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections;

namespace TrumpSoftware.Wpf.Test.MarkupExtensions
{
    public class ViewModel : ReactiveObject
    {
        #region DirectValue

        private object _directValue;

        public object DirectValue
        {
            get { return _directValue; }
            set { this.RaiseAndSetIfChanged(ref _directValue, value); }
        }

        #endregion

        #region Key

        private object _key;

        public object Key
        {
            get { return _key; }
            set { this.RaiseAndSetIfChanged(ref _key, value); }
        }

        #endregion

        #region Value

        private object _value;

        public object Value
        {
            get { return _value; }
            set { this.RaiseAndSetIfChanged(ref _value, value); }
        }

        #endregion

        #region IsMaxStrictly

        private bool _isMaxStrictly;

        public bool IsMaxStrictly
        {
            get { return _isMaxStrictly; }
            set { this.RaiseAndSetIfChanged(ref _isMaxStrictly, value); }
        }

        #endregion

        #region IsMinStrictly

        private bool _isMinStrictly;

        public bool IsMinStrictly
        {
            get { return _isMinStrictly; }
            set { this.RaiseAndSetIfChanged(ref _isMinStrictly, value); }
        }

        #endregion

        #region Max

        private double _max;

        public double Max
        {
            get { return _max; }
            set { this.RaiseAndSetIfChanged(ref _max, value); }
        }

        #endregion

        #region Min

        private double _min;

        public double Min
        {
            get { return _min; }
            set { this.RaiseAndSetIfChanged(ref _min, value); }
        }

        #endregion
    }

    [TestClass]
    public class ConverterBindingTest
    {
        [TestMethod]
        [ExpectedException(typeof (NullReferenceException))]
        public void IfDoNotSetBinding_ThrowException()
        {
            var converterBinding = new ConverterBinding();
            converterBinding.ProvideValue(new ServiceProviders());
        }

        [TestMethod]
        public void ProvideValueReturns_MultiBinding()
        {
            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding()
            };
            var value = converterBinding.ProvideValue(new ServiceProviders());
            Assert.IsInstanceOfType(value, typeof (MultiBinding));
        }

        [TestMethod]
        public void ProvidedValueConverterIsOfTypeConverterBindingAdapter()
        {
            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding()
            };
            var multiBinding = (MultiBinding) converterBinding.ProvideValue(new ServiceProviders());
            Assert.IsInstanceOfType(multiBinding.Converter, typeof (ConverterBindingAdapter));
        }

        [TestMethod]
        public void CanSimpleBind()
        {
            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
            };
            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            const double directValue = 5.6;
            viewModel.DirectValue = directValue;

            Assert.AreEqual(directValue, control.Content);
        }

        [TestMethod]
        public void CanBindEqualsCaseProperties()
        {
            var converter = Parse<SwitchConverter>(@"
<converters:SwitchConverter>
    <cases:EqualsCase />
</converters:SwitchConverter>
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new EqualsCasePropertyInjection()
                {
                    KeyBinding = new Binding("Key"),
                    ValueBinding = new Binding("Value"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.Key = new object();
            viewModel.Value = new object();

            Assert.AreEqual(viewModel.Key, ((EqualsCase) converter.Cases[0]).Key, "key");
            Assert.AreEqual(viewModel.Value, converter.Cases[0].Value, "value");
        }

        [TestMethod]
        public void CanBindToSecondCaseProperty()
        {
            var converter = Parse<SwitchConverter>(@"
<converters:SwitchConverter>
    <cases:EqualsCase />
    <cases:EqualsCase>
        <cases:EqualsCase.Value>
            <system:Object />
        </cases:EqualsCase.Value>
    </cases:EqualsCase>
</converters:SwitchConverter>
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new EqualsCasePropertyInjection
                {
                    CaseIndex = 1,
                    KeyBinding = new Binding("Key"),
                    ValueBinding = new Binding("Value"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.Key = new object();
            viewModel.Value = new object();

            Assert.AreEqual(viewModel.Key, ((EqualsCase) converter.Cases[1]).Key, "key");
            Assert.AreEqual(viewModel.Value, converter.Cases[1].Value, "value");
        }

        [TestMethod]
        public void CanBindRangeCaseProperties()
        {
            var converter = Parse<SwitchConverter>(@"
<converters:SwitchConverter>
    <cases:RangeCase />
</converters:SwitchConverter>
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new RangeCasePropertyInjection()
                {
                    IsMaxStrictlyBinding = new Binding("IsMaxStrictly"),
                    IsMinStrictlyBinding = new Binding("IsMinStrictly"),
                    MaxBinding = new Binding("Max"),
                    MinBinding = new Binding("Min"),
                    ValueBinding = new Binding("Value"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.IsMaxStrictly = true;
            viewModel.IsMinStrictly = true;
            viewModel.Max = 4.3;
            viewModel.Min = 6.2;
            viewModel.Value = new object();

            Assert.AreEqual(viewModel.IsMaxStrictly, ((RangeCase)converter.Cases[0]).IsMaxStrictly, "IsMaxStrictly (0)");
            Assert.AreEqual(viewModel.IsMinStrictly, ((RangeCase)converter.Cases[0]).IsMinStrictly, "IsMinStrictly (0)");
            Assert.AreEqual(viewModel.Max, ((RangeCase)converter.Cases[0]).Max, "Max");
            Assert.AreEqual(viewModel.Min, ((RangeCase)converter.Cases[0]).Min, "Min");
            Assert.AreEqual(viewModel.Value, converter.Cases[0].Value, "value");

            viewModel.IsMaxStrictly = false;
            viewModel.IsMinStrictly = false;

            Assert.AreEqual(viewModel.IsMaxStrictly, ((RangeCase)converter.Cases[0]).IsMaxStrictly, "IsMaxStrictly (1)");
            Assert.AreEqual(viewModel.IsMinStrictly, ((RangeCase)converter.Cases[0]).IsMinStrictly, "IsMinStrictly (1)");
        }

        private static ContentControl GetControl(ConverterBinding converterBinding)
        {
            var contentControl = new ContentControl();
            var viewModel = new ViewModel();
            contentControl.DataContext = viewModel;

            var multiBinding = (MultiBinding) converterBinding.ProvideValue(new ServiceProviders());
            BindingOperations.SetBinding(contentControl, ContentControl.ContentProperty, multiBinding);

            return contentControl;
        }

        private static T Parse<T>(string xaml)
        {
            var parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", @"http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", @"http://schemas.microsoft.com/winfx/2006/xaml");
            parserContext.XmlnsDictionary.Add("system", @"clr-namespace:System;assembly=mscorlib");
            parserContext.XmlnsDictionary.Add("ex", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("converters", @"clr-namespace:TrumpSoftware.Wpf.Converters;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("cases", @"clr-namespace:TrumpSoftware.Wpf.Converters.Cases;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("injections", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("local", @"clr-namespace:TrumpSoftware.Wpf.Test.MarkupExtensions;assembly=TrumpSoftware.Wpf.Test");
            parserContext.XmlnsDictionary.Add("wpf", @"clr-namespace:TrumpSoftware.Wpf;assembly=TrumpSoftware.Wpf");

            return (T) XamlReader.Parse(xaml, parserContext);
        }
    }
}