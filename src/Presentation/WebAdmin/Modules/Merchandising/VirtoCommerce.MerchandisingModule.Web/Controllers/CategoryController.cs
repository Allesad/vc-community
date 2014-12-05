﻿using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Practices.Unity;
using VirtoCommerce.CatalogModule.Services;
using VirtoCommerce.MerchandisingModule.Web.Converters;
using moduleModel = VirtoCommerce.CatalogModule.Model;
using webModel = VirtoCommerce.MerchandisingModule.Web.Model;
namespace VirtoCommerce.MerchandisingModule.Web.Controllers
{
	[RoutePrefix("api/mp/{catalogId}/{language}")]
	public class CategoryController : ApiController
	{
		private readonly ICatalogSearchService _searchService;
		private readonly ICategoryService _categoryService;
		private readonly IPropertyService _propertyService;

		public CategoryController([Dependency("MP")]ICatalogSearchService searchService,
								  [Dependency("MP")]ICategoryService categoryService,
								  [Dependency("MP")]IPropertyService propertyService)
		{
			_searchService = searchService;
			_categoryService = categoryService;
			_propertyService = propertyService;
		}

	
		/// <summary>
		///  GET: api/mp/apple/en-us/categories?parentId='22'
		/// </summary>
		/// <param name="catalogId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		[HttpGet]
		[ResponseType(typeof(webModel.GenericSearchResult<webModel.Category>))]
        [Route("categories")]
		public IHttpActionResult Search(string catalogId, string language="en-us", [FromUri]string parentId = null)
		{
			var criteria = new moduleModel.SearchCriteria
			{
				CatalogId = catalogId,
				CategoryId = parentId,
				Start = 0,
				Count = int.MaxValue,
				ResponseGroup = moduleModel.ResponseGroup.WithCategories
			};
			var result = _searchService.Search(criteria);
			var retVal = new webModel.GenericSearchResult<webModel.Category>
			{
				TotalCount = result.Categories.Count(),
				Items = result.Categories.Select(x => x.ToWebModel()).ToList()
			};
		
			return Ok(retVal);
		}

		
	}
}
