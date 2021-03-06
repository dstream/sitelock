﻿using Cogworks.SiteLock.Web.Configuration;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace Cogworks.SiteLock.Web.Helpers
{
    internal class RequestHelper
    {
        internal static bool IsUmbracoAllowedPath(ISiteLockConfiguration config, string absolutePath, Uri urlReferrer)
        {
            var absolutePathLowered = absolutePath.ToLowerInvariant();

            if (absolutePathLowered == "/umbraco/default") { return true; }

            if (urlReferrer == null) { return false; }

            var urlReferrerLowered = urlReferrer.AbsolutePath.ToLowerInvariant();

            if (urlReferrerLowered.StartsWith("/dependencyhandler.axd")) { return true; }

            var isUmbracoUrl = urlReferrerLowered.StartsWith("/umbraco");

            return isUmbracoUrl;
        }


        internal static bool IsAllowedReferrerPath(ISiteLockConfiguration config, string absolutePath, Uri urlReferrer)
        {
            if (urlReferrer == null) { return false; }

            var absolutePathLowered = absolutePath.ToLowerInvariant();

            var urlReferrerLowered = urlReferrer.AbsolutePath.ToLowerInvariant();

            var isAllowedReferrer = IsAllowedPath(config, urlReferrerLowered);

            if (isAllowedReferrer)
            {
                // handles css files linking to images.
                config.AppendAllowedPath(absolutePathLowered);
            }

            return isAllowedReferrer;
        }



        internal static bool IsAllowedIP(ISiteLockConfiguration config, string userHostAddress)
        {
            var ips = config.GetAllowedIPs();

            return ips.Contains(userHostAddress);
        }



        internal static bool IsAllowedPath(ISiteLockConfiguration config, string absolutePath)
        {
            var absolutePathLowered = absolutePath.ToLowerInvariant();

            var allowedPaths = config.GetAllowedPaths().Select(path => path.ToLowerInvariant());

            foreach (var item in allowedPaths)
            {
                var regex = new Regex(item);

                Match match = regex.Match(absolutePathLowered);
                if (match.Success)
                {
                    return true;
                }
            }

            return false;
        }


        internal static bool IsLockedDomain(ISiteLockConfiguration config, string hostDomain)
        {
            var domains = config.GetLockedDomains();

            if (domains.Any(x => x == "*"))
            {
                return true;
            }

            var hostDomainLowered = hostDomain.ToLowerInvariant();

            var isLockedDomain = domains.Any(x => hostDomainLowered.Contains(x));

            return isLockedDomain;
        }
    }
}
