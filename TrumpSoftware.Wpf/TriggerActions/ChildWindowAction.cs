using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using Prism;
using Prism.Interactivity.InteractionRequest;

namespace TrumpSoftware.Wpf.TriggerActions
{
    [ContentProperty("WindowContent")]
    public class ChildWindowAction : Prism.Interactivity.PopupWindowAction
    {
        #region Properties

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (ChildWindowAction));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region WindowStartupLocation

        public static readonly DependencyProperty WindowStartupLocationProperty =
            DependencyProperty.Register("WindowStartupLocation", typeof (WindowStartupLocation), typeof (ChildWindowAction), new PropertyMetadata(WindowStartupLocation.CenterScreen));

        public WindowStartupLocation WindowStartupLocation
        {
            get { return (WindowStartupLocation) GetValue(WindowStartupLocationProperty); }
            set { SetValue(WindowStartupLocationProperty, value); }
        }

        #endregion

        #region Width

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof (double?), typeof (ChildWindowAction));

        public double? Width
        {
            get { return (double?) GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        #endregion

        #region Height

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof (double?), typeof (ChildWindowAction));

        public double? Height
        {
            get { return (double?) GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        #endregion

        #region MinWidth

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register("MinWidth", typeof (double), typeof (ChildWindowAction));

        public double MinWidth
        {
            get { return (double) GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
        }

        #endregion

        #region MinHeight

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register("MinHeight", typeof (double), typeof (ChildWindowAction));

        public double MinHeight
        {
            get { return (double) GetValue(MinHeightProperty); }
            set { SetValue(MinHeightProperty, value); }
        }

        #endregion

        #region SizeToContent

        public static readonly DependencyProperty SizeToContentProperty =
            DependencyProperty.Register("SizeToContent", typeof(SizeToContent), typeof(ChildWindowAction), new PropertyMetadata(SizeToContent.Manual));

        public SizeToContent SizeToContent
        {
            get { return (SizeToContent)GetValue(SizeToContentProperty); }
            set { SetValue(SizeToContentProperty, value); }
        }

        #endregion

        #region ResizeMode

        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ResizeMode), typeof(ChildWindowAction), new PropertyMetadata(ResizeMode.CanResize));

        public ResizeMode ResizeMode
        {
            get { return (ResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        #endregion

        #region WindowStyleType

        public static readonly DependencyProperty WindowStyleTypeProperty =
            DependencyProperty.Register("WindowStyleType", typeof(WindowStyle), typeof(ChildWindowAction), new PropertyMetadata(System.Windows.WindowStyle.SingleBorderWindow));

        public WindowStyle WindowStyleType
        {
            get { return (WindowStyle)GetValue(WindowStyleTypeProperty); }
            set { SetValue(WindowStyleTypeProperty, value); }
        }

        #endregion

        #region Icon

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(ChildWindowAction));

        public ImageSource Icon
        {
            get { return (ImageSource) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        #endregion

        #endregion

        #region Overridden members

        protected override Window GetWindow(INotification notification)
        {
            notification.Title = notification.Title ?? Title ?? string.Empty;
            var window = base.GetWindow(notification);
            window.WindowStartupLocation = WindowStartupLocation;
            window.SizeToContent = SizeToContent;
            window.ResizeMode = ResizeMode;
            window.MinWidth = MinWidth;
            window.MinHeight = MinHeight;
            window.WindowStyle = WindowStyleType;
            if (Width != null)
                window.Width = Width.Value;
            if (Height != null)
                window.Height = Height.Value;
            window.Icon = Icon;
            window.Closed += Window_Closed;
            return window;
        }

        protected override void PrepareContentForWindow(INotification notification, Window wrapperWindow)
        {
            WindowContent.DataContext = notification.Content;
            var activeAwareContent = notification.Content as IActiveAware;
            if (activeAwareContent != null)
                activeAwareContent.IsActive = true;

            base.PrepareContentForWindow(notification, wrapperWindow);
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window == null)
                return;

            window.Closed -= Window_Closed;

            var activeAwareContent = WindowContent.DataContext as IActiveAware;
            if (activeAwareContent != null)
                activeAwareContent.IsActive = false;
        }
    }
}
