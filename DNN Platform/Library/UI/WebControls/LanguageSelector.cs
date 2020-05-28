﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information

#region Usings

using System;
using System.Collections;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{

	/// <summary>
	/// Language Selector control
	/// </summary>
	public class LanguageSelector : Control, INamingContainer
	{

		#region LanguageItemStyle enum

		public enum LanguageItemStyle
		{
			FlagOnly = 1,
			FlagAndCaption = 2,
			CaptionOnly = 3
		}

		#endregion

		#region LanguageListDirection enum

		public enum LanguageListDirection
		{
			Horizontal = 1,
			Vertical = 2
		}

		#endregion

		#region LanguageSelectionMode enum

		/// <summary>
		/// Language Selection mode, offered to the user: single select or multiple select.
		/// </summary>
		public enum LanguageSelectionMode
		{
			Multiple = 1,
			Single = 2
		}

		#endregion

		#region LanguageSelectionObject enum

		/// <summary>
		/// Selection object: Language ("de", "en") or Locale ("de-DE", "en-US")
		/// </summary>
		public enum LanguageSelectionObject
		{
			NeutralCulture = 1,
			SpecificCulture = 2
		}

		#endregion

		private Panel pnlControl;

		#region Public Properties
		
		/// <summary>
		/// Gets or sets selection mode (single, multiple)
		/// </summary>
		public LanguageSelectionMode SelectionMode
		{
			get
			{
				if (ViewState["SelectionMode"] == null)
				{
					return LanguageSelectionMode.Single;
				}
				else
				{
					return (LanguageSelectionMode) ViewState["SelectionMode"];
				}
			}
			set
			{
				if (SelectionMode != value)
				{
					ViewState["SelectionMode"] = value;
					if (Controls.Count > 0)
					{
						CreateChildControls(); //Recreate if already created
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the type of objects to be selectable: NeutralCulture ("de") or SpecificCulture ("de-DE")
		/// </summary>
		public LanguageSelectionObject SelectionObject
		{
			get
			{
				if (ViewState["SelectionObject"] == null)
				{
					return LanguageSelectionObject.SpecificCulture;
				}
				else
				{
					return (LanguageSelectionObject) ViewState["SelectionObject"];
				}
			}
			set
			{
				if ((int) SelectionMode != (int) value)
				{
					ViewState["SelectionObject"] = value;
					if (Controls.Count > 0)
					{
						CreateChildControls(); //Recreate if already created 
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the style of the language items
		/// </summary>
		public LanguageItemStyle ItemStyle
		{
			get
			{
				if (ViewState["ItemStyle"] == null)
				{
					return LanguageItemStyle.FlagAndCaption;
				}
				else
				{
					return (LanguageItemStyle) ViewState["ItemStyle"];
				}
			}
			set
			{
				if (ItemStyle != value)
				{
					ViewState["ItemStyle"] = value;
					if (Controls.Count > 0)
					{
						CreateChildControls(); //Recreate if already created 
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the direction of the language list
		/// </summary>
		public LanguageListDirection ListDirection
		{
			get
			{
				if (ViewState["ListDirection"] == null)
				{
					return LanguageListDirection.Vertical;
				}
				else
				{
					return (LanguageListDirection) ViewState["ListDirection"];
				}
			}
			set
			{
				if (ListDirection != value)
				{
					ViewState["ListDirection"] = value;
					if (Controls.Count > 0)
					{
						CreateChildControls(); //Recreate if already created 
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the list of selected languages
		/// </summary>
		public string[] SelectedLanguages
		{
			get
			{
				EnsureChildControls();
				var a = new ArrayList();
				if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
				{
					//return single language
					PortalSettings _Settings = PortalController.Instance.GetCurrentPortalSettings();
					foreach (string strLocale in LocaleController.Instance.GetLocales(_Settings.PortalId).Keys)
					{
						a.Add(strLocale);
					}
				}
				else
				{
					//create list of selected languages
					foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
					{
						if (SelectionMode == LanguageSelectionMode.Single)
						{
							if (((RadioButton) pnlControl.FindControl("opt" + c.Name)).Checked)
							{
								a.Add(c.Name);
							}
						}
						else
						{
							if (((CheckBox) pnlControl.FindControl("chk" + c.Name)).Checked)
							{
								a.Add(c.Name);
							}
						}
					}
				}
				return a.ToArray(typeof (string)) as string[];
			}
			set
			{
				EnsureChildControls();
				if (SelectionMode == LanguageSelectionMode.Single && value.Length > 1)
				{
					throw new ArgumentException("Selection mode 'single' cannot have more than one selected item.");
				}
				foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
				{
					if (SelectionMode == LanguageSelectionMode.Single)
					{
						((RadioButton) pnlControl.FindControl("opt" + c.Name)).Checked = false;
					}
					else
					{
						((CheckBox) pnlControl.FindControl("chk" + c.Name)).Checked = false;
					}
				}
				foreach (string strLocale in value)
				{
					if (SelectionMode == LanguageSelectionMode.Single)
					{
						Control ctl = pnlControl.FindControl("opt" + strLocale);
						if (ctl != null)
						{
							((RadioButton) ctl).Checked = true;
						}
					}
					else
					{
						Control ctl = pnlControl.FindControl("chk" + strLocale);
						if (ctl != null)
						{
							((CheckBox) ctl).Checked = true;
						}
					}
				}
			}
		}
		
		#endregion

		#region Protected Methods

		/// <summary>
		/// Create Child Controls
		/// </summary>
		protected override void CreateChildControls()
		{
			Controls.Clear();
			pnlControl = new Panel();
			pnlControl.CssClass = "dnnLangSelector";

			Controls.Add(pnlControl);
			pnlControl.Controls.Add(new LiteralControl("<ul>"));

			foreach (var c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
			{
				pnlControl.Controls.Add(new LiteralControl("<li>"));

				var lblLocale = new HtmlGenericControl("label");
				if (SelectionMode == LanguageSelectionMode.Single)
				{
					var optLocale = new RadioButton();
					optLocale.ID = "opt" + c.Name;
					optLocale.GroupName = pnlControl.ID + "_Locale";
					if (c.Name == Localization.SystemLocale)
					{
						optLocale.Checked = true;
					}
					pnlControl.Controls.Add(optLocale);
					lblLocale.Attributes["for"] = optLocale.ClientID;
				}
				else
				{
					var chkLocale = new CheckBox();
					chkLocale.ID = "chk" + c.Name;
					pnlControl.Controls.Add(chkLocale);
					lblLocale.Attributes["for"] = chkLocale.ClientID;
				}

				pnlControl.Controls.Add(lblLocale);
				if (ItemStyle != LanguageItemStyle.CaptionOnly)
				{
					var imgLocale = new Image();
					imgLocale.ImageUrl = ResolveUrl("~/images/Flags/" + c.Name + ".gif");
					imgLocale.AlternateText = c.DisplayName;
					imgLocale.Style["vertical-align"] = "middle";
					lblLocale.Controls.Add(imgLocale);
				}
				if (ItemStyle != LanguageItemStyle.FlagOnly)
				{
					lblLocale.Controls.Add(new LiteralControl("&nbsp;" + c.DisplayName));
				}
				pnlControl.Controls.Add(new LiteralControl("</li>"));
			}
			pnlControl.Controls.Add(new LiteralControl("</ul>"));

			//Hide if not more than one language
			if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
			{
				Visible = false;
			}
		}
		
		#endregion

		#region " Private Methods "

		/// <summary>
		/// retrieve the cultures, currently supported by the portal
		/// </summary>
		/// <param name="specific">true: locales, false: neutral languages</param>
		/// <returns>Array of cultures</returns>
		private CultureInfo[] GetCultures(bool specific)
		{
			var a = new ArrayList();
			PortalSettings _Settings = PortalController.Instance.GetCurrentPortalSettings();
			foreach (string strLocale in LocaleController.Instance.GetLocales(_Settings.PortalId).Keys)
			{
				var c = new CultureInfo(strLocale);
				if (specific)
				{
					a.Add(c);
				}
				else
				{
					CultureInfo p = c.Parent;
					if (!a.Contains(p))
					{
						a.Add(p);
					}
				}
			}
			return (CultureInfo[]) a.ToArray(typeof (CultureInfo));
		}
		
		#endregion

	}
}
