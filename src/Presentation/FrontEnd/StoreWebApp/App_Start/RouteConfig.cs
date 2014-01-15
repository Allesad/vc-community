﻿using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using VirtoCommerce.Foundation.Catalogs.Model;
using VirtoCommerce.Web.Client.Extensions;
using VirtoCommerce.Web.Client.Extensions.Routing;
using VirtoCommerce.Web.Client.Extensions.Routing.Constraints;
using VirtoCommerce.Web.Client.Extensions.Routing.Routes;

namespace VirtoCommerce.Web
{
    /// <summary>
    /// Class RouteConfig.
    /// </summary>
    public class RouteConfig
    {

        private static RouteValueDictionary CreateRouteValueDictionary(object values)
        {
            var dictionary = values as IDictionary<string, object>;
            if (dictionary != null)
            {
                return new RouteValueDictionary(dictionary);
            }

            return new RouteValueDictionary(values);
        }
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("virto/services/{MyJob}.svc/{*pathInfo}");
            routes.IgnoreRoute("virto/dataservices/{MyJob}.svc/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute(".html");

            routes.MapRoute(
                "FailWhale",
                "FailWhale/{action}/{id}", new { controller = "Error", action = "FailWhale", id = UrlParameter.Optional });

            routes.MapRoute(
              "Assets",
              "asset/{*path}",
              new { controller = "Asset", action = "Index", path = UrlParameter.Optional }
          );

            var itemRoute = new NormalizeRoute(
                new ItemRoute(Constants.ItemRoute,
                CreateRouteValueDictionary(
                    new
                    {
                        controller = "Catalog",
                        action = "DisplayItem"
                    }),
                    new RouteValueDictionary
                    {
                        {Constants.Language, new LanguageRouteConstraint()},
                        {Constants.Store, new StoreRouteConstraint()},
                        {Constants.Category, new CategoryRouteConstraint()},
                        {Constants.Item, new ItemRouteConstraint()}
                    },
                    new RouteValueDictionary { { "namespaces", new[] { "VirtoCommerce.Web.Controllers" } } },
                new MvcRouteHandler()));

            var categoryRoute = new NormalizeRoute(
                new CategoryRoute(Constants.CategoryRoute,
                CreateRouteValueDictionary(
                    new
                    {
                        controller = "Catalog",
                        action = "Display"
                    }),
                 new RouteValueDictionary
                    {
                        {Constants.Language, new LanguageRouteConstraint()},
                        {Constants.Store, new StoreRouteConstraint()},
                        {Constants.Category, new CategoryRouteConstraint()}
                    },
                new RouteValueDictionary { { "namespaces", new[] { "VirtoCommerce.Web.Controllers" } } },
                new MvcRouteHandler()));

            var storeRoute = new NormalizeRoute(
                new StoreRoute(Constants.StoreRoute,
                 new RouteValueDictionary
                    {
                        {"controller", "Home"},
                        {"action", "Index"}
                    },
                new RouteValueDictionary
                    {
                        {Constants.Language, new LanguageRouteConstraint()},
                        {Constants.Store, new StoreRouteConstraint()}
                    },
                new RouteValueDictionary { { "namespaces", new[] { "VirtoCommerce.Web.Controllers" } } },
                new MvcRouteHandler()));

            routes.Add("Item", itemRoute);
            routes.Add("Category", categoryRoute);
            routes.Add("Store", storeRoute);

            //Legacy redirects
            routes.Redirect(r => r.MapRoute("old_Category", string.Format("c/{{{0}}}", Constants.Category))).To(categoryRoute);
            routes.Redirect(r => r.MapRoute("old_Item", string.Format("p/{{{0}}}", Constants.Item))).To(itemRoute);

            var otherRoute = new NormalizeRoute(new Route(string.Format("{{{0}}}/{{controller}}/{{action}}/{{id}}", Constants.Language),
                CreateRouteValueDictionary(new { id = UrlParameter.Optional }),
                new RouteValueDictionary
                    {
                        {Constants.Language, new LanguageRouteConstraint()}
                    },
                    new RouteValueDictionary { { "namespaces", new[] { "VirtoCommerce.Web.Controllers" } } },
                    new MvcRouteHandler()));

            //Other actions
            routes.Add("Default", otherRoute);

            //Needed for some post requests
            routes.MapRoute(
                "Default_Fallback", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new
                {
                    id = UrlParameter.Optional
                }, // Parameter defaults
                new[] { "VirtoCommerce.Web.Controllers" });
        }
    }
}