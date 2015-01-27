﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.OrderModule.Web.Model
{
	public class CustomerOrderItem
	{
		public string Id { get; set; }

		public DateTime CreatedDate { get; set; }
		public string CreatedBy { get; set; }
		public DateTime ModifiedDate { get; set; }
		public string ModifiedBy { get; set; }

		/// <summary>
		/// Price with tax and without dicount
		/// </summary>
		public decimal BasePrice { get; set; }
		/// <summary>
		/// Price with tax and discount
		/// </summary>
		public decimal Price { get; set; }
		/// <summary>
		/// Static discount amount
		/// </summary>
		public decimal StaticDiscount { get; set; }
		/// <summary>
		/// Tax sum
		/// </summary>
		public decimal Tax { get; set; }

		/// <summary>
		/// Reserve quantity
		/// </summary>
		public long ReserveQuantity { get; set; }
		public long Quantity { get; set; }

		public string ItemId { get; set; }
		public string CatalogId { get; set; }
		public string CategoryId { get; set; }

		public string Name { get; set; }

		public string ImageUrl { get; set; }

		public string DisplayName { get; set; }

		public bool IsGift { get; set; }
		public string ShippingMethodCode { get; set; }
		public string FulfilmentLocationCode { get; set; }

		public Discount Discount { get; set; }
	}
}