﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using System;
using System.Collections.Generic;
using System.Web;
#endregion

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    ///   ClientCapability provides capabilities supported by the http requester (e.g. Mobile Device, TV, Desktop)
    /// </summary>
    /// <remarks>
    ///   The capabilities are primarily derived based on UserAgent.  
    /// </remarks>          
    public interface IClientCapability
    {
        /// <summary>
        ///   Unique ID of the client making request.
        /// </summary>
        string ID { get; set; }

        /// <summary>
        ///   User Agent of the client making request
        /// </summary>
        string UserAgent { get; set; }

        /// <summary>
        ///   Is request coming from a mobile device.
        /// </summary>
        bool IsMobile { get; set; }

        /// <summary>
        ///   Is request coming from a tablet device.
        /// </summary>
        bool IsTablet { get; set; }

        /// <summary>
        ///   Does the requesting device supports touch screen.
        /// </summary>
        bool IsTouchScreen { get; set; }

        /// <summary>
        ///   FacebookRequest property is filled when request is coming though Facebook iFrame (e.g. fan pages).
        /// </summary>
        /// <remarks>
        ///   FacebookRequest property is populated based on data in "signed_request" headers coming from Facebook.  
        ///   In order to ensure request is coming from Facebook, FacebookRequest.IsValidSignature method should be called with the secrety key provided by Facebook.
        ///   Most of the properties in IClientCapability doesnot apply to Facebook
        /// </remarks>                
        FacebookRequest FacebookRequest { get; set; }

        /// <summary>
        ///   ScreenResolution Width of the requester in Pixels.
        /// </summary>
        int ScreenResolutionWidthInPixels { get; set; }

        /// <summary>
        ///   ScreenResolution Height of the requester in Pixels.
        /// </summary>
        int ScreenResolutionHeightInPixels { get; set; }

        /// <summary>
        /// Represents the name of the broweser in the request
        /// </summary>        
        string BrowserName { get; set; }

        /// <summary>
        ///   Does requester support Flash.
        /// </summary>
        bool SupportsFlash { get; set; }

        /// <summary>
        /// A key-value collection containing all capabilities supported by requester
        /// </summary>    
        [Obsolete("This method is not memory efficient and should be avoided as the Match class now exposes an accessor keyed on property name.. Scheduled removal in v10.0.0.")]    
        IDictionary<string, string> Capabilities { get; set; }

        /// <summary>
        /// Returns the request prefered HTML DTD
        /// </summary>
        string HtmlPreferedDTD { get; set; }

        /// <summary>
        ///   Http server variable used for SSL offloading - if this value is empty offloading is not enabled
        /// </summary>
        string SSLOffload { get; set; }

        /// <summary>
        /// Get client capability value by property name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string this[string name] { get; }
    }
}
