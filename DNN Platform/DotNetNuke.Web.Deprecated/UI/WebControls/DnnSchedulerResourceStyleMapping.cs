﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    public class DnnSchedulerResourceStyleMapping : ResourceStyleMapping
    {
        public DnnSchedulerResourceStyleMapping()
        {
        }

        public DnnSchedulerResourceStyleMapping(string type, string key, string applyCssClass) : base(type, key, applyCssClass)
        {
        }

        public DnnSchedulerResourceStyleMapping(string type, string key, string text, string applyCssClass) : base(type, key, text, applyCssClass)
        {
        }
    }
}
