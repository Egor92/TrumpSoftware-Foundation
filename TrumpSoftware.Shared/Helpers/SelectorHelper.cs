#if WPF
using System.Windows;
using System.Windows.Controls.Primitives;
#elif WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Helpers
#elif WINRT
namespace TrumpSoftware.WinRT.Helpers
#endif
{
    public static class SelectorHelper
    {
        public static readonly DependencyProperty IsNullItemSelectableProperty =
            DependencyProperty.RegisterAttached("IsNullItemSelectable", typeof (bool), typeof (SelectorHelper), new PropertyMetadata(false, OnIsNullItemSelectableChanged));

        public static void SetIsNullItemSelectable(DependencyObject element, bool value)
        {
            element.SetValue(IsNullItemSelectableProperty, value);
        }

        public static bool GetIsNullItemSelectable(DependencyObject element)
        {
            return (bool)element.GetValue(IsNullItemSelectableProperty);
        }

        private static void OnIsNullItemSelectableChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = sender as Selector;
            if (selector == null)
                return;

            if ((bool) e.NewValue)
            {
                SubscribeToPressing(selector);
            }
            else
            {
                UnsubscribeFromPressing(selector);
            }
        }

        private static void OnPressing(object sender, RoutedEventArgs e)
        {
            var selector = sender as Selector;
            if (selector == null)
                return;

            var originalSource = e.OriginalSource as DependencyObject;
            if (originalSource == null)
                return;

            var itemContainer = GetItemContainer(selector, originalSource);
            if (itemContainer == null)
                return;

            var item = GetItemFromConteiner(selector, itemContainer);
            if (item != null)
                return;

            selector.SelectedItem = null;
        }

        private static void SubscribeToPressing(Selector selector)
        {
#if WPF
            selector.PreviewMouseDown += OnPressing;
#elif WINRT
            selector.PointerPressed += OnPressing;
#endif
        }

        private static void UnsubscribeFromPressing(Selector selector)
        {
#if WPF
            selector.PreviewMouseDown -= OnPressing;
#elif WINRT
            selector.PointerPressed -= OnPressing;
#endif
        }

        private static DependencyObject GetItemContainer(Selector selector, DependencyObject originalSource)
        {
#if WPF
            return selector.ContainerFromElement(originalSource);
#elif WINRT
            var itemContainers = selector.Items;
            if (itemContainers == null)
                return null;

            return originalSource.GetAncestor<UIElement>(x => itemContainers.Contains(x), true);
#endif
        }

        private static object GetItemFromConteiner(Selector selector, DependencyObject itemContainer)
        {
#if WPF
            var itemContainerGenerator = selector.ItemContainerGenerator;
            if (itemContainerGenerator == null)
                return null;

            return itemContainerGenerator.ItemFromContainer(itemContainer);
#elif WINRT
            return selector.ItemFromContainer(itemContainer);
#endif
        }
    }
}