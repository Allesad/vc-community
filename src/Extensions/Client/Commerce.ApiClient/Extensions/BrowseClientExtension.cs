﻿namespace VirtoCommerce.ApiClient.Extensions
{
    #region

    using System;
    using System.Configuration;
    using System.Threading;

    using VirtoCommerce.ApiClient.Utilities;

    #endregion

    public static class BrowseClientExtension
    {
        #region Public Methods and Operators

        public static BrowseClient CreateBrowseClient(this CommerceClients source)
        {
            return source.CreateBrowseClient("samplestore", Thread.CurrentThread.CurrentUICulture.ToString());
        }

        public static BrowseClient CreateBrowseClient(this CommerceClients source, string storeId, string language)
        {
            // http://localhost/admin/api/mp/{0}/{1}/
            var connectionString = String.Format(
                "{0}{1}/{2}/{3}/",
                ClientContext.Configuration.ConnectionString,
                "mp",
                storeId,
                language);
            return CreateBrowseClientWithUri(source, connectionString);
        }

        public static BrowseClient CreateBrowseClientWithUri(this CommerceClients source, string serviceUrl)
        {
            var connectionString = serviceUrl;
            var subscriptionKey = ConfigurationManager.AppSettings["vc-public-apikey"];
            var client = new BrowseClient(
                new Uri(connectionString),
                new AzureSubscriptionMessageProcessingHandler(subscriptionKey, "secret"));
            return client;
        }

        #endregion
    }
}