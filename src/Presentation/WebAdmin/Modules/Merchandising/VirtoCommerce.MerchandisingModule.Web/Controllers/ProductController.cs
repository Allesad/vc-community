﻿using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using VirtoCommerce.CatalogModule.Repositories;
using VirtoCommerce.CatalogModule.Services;
using VirtoCommerce.Foundation.AppConfig.Model;
using VirtoCommerce.Foundation.Catalogs.Search;
using VirtoCommerce.Foundation.Catalogs.Services;
using VirtoCommerce.Foundation.Search;
using VirtoCommerce.MerchandisingModule.Web.Binders;
using VirtoCommerce.MerchandisingModule.Web.Converters;
using VirtoCommerce.MerchandisingModule.Web.Model;
using moduleModel = VirtoCommerce.CatalogModule.Model;

namespace VirtoCommerce.MerchandisingModule.Web.Controllers
{
	[RoutePrefix("api/mp/{catalog}/{language}/products")]
	public class ProductController : ApiController
	{
		private readonly IItemService _itemService;
		private readonly ISearchProvider _searchService;
		private readonly ISearchConnection _searchConnection;
		private readonly Func<IFoundationCatalogRepository> _foundationCatalogRepositoryFactory;
	    private readonly Func<IFoundationAppConfigRepository> _foundationAppConfigRepFactory;
	    private readonly Func<ICatalogOutlineBuilder> _catalogOutlineBuilderFactory;
	    private readonly Uri _assetBaseUri;

		public ProductController(IItemService itemService,
								 ISearchProvider indexedSearchProvider,
								 ISearchConnection searchConnection,
								 Func<IFoundationCatalogRepository> foundationCatalogRepositoryFactory,
                                 Func<IFoundationAppConfigRepository> foundationAppConfigRepFactory,
                                 Func<ICatalogOutlineBuilder> catalogOutlineBuilderFactory,
								 Uri assetBaseUri)
		{
			_searchService = indexedSearchProvider;
			_searchConnection = searchConnection;
			_itemService = itemService;
			_foundationCatalogRepositoryFactory = foundationCatalogRepositoryFactory;
		    _foundationAppConfigRepFactory = foundationAppConfigRepFactory;
		    _catalogOutlineBuilderFactory = catalogOutlineBuilderFactory;
		    _assetBaseUri = assetBaseUri;
		}

        /// <summary>
        /// Searches the specified catalog.
        /// </summary>
        /// <param name="catalog">The catalog.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <param name="outline">The outline.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
	    [HttpGet]
        [Route("")]
		[ResponseType(typeof(GenericSearchResult<CatalogItem>))]
		public IHttpActionResult Search(string catalog, [ModelBinder(typeof(CatalogItemSearchCriteriaBinder))] CatalogItemSearchCriteria criteria,[FromUri]moduleModel.ItemResponseGroup responseGroup = moduleModel.ItemResponseGroup.ItemMedium, [FromUri]string outline="", string language = "en-us")
		{
			criteria.Locale = language;
			criteria.Catalog = catalog.ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(outline))
            {
                criteria.Outlines.Add(String.Format("{0}/{1}*", catalog, outline));
            }
			var result = _searchService.Search(_searchConnection.Scope, criteria) as SearchResults;
			var items = result.GetKeyAndOutlineFieldValueMap<string>();

			var retVal = new GenericSearchResult<CatalogItem> {TotalCount = result.TotalCount};
		    //Load ALL products 
            var products = _itemService.GetByIds(items.Keys.ToArray(), responseGroup);

            foreach (var product in products)
            {
                var webModelProduct = product.ToWebModel(_assetBaseUri);

                var searchTags = items[product.Id];

                var catalogPath = criteria.Catalog + "/";

                webModelProduct.Outline =
                    searchTags[criteria.OutlineField].ToString()
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault(x => x.StartsWith(catalogPath, StringComparison.OrdinalIgnoreCase))
                    ?? string.Empty;

                webModelProduct.Outline = webModelProduct.Outline.Replace(catalogPath, "");

                int reviewTotal;
                if (searchTags.ContainsKey(criteria.ReviewsTotalField)
                    && int.TryParse(searchTags[criteria.ReviewsTotalField].ToString(), out reviewTotal))
                {
                    webModelProduct.ReviewsTotal = reviewTotal;
                }
                double reviewAvg;
                if (searchTags.ContainsKey(criteria.ReviewsAverageField)
                    && double.TryParse(searchTags[criteria.ReviewsAverageField].ToString(), out reviewAvg))
                {
                    webModelProduct.Rating = reviewAvg;
                }

                retVal.Items.Add(webModelProduct);
            }

            return Ok(retVal);
		}

		/// GET: api/mp/apple/en-us/products?code='22'
		[HttpGet]
		[ResponseType(typeof(Product))]
		[Route("")]
        public IHttpActionResult GetProductByCode(string catalog, [FromUri]string code, [FromUri]moduleModel.ItemResponseGroup responseGroup = moduleModel.ItemResponseGroup.ItemLarge, string language = "en-us")
		{
			using(var repository = _foundationCatalogRepositoryFactory())
			{
                //Cannot filter by catalogId here because it fails when catalog is virtual
				var itemId = repository.Items.Where(x => x.Code == code).Select(x => x.ItemId).FirstOrDefault();
				if(itemId != null)
				{
					return GetProduct(catalog, itemId,responseGroup);
				}
			}
			return StatusCode(HttpStatusCode.NotFound);
		}



		[HttpGet]
		[ResponseType(typeof(Product))]
		[Route("{product}")]
        public IHttpActionResult GetProduct(string catalog, string product, [FromUri]moduleModel.ItemResponseGroup responseGroup = moduleModel.ItemResponseGroup.ItemLarge, string language = "en-us")
		{
            var result = _itemService.GetById(product, responseGroup);

            if (result == null)
            {
                //Lets treat product as slug
                using (var appConfigRepo = _foundationAppConfigRepFactory())
                {
                    var keyword = appConfigRepo.SeoUrlKeywords.FirstOrDefault(x => x.KeywordType == (int)SeoUrlKeywordTypes.Item
                        && x.Keyword.Equals(product, StringComparison.InvariantCultureIgnoreCase));

                    if (keyword != null)
                    {
                        result = _itemService.GetById(keyword.KeywordValue, responseGroup);
                    }
                }
            }

            if (result != null)
		    {
                var webModelProduct = result.ToWebModel(_assetBaseUri);
                //Build category path outline for requested catalog, can be virtual catalog as well
                webModelProduct.Outline = _catalogOutlineBuilderFactory().BuildCategoryOutline(catalog, result.Id).ToString("/").ToLowerInvariant();
                webModelProduct.Outline = webModelProduct.Outline.Replace(catalog + "/", "");
                return Ok(webModelProduct);
		    }
		    return StatusCode(HttpStatusCode.NotFound);
		}
	}
}
