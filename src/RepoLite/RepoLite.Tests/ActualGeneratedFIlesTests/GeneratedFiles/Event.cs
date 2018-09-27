using NS.Base;
using NS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Dapper;

namespace NS
{
	public partial interface IEventRepository : IPkRepository<Event>
	{
		Event Get(String eventId);
		IEnumerable<Event> Get(List<String> eventIds);
		IEnumerable<Event> Get(params String[] eventIds);

		bool Update(Event item);
		bool Delete(String eventId);
		bool Delete(IEnumerable<String> eventIds);
		bool Merge(List<Event> items);

		IEnumerable<Event> Search(
			String eventId = null,
			String eventName = null);

		IEnumerable<Event> FindByEventName(String eventName);
		IEnumerable<Event> FindByEventName(FindComparison comparison, String eventName);
	}
	public sealed partial class EventRepository : BaseRepository<Event>, IEventRepository
	{
		public EventRepository(string connectionString) : this(connectionString, exception => { }) { }
		public EventRepository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,
			"dbo", "Event", 2)
		{
			Columns.Add(new ColumnDefinition("EventId", typeof(System.String), "[NVARCHAR](20)", false, true, false));
			Columns.Add(new ColumnDefinition("EventName", typeof(System.String), "[NVARCHAR](100)", false, false, false));
		}

		public Event Get(String eventId)
		{
			return Where("EventId", Comparison.Equals, eventId).Results().FirstOrDefault();
		}

		public IEnumerable<Event> Get(List<String> eventIds)
		{
			return Get(eventIds.ToArray());
		}

		public IEnumerable<Event> Get(params String[] eventIds)
		{
			return Where("EventId", Comparison.In, eventIds).Results();
		}

		public override bool Create(Event item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.EventId, item.EventName);
			if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))
				return false;

			item.EventId = (String)createdKeys[nameof(Event.EventId)];
			item.ResetDirty();

			return true;
		}

		public override bool BulkCreate(params Event[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.EventId, item.EventName); 
			}

			return BulkInsert(dt);
		}
		public override bool BulkCreate(List<Event> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(Event item)
		{
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.EventId, item.EventName);

			if (success)
			item.ResetDirty();

			return success;
		}
		public bool Delete(Event @event)
		{
			if (@event == null)
				return false;

			var deleteColumn = new DeleteColumn("EventId", @event.EventId);

			return BaseDelete(deleteColumn);
		}
		public bool Delete(IEnumerable<Event> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.EventId);
			}

			return BaseDelete("EventId", deleteValues);
		}

		public bool Delete(String eventId)
		{
			return Delete(new Event { EventId = eventId });
		}


		public bool Delete(IEnumerable<String> eventIds)
		{
			return Delete(eventIds.Select(x => new Event { EventId = x }));
		}


		public bool Merge(List<Event> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.EventId,
					item.EventName, item.DirtyColumns.Contains("EventName")
				});
			}
			return BaseMerge(mergeTable);
		}

		protected override Event ToItem(DataRow row)
		{
			 var item = new Event
			{
				EventId = GetString(row, "EventId"),
				EventName = GetString(row, "EventName"),
			};

			item.ResetDirty();
			return item;
		}

		public IEnumerable<Event> Search(
			String eventId = null,
			String eventName = null)
		{
			var queries = new List<QueryItem>(); 

			if (!string.IsNullOrEmpty(eventId))
				queries.Add(new QueryItem("EventId", eventId));
			if (!string.IsNullOrEmpty(eventName))
				queries.Add(new QueryItem("EventName", eventName));

			return BaseSearch(queries);
		}

		public IEnumerable<Event> FindByEventName(String eventName)
		{
			return FindByEventName(FindComparison.Equals, eventName);
		}

		public IEnumerable<Event> FindByEventName(FindComparison comparison, String eventName)
		{
			return Where("EventName", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), eventName).Results();
		}
	}
}
