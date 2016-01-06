using System.Windows;
using System.Windows.Controls;

namespace TrumpSoftware.Wpf.Controls
{
    public class RowDefinitionExtended : RowDefinition
    {
        #region Ctor

        static RowDefinitionExtended()
        {
            HeightProperty.OverrideMetadata(typeof (RowDefinitionExtended),
                new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, CoerceWidth));
        }

        #endregion

        #region 

        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsIsVisible", typeof (bool), typeof (RowDefinitionExtended),
                                        new PropertyMetadata(true, OnIsVisibleChanged));

        public bool IsVisible
        {
            get { return (bool) GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        public static void SetIsVisible(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(DependencyObject obj)
        {
            return (bool) obj.GetValue(IsVisibleProperty);
        }

        private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(HeightProperty);
        }

        private static object CoerceWidth(DependencyObject obj, object newValue)
        {
            return (((RowDefinitionExtended)obj).IsVisible) ? newValue : new GridLength(0);
        }

        #endregion
    }
}
