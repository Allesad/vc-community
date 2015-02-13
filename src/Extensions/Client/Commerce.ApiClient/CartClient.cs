﻿namespace VirtoCommerce.ApiClient
{
    #region

    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using VirtoCommerce.ApiClient.DataContracts.Cart;
    using VirtoCommerce.ApiClient.Utilities;

    #endregion

    public class CartClient : BaseClient
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the CartClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="token">Access token</param>
        public CartClient(Uri adminBaseEndpoint, string token)
            : base(adminBaseEndpoint, new TokenMessageProcessingHandler(token))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the CartClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="handler"></param>
        public CartClient(Uri adminBaseEndpoint, MessageProcessingHandler handler)
            : base(adminBaseEndpoint, handler)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the current cart
        /// </summary>
        public Task<ShoppingCart> GetCurrentCartAsync()
        {
            return this.GetAsync<ShoppingCart>(
                this.CreateRequestUri(string.Format(RelativePaths.CurrentCart, "samplestore")),
                useCache: false); // service should already know the cart

            // TODO: remove storeid from the API's
        }

        public Task<ShoppingCart> UpdateCurrentCartAsync(ShoppingCart cart)
        {
            return this.SendAsync<ShoppingCart, ShoppingCart>(
                this.CreateRequestUri(RelativePaths.UpdateCart),
                HttpMethod.Put,
                cart);
        }

        #endregion

        protected class RelativePaths
        {
            #region Constants

            public const string CurrentCart = "cart/{0}/carts/current";

            public const string UpdateCart = "cart/carts";

            #endregion
        }
    }
}