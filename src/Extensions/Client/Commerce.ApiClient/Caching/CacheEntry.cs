﻿namespace VirtoCommerce.ApiClient.Caching
{
    internal class CacheEntry
    {
        #region Fields

        private readonly object _Lock;

        #endregion

        #region Constructors and Destructors

        internal CacheEntry()
        {
            this._Lock = new object();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the lock.
        /// </summary>
        /// <value>The lock.</value>
        public object Lock
        {
            get
            {
                return this._Lock;
            }
        }

        #endregion
    }
}