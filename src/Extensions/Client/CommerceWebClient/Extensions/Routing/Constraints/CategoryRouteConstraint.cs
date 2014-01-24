﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using VirtoCommerce.Client;
using VirtoCommerce.Foundation.AppConfig.Model;
using VirtoCommerce.Foundation.Catalogs.Services;
using VirtoCommerce.Web.Client.Helpers;

namespace VirtoCommerce.Web.Client.Extensions.Routing.Constraints
{

    /// <summary>
    /// Route constraint checks if category exists in database
    /// </summary>
    public class CategoryRouteConstraint : BaseRouteConstraint
    {

        protected override bool IsMatch(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!base.IsMatch(httpContext, route, parameterName, values, routeDirection))
            {
                return false;
            }

            var encoded = values[parameterName].ToString();
            var decoded = SettingsHelper.SeoDecodeMultiVal(encoded, SeoUrlKeywordTypes.Category, 
                values.ContainsKey(Constants.Language) ? values[Constants.Language].ToString() : null);

            var childCategryCode = decoded.Split(new[] {'/'}).Last();

            var outline = new BrowsingOutline(CartHelper.CatalogOutlineBuilder.BuildCategoryOutline(StoreHelper.CustomerSession.CatalogId, CartHelper.CatalogClient.GetCategory(childCategryCode)));

            return ValidateCategoryPath(outline, decoded);
        }

    }
}
