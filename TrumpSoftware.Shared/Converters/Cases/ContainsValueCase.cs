using System.Collections.Generic;
using System.Linq;

#if WPF
using System.Windows.Markup;
#elif WINRT
using Windows.UI.Xaml.Markup;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT
namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
#if WPF
    [ContentProperty("Keys")]
#elif WINRT
    [ContentProperty(Name = "Keys")]
#endif
    public class ContainsValueCase : ICase
    {
        public List<object> Keys { get; set; }

        public object Value { get; set; }

        #region Ctor

        public ContainsValueCase()
        {
            Keys = new List<object>();
        }

        #endregion

        public bool IsMatched(object value)
        {
            return Keys.Any(key => ObjectHelper.Equals(key, value));
        }
    }
}
