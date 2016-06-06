using System;
using System.Windows.Data;
using System.Windows.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrumpSoftware.Wpf.MarkupExtensions;

namespace TrumpSoftware.Wpf.Test.MarkupExtensions
{
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
    }
}
