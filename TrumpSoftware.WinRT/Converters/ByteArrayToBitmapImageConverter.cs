using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TrumpSoftware.WinRT.Converters
{
    public class ByteArrayToBitmapImageConverter : ChainConverter
    {
        protected override object Convert(object value, Type targetType, object parameter, string language)
        {
            var byteArray = (byte[]) value;
            if (byteArray == null)
                return null;
            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                stream.WriteAsync(byteArray.AsBuffer()).GetResults();
                stream.Seek(0);
                image.SetSource(stream);
            }
            return image;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
