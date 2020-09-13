using NS.Base;
using NS.Models;
using NS.Models.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml;

namespace NS
{
	public partial interface IEventRepository : IPkRepository<EventDto>
	{
		EventDto Get(string eventid);
		IEnumerable<EventDto> Get(List<string> eventids);
		IEnumerable<EventDto> Get(params string[] eventids);
		bool Delete(string eventid);
		bool Delete(IEnumerable<string> eventids);
		bool DeleteByEventName(string eventname);
		IEnumerable<EventDto> Search(
			string eventname = null);
		IEnumerable<EventDto> FindByEventName(string eventname);
		IEnumerable<EventDto> FindByEventName(FindComparison comparison, string eventname);
	}
	public sealed partial class EventRepository : BaseRepository<EventDto>, IEventRepository
	{
		partial void InitializeExtension();
		public EventRepository(string connectionString) : this(connectionString, exception => { }) { }
		public EventRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public EventRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public EventRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			EventDto.Schema, EventDto.TableName, EventDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}

		public EventDto Get(string eventid)
		{
			return Where("EventId", Comparison.Equals, eventid).Results().FirstOrDefault();
		}

		public IEnumerable<EventDto> Get(List<string> eventids)
		{
			return Get(eventids.ToArray());
		}

		public IEnumerable<EventDto> Get(params string[] eventids)
		{
			return Where("EventId", Comparison.In, eventids).Results();
		}

		public override bool Create(EventDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.EventId, item.EventName);
			if (createdKeys.Count != EventDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.EventId = (string)createdKeys[nameof(EventDto.EventId)];
			item.ResetDirty();

				if (CacheEnabled)
				{
					SaveToCache(item);
				}
			return true;
		}

		public override bool BulkCreate(params EventDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in EventDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.EventId, item.EventName); 
			}

			if (BulkInsert(dt))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						SaveToCache(item);
					}
				}
				return true;
			}
			return false;
		}
		public override bool BulkCreate(List<EventDto> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(EventDto item, bool clearDirty = true)
		{
			if (item == null)
				return false;
			if (CacheEnabled)
			{
				RemoveFromCache(item.EventId);
			}

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.EventId, item.EventName);

			if (success && clearDirty)
				item.ResetDirty();
			if (success && CacheEnabled)
			{
				SaveToCache(item);
			}

			return success;
		}
		public bool Delete(EventDto eventdto)
		{
			if (eventdto == null)
				return false;

			var deleteColumn = new DeleteColumn("EventId", eventdto.EventId, SqlDbType.NVarChar);

			return BaseDelete(deleteColumn, out var items);
		}
		public bool Delete(IEnumerable<EventDto> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.EventId);
			}

			return BaseDelete("EventId", deleteValues);
		}

		public bool Delete(string eventid)
		{
			return Delete(new EventDto { EventId = eventid });
		}


		public bool Delete(IEnumerable<string> eventids)
		{
			if (!eventids.Any()) return true;
			var deleteValues = new List<object>();
			deleteValues.AddRange(eventids.Cast<object>()); 
			return BaseDelete("EventId", deleteValues);
		}

		public bool DeleteByEventName(string eventname)
		{
			if (BaseDelete(new DeleteColumn("EventName", eventname, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.EventId);
					}
				}
				return true;
			}
			return false;
		}

		public bool Merge(List<EventDto> items)
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
		public bool Merge(string csvPath)
		{
			var mergeTable = new List<object[]>();
			using (var sr = new StreamReader(csvPath))
			{
				var line = sr.ReadLine();
				if (line == null) return false;

				var firstItem = line.Split(',')[0];
				if (firstItem == "EventId")
				{
					//CSV has headers
					//Run to the next line
					line = sr.ReadLine();
					if (line == null) return true;
				}

				do
				{
					var blocks = line.Split(',');
					mergeTable.Add(new object[]
					{
						Cast<string>(blocks[0]),
						Cast<string>(blocks[1]), true,
					});
				} while ((line = sr.ReadLine()) != null);

				
				return BaseMerge(mergeTable);
			}
		}

		public override EventDto ToItem(DataRow row, bool skipBase)
		{
			var item = new EventDto
			{
				EventId = GetString(row, "EventId"),
				EventName = GetString(row, "EventName"),
			};

			item.ResetDirty();
			return item;
		}

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
			{
				EventId = GetString(row, "EventId"),
				EventName = GetString(row, "EventName"),
			};

			item.ResetDirty();
			return item as TK;
		}

		public IEnumerable<EventDto> Search(
			string eventname = null)
		{
			var queries = new List<QueryItem>(); 

			if (eventname != null)
				queries.Add(new QueryItem("EventName", eventname));

			return BaseSearch(queries);
		}

		public IEnumerable<EventDto> FindByEventName(string eventname)
		{
			return FindByEventName(FindComparison.Equals, eventname);
		}

		public IEnumerable<EventDto> FindByEventName(FindComparison comparison, string eventname)
		{
			var items = Where("EventName", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), eventname).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}
		private void SaveToCache(EventDto eventdto)
		{
			CacheHelper.SaveToCache(EventDto.CacheKey(eventdto.EventId), eventdto);
		}
		private EventDto GetFromCache(System.String eventid)
		{
			return CacheHelper.GetFromCache<EventDto>(EventDto.CacheKey(eventid));
		}
		private void RemoveFromCache(System.String eventid)
		{
			CacheHelper.RemoveFromCache(EventDto.CacheKey(eventid));
		}
		private bool IsInCache(System.String eventid)
		{
			return CacheHelper.IsInCache(EventDto.CacheKey(eventid));
		}

	}
}
