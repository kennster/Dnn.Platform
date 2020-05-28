﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace DotNetNuke.Web.Models
{
    [Serializable]
    public class TabModule
    {
        public TabInfo TabInfo { get; set; }

        public ModuleInfo ModuleInfo { get; set; }

        public string ModuleVersion { get; set; }
    }
}
