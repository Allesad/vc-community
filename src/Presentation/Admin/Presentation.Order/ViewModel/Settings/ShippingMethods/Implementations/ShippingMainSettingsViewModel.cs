﻿using System.Collections.Generic;
using VirtoCommerce.Foundation.Orders.Model.Jurisdiction;
using VirtoCommerce.Foundation.Security.Model;
using VirtoCommerce.ManagementClient.Core.Infrastructure;
using VirtoCommerce.ManagementClient.Core.Infrastructure.Common.Model;
using VirtoCommerce.ManagementClient.Order.ViewModel.Settings.JurisdictionGroups.Interfaces;
using VirtoCommerce.ManagementClient.Order.ViewModel.Settings.Jurisdictions.Interfaces;
using VirtoCommerce.ManagementClient.Order.ViewModel.Settings.ShippingMethods.Interfaces;
using VirtoCommerce.ManagementClient.Order.ViewModel.Settings.ShippingOptions.Interfaces;
using VirtoCommerce.ManagementClient.Order.ViewModel.Settings.ShippingPackages.Interfaces;

namespace VirtoCommerce.ManagementClient.Order.ViewModel.Settings.ShippingMethods.Implementations
{
	public class ShippingMainSettingsViewModel : SubTabsDefaultViewModel, IShippingMainSettingsViewModel
	{
		public ShippingMainSettingsViewModel(
			IShippingOptionSettingsViewModel shippingOptionSettingsViewModel,
			IShippingMethodSettingsViewModel shippingMethodSettingsViewModel,
			IShippingPackageSettingsViewModel shippingPackageSettingsViewModel,
			IViewModelsFactory<IJurisdictionSettingsViewModel> jurisdictionSettingsVmFactory,
			IViewModelsFactory<IJurisdictionGroupSettingsViewModel> jGroupVmFactory,
			IAuthenticationContext authContext)
		{

			SubItems = new List<ItemTypeHomeTab>();

			if (authContext.CheckPermission(PredefinedPermissions.SettingsShippingOptions))
			{
				SubItems.Add(new ItemTypeHomeTab { Caption = "Shipping options".Localize(), ViewModel = shippingOptionSettingsViewModel });
			}
			if (authContext.CheckPermission(PredefinedPermissions.SettingsShippingMethods))
			{
				SubItems.Add(new ItemTypeHomeTab { Caption = "Shipping methods".Localize(), ViewModel = shippingMethodSettingsViewModel });
			}
			if (authContext.CheckPermission(PredefinedPermissions.SettingsShippingPackages))
			{
				SubItems.Add(new ItemTypeHomeTab { Caption = "Shipping packages".Localize(), ViewModel = shippingPackageSettingsViewModel });
			}
			if (authContext.CheckPermission(PredefinedPermissions.SettingsJurisdiction))
			{
				SubItems.Add(new ItemTypeHomeTab { Caption = "Jurisdictions".Localize(), ViewModel = jurisdictionSettingsVmFactory.GetViewModelInstance(new KeyValuePair<string, object>("jurisdictionType", JurisdictionTypes.Shipping)) });
			}
			if (authContext.CheckPermission(PredefinedPermissions.SettingsJurisdictionGroups))
			{
				SubItems.Add(new ItemTypeHomeTab { Caption = "Jurisdiction groups".Localize(), ViewModel = jGroupVmFactory.GetViewModelInstance(new KeyValuePair<string, object>("jurisdictionType", JurisdictionTypes.Shipping)) });
			}
			CurrentTab = SubItems[0];
		}
	}
}
