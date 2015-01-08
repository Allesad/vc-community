﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Foundation.Frameworks;

namespace VirtoCommerce.CatalogModule.Web.Model
{
	public class Coupon
	{
		public string CouponCode { get; set; }
		public bool IsValid { get; set; }
		public string InvalidDescription { get; set; }
	}
}
