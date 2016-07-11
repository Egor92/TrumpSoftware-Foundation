using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrumpSoftware.Wpf.Helpers;

namespace TrumpSoftware.Wpf.Test.Helpers
{
    [TestClass]
    public class UIHelperTest
    {
        [TestMethod]
        public void IfElementLoaded_CanFindElementByName()
        {
            var window = XamlParser.Parse<FrameworkElement>(@"
<StackPanel x:Name='Element1'>
    <ContentControl x:Name='Element6'>
        <Button x:Name='Element7'>
            <Label x:Name='Element8' />
        </Button>
    </ContentControl>
    <Button x:Name='Element2' />
    <FrameworkElement x:Name='Element3' />
    <Button />
    <DockPanel x:Name='Element4' />
    <FrameworkElement x:Name='Element5' />
    <FrameworkElement />
</StackPanel>
            ");

            var element = window.GetDescendantByName<FrameworkElement>("Element8");
            Assert.IsNotNull(element);
            Assert.IsInstanceOfType(element, typeof(Label));
        }

        [TestMethod]
        public void IfElementDoesNotLoaded_CannotFindElementByName()
        {
            const bool toLoad = false;
            var window = XamlParser.Parse<FrameworkElement>(@"
<StackPanel x:Name='Element1'>
    <ContentControl x:Name='Element6'>
        <Button x:Name='Element7'>
            <Label x:Name='Element8' />
        </Button>
    </ContentControl>
</StackPanel>
            ", toLoad);

            var element = window.GetDescendantByName<FrameworkElement>("Element8");
            Assert.IsNull(element);
        }

        [TestMethod]
        public void IfHasNoParent_ReturnNull()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<UIElement />
            ");
            var parent = rootObject.GetParent();

            Assert.IsNull(parent);
        }

        [TestMethod]
        public void IfElementIsInsidePanel_ReturnPanel()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Grid x:Name='Parent'>
        <Button x:Name='Target' />
    </Grid>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var parentElement = targetElement.GetParent();

            Assert.IsInstanceOfType(parentElement, typeof(Grid));
            Assert.AreEqual("Parent", ((Grid)parentElement).Name);
        }

        [TestMethod]
        public void IfElementIsInsideContentControl_ReturnContentPresenter()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <ContentControl x:Name='Parent'>
        <Button x:Name='Target' />
    </ContentControl>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var parentElement = targetElement.GetParent();

            Assert.IsInstanceOfType(parentElement, typeof(ContentPresenter));
        }

        [TestMethod]
        public void CanGetAncestor()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Label x:Name='Ancestor'>
            <Button>
                <ContentControl>
                    <Button x:Name='Target' />
                </ContentControl>
            </Button>
        </Label>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var targetAcestor = targetElement.GetAncestor<Label>();

            Assert.IsInstanceOfType(targetAcestor, typeof(Label));
            Assert.AreEqual("Ancestor", targetAcestor.Name);
        }

        [TestMethod]
        public void IfGetAncestorUsingSameTypeAndIncludingItselt_ReturnItself()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Label x:Name='Ancestor'>
            <Button>
                <ContentControl>
                    <Button x:Name='Target' />
                </ContentControl>
            </Button>
        </Label>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var targetAcestor = targetElement.GetAncestor<Button>(true);

            Assert.IsInstanceOfType(targetAcestor, typeof(Button));
            Assert.AreEqual(targetElement, targetAcestor);
        }

        [TestMethod]
        public void IfGetAncestorUsingSameTypeAndNotIncludingItselt_DoNotReturnItself()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Label x:Name='Ancestor'>
            <Button>
                <ContentControl>
                    <Button x:Name='Target' />
                </ContentControl>
            </Button>
        </Label>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var targetAcestor = targetElement.GetAncestor<Button>(false);

            Assert.AreNotEqual(targetElement, targetAcestor);
        }

        [TestMethod]
        public void IfGetAncestorUsingOtherTypeAndIncludingItselt_DoNotReturnItself()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Label x:Name='Ancestor'>
            <Button>
                <ContentControl>
                    <Button x:Name='Target' />
                </ContentControl>
            </Button>
        </Label>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<FrameworkElement>("Target");
            var targetAcestor = targetElement.GetAncestor<Label>(true);

            Assert.AreNotEqual(targetElement, targetAcestor);
        }

        [TestMethod]
        public void CanGetAncestorUsingCondition()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<Border>
    <Border x:Name='Friend'>
        <Border x:Name='Ancestor'>
            <Border>
                <Border>
                    <Border x:Name='Target' />
                </Border>
            </Border>
        </Border>
    </Border>
</Border>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            var targetAcestor = targetElement.GetAncestor<FrameworkElement>(x => x.Name == "Friend");

            Assert.IsInstanceOfType(targetAcestor, typeof(Border));
            Assert.AreEqual("Friend", targetAcestor.Name);
        }

        [TestMethod]
        public void IfUseAbortCondition_CanAbortFindingAncestor()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<Border>
    <Border x:Name='Friend'>
        <Border x:Name='Ancestor'>
            <Border>
                <Border>
                    <Border x:Name='Target' />
                </Border>
            </Border>
        </Border>
    </Border>
</Border>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            Func<FrameworkElement, bool> targetElementCondition = (x => x.Name == "Friend");
            Func<DependencyObject, bool> abortCondition = x =>
            {
                return x is Border && ((Border) x).Name == "Ancestor";
            };
            var targetAcestor = targetElement.GetAncestor(targetElementCondition, abortCondition);

            Assert.IsNull(targetAcestor);
        }

        [TestMethod]
        public void CanGetAncestors()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Border>
            <ContentControl>
                <ContentControl>
                    <Border x:Name='Target' />
                </ContentControl>
            </ContentControl>
        </Border>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            var targetAcestors = targetElement.GetAncestors<Border>(null, null, true);

            Assert.AreEqual(3, targetAcestors.Count);
        }

        [TestMethod]
        public void CanGetAncestorsUsingCondition()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border Background='Red'>
        <Border>
            <ContentControl>
                <ContentControl>
                    <Border x:Name='Target' Background='Red' />
                </ContentControl>
            </ContentControl>
        </Border>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            var targetAcestors = targetElement.GetAncestors<Border>(x => Equals(x.Background, Brushes.Red), null, true);

            Assert.AreEqual(2, targetAcestors.Count);
        }

        [TestMethod]
        public void CanGetAncestorsUsingAbortCondition()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <ContentControl x:Name='Stopper'>
            <Border>
                <ContentControl>
                    <Border x:Name='Target' />
                </ContentControl>
            </Border>
        </ContentControl>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            Func<DependencyObject, bool> abortCondition = x =>
            {
                return x is FrameworkElement && ((FrameworkElement) x).Name == "Stopper";
            };
            var targetAcestors = targetElement.GetAncestors<Border>(null, abortCondition, true);

            Assert.AreEqual(2, targetAcestors.Count);
        }

        [TestMethod]
        public void AbortConditionControlIsInclidedIntoAncestorsList()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Border x:Name='Stopper'>
            <Border>
                <ContentControl>
                    <Border x:Name='Target' />
                </ContentControl>
            </Border>
        </Border>
    </Border>
</ContentControl>
            ");
            var targetElement = rootObject.GetDescendantByName<Border>("Target");
            Func<DependencyObject, bool> abortCondition = x =>
            {
                return x is FrameworkElement && ((FrameworkElement) x).Name == "Stopper";
            };
            var targetAcestors = targetElement.GetAncestors<Border>(null, abortCondition, true);

            Assert.AreEqual(3, targetAcestors.Count);
        }

        [TestMethod]
        public void CanGetDescendantsByType()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<ContentControl>
    <Border>
        <Border>
            <Border>
                <ContentControl>
                    <Border />
                </ContentControl>
            </Border>
        </Border>
    </Border>
</ContentControl>
            ");
            var borders = rootObject.GetDescendants<Border>().ToList();
            Assert.AreEqual(4, borders.Count);
        }

        [TestMethod]
        public void CanGetDescendantsIncludingItself()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<Border>
    <Border>
        <Border>
            <Border>
                <Border />
            </Border>
        </Border>
    </Border>
</Border>
            ");
            var borders = rootObject.GetDescendants<Border>(true).ToList();
            Assert.AreEqual(5, borders.Count);
        }

        [TestMethod]
        public void CanGetDescendantsExcludingItself()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<Border>
    <Border>
        <Border>
            <Border>
                <Border />
            </Border>
        </Border>
    </Border>
</Border>
            ");
            var borders = rootObject.GetDescendants<Border>(false).ToList();
            Assert.AreEqual(4, borders.Count);
        }

        [TestMethod]
        public void CanGetDescendantsByCondition()
        {
            var rootObject = XamlParser.Parse<UIElement>(@"
<Border Background='Red'>
    <Border Background='Green'>
        <Border Background='Red'>
            <Border Background='{x:Null}'>
                <Border Background='Blue' />
            </Border>
        </Border>
    </Border>
</Border>
            ");
            var borders = rootObject.GetDescendants<Border>(x => Equals(x.Background, Brushes.Red), true).ToList();
            Assert.AreEqual(2, borders.Count);
        }
    }
}
