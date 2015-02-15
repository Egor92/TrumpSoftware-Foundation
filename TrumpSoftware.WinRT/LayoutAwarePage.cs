using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TrumpSoftware.Common;

namespace TrumpSoftware.WinRT
{
    public abstract class LayoutAwarePage : Page
    {
        private bool _isViewContainerSearchCompleted;
        private ContentControl _viewContainer;
        private UIElement _landscapeView;
        private UIElement _portraitView;

        #region CurrentOrientation

        public ApplicationViewOrientation CurrentOrientation { get; private set; }

        #endregion

        #region DesignTimeOrientation

        private ApplicationViewOrientation _designTimeOrientation;

        public ApplicationViewOrientation DesignTimeOrientation
        {
            get { return _designTimeOrientation; }
            set
            {
                if (_designTimeOrientation == value)
                    return;
                _designTimeOrientation = value;
                SetActualOrientation();
            }
        }

        #endregion

        #region ActualView

        public UIElement ActualView { get; private set; }

        #endregion

        #region LandscapeViewTemplate

        public static readonly DependencyProperty LandscapeViewTemplateProperty =
            DependencyProperty.Register("LandscapeViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate LandscapeViewTemplate
        {
            get { return (DataTemplate)GetValue(LandscapeViewTemplateProperty); }
            set { SetValue(LandscapeViewTemplateProperty, value); }
        }

        #endregion

        #region PortraitViewTemplate

        public static readonly DependencyProperty PortraitViewTemplateProperty =
            DependencyProperty.Register("PortraitViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate PortraitViewTemplate
        {
            get { return (DataTemplate)GetValue(PortraitViewTemplateProperty); }
            set { SetValue(PortraitViewTemplateProperty, value); }
        }

        #endregion

        protected LayoutAwarePage()
        {
            Loaded += OnLoaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                SizeChanged += OnSizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                SizeChanged -= OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            SetActualOrientation();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetActualOrientation();
        }

        private void SetActualOrientation()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                CurrentOrientation = DesignTimeOrientation;
            }
            else
            {
                CurrentOrientation = ActualWidth > ActualHeight
                    ? ApplicationViewOrientation.Landscape
                    : ApplicationViewOrientation.Portrait;
            }
            switch (CurrentOrientation)
            {
                case ApplicationViewOrientation.Landscape:
                    SetView(ref _landscapeView, LandscapeViewTemplate);
                    break;
                case ApplicationViewOrientation.Portrait:
                    SetView(ref _portraitView, PortraitViewTemplate);
                    break;
                default:
                    throw new UnhandledCaseException(typeof(ApplicationViewOrientation), CurrentOrientation);
            }
        }

        private void SetView(ref UIElement view, DataTemplate viewTemplate)
        {
            if (view == null && viewTemplate != null)
                view = viewTemplate.LoadContent() as UIElement;
            SetView(view);
        }

        private void SetView(UIElement view)
        {
            if (!_isViewContainerSearchCompleted)
            {
                _viewContainer = FindName("ViewContainer") as ContentControl;
                _isViewContainerSearchCompleted = true;
            }
            if (_viewContainer != null)
                _viewContainer.Content = view;
            else
                Content = view;
            ActualView = view;
        }
    }
}
