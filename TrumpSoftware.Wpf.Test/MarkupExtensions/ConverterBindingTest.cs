using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Wpf.Converters;
using TrumpSoftware.Wpf.Converters.Cases;
using TrumpSoftware.Wpf.Interfaces;
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

        #region IsStrictly

        private bool _isStrictly;

        public bool IsStrictly
        {
            get { return _isStrictly; }
            set { this.RaiseAndSetIfChanged(ref _isStrictly, value); }
        }

        #endregion

        #region Type

        private Type _type;

        public Type Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }

        #endregion

        #region Default

        private object _default;

        public object Default
        {
            get { return _default; }
            set { this.RaiseAndSetIfChanged(ref _default, value); }
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

            viewModel.DirectValue = new object();

            Assert.AreEqual(viewModel.DirectValue, control.Content);
        }

        [TestMethod]
        public void CanUseLevelInConverterPropertyInjection()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
<converters:SwitchConverter>
    <converters:SwitchConverter.Converter>
        <converters:SwitchConverter>
            <converters:SwitchConverter.Converter>
                <converters:SwitchConverter>
                    <converters:SwitchConverter.Converter>
                        <converters:SwitchConverter />
                    </converters:SwitchConverter.Converter>
                </converters:SwitchConverter>
            </converters:SwitchConverter.Converter>
        </converters:SwitchConverter>
    </converters:SwitchConverter.Converter>
</converters:SwitchConverter>
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new SwitchConverterPropertyInjection()
                {
                    Level = 2,
                    DefaultBinding = new Binding("Default"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.Default = new object();

            var thirdConverter = ((IHaveConverter)((IHaveConverter)converter).Converter).Converter;

            AssertAreEqual(viewModel, thirdConverter, "Default");
        }

        [TestMethod]
        public void CanBindEqualsCaseProperties()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
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

            var @case = (EqualsCase) converter.Cases[0];

            AssertAreEqual(viewModel, @case, "Key");
            AssertAreEqual(viewModel, @case, "Value");
        }

        [TestMethod]
        public void CaseUseCaseIndexInPropertyInjection()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
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

            var @case = (EqualsCase) converter.Cases[1];

            AssertAreEqual(viewModel, @case, "Key");
            AssertAreEqual(viewModel, @case, "Value");
        }

        [TestMethod]
        public void CanBindRangeCaseProperties()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
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

            var @case = (RangeCase)converter.Cases[0];

            AssertAreEqual(viewModel, @case, "IsMaxStrictly");
            AssertAreEqual(viewModel, @case, "IsMinStrictly");
            AssertAreEqual(viewModel, @case, "Max");
            AssertAreEqual(viewModel, @case, "Min");
            AssertAreEqual(viewModel, @case, "Value");

            viewModel.IsMaxStrictly = false;
            viewModel.IsMinStrictly = false;

            AssertAreEqual(viewModel, @case, "IsMaxStrictly");
            AssertAreEqual(viewModel, @case, "IsMinStrictly");
        }

        [TestMethod]
        public void CanBindTypeCaseProperties()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
<converters:SwitchConverter>
    <cases:TypeCase />
</converters:SwitchConverter>
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new TypeCasePropertyInjection()
                {
                    TypeBinding = new Binding("Type"),
                    IsStrictlyBinding = new Binding("IsStrictly"),
                    ValueBinding = new Binding("Value"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.Type = typeof(ViewModel);
            viewModel.IsStrictly = true;
            viewModel.Value = new object();

            var @case = (TypeCase)converter.Cases[0];

            AssertAreEqual(viewModel, @case, "Type");
            AssertAreEqual(viewModel, @case, "IsStrictly");
            AssertAreEqual(viewModel, @case, "Value");

            viewModel.IsStrictly = false;

            AssertAreEqual(viewModel, @case, "IsStrictly");
        }

        [TestMethod]
        public void CanBindSwitchConverterProperties()
        {
            var converter = XamlParser.Parse<SwitchConverter>(@"
<converters:SwitchConverter />
");

            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding("DirectValue"),
                Converter = converter
            };
            converterBinding.PropertyInjections.AddRange(new IConverterPropertyInjection[]
            {
                new SwitchConverterPropertyInjection()
                {
                    DefaultBinding = new Binding("Default"),
                }
            });

            var control = GetControl(converterBinding);
            var viewModel = (ViewModel) control.DataContext;

            viewModel.Default = new object();

            AssertAreEqual(viewModel, converter, "Default");
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

        private void AssertAreEqual(object source, object target, string propertyName)
        {
            var expected = GetPropertyValue(source, propertyName);
            var actual = GetPropertyValue(target, propertyName);
            Assert.AreEqual(expected, actual, propertyName);
        }

        private object GetPropertyValue(object source, string propertyName)
        {
            var type = source.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            return propertyInfo.GetValue(source, null);
        }
    }
}