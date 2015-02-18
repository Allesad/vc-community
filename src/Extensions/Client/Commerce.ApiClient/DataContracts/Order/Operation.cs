﻿using System;

namespace VirtoCommerce.ApiClient.DataContracts.Order
{
    public class Operation
    {
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ModifiedBy { get; set; }

        public string Number { get; set; }

        public bool IsApproved { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public string Currency { get; set; }

        public bool TaxIncluded { get; set; }

        public decimal Sum { get; set; }

        public decimal Tax { get; set; }
    }
}