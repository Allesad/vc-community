﻿using Microsoft.Practices.Unity;
using System;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Services;
using VirtoCommerce.CatalogModule.Repositories;
using VirtoCommerce.CatalogModule.Services;
using VirtoCommerce.Foundation.AppConfig.Repositories;
using VirtoCommerce.Foundation.Assets.Factories;
using VirtoCommerce.Foundation.Assets.Services;
using VirtoCommerce.Foundation.Data.AppConfig;
using VirtoCommerce.Foundation.Data.Asset;
using VirtoCommerce.Foundation.Data.Importing;
using VirtoCommerce.Foundation.Data.Infrastructure;
using VirtoCommerce.Foundation.Frameworks.Caching;
using VirtoCommerce.Foundation.Importing.Repositories;
using VirtoCommerce.Foundation.Importing.Services;
using VirtoCommerce.Foundation.Search;
using VirtoCommerce.Framework.Web.Modularity;
using VirtoCommerce.Search.Providers.Elastic;
using ICatalogService = VirtoCommerce.CatalogModule.Services.ICatalogService;
using VirtoCommerce.Framework.Web.Notification;
using VirtoCommerce.CoreModule.Web.Notification;

namespace VirtoCommerce.CatalogModule.Web
{
    public class Module : IModule
    {
        private readonly IUnityContainer _container;
        public Module(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
			#region Catalog dependencies
			var cacheManager = new CacheManager(x => new InMemoryCachingProvider(), x => new CacheSettings("", TimeSpan.FromMinutes(1), "", true));
			Func<IFoundationCatalogRepository> catalogRepFactory = () => new FoundationCatalogRepositoryImpl("VirtoCommerce");
			Func<IFoundationAppConfigRepository> appConfigRepFactory = () => new FoundationAppConfigRepositoryImpl("VirtoCommerce");

			var catalogService = new CatalogServiceImpl(catalogRepFactory, cacheManager);
			var propertyService = new PropertyServiceImpl(catalogRepFactory, cacheManager);
			var categoryService = new CategoryServiceImpl(catalogRepFactory, appConfigRepFactory, cacheManager);
			var itemService = new ItemServiceImpl(catalogRepFactory, appConfigRepFactory, cacheManager);
			var itemSearchService = new CatalogSearchServiceImpl(catalogRepFactory, itemService, catalogService, categoryService);

			_container.RegisterInstance<ICatalogService>("Catalog", catalogService);
			_container.RegisterInstance<IPropertyService>("Catalog", propertyService);
			_container.RegisterInstance<ICategoryService>("Catalog", categoryService);
			_container.RegisterInstance<IItemService>("Catalog", itemService);
			_container.RegisterInstance<ICatalogSearchService>("Catalog", itemSearchService);
			#endregion

			#region Search dependencies
			var searchConnection = new SearchConnection(ConnectionHelper.GetConnectionString("SearchConnectionString"));
			var elasticSearchProvider = new ElasticSearchProvider(new ElasticSearchQueryBuilder(), searchConnection);
			_container.RegisterInstance<ISearchProvider>("Catalog", elasticSearchProvider);
			#endregion


            #region Import dependencies
			Func<IImportRepository> importRepFactory = () => new EFImportingRepository("VirtoCommerce");
			
			var fileSystemAssetRep = new FileSystemBlobAssetRepository("~", new AssetEntityFactory());
			var assetService = new AssetService(fileSystemAssetRep, fileSystemAssetRep);
			Func<IImportService> imporServiceFactory = () => new ImportService(importRepFactory(), assetService, catalogRepFactory(), null, null);
			
			_container.RegisterType<Func<IImportRepository>>("Catalog", new InjectionFactory(x => importRepFactory));
			_container.RegisterType<Func<IImportService>>("Catalog", new InjectionFactory(x => imporServiceFactory));
			_container.RegisterType<Func<IFoundationCatalogRepository>>("Catalog", new InjectionFactory(x => catalogRepFactory));
			
            #endregion

       
            #region Mock
            //var localPath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_data\priceRuCategoryTest.yml");
            //var mockCatalogService = new MockCatalogService(localPath);
            //_container.RegisterInstance<ICatalogService>(mockCatalogService);
            //_container.RegisterInstance<ICategoryService>(mockCatalogService);
            //_container.RegisterInstance<IItemSearchService>(mockCatalogService);
            //_container.RegisterInstance<IItemService>(mockCatalogService);

            //_container.RegisterInstance<IPropertyService>(mockCatalogService);

            #endregion

        }
    }
}
