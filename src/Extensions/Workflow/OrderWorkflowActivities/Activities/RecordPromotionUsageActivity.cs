﻿using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Foundation.Customers.Services;
using VirtoCommerce.Foundation.Marketing.Model;
using VirtoCommerce.Foundation.Marketing.Repositories;
using VirtoCommerce.Foundation.Orders.Model;

namespace VirtoCommerce.OrderWorkflow
{
    public class RecordPromotionUsageActivity : OrderActivityBase
    {
		ICustomerSessionService _customerSessionService;
		protected ICustomerSessionService CustomerSessionService
		{
			get
			{
				return _customerSessionService ??
					   (_customerSessionService = ServiceLocator.GetInstance<ICustomerSessionService>());
			}
			set
			{
				_customerSessionService = value;
			}
		}

		IMarketingRepository _marketingRepository;
		protected IMarketingRepository MarketingRepository
		{
			get { return _marketingRepository ?? (_marketingRepository = ServiceLocator.GetInstance<IMarketingRepository>()); }
			set
			{
				_marketingRepository = value;
			}
		}

        public PromotionUsageStatus UsageStatus { get; set; }

		public RecordPromotionUsageActivity()
		{
		}

        public RecordPromotionUsageActivity(ICustomerSessionService customerService, IMarketingRepository marketingRepository)
		{
			_marketingRepository = marketingRepository;
			_customerSessionService = customerService;
		}


        protected override void Execute(System.Activities.CodeActivityContext context)
        {
            base.Execute(context);

            if (ServiceLocator == null)
                return;

            if (CurrentOrderGroup == null || CurrentOrderGroup.OrderForms.Count == 0)
                return;

            var currentUsages = MarketingRepository.PromotionUsages.Where(p => p.OrderGroupId == CurrentOrderGroup.OrderGroupId).ToList();

            var usedPromotionIds = new List<string>();

            foreach (var orderForm in CurrentOrderGroup.OrderForms)
            {
                //create records for order form discounts
                usedPromotionIds.AddRange(orderForm.Discounts
                    .Select(formDiscount => UpdatePromotionUsage(currentUsages, formDiscount))
                    .Select(usage => usage.PromotionId));

                //create records for line item discounts
                usedPromotionIds.AddRange(orderForm.LineItems.SelectMany(x => x.Discounts)
                    .Select(lineItemDiscount => UpdatePromotionUsage(currentUsages, lineItemDiscount))
                    .Select(usage => usage.PromotionId));

                //create records for shipment discounts
                usedPromotionIds.AddRange(orderForm.Shipments.SelectMany(x => x.Discounts)
                   .Select(shipmentDiscount => UpdatePromotionUsage(currentUsages, shipmentDiscount))
                   .Select(usage => usage.PromotionId));
            }

            usedPromotionIds = usedPromotionIds.Distinct().ToList();

            //Expire all unused usages (they could be removed from cart)
            foreach (var unusedUsage in currentUsages.Where(x => !usedPromotionIds.Contains(x.PromotionId)))
            {
                unusedUsage.Status = (int)PromotionUsageStatus.Expired;
            }

            MarketingRepository.UnitOfWork.Commit();
        }

        private PromotionUsage UpdatePromotionUsage(ICollection<PromotionUsage> currentUsages, Discount discount)
        {
            var usage = currentUsages.FirstOrDefault(x => x.PromotionId == discount.PromotionId);

            if (usage != null)
            {
                usage.Status = (int)UsageStatus;
                usage.UsageDate = DateTime.UtcNow;
            }
            else
            {
                usage = new PromotionUsage
                {
                    CouponCode = discount.DiscountCode,
                    MemberId = CustomerSessionService.CustomerSession.CustomerId,
                    OrderGroupId = CurrentOrderGroup.OrderGroupId,
                    PromotionId = discount.PromotionId,
                    Status = (int)UsageStatus,
                    UsageDate = DateTime.UtcNow
                };

                //Need to add here too to avoid duplicates
                currentUsages.Add(usage);

                MarketingRepository.Add(usage);
            }

            return usage;
        }
	}
}
