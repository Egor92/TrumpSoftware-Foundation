namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal interface IResourceConverter<out T>
    {
        T Convert(IResource resource);
    }
}