using System;

#if WPF
using System.IO;
using System.Windows.Media.Imaging;
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class ByteArrayToBitmapImageConverter : ChainConverter<byte[],BitmapImage>
    {
        protected override BitmapImage Convert(byte[] value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return null;
            var image = new BitmapImage();
#if WPF
            using (var stream = new MemoryStream(value))
            {
                stream.Seek(0, SeekOrigin.Begin);
                image.StreamSource = stream;
#elif WINRT
            using (var stream = new InMemoryRandomAccessStream())
            {
                stream.WriteAsync(value.AsBuffer()).GetResults();
                stream.Seek(0);
                image.SetSource(stream);
#endif
            }
            return image;
        }

        protected override byte[] ConvertBack(BitmapImage value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }
    }
}
