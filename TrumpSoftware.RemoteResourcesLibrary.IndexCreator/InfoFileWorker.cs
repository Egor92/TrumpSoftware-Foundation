using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.RemoteResourcesLibrary.IndexCreator
{
    internal static class InfoFileWorker
    {
        private static readonly IDictionary<string, string> Types = new Dictionary<string, string>
        {
            { ".txt", "text" },
        };

        public static ResourceInfo GetResourceInfo(IEnumerable<string> lines)
        {
            var infos = lines.Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !x.EndsWith("#"))
                .Select(x => x.Split(':'))
                .Where(x => x.Count() == 2)
                .ToDictionary(x => x[0].Trim(), x => x[1].Trim());
            var isIgnored = infos.Any(x => x.Key == "Ignored" && (x.Value.ToLower() == "true" || x.Value == "1"));
            if (isIgnored)
                return null;
            var resourceInfo = new ResourceInfo();
            foreach (var info in infos)
            {
                resourceInfo.AddValue(info.Key, info.Value);
            }
            if (!resourceInfo.IsAvailable())
                return null;
            return resourceInfo;
        }

        public static string GetInfoFtpFileTemplate(string relativePath, string extention)
        {
            var resourceType = GetResourceType(extention);
            return string.Format(
@"RelativePath: {0}
{1}Type: {2}
#Group:
Version: 1
#Ignored:",
                relativePath, 
                resourceType != null ? string.Empty : "#",
                resourceType);
        }

        private static string GetResourceType(string extention)
        {
            if (Types.Keys.Contains(extention))
                return Types[extention];
            return null;
        }
    }
}