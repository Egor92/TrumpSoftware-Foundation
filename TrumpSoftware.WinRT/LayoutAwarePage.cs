using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
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
        private bool _isLoaded;
        private ContentControl _viewContainer;
        private UIElement _landscapeView;
        private UIElement _portraitView;
        private UIElement _largeFilledView;
        private UIElement _smallFilledView;
        private UIElement _snapped500View;
        private UIElement _snapped320View;

        #region LandscapeOrientationRedirect

        public static readonly DependencyProperty LandscapeOrientationRedirectProperty = 
            DependencyProperty.Register("LandscapeOrientationRedirect", typeof (WindowOrientation), typeof (LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.Landscape, RedisplayView));

        public WindowOrientation LandscapeOrientationRedirect
        {
            get { return (WindowOrientation) GetValue(LandscapeOrientationRedirectProperty); }
            set { SetValue(LandscapeOrientationRedirectProperty, value); }
        }

        #endregion

        #region PortraitOrientationRedirect

        public static readonly DependencyProperty PortraitOrientationRedirectProperty = 
            DependencyProperty.Register("PortraitOrientationRedirect", typeof (WindowOrientation), typeof (LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.Landscape, RedisplayView));

        public WindowOrientation PortraitOrientationRedirect
        {
            get { return (WindowOrientation) GetValue(PortraitOrientationRedirectProperty); }
            set { SetValue(PortraitOrientationRedirectProperty, value); }
        }

        #endregion

        #region LargeFilledOrientationRedirect

        public static readonly DependencyProperty LargeFilledOrientationRedirectProperty =
            DependencyProperty.Register("LargeFilledOrientationRedirect", typeof(WindowOrientation), typeof(LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.Portrait, RedisplayView));

        public WindowOrientation LargeFilledOrientationRedirect
        {
            get { return (WindowOrientation) GetValue(LargeFilledOrientationRedirectProperty); }
            set { SetValue(LargeFilledOrientationRedirectProperty, value); }
        }

        #endregion

        #region SmallFilledOrientationRedirect

        public static readonly DependencyProperty SmallFilledOrientationRedirectProperty =
            DependencyProperty.Register("SmallFilledOrientationRedirect", typeof(WindowOrientation), typeof(LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.LargeFilled, RedisplayView));

        public WindowOrientation SmallFilledOrientationRedirect
        {
            get { return (WindowOrientation)GetValue(SmallFilledOrientationRedirectProperty); }
            set { SetValue(SmallFilledOrientationRedirectProperty, value); }
        }

        #endregion

        #region Snapped500OrientationRedirect

        public static readonly DependencyProperty Snapped500OrientationRedirectProperty = 
            DependencyProperty.Register("Snapped500OrientationRedirect", typeof (WindowOrientation), typeof (LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.SmallFilled, RedisplayView));

        public WindowOrientation Snapped500OrientationRedirect
        {
            get { return (WindowOrientation) GetValue(Snapped500OrientationRedirectProperty); }
            set { SetValue(Snapped500OrientationRedirectProperty, value); }
        }

        #endregion

        #region Snapped320OrientationRedirect

        public static readonly DependencyProperty Snapped320OrientationRedirectProperty =
            DependencyProperty.Register("Snapped320OrientationRedirect", typeof(WindowOrientation), typeof(LayoutAwarePage),
                new PropertyMetadata(WindowOrientation.Snapped500, RedisplayView));

        public WindowOrientation Snapped320OrientationRedirect
        {
            get { return (WindowOrientation)GetValue(Snapped320OrientationRedirectProperty); }
            set { SetValue(Snapped320OrientationRedirectProperty, value); }
        }

        #endregion

        #region CurrentOrientation

        private WindowOrientation _currentOrientation;

        public WindowOrientation CurrentOrientation
        {
            get { return _currentOrientation; }
            private set
            {
                if (_currentOrientation == value)
                    return;
                _currentOrientation = value;
                VisualStateManager.GoToState(this, value.ToString(), true);
                RaiseCurrentOrientationChanged();
            }
        }

        public event EventHandler CurrentOrientationChanged;

        private void RaiseCurrentOrientationChanged()
        {
            var handler = CurrentOrientationChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region DesignTimeOrientation

        private WindowOrientation _designTimeOrientation;

        public WindowOrientation DesignTimeOrientation
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

        #region SmallFilledProportion

        public static readonly DependencyProperty SmallFilledProportionProperty =
            DependencyProperty.Register("SmallFilledProportion", typeof(FractionalOne), typeof(LayoutAwarePage),
                new PropertyMetadata(new FractionalOne(0.55), RedisplayView));

        public double SmallFilledProportion
        {
            get { return (FractionalOne)GetValue(SmallFilledProportionProperty); }
            set { SetValue(SmallFilledProportionProperty, value); }
        }

        #endregion

        #region LargeFilledMinWidth

        public static readonly DependencyProperty LargeFilledMinWidthProperty =
            DependencyProperty.Register("LargeFilledMinWidth", typeof(double), typeof(LayoutAwarePage),
                new PropertyMetadata(750.0, RedisplayView));

        public double LargeFilledMinWidth
        {
            get { return (double)GetValue(LargeFilledMinWidthProperty); }
            set { SetValue(LargeFilledMinWidthProperty, value); }
        }

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

        #region LargeFilledViewTemplate

        public static readonly DependencyProperty LargeFilledViewTemplateProperty =
            DependencyProperty.Register("LargeFilledViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate LargeFilledViewTemplate
        {
            get { return (DataTemplate)GetValue(LargeFilledViewTemplateProperty); }
            set { SetValue(LargeFilledViewTemplateProperty, value); }
        }

        #endregion

        #region SmallFilledViewTemplate

        public static readonly DependencyProperty SmallFilledViewTemplateProperty =
            DependencyProperty.Register("SmallFilledViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate SmallFilledViewTemplate
        {
            get { return (DataTemplate)GetValue(SmallFilledViewTemplateProperty); }
            set { SetValue(SmallFilledViewTemplateProperty, value); }
        }

        #endregion

        #region Snapped500ViewTemplate

        public static readonly DependencyProperty Snapped500ViewTemplateProperty =
            DependencyProperty.Register("Snapped500ViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate Snapped500ViewTemplate
        {
            get { return (DataTemplate)GetValue(Snapped500ViewTemplateProperty); }
            set { SetValue(Snapped500ViewTemplateProperty, value); }
        }

        #endregion

        #region Snapped320ViewTemplate

        public static readonly DependencyProperty Snapped320ViewTemplateProperty =
            DependencyProperty.Register("Snapped320ViewTemplate", typeof(DataTemplate), typeof(LayoutAwarePage),
                new PropertyMetadata(null));

        public DataTemplate Snapped320ViewTemplate
        {
            get { return (DataTemplate)GetValue(Snapped320ViewTemplateProperty); }
            set { SetValue(Snapped320ViewTemplateProperty, value); }
        }

        #endregion

        protected LayoutAwarePage()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (!DesignMode.DesignModeEnabled)
                SizeChanged += OnSizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (!DesignMode.DesignModeEnabled)
                SizeChanged -= OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            _isLoaded = true;
            SetActualOrientation();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;
            _isLoaded = false;
        }

        private static void RedisplayView(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var layoutAwarePage = (LayoutAwarePage)sender;
            layoutAwarePage.SetActualOrientation();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetActualOrientation();
        }

        private void SetActualOrientation()
        {
            if (!_isLoaded)
                return;
            if (DesignMode.DesignModeEnabled)
            {
                CurrentOrientation = DesignTimeOrientation;
            }
            else
            {
                double proportion = ActualWidth / ActualHeight;
                if (ApplicationView.GetForCurrentView().IsFullScreen)
                {
                    CurrentOrientation = proportion >= 1
                        ? WindowOrientation.Landscape
                        : WindowOrientation.Portrait;
                }
                else
                {
                    if (ActualWidth <= 320)
                        CurrentOrientation = WindowOrientation.Snapped320;
                    else if (ActualWidth <= 500)
                        CurrentOrientation = WindowOrientation.Snapped500;
                    else if (ActualWidth <= LargeFilledMinWidth || proportion <= SmallFilledProportion)
                        CurrentOrientation = WindowOrientation.SmallFilled;
                    else
                        CurrentOrientation = WindowOrientation.LargeFilled;
                }
            }
            var view = GetView(CurrentOrientation);
            SetView(view);
        }

        private UIElement GetView(WindowOrientation orientation)
        {
            return GetView(orientation, new List<WindowOrientation>());
        }

        private UIElement GetView(WindowOrientation orientation, IList<WindowOrientation> watchedOrientations)
        {
            if (watchedOrientations.Contains(orientation))
                throw new Exception("Orientation redirect loop");
            watchedOrientations.Add(orientation);

            switch (orientation)
            {
                case WindowOrientation.Landscape:
                    return GetView(_landscapeView, LandscapeViewTemplate, LandscapeOrientationRedirect, watchedOrientations);
                case WindowOrientation.Portrait:
                    return GetView(_portraitView, PortraitViewTemplate, PortraitOrientationRedirect, watchedOrientations);
                case WindowOrientation.LargeFilled:
                    return GetView(_largeFilledView, LargeFilledViewTemplate, LargeFilledOrientationRedirect, watchedOrientations);
                case WindowOrientation.SmallFilled:
                    return GetView(_smallFilledView, SmallFilledViewTemplate, SmallFilledOrientationRedirect, watchedOrientations);
                case WindowOrientation.Snapped500:
                    return GetView(_snapped500View, Snapped500ViewTemplate, Snapped500OrientationRedirect, watchedOrientations);
                case WindowOrientation.Snapped320:
                    return GetView(_snapped320View, Snapped320ViewTemplate, Snapped320OrientationRedirect, watchedOrientations);
                default:
                    throw new UnhandledCaseException(typeof(WindowOrientation), CurrentOrientation);
            }
        }

        private UIElement GetView(UIElement view, DataTemplate viewTemplate, WindowOrientation orientationRedirect, IList<WindowOrientation> watchedOrientations)
        {
            if (view != null)
                return view;
            if (viewTemplate != null)
                return viewTemplate.LoadContent() as UIElement;
            return GetView(orientationRedirect, watchedOrientations);
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
