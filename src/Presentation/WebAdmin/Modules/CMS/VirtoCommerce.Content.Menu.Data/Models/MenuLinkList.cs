﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Foundation.Frameworks;

namespace VirtoCommerce.Content.Menu.Data.Models
{
	public class MenuLinkList : Entity, IAuditable
	{
		public MenuLinkList()
		{
			MenuLinks = new NullCollection<MenuLink>();
		}

		[Required]
		public string Name { get; set; }
		[Required]
		public string StoreId { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; }

		[Required]
		public string CreatedBy { get; set; }

		public DateTime? ModifiedDate { get; set; }

		public string ModifiedBy { get; set; }

		public virtual ICollection<MenuLink> MenuLinks { get; set; }
	}
}
