﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;

#endregion

namespace DotNetNuke.Entities.Urls
{
    /// <summary>
    /// This abstract class is to provide a inherited base class for a custom module friendly url provider.  All public methods must be overridden to provide the basis for a custom module provider.
    /// </summary>
    [Serializable]
    public abstract class ExtensionUrlProvider
    {
        public ExtensionUrlProviderInfo ProviderConfig { get; internal set; }

        #region Protected Methods

        protected string CleanNameForUrl(string urlValue, FriendlyUrlOptions options)
        {
            bool changed;
            string result = options != null
                                ? FriendlyUrlController.CleanNameForUrl(urlValue, options, out changed)
                                : FriendlyUrlController.CleanNameForUrl(urlValue, null, out changed);

            return result;
        }

        protected string CleanNameForUrl(string urlValue, FriendlyUrlOptions options, out bool replacedUnwantedChars)
        {
            return FriendlyUrlController.CleanNameForUrl(urlValue, options, out replacedUnwantedChars);
        }

        protected string CreateQueryStringFromParameters(string[] urlParms, int skipUpToPosition)
        {
            string result = "";
            int i = 0;
            bool odd = true;
            int size = urlParms.GetUpperBound(0) - skipUpToPosition;
            if (size >= 0 && urlParms.GetUpperBound(0) >= 0)
            {
                var qs = new StringBuilder(urlParms.GetUpperBound(0));
                foreach (string urlPathPart in urlParms)
                {
                    if (i > skipUpToPosition) //skip over the parts we don't want
                    {
                        if (odd)
                        {
                            qs.Append("&" + urlPathPart);
                        }
                        else
                        {
                            qs.Append("=" + urlPathPart);
                        }
                        //switch odd/even
                        odd = !odd;
                    }
                    i++;
                }
                result = qs.ToString();
            }
            return result;
        }

        protected string EnsureLeadingChar(string leading, string path)
        {
            return FriendlyUrlController.EnsureLeadingChar(leading, path);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// When true, output Urls from the provider for the specified portalId always include the current DotNetNuke page path (ie example.com/pagename/friendlyUrl)
        /// When false, output Urls from the provider for the specified portalId may sometimes not include the current DotNetNUke page path (ie example.com/friendlyUrl)
        /// </summary>
        /// <remarks>
        /// Defaults to true.  Must be set to false by the provider if any call to the 'ChangeFriendlyUrl' method results in the output 
        /// parameter 'useDnnPagePath' is false.  If 'false' is possible, then 'false' must be returned in this method.
        /// </remarks>
        public abstract bool AlwaysUsesDnnPagePath(int portalId);

        /// <summary>
        /// Generates a new friendly Url based on the parameters supplied
        /// </summary>
        /// <param name="tab">The current Tab the Friendly Url is for</param>
        /// <param name="friendlyUrlPath">The current friendly Url Path (minus alias) as generated by the Advanced Friendly Url provider</param>
        /// <param name="options">The current friendly Url options that apply to the current portal, as configured within the Extension Url Provider settings.  These include space replacement values and other settings which should be incorporated into the Friendly Url generation.</param>
        /// <param name="endingPageName">The 'pageName' value that comes from the FriendlyUrl API of DNN.  Normally this is the 'default.aspx' value (DotNetNuke.Common.Globals.glbDefaultPage).  A value of 'default.aspx' is discarded. However, it may be a different value for other modules and if not default.aspx will be appended to the end of the Url.  
        /// This is a ref parameter, so it can either be left as-is, or changed to default.aspx or "" if no specific value is required.</param>
        /// <param name="useDnnPagePath">Output parameter, must be set by module Friendly Url Provider.  If true, the /pagename/ part of the Url will be removed, and the Url path will be relative from the site root (example.com/custom-module-path instead of example.com/pagename/custom-module-path)</param>
        /// <param name="messages">A list of information messages used for both debug output and UI information.  Add any informational message to this collection if desired.</param>
        /// <remarks>Note using 'useDnnPagePath' = true requires having a specific tab returned from the TransformFriendlyUrlToQueryString below.  Usage of the 'useDnnPagePath' implies the TransformFriendlyUrlToQueryString method returns a ?tabid=xx value in the querystring.  
        /// It also means the provider level property 'AlwaysUsesDnnPagePath' must return 'false'</remarks>
        /// <returns>Friendly Url for specified values.  Return friendlyUrlPath if no change is made.</returns>
        public abstract string ChangeFriendlyUrl(TabInfo tab, 
                                                    string friendlyUrlPath, 
                                                    FriendlyUrlOptions options,
                                                    string cultureCode, 
                                                    ref string endingPageName, 
                                                    out bool useDnnPagePath,
                                                    ref List<string> messages);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="portalid"></param>
        /// <param name="requestUri"></param>
        /// <param name="queryStringCol"></param>
        /// <param name="options"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public abstract bool CheckForRedirect(int tabId, 
                                                    int portalid, 
                                                    string httpAlias, 
                                                    Uri requestUri,
                                                    NameValueCollection queryStringCol, 
                                                    FriendlyUrlOptions options,
                                                    out string redirectLocation, 
                                                    ref List<string> messages);

