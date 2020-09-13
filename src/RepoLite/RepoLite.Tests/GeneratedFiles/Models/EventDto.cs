using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class EventDto : BaseModel
	{
		public override string EntityName => "Event";
		public static string CacheKey(object identifier) => $"Event_{identifier}";
		protected string _eventid;
		protected string _eventname;

		public string EventId
		{
			get => _eventid;
			set => SetValue(ref _eventid, value);
		}
		public string EventName
		{
			get => _eventname;
			set => SetValue(ref _eventname, value);
		}
		public EventDto() { }
		public EventDto(
			string eventid,
			string eventname)
		{
			_eventid = eventid;
			_eventname = eventname;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_eventid = row.GetText($"{propertyPrefix}EventId");
			_eventname = row.GetText($"{propertyPrefix}EventName");
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (EventId == null)
				validationErrors.Add(new ValidationError(nameof(EventId), "Value cannot be null"));
			if (!string.IsNullOrEmpty(EventId) && EventId.Length > 20)
				validationErrors.Add(new ValidationError(nameof(EventId), "Max length is 20"));
			if (EventName == null)
				validationErrors.Add(new ValidationError(nameof(EventName), "Value cannot be null"));
			if (!string.IsNullOrEmpty(EventName) && EventName.Length > 100)
				validationErrors.Add(new ValidationError(nameof(EventName), "Max length is 100"));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "Event";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("EventId", typeof(string), "[NVARCHAR](20)", SqlDbType.NVarChar, false, true, false),
			new ColumnDefinition("EventName", typeof(string), "[NVARCHAR](100)", SqlDbType.NVarChar, false, false, false),
		};
	}
}

