using System.Windows;
using System.Windows.Markup;

namespace TrumpSoftware.Wpf.Test
{
    internal static class XamlParser
    {
        private static readonly ParserContext ParserContext;

        static XamlParser()
        {
            ParserContext = new ParserContext();
            ParserContext.XmlnsDictionary.Add("", @"http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            ParserContext.XmlnsDictionary.Add("x", @"http://schemas.microsoft.com/winfx/2006/xaml");
            ParserContext.XmlnsDictionary.Add("system", @"clr-namespace:System;assembly=mscorlib");
            ParserContext.XmlnsDictionary.Add("ex", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings;assembly=TrumpSoftware.Wpf");
            ParserContext.XmlnsDictionary.Add("converters", @"clr-namespace:TrumpSoftware.Wpf.Converters;assembly=TrumpSoftware.Wpf");
            ParserContext.XmlnsDictionary.Add("cases", @"clr-namespace:TrumpSoftware.Wpf.Converters.Cases;assembly=TrumpSoftware.Wpf");
            ParserContext.XmlnsDictionary.Add("injections", @"clr-namespace:TrumpSoftware.Wpf.MarkupExtensions.Bindings.Injections;assembly=TrumpSoftware.Wpf");
            ParserContext.XmlnsDictionary.Add("local", @"clr-namespace:TrumpSoftware.Wpf.Test.MarkupExtensions;assembly=TrumpSoftware.Wpf.Test");
            ParserContext.XmlnsDictionary.Add("wpf", @"clr-namespace:TrumpSoftware.Wpf;assembly=TrumpSoftware.Wpf");
        }

        internal static T Parse<T>(string xaml, bool toLoad = true)
        {
            var element = (T)XamlReader.Parse(xaml, ParserContext);
            if (toLoad)
            {
                LoadElement(element);
            }
            return element;
        }

        private static void LoadElement<T>(T element)
        {
            var window = new Window
            {
                Content = element,
                Visibility = Visibility.Collapsed,
            };
            window.Show();
            window.Hide();
            window.Content = null;
        }
    }
}
