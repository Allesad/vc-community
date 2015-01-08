﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using VirtoCommerce.Foundation.Money;
using coreModel = VirtoCommerce.Domain.Cart.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
	public static class CartItemConverter
	{
		public static webModel.CartItem ToWebModel(this coreModel.CartItem cartItem)
		{
			var retVal = new webModel.CartItem();
			retVal.InjectFrom(cartItem);
			retVal.Currency = cartItem.Currency;
			if (cartItem.Discounts != null)
				retVal.Discounts = cartItem.Discounts.Select(x => x.ToWebModel()).ToList();
			return retVal;
		}

		public static coreModel.CartItem ToCoreModel(this webModel.CartItem cartItem)
		{
			var retVal = new coreModel.CartItem();
			retVal.InjectFrom(cartItem);

			retVal.Currency = cartItem.Currency;

	
			if (retVal.IsTransient())
			{
				retVal.Id = Guid.NewGuid().ToString();
			}

			if(cartItem.Discounts != null)
				retVal.Discounts = cartItem.Discounts.Select(x => x.ToCoreModel()).ToList();
			return retVal;
		}


	}
}
