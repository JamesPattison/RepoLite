using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Base;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class Event : BaseModel
	{
		public override string EntityName => "Event";
		private String _eventid;
		private String _eventname;

		public String EventId
		{
			get => _eventid;
			set => SetValue(ref _eventid, value);
		}
		public String EventName
		{
			get => _eventname;
			set => SetValue(ref _eventname, value);
		}
		public Event() { }
		public Event(
			String eventid,
			String eventname)
		{
			_eventid = eventid;
			_eventname = eventname;
		}
		public Event(params object[] csvValues)
		{
			if (csvValues.Length != 2) throw new Exception("Could not parse Csv");
			EventId = Cast<String>(csvValues[0]);
			EventName = Cast<String>(csvValues[1]);
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
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("EventId", typeof(System.String), "[NVARCHAR](20)", SqlDbType.NVarChar, false, true, false),
			new ColumnDefinition("EventName", typeof(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, false, false, false),
		};
	}
}

