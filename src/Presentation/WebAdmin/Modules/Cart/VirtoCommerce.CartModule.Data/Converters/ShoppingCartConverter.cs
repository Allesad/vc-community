﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CartModule.Data.Model;
using VirtoCommerce.Domain.Cart.Model;
using VirtoCommerce.Foundation.Frameworks.Extensions;
using VirtoCommerce.Foundation.Money;
using Omu.ValueInjecter;
using System.Collections.ObjectModel;

namespace VirtoCommerce.CartModule.Data.Converters
{
	public static class ShoppingCartConverter
	{
		public static ShoppingCart ToCoreModel(this ShoppingCartEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");

			var retVal = new ShoppingCart();
			retVal.InjectFrom(entity);

			if (entity.Currency != null)
			{
				retVal.Currency = (CurrencyCodes)Enum.Parse(typeof(CurrencyCodes), entity.Currency);
			}

			if (entity.Items != null)
			{
				retVal.Items = entity.Items.Select(x => x.ToCoreModel()).ToList();
			}
			if (entity.Addresses != null)
			{
				retVal.Addresses = entity.Addresses.Select(x => x.ToCoreModel()).ToList();
			}
			if (entity.Shipments != null)
			{
				retVal.Shipments = entity.Shipments.Select(x => x.ToCoreModel()).ToList();
			}
			if (entity.Payments != null)
			{
				retVal.Payments = entity.Payments.Select(x => x.ToCoreModel()).ToList();
			}

			return retVal;
		}

		public static ShoppingCartEntity ToEntity(this ShoppingCart order)
		{
			if (order == null)
				throw new ArgumentNullException("order");

			var retVal = new ShoppingCartEntity();
			retVal.InjectFrom(order);

			if (retVal.IsTransient())
			{
				retVal.Id = Guid.NewGuid().ToString();
			}

			if (order.Currency != null)
			{
				retVal.Currency = order.Currency.ToString();
			}

			if (order.Addresses != null)
			{
				retVal.Addresses = new ObservableCollection<AddressEntity>(order.Addresses.Select(x => x.ToEntity()));
			}
			if (order.Items != null)
			{
				retVal.Items = new ObservableCollection<LineItemEntity>(order.Items.Select(x => x.ToEntity()));
			}
			if (order.Shipments != null)
			{
				retVal.Shipments = new ObservableCollection<ShipmentEntity>(order.Shipments.Select(x => x.ToEntity()));
			}
			if (order.Payments != null)
			{
				retVal.Payments = new ObservableCollection<PaymentEntity>(order.Payments.Select(x => x.ToEntity()));
			}
		
			return retVal;
		}


		/// <summary>
		/// Patch CatalogBase type
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		public static void Patch(this ShoppingCartEntity source, ShoppingCartEntity target)
		{
			if (target == null)
				throw new ArgumentNullException("target");


			//Simply properties patch
			if (source.Name != null)
				target.Name = source.Name;
			if (source.Currency  != null)
				target.Currency = source.Currency;
			if (source.CustomerId != null)
				target.CustomerId = source.CustomerId;
			if (source.CustomerName != null)
				target.CustomerName = source.CustomerName;
		
			if(source.IsAnonymous != null)
				target.IsAnonymous = source.IsAnonymous;

			if(source.IsRecuring != null)
				target.IsRecuring = source.IsRecuring;
			if (source.LanguageCode != null)
				target.LanguageCode = source.LanguageCode;
			if (source.Note != null)
				target.Note = source.Note;
			if (source.OrganizationId != null)
				target.OrganizationId = source.OrganizationId;

			target.Total = source.Total;
			target.SubTotal = source.SubTotal;
			target.ShippingTotal = source.ShippingTotal;
			target.HandlingTotal = source.HandlingTotal;
			target.DiscountTotal = source.DiscountTotal;
			target.TaxTotal = source.TaxTotal;
					
			if(!source.Items.IsNullCollection())
			{
				if (target.Items == null)
					target.Items = new ObservableCollection<LineItemEntity>();
				source.Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
			}

			if (!source.Payments.IsNullCollection())
			{
				if (target.Payments == null)
					target.Payments = new ObservableCollection<PaymentEntity>();

				source.Payments.Patch(target.Payments, (sourcePayment, targetPayment) => sourcePayment.Patch(targetPayment));
			}

			if (!source.Addresses.IsNullCollection())
			{
				if (target.Addresses == null)
					target.Addresses = new ObservableCollection<AddressEntity>();

				source.Addresses.Patch(target.Addresses, new AddressComparer(),  (sourceAddress, targetAddress) => sourceAddress.Patch(targetAddress));
			}

			if (!source.Shipments.IsNullCollection())
			{
				if (target.Shipments == null)
					target.Shipments = new ObservableCollection<ShipmentEntity>();

				source.Shipments.Patch(target.Shipments, (sourceShipment, targetShipment) => sourceShipment.Patch(targetShipment));
			}
		}

	}
}
