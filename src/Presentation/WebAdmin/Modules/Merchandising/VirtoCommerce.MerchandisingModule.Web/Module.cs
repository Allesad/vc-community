﻿using Microsoft.Practices.Unity;
using System;
using VirtoCommerce.Caching.HttpCache;
using VirtoCommerce.CatalogModule.Data.Repositories;
using VirtoCommerce.CatalogModule.Data.Services;
using VirtoCommerce.CatalogModule.Repositories;
using VirtoCommerce.Foundation.Catalogs;
using VirtoCommerce.Foundation.Catalogs.Services;
using VirtoCommerce.Foundation.Data.Infrastructure;
using VirtoCommerce.Foundation.Data.Marketing;
using VirtoCommerce.Foundation.Data.Reviews;
using VirtoCommerce.Foundation.Data.Stores;
using VirtoCommerce.Foundation.Frameworks.Caching;
using VirtoCommerce.Foundation.Marketing.Model.DynamicContent;
using VirtoCommerce.Foundation.Marketing.Repositories;
using VirtoCommerce.Foundation.Marketing.Services;
using VirtoCommerce.Foundation.Reviews.Repositories;
using VirtoCommerce.Foundation.Search;
using VirtoCommerce.Foundation.Stores.Repositories;
using VirtoCommerce.Framework.Web.Modularity;
using VirtoCommerce.MerchandisingModule.Web.Controllers;
using VirtoCommerce.Search.Providers.Elastic;

namespace VirtoCommerce.MerchandisingModule.Web
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
            var cacheManager = new CacheManager(x => new InMemoryCachingProvider(), x => new CacheSettings("", TimeSpan.FromMinutes(1), "", true));
            Func<IFoundationCatalogRepository> catalogRepFactory = () => new FoundationCatalogRepositoryImpl("VirtoCommerce");
            Func<IFoundationAppConfigRepository> appConfigRepFactory = () => new FoundationAppConfigRepositoryImpl("VirtoCommerce");

            var catalogService = new CatalogServiceImpl(catalogRepFactory, cacheManager);
            var propertyService = new PropertyServiceImpl(catalogRepFactory, cacheManager);
            var categoryService = new CategoryServiceImpl(catalogRepFactory, appConfigRepFactory, cacheManager);
            var itemService = new ItemServiceImpl(catalogRepFactory, appConfigRepFactory, cacheManager);
            var itemSearchService = new CatalogSearchServiceImpl(catalogRepFactory, itemService, catalogService, categoryService);

            #region VCF dependencies
            var searchConnection = new SearchConnection(ConnectionHelper.GetConnectionString("SearchConnectionString"));
            var elasticSearchProvider = new ElasticSearchProvider(new ElasticSearchQueryBuilder(), searchConnection);

            Func<IReviewRepository> reviewRepFactory = () => new EFReviewRepository("VirtoCommerce");
            Func<IStoreRepository> storeRepFactory = () => new EFStoreRepository("VirtoCommerce");

            #endregion

            #region Dynamic content
            Func<IDynamicContentRepository> dynamicRepositoryFactory = () => new EFDynamicContentRepository("VirtoCommerce");
            Func<IDynamicContentEvaluator> dynamicContentEval = () => new DynamicContentEvaluator(dynamicRepositoryFactory(), null, new HttpCacheRepository());
            Func<IDynamicContentService> dynamicContentServiceFactory = () => new DynamicContentService(dynamicRepositoryFactory(), dynamicContentEval());
            #endregion

            Func<ICatalogOutlineBuilder> catalogOutlineBuilderFactory = () => new CatalogOutlineBuilder(catalogRepFactory(), new HttpCacheRepository());

            var assetBaseUri = new Uri(@"http://virtotest.blob.core.windows.net/");

            _container.RegisterType<ReviewController>(new InjectionConstructor(reviewRepFactory));
            _container.RegisterType<ProductController>(new InjectionConstructor(itemService, elasticSearchProvider, searchConnection, catalogRepFactory, appConfigRepFactory, catalogOutlineBuilderFactory, assetBaseUri));
            _container.RegisterType<ContentController>(new InjectionConstructor(dynamicContentServiceFactory));
            _container.RegisterType<CategoryController>(new InjectionConstructor(itemSearchService, categoryService, propertyService, catalogRepFactory, appConfigRepFactory));
            _container.RegisterType<StoreController>(new InjectionConstructor(storeRepFactory, appConfigRepFactory));
            _container.RegisterType<KeywordController>(new InjectionConstructor(appConfigRepFactory));


        }
    }
}
