﻿using System;
using System.Collections.Generic;

namespace VirtoCommerce.ApiClient.DataContracts.Order
{
    public class PaymentIn : Operation
    {
        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string CustomerId { get; set; }

        public DateTime? IncomingDate { get; set; }

        public string OuterId { get; set; }

        public string Purpose { get; set; }

        public string GatewayCode { get; set; }

        public List<Address> Addresses { get; set; }

        public string CustomerOrderId { get; set; }

        public string ShipmentId { get; set; }
    }
}