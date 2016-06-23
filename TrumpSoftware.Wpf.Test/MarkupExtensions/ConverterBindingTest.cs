using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveUI;
using TrumpSoftware.Wpf.MarkupExtensions.Bindings;

namespace TrumpSoftware.Wpf.Test.MarkupExtensions
{
    public class ViewModel : ReactiveObject
    {
        #region Property1

        private double _property1;

        public double Property1
        {
            get { return _property1; }
            set { this.RaiseAndSetIfChanged(ref _property1, value); }
        }

        #endregion

        #region Property2

        private double _property2;

        public double Property2
        {
            get { return _property2; }
            set { this.RaiseAndSetIfChanged(ref _property2, value); }
        }

        #endregion

        #region Property3

        private double _property3;

        public double Property3
        {
            get { return _property3; }
            set { this.RaiseAndSetIfChanged(ref _property3, value); }
        }

        #endregion

        #region Bool

        private bool _bool;

        public bool Bool
        {
            get { return _bool; }
            set { this.RaiseAndSetIfChanged(ref _bool, value); }
        }

        #endregion
    }

    [TestClass]
    public class ConverterBindingTest
    {
        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
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
            Assert.IsInstanceOfType(value, typeof(MultiBinding));
        }

        [TestMethod]
        public void ProvidedValueConverterIsOfTypeConverterBindingAdapter()
        {
            var converterBinding = new ConverterBinding()
            {
                Binding = new Binding()
            };
            var multiBinding = (MultiBinding)converterBinding.ProvideValue(new ServiceProviders());
            Assert.IsInstanceOfType(multiBinding.Converter, typeof(ConverterBindingAdapter));
        }
     
        [TestMethod]
        public void CanSimpleBind()
        {
            const string xaml = @"
<ContentControl>
    <ContentControl.DataContext>
        <local:ViewModel />
    </ContentControl.DataContext>
    <ContentControl>
        <ex:ConverterBinding Binding='{Binding Property1}' />
    </ContentControl>
</ContentControl>
";
            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", @"http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", @"http://schemas.microsoft.com/winfx/2006/xaml");
            parserContext.XmlnsDictionary.Add("ex", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("local", @"clr-namespace:TrumpSoftware.Wpf.Test.MarkupExtensions;assembly=TrumpSoftware.Wpf.Test");

            var parentControl = (ContentControl)XamlReader.Parse(xaml, parserContext);
            var viewModel = (ViewModel)parentControl.DataContext;
            var control = (ContentControl)parentControl.Content;

            const double value1 = 5.6;
            viewModel.Property1 = value1;

            Assert.AreEqual(value1, control.Content);
        }
     
        [TestMethod]
        public void CanMultipleBind()
        {
            const string xaml = @"
<ContentControl>
    <ContentControl.Resources>
        <converters:SwitchConverter x:Key='Converter'
                                    Default='{x:Static Visibility.Collapsed}'>
            <cases:EqualsCase />
        </converters:SwitchConverter>
    </ContentControl.Resources>
    <ContentControl.DataContext>
        <local:ViewModel />
    </ContentControl.DataContext>
    <ContentControl>
        <ex:ConverterBinding Binding='{Binding Property1}'
                             Converter='{StaticResource Converter}'>
            <injections:EqualsCasePropertyInjection Depth='0'
                                         CaseDepth='0'
                                         KeyBinding='{Binding Property2}'
                                         ValueBinding='{Binding Bool}' />
        </ex:ConverterBinding>
    </ContentControl>
</ContentControl>
";
            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", @"http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            parserContext.XmlnsDictionary.Add("x", @"http://schemas.microsoft.com/winfx/2006/xaml");
            parserContext.XmlnsDictionary.Add("ex", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("converters", @"clr-namespace:TrumpSoftware.Wpf.Converters;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("cases", @"clr-namespace:TrumpSoftware.Wpf.Converters.Cases;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("injections", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings.ConverterPropertyInjections;assembly=TrumpSoftware.Wpf");
            parserContext.XmlnsDictionary.Add("local", @"clr-namespace:TrumpSoftware.Wpf.Test.MarkupExtensions;assembly=TrumpSoftware.Wpf.Test");

            var parentControl = (ContentControl)XamlReader.Parse(xaml, parserContext);
            var viewModel = (ViewModel)parentControl.DataContext;
            var control = (ContentControl)parentControl.Content;

            viewModel.Property1 = 4.7;
            viewModel.Property2 = 4.7;
            viewModel.Bool = true;

            Assert.AreEqual(true, control.Content);
        }
    }
}
