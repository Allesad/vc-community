﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Notification
{
	public class Notification
	{
		private readonly INotificationSendingGateway _notificationSendingGateway;

		public Notification()
		{
			Id = Guid.NewGuid().ToString("N");
			NotificationTemplate = new NotificationTemplate();
			Type = this.GetType().FullName;
		}

		public Notification(INotificationSendingGateway notificationSendingGateway)
		{
			Id = Guid.NewGuid().ToString("N");
			NotificationTemplate = new NotificationTemplate();
			_notificationSendingGateway = notificationSendingGateway;
			Type = this.GetType().FullName;
		}

		public string Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public string CreatedBy { get; set; }
		public string ModifiedBy { get; set; }

		public string Type { get; set; }

		/// <summary>
		/// Must be made sending
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Notification was successfully sent
		/// </summary>
		public bool IsSuccessSend { get; set; }

		public string ObjectId { get; set; }

		/// <summary>
		/// Subject of notification
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// Body of notification
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Sender info (e-mail, phone number and etc.) of notification
		/// </summary>
		public string Sender { get; set; }

		/// <summary>
		/// Recipient info (e-mail, phone number and etc.) of notification
		/// </summary>
		public string Recipient { get; set; }

		/// <summary>
		/// Number of current attemp
		/// </summary>
		public int AttemptCount { get; set; }

		/// <summary>
		/// Maximum number of attempts to send a message
		/// </summary>
		public int MaxAttemptCount { get; set; }

		/// <summary>
		/// Last fail attempt error message
		/// </summary>
		public string LastFailAttemptMessage { get; set; }

		/// <summary>
		/// Date of last fail attempt
		/// </summary>
		public DateTime? LastFailAttemptDate { get; set; }

		/// <summary>
		/// Date of start sending notification
		/// </summary>
		public DateTime? StartSendingDate { get; set; }

		/// <summary>
		/// Date of success sent result
		/// </summary>
		public DateTime? SentDate { get; set; }

		public INotificationSendingGateway NotificationSendingGateway { get { return _notificationSendingGateway; } }

		public NotificationTemplate NotificationTemplate { get; set; }
	}
}
