using System;

namespace TrumpSoftware.RemoteResourcesLibrary
{
    public class ResourceConverterNotFoundException : Exception
    {
        public Type ParserTargetType { get; private set; }

        internal ResourceConverterNotFoundException(Type parserTargetType)
        {
            ParserTargetType = parserTargetType;
        }

        public override string Message
        {
            get { return string.Format("ResourceConverter for type '{0}' didn't founded", ParserTargetType); }
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
