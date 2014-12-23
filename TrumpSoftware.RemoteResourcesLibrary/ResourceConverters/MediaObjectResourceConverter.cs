using TrumpSoftware.Xaml.Media;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public class MediaObjectResourceConverter : IResourceConverter<MediaObject>
    {
        MediaObject IResourceConverter<MediaObject>.Convert(IResource resource)
        {
            var uri = resource.GetUri();
            return new MediaObject(uri);
        }
    }
}
