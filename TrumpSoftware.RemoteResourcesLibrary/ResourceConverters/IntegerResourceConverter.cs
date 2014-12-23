using System.Threading.Tasks;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class IntegerResourceConverter : StringResourceConverter, IResourceConverter<int>, IResourceConverter<object>
    {
        int IResourceConverter<int>.Convert(IResource resource)
        {
            var str = (this as IResourceConverter<string>).Convert(resource);
            int value;
            if (int.TryParse(str, out value))
                return value;
            throw new ResourceConvertionException(string.Format("Couldn't convert string \"{0}\" to type \"{1}\"", str, typeof(int)));
        }

        object IResourceConverter<object>.Convert(IResource resource)
        {
            return (this as IResourceConverter<int>).Convert(resource);
        }
    }
}