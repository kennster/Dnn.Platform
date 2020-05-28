﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Dnn.PersonaBar.Security.Components;
using DotNetNuke.Services.Localization;

namespace Dnn.PersonaBar.Security.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    class AuthProviderSupportedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyName = validationContext.DisplayName;

            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(string.Format(Localization.GetString(Constants.EmptyValue, Constants.LocalResourcesFile), propertyName));
            }

            var allAuthProviders = new SecurityController().GetAuthenticationProviders();

            if (!allAuthProviders.Contains(value))
            {
                return new ValidationResult(string.Format(Localization.GetString(Constants.NotValid, Constants.LocalResourcesFile), propertyName, value.ToString()));
            }

            return ValidationResult.Success;
        }
    }
}
