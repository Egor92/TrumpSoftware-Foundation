using System;
using System.Collections.Generic;
using TrumpSoftware.Xaml.Media;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    internal class ResourceConverterFactory
    {
        private readonly IDictionary<Type, IResourceConverter<object>> _resourceConverters = new Dictionary<Type, IResourceConverter<object>>();
        private static ResourceConverterFactory _instance;

        private static ResourceConverterFactory Instance
        {
            get { return _instance ?? (_instance = new ResourceConverterFactory()); }
        }

        private ResourceConverterFactory()
        {
            AddCommonResourceConverters();
        }

        internal static IResourceConverter<T> GetConverter<T>()
        {
            if (!Instance._resourceConverters.Keys.Contains(typeof(T)))
                throw new ResourceConverterNotFoundException(typeof(T));
            return (IResourceConverter<T>)Instance._resourceConverters[typeof(T)];
        }

        private void AddCommonResourceConverters()
        {
            AddResourceConverter<string>(new StringResourceConverter());
            AddResourceConverter<int>(new IntegerResourceConverter());
            AddResourceConverter<double>(new DoubleResourceConverter());
            AddResourceConverter<Uri>(new UriResourceConverter());
            AddResourceConverter<MediaObject>(new MediaObjectResourceConverter());
        }

        private void AddResourceConverter<T>(IResourceConverter<T> resourceConverter)
        {
            if (_resourceConverters.Keys.Contains(typeof(T)))
                throw new Exception(string.Format("ResourceConverter with type {0} was added", typeof(T)));
            var objectConverter = resourceConverter as IResourceConverter<object>;
            if (objectConverter == null)
                throw new Exception("ResourceConverter doesn't implement IResourceConverter<object> interface");
            _resourceConverters.Add(typeof(T), objectConverter);
        }
    }
}