using System;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal class FtpItem
    {
        private const int NameStartIndex = 62;
        private readonly char _typeChar;

        public Uri Uri { get; private set; }

        public string Name { get; private set; }

        public bool IsFile
        {
            get { return _typeChar == '-'; }
        }

        public bool IsDirectory
        {
            get { return _typeChar == 'd'; }
        }

        public bool EndsWithDots
        {
            get { return Name == "." || Name == ".."; }
        }

        public bool IsIndexFile
        {
            get { return IsFile && Name == @"index.txt"; }
        }

        public FtpItem(Uri directoryUri, string details)
        {
            var nameLenght = details.Length - NameStartIndex;
            Name = details.Substring(NameStartIndex, nameLenght);
            _typeChar = details[0];
            Uri = directoryUri.Combine(Name);
        }
    }
}