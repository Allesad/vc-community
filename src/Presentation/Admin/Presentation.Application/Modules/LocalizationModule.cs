﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using VirtoCommerce.Client.Globalization;
using VirtoCommerce.Client.Globalization.Repository;
using VirtoCommerce.Foundation.AppConfig.Factories;
using VirtoCommerce.Foundation.AppConfig.Repositories;
using VirtoCommerce.Foundation.Data.AppConfig;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.ManagementClient.Security.ViewModel.Interfaces;

namespace VirtoCommerce.ManagementClient.Localization
{
	public class LocalizationModule : IModule
	{
		private readonly IUnityContainer _container;

		public LocalizationModule(IUnityContainer container)
		{
			_container = container;
		}

		#region IModule Members

		public void Initialize()
		{
			RegisterViewsAndServices();
		}

		#endregion

		protected void RegisterViewsAndServices()
		{
			_container.RegisterType<IAppConfigEntityFactory, AppConfigEntityFactory>(new ContainerControlledLifetimeManager());
			_container.RegisterType<IAppConfigRepository, DSAppConfigClient>();
			var loginViewModel = _container.Resolve<ILoginViewModel>();
			var baseUrlHash = loginViewModel.CurrentUser.BaseUrl.ToLower().GetHashCode().ToString();

			var repositoryFactory = _container.Resolve<IRepositoryFactory<IAppConfigRepository>>();
			var localElements = new XmlElementRepository(Path.Combine(Path.GetTempPath(), "VirtoCommerceCMLocalization", baseUrlHash));
			var cachedElements = new CacheElementRepository(localElements);
			var _elementRepository = new CachedDatabaseElementRepository(repositoryFactory, cachedElements, x => x.Category != "");
			_container.RegisterInstance<IElementRepository>(_elementRepository);

			// check cache date and update if needed
			var repository = _container.Resolve<IAppConfigRepository>();
			var lastItem = repository.Localizations.OrderByDescending(x => x.Created).Take(1).FirstOrDefault();
			DateTime? dbDate = lastItem == null ? null : lastItem.Created;

			var cacheDate = _elementRepository.GetStatusDate(CultureInfo.DefaultThreadCurrentUICulture.Name);
			if (!dbDate.HasValue || dbDate > cacheDate)
			{
				_elementRepository.Clear();

				// force Elements re-caching
				var eee = _elementRepository.Elements();
				var cultures = eee.Select(x => x.Culture).Distinct().ToList();
				cultures.ForEach(x => _elementRepository.SetStatusDate(x));
			}
		}
	}
}
