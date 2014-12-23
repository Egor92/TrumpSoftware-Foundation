using System.Threading.Tasks;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class DoubleResourceConverter : StringResourceConverter, IResourceConverter<double>, IResourceConverter<object>
    {
        double IResourceConverter<double>.Convert(IResource resource)
        {
            var str = (this as IResourceConverter<string>).Convert(resource);
            double value;
            if (double.TryParse(str, out value))
                return value;
            throw new ResourceConvertionException(string.Format("Couldn't convert string \"{0}\" to type \"{1}\"", str, typeof(double)));
        }

        object IResourceConverter<object>.Convert(IResource resource)
        {
            return (this as IResourceConverter<double>).Convert(resource);
        }
    }
}
