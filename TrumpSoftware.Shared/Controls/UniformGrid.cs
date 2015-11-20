using System;
using System.Linq;
#if WPF
using System.Windows;
using System.Windows.Controls;
#elif WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#endif

#if WPF
namespace TrumpSoftware.Wpf.Controls
#elif WINRT

namespace TrumpSoftware.WinRT.Controls
#endif
{
    public class UniformGrid : Panel
    {
        private UIElementCollection ChildrenCollection
        {
#if WPF
            get { return InternalChildren; }
#elif WINRT
            get { return Children; }
#endif
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ChildrenCollection.Count == 0)
                return base.MeasureOverride(availableSize);

            foreach (UIElement child in ChildrenCollection)
                child.Measure(availableSize);

            var childrenDesiredSizes = ChildrenCollection.OfType<UIElement>()
                                                         .Select(x => x.DesiredSize)
                                                         .ToList();
            var itemDesiredWidth = childrenDesiredSizes.Max(x => x.Width);
            var itemDesiredHeight = childrenDesiredSizes.Max(x => x.Height);

            if (double.IsPositiveInfinity(availableSize.Width))
            {
                var panelDesiredWidth = itemDesiredWidth*ChildrenCollection.Count;
                var panelDesiredHeight = itemDesiredHeight;
                return new Size(panelDesiredWidth, panelDesiredHeight);
            }
            else
            {
                itemDesiredWidth = Math.Min(itemDesiredWidth, availableSize.Width);
                var itemsPerLine = (int)(availableSize.Width/itemDesiredWidth);
                itemsPerLine = Math.Min(itemsPerLine, ChildrenCollection.Count);
                var panelDesiredWidth = itemsPerLine * itemDesiredWidth;
                var itemsPerColumn = ChildrenCollection.Count/itemsPerLine;
                var panelDesiredHeight = itemDesiredHeight*itemsPerColumn;
                return new Size(panelDesiredWidth, panelDesiredHeight);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ChildrenCollection.Count == 0)
                return finalSize;

            var childrenDesiredSizes = ChildrenCollection.OfType<UIElement>()
                .Select(x => x.DesiredSize)
                .ToList();
            var itemDesiredWidth = childrenDesiredSizes.Max(x => x.Width);
            var itemDesiredHeight = childrenDesiredSizes.Max(x => x.Height);

            itemDesiredWidth = Math.Min(itemDesiredWidth, finalSize.Width);
            itemDesiredHeight = Math.Min(itemDesiredHeight, finalSize.Height);

            var itemsPerLine = (int) (finalSize.Width/itemDesiredWidth);
            itemsPerLine = Math.Min(itemsPerLine, ChildrenCollection.Count);
            var itemFinalWidth = finalSize.Width / itemsPerLine;
            var itemFinalHeight = itemDesiredHeight;

            var itemFinalSize = new Size(itemFinalWidth, itemFinalHeight);

            for (var i = 0; i < ChildrenCollection.Count; i++)
            {
                var child = ChildrenCollection[i];
                var x = (i%itemsPerLine)*itemFinalWidth;
                var y = (i/itemsPerLine)*itemFinalHeight;
                var location = new Point(x, y);
                var rect = new Rect(location, itemFinalSize);
                child.Arrange(rect);
            }

            return finalSize;
        }
    }
}