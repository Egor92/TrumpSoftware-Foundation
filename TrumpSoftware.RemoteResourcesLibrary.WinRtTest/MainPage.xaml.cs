using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TrumpSoftware.RemoteResourcesLibrary.WinRtTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ResourceManager _resourceManager;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void ButtonLoad_OnClick(object sender, RoutedEventArgs e)
        {
            await _resourceManager.LoadIndex();
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            _resourceManager = new ResourceManager(new Uri("ms-appx://Resources/"), new Uri("ms-appx://local/"), new Uri("http://localhost:5050/"));
        }

        private async void ButtonShow_OnClick(object sender, RoutedEventArgs e)
        {
            var resource = await _resourceManager.GetResource<string>("Songs/Irish blood.txt");
            SongTextBlock.Text = resource;
        }
    }
}
