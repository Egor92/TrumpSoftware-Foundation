using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace TrumpSoftware.WinRT.Converters
{
    public class ByteArrayToBitmapImageConverter : ChainConverter<byte[],BitmapImage>
    {
        protected override BitmapImage Convert(byte[] value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;
            var image = new BitmapImage();
            using (var stream = new InMemoryRandomAccessStream())
            {
                stream.WriteAsync(value.AsBuffer()).GetResults();
                stream.Seek(0);
                image.SetSource(stream);
            }
            return image;
        }

        protected override byte[] ConvertBack(BitmapImage value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
