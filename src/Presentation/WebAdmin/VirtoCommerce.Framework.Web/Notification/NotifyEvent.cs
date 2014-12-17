﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Framework.Web.Notification
{
	public class NotifyEvent
	{
		public NotifyEvent(string creator)
		{
			Created = DateTime.UtcNow;
			New = true;
			Id = Guid.NewGuid().ToString();
			Creator = creator;
			ExtendedProperties = new Dictionary<string, string>();
		}
		public string Id { get; set; }
		public string Creator { get; set; }
		public DateTime Created { get; set; }
		public DateTime? FinishDate { get; set; }

		public bool New { get; set; }
		public string NotifyType { get; set; }
		public NotifyStatus Status { get; set; }

		public string Description { get; set; }
		public string Title { get; set; }
		public int RepeatCount { get; set; }
		public IDictionary<string, string> ExtendedProperties { get; set; }
	}
}
