using System.Linq;
using System.Resources;
using System;
using System.Reflection;
#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
using TTo = System.Object;
#elif WINRT
using System.Collections.Generic;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using CultureArgumentType = System.String;
using TTo = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public class GetResxResourceConverter : ChainConverter<string, TTo>
    {
#if WINRT
        private readonly IEnumerable<string> _assemblyFileExtensions = new[]
        {
            "exe",
            "dll",
        };
#endif

        public string ClassFullName { get; set; }

        public string AssemblyName { get; set; }

        protected override TTo Convert(string value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            if (value == null)
                return null;
            var resourceManager = GetResourceManager();
            if (resourceManager == null)
                return null;
#if WPF
            return resourceManager.GetObject(value);
#elif WINRT
            return resourceManager.GetString(value);
#endif
        }

        protected override string ConvertBack(TTo value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            throw new NotSupportedException();
        }

        private ResourceManager GetResourceManager()
        {
            if (AssemblyName == null)
                return null;
#if WPF
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == AssemblyName);
#elif WINRT
            var assemblyFiles = Package.Current.InstalledLocation.GetFilesAsync().GetResults();
            var assemblyFile = assemblyFiles.FirstOrDefault(IsMatchedAssembly);
            if (assemblyFile == null)
                return null;
            var assemblyName = new AssemblyName(assemblyFile.Name);
            var assembly = Assembly.Load(assemblyName);
#endif
            if (assembly == null)
                return null;

            var classFullName = ClassFullName;
            if (classFullName != null)
                classFullName = classFullName.Trim();
            if (string.IsNullOrEmpty(classFullName))
            {
                classFullName = string.Format("{0}.Properties.Resources", AssemblyName);
            }

#if WPF
            var types = assembly.GetTypes();
#elif WINRT
            var types = assembly.DefinedTypes;
#endif
            if (types.All(x => x.FullName != classFullName))
                return null;

            return new ResourceManager(classFullName, assembly);
        }

#if WINRT
        private bool IsMatchedAssembly(StorageFile storageFile)
        {
            return Path.GetFileNameWithoutExtension(storageFile.Name) == AssemblyName
                && _assemblyFileExtensions.Contains(Path.GetExtension(storageFile.Name));
        }
#endif
    }
}
