using Windows.ApplicationModel;
using Windows.Storage;

namespace TrumpSoftware.RemoteResourcesLibrary.Test
{
    public static class SpecialPaths
    {
        public static string CompiledFolderPath
        {
            get { return string.Format("{0}/Resources/", Package.Current.InstalledLocation.Path); }
        }

        public static string LocalFolderPath
        {
            get { return string.Format("{0}/Local/", ApplicationData.Current.LocalFolder); }
        }

        public static string RemoteFolderPath
        {
            get { return string.Format("{0}/Remote/", Package.Current.InstalledLocation.Path); }
        }

        public static string RemoteStorageFolderPath
        {
            get { return string.Format("{0}/Remote/", ApplicationData.Current.LocalFolder); }
        }

        public static string ServerPath
        {
            get { return "http://localhost:5050/"; }
        }
    }
}