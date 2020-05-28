﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using Newtonsoft.Json;

namespace Dnn.PersonaBar.SiteSettings.Services.Dto
{
    [JsonObject]
    public class LanguageTabDto
    {
        public int PageId { get; set; }
        public string PageName { get; set; }
        public string ViewUrl { get; set; }
    }
}
