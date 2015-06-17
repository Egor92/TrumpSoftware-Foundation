using System;
using TrumpSoftware.Common;
using TrumpSoftware.Common.Exceptions;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    public enum FtpItemType
    {
        Directory,
        File
    }

    public static class FtpItemTypeHelper
    {
        public static FtpItemType GetFtpItemType(this string str)
        {
            if (str.StartsWith("-"))
                return FtpItemType.File;
            if (str.EndsWith("d"))
                return FtpItemType.Directory;
            throw new UnhandledCaseException();
        }
    }

}