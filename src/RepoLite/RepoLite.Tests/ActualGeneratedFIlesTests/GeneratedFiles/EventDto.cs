using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.Models
{
	[Table("Event", Schema="dbo")]
	public partial class Event : BaseModel
	{
		private String _eventId;
		private String _eventName;

		[Key]
		public virtual String EventId
		{
			get => _eventId;
			set => SetValue(ref _eventId, value);
		}
		public virtual String EventName
		{
			get => _eventName;
			set => SetValue(ref _eventName, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(EventId))
				validationErrors.Add(new ValidationError(nameof(EventId), "Value cannot be null"));
			if (!string.IsNullOrEmpty(EventId) && EventId.Length > 20)
				validationErrors.Add(new ValidationError(nameof(EventId), "Max length is 20"));
			if (string.IsNullOrEmpty(EventName))
				validationErrors.Add(new ValidationError(nameof(EventName), "Value cannot be null"));
			if (!string.IsNullOrEmpty(EventName) && EventName.Length > 100)
				validationErrors.Add(new ValidationError(nameof(EventName), "Max length is 100"));

			return validationErrors;
		}
	}
}

