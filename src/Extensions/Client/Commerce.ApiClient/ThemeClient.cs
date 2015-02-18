﻿#region

using System;
using System.Net.Http;
using System.Threading.Tasks;
using VirtoCommerce.ApiClient.DataContracts.Themes;
using VirtoCommerce.ApiClient.Utilities;

#endregion

namespace VirtoCommerce.ApiClient
{

    #region

    #endregion

    public class ThemeClient : BaseClient
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the ThemeClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="appId">The API application ID.</param>
        /// <param name="secretKey">The API secret key.</param>
        public ThemeClient(Uri adminBaseEndpoint, string appId, string secretKey)
            : base(adminBaseEndpoint, new HmacMessageProcessingHandler(appId, secretKey))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the ThemeClient class.
        /// </summary>
        /// <param name="adminBaseEndpoint">Admin endpoint</param>
        /// <param name="handler"></param>
        public ThemeClient(Uri adminBaseEndpoint, MessageProcessingHandler handler)
            : base(adminBaseEndpoint, handler)
        {
        }

        #endregion

        #region Public Methods and Operators

        public Task<ThemeAsset[]> GetThemeAssetsAsync(string storeId, string themeId)
        {
            return
                GetAsync<ThemeAsset[]>(
                    CreateRequestUri(
                        String.Format(RelativePaths.ThemeAssets, storeId, themeId)));
        }

        public Task<Theme[]> GetThemesAsync(string storeId)
        {
            return
                GetAsync<Theme[]>(
                    CreateRequestUri(
                        String.Format(RelativePaths.Themes, storeId)));
        }

        #endregion

        protected class RelativePaths
        {
            #region Constants

            public const string ThemeAssets = "{0}/themes/{1}/assets";
            public const string Themes = "{0}/themes";

            #endregion
        }
    }
}
