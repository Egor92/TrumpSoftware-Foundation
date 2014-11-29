using System;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal class HeliohostFtpResponser : FtpResponser
    {
        public static readonly Uri HelioHostUri = new Uri(@"ftp://ftp.trumpsoftware.heliohost.org");

        public HeliohostFtpResponser()
            : base(@"program@trumpsoftware.heliohost.org", "%RTc4diRlc3}")
        {
        }
    }
}
