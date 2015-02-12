﻿namespace VirtoCommerce.ApiClient.Extensions
{
    #region

    using System;
    using System.Configuration;

    using VirtoCommerce.ApiClient.Utilities;

    #endregion

    public static class SeoClientExtensions
    {
        #region Public Methods and Operators

        public static SeoClient CreateDefaultSeoClient(this CommerceClients source)
        {
            var connectionString = String.Format("{0}{1}/", ClientContext.Configuration.ConnectionString, "mp");
            return CreateSeoClient(source, connectionString);
        }

        public static SeoClient CreateSeoClient(this CommerceClients source, string serviceUrl)
        {
            var connectionString = serviceUrl;
            var subscriptionKey = ConfigurationManager.AppSettings["vc-public-apikey"];
            var client = new SeoClient(
                new Uri(connectionString),
                new AzureSubscriptionMessageProcessingHandler(subscriptionKey, "secret"));
            return client;
        }

        #endregion
    }
}