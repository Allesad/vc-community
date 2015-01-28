﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Foundation.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using VirtoCommerce.OrderModule.Data.Model;
using VirtoCommerce.Domain.Order.Model;
using System.Data.Entity;

namespace VirtoCommerce.OrderModule.Data.Repositories
{
	public class OrderRepositoryImpl : EFRepositoryBase, IOrderRepository
	{
		public OrderRepositoryImpl(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			Database.SetInitializer<OrderRepositoryImpl>(null);
			Configuration.LazyLoadingEnabled = false;

		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

			#region CustomerOrder
			modelBuilder.Entity<CustomerOrderEntity>().HasKey(x => x.Id)
					.Property(x => x.Id);

			modelBuilder.Entity<CustomerOrderEntity>().HasOptional(x => x.Discount)
													  .WithRequired(x=>x.CustomerOrder);

			modelBuilder.Entity<CustomerOrderEntity>().ToTable("order_CustomerOrder");
			#endregion

			#region LineItem
			modelBuilder.Entity<LineItemEntity>().HasKey(x => x.Id)
					.Property(x => x.Id);

			modelBuilder.Entity<LineItemEntity>().Property(x => x.CustomerOrderId).HasColumnName("CustomerOrder_Id");
			modelBuilder.Entity<LineItemEntity>().Property(x => x.ShipmentId).HasColumnName("Shipment_Id");

			modelBuilder.Entity<LineItemEntity>().HasOptional(x => x.Discount)
												.WithRequired(x => x.LineItem);

			modelBuilder.Entity<LineItemEntity>().HasOptional(x=>x.CustomerOrder)
									   .WithMany(x => x.Items)
									   .HasForeignKey(x => x.CustomerOrderId);

			modelBuilder.Entity<LineItemEntity>().HasOptional(x => x.Shipment)
									   .WithMany(x => x.Items)
									   .HasForeignKey(x => x.ShipmentId);

			modelBuilder.Entity<LineItemEntity>().ToTable("order_LineItem");
			#endregion

			#region Shipment
			modelBuilder.Entity<ShipmentEntity>().HasKey(x => x.Id)
				.Property(x => x.Id);

			modelBuilder.Entity<ShipmentEntity>().Property(x => x.CustomerOrderId).HasColumnName("CustomerOrder_Id");


			modelBuilder.Entity<ShipmentEntity>().HasRequired(x => x.CustomerOrder)
										   .WithMany(x => x.Shipments)
										   .HasForeignKey(x => x.CustomerOrderId);

			modelBuilder.Entity<ShipmentEntity>().HasOptional(x => x.Discount)
												.WithRequired(x => x.Shipment);


			modelBuilder.Entity<ShipmentEntity>().ToTable("order_Shipment");
			#endregion

			#region Address
			modelBuilder.Entity<AddressEntity>().HasKey(x => x.Id)
				.Property(x => x.Id);

			modelBuilder.Entity<AddressEntity>().Property(x => x.CustomerOrderId).HasColumnName("CustomerOrder_Id");
			modelBuilder.Entity<AddressEntity>().Property(x => x.ShipmentId).HasColumnName("Shipment_Id");
			modelBuilder.Entity<AddressEntity>().Property(x => x.PaymentInId).HasColumnName("PaymentIn_Id");

			modelBuilder.Entity<AddressEntity>().HasOptional(x => x.CustomerOrder)
									   .WithMany(x => x.Addresses)
									   .HasForeignKey(x => x.CustomerOrderId);

			modelBuilder.Entity<AddressEntity>()
				  .HasOptional(x=> x.Shipment) // Mark  is optional 
				  .WithRequired(x => x.DeliveryAddress); // Create inverse relationship

			modelBuilder.Entity<AddressEntity>()
				  .HasOptional(x => x.PaymentIn) // Mark  is optional 
				  .WithRequired(x => x.BillingAddress); // Create inverse relationship

			modelBuilder.Entity<AddressEntity>().ToTable("order_Address");
			#endregion

			#region PaymentIn
			modelBuilder.Entity<PaymentInEntity>().HasKey(x => x.Id)
				.Property(x => x.Id);

			modelBuilder.Entity<PaymentInEntity>().Property(x => x.CustomerOrderId).HasColumnName("CustomerOrder_Id");
			modelBuilder.Entity<PaymentInEntity>().Property(x => x.ShipmentId).HasColumnName("Shipment_Id");

			modelBuilder.Entity<PaymentInEntity>().HasOptional(x => x.CustomerOrder)
									   .WithMany(x => x.InPayments)
									   .HasForeignKey(x => x.CustomerOrderId);

			modelBuilder.Entity<PaymentInEntity>().HasOptional(x => x.Shipment)
									   .WithMany(x => x.InPayments)
									   .HasForeignKey(x => x.ShipmentId);

			modelBuilder.Entity<PaymentInEntity>().ToTable("order_PaymentIn");
			#endregion

			#region Discount
			modelBuilder.Entity<DiscountEntity>().HasKey(x => x.Id);
		
			modelBuilder.Entity<DiscountEntity>().ToTable("order_Discount");
			#endregion
		}

		#region IOrderRepository Members

		public IQueryable<CustomerOrderEntity> CustomerOrders
		{
			get { return GetAsQueryable<CustomerOrderEntity>(); }
		}

		public IQueryable<ShipmentEntity> Shipments
		{
			get { return GetAsQueryable<ShipmentEntity>(); }
		}

		public IQueryable<PaymentInEntity> InPayments
		{
			get { return GetAsQueryable<PaymentInEntity>(); }
		}

		public CustomerOrderEntity GetCustomerOrderById(string id, ResponseGroup responseGroup)
		{
			return CustomerOrders.Include(x => x.Shipments)
								 .Include(x => x.InPayments)
								 .Include(x => x.Items)
								 .FirstOrDefault(x => x.Id == id);
			
		}

		public ShipmentEntity GetShipmentById(string id, ResponseGroup responseGroup)
		{
			throw new NotImplementedException();
		}

		public PaymentInEntity GetInPaymentById(string id, ResponseGroup responseGroup)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
