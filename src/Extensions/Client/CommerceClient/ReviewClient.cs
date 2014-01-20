﻿using System.Linq;
using VirtoCommerce.Foundation.Frameworks;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using VirtoCommerce.Foundation.Reviews;
using VirtoCommerce.Foundation.Reviews.Model;
using VirtoCommerce.Foundation.Reviews.Repositories;

namespace VirtoCommerce.Client
{
    public class ReviewClient
    {
        #region Cache Constants
        public const string ReviewsCacheKey = "R:all";
        public const string ReviewCommentsCacheKey = "RC:{0}";
        #endregion

        #region Private Variables
        private readonly bool _isEnabled;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICacheRepository _cacheRepository;
        #endregion

        public ReviewClient(IReviewRepository reviewRepository, ICacheRepository cacheRepository)
        {
            _reviewRepository = reviewRepository;
            _cacheRepository = cacheRepository;
            _isEnabled = ReviewConfiguration.Instance.Cache.IsEnabled;
        }

        /// <summary>
        /// Gets the reviews.
        /// </summary>
        /// <returns></returns>
        public Review[] GetReviews()
        {
            var query = _reviewRepository.Reviews.Where(r => r.Status == (int) ReviewStatus.Approved).ExpandAll();
            return CacheHelper.Get(string.Format(ReviewsCacheKey),
                query.ToArray, 
                ReviewConfiguration.Instance.Cache.ReviewsTimeout, 
                _isEnabled);
        }

        /// <summary>
        /// Gets the review comments.
        /// </summary>
        /// <param name="reviewId">The review identifier.</param>
        /// <returns></returns>
        public ReviewComment[] GetReviewComments(string reviewId)
        {
            var query =
                _reviewRepository.ReviewComments.Where(
                    r => r.ReviewId == reviewId && r.Status == (int) ReviewStatus.Approved);
            return CacheHelper.Get(string.Format(ReviewCommentsCacheKey, reviewId),
                query.ToArray,
                ReviewConfiguration.Instance.Cache.ReviewsTimeout,
                _isEnabled);
        }

        CacheHelper _cacheHelper;
        public CacheHelper CacheHelper
        {
            get { return _cacheHelper ?? (_cacheHelper = new CacheHelper(_cacheRepository)); }
        }
    }
}