        /// <summary>
        /// This module returns any custom settings for the provider in a key/value pair.  This is used when any customised settings are saved to the web.config file.
        /// </summary>
        /// <returns>A dictionary of key/value pairs, where the key represents the web.config attribute name, and the value is the value to be stored in the web.config file</returns>
        /// <remarks>Note: the key values are case sensitive, and should match any values read from the attributes collection in the provider constructor.  If the provider has no custom attributes, return null or an empty dictionary. To remove a setting, add the value as a null.  Null values in the dictionary are removed as attributes.</remarks>
        public abstract Dictionary<string, string> GetProviderPortalSettings();

        /// <summary>
        /// Transforms a friendly Url into a querystring.  Used as input into the rewriting process, after the Url Rewriting process has identified the correct DNN Page.
        /// </summary>
        /// <param name="urlParms">string array of the friendly Url Path elements, separated by /</param>
        /// <param name="tabId">TabId of page the Friendly Url </param>
        /// <param name="portalId">PortalId of the Friendly Url</param>
        /// <remarks>This method will be only called if there is no match between the Page Index entries this Provider supplies via the 'CreatePageIndex' method.  This method is called
        /// when a DNN page match is found in the requested path, and there are other parameters behind the page path. You should only return a TabId in the querystring, when the ChangeFriendlyUrl function is returning 'true' for the output value of 'useDnnPagePath'.</remarks>
        /// <example>
        /// Given a Url of example.com/pagename/key/value - this method will be called with key,value in the urlParms array with a page match on 'pagename'.  The method should return 'key=value'.
        /// Or, if given a Url of example.com/pagename/my-friendly-module-url, it should transform 'my-friendly-module-url' into whatever the module actually uses to build content.  This might mean returning 'article=2354' derived from doing a specific lookup on 'my-friendly-module-url'.
        /// Warning: It's unwise to do a specific database lookup for each call of this method.  This method needs to be high-performance so should use a stateless method (ie, regex parse) or, if looking up database values, cached hashtables or thread-safe dictionaries.
        /// </example>
        /// <returns>Querystring value in key=value format, which will be used as an input to the rewriting function.</returns>
        public abstract string TransformFriendlyUrlToQueryString(string[] urlParms, 
                                                                    int tabId, int portalId,
                                                                    FriendlyUrlOptions options, 
                                                                    string cultureCode,
                                                                    PortalAliasInfo portalAlias, 
                                                                    ref List<string> messages,
                                                                    out int status, 
                                                                    out string location);

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is called to check whether to do a Url Rewrite on all Tabs specified by the provider
        /// </summary>
        /// <param name="portalId">The current portalId</param>
        /// <returns>True if the rewriter should be called, even if there are no Url parameters (ie, just plain DNN page Url).  False if not.
        /// Does not affect the calling of this provider when there are parameters supplied in the Url - that is determined by the tabIds property
        /// of the provider.</returns>
        public virtual bool AlwaysCallForRewrite(int portalId)
        {
            return false;
        }

        public string EnsureNotLeadingChar(string leading, string path)
        {
            return FriendlyUrlController.EnsureNotLeadingChar(leading, path);
        }

        #endregion
    }
}
