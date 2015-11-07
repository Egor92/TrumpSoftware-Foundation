using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace TrumpSoftware.WinRT.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static async Task CaptureScreenshot(this FrameworkElement element, StorageFile file, Guid bitmapEncoderId)
        {
            var renderTargetBitmap = new RenderTargetBitmap();
            var width = (int)element.ActualWidth;
            var height = (int)element.ActualHeight;
            await renderTargetBitmap.RenderAsync(element, width, height);
            var pixels = await renderTargetBitmap.GetPixelsAsync();

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(bitmapEncoderId, stream);
                byte[] bytes = pixels.ToArray();
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)width, (uint)height, 96, 96, bytes);

                await encoder.FlushAsync();
            }
        }
    }
}
