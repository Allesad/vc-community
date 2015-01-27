﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Foundation.Money;

namespace VirtoCommerce.Domain.Order.Model
{
	public class CustomerOrderItem : Position
	{
		public string DisplayName { get; set; }
		public string PreviewImageUrl { get; set; }
		public string ThumbnailImageUrl { get; set; }
		public bool IsGift { get; set; }
		public string ShippingMethodCode { get; set; }
		public string FulfilmentLocationCode { get; set; }
	}
}
