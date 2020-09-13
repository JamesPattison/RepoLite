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
	public partial interface IPersonRepository : IPkRepository<PersonDto>
	{
		PersonDto Get(int id);
		IEnumerable<PersonDto> Get(List<int> ids);
		IEnumerable<PersonDto> Get(params int[] ids);
		int GetMaxId();
		bool Delete(int id);
		bool Delete(IEnumerable<int> ids);
		bool DeleteByName(string name);
		bool DeleteByAge(int age);
		bool DeleteByNationality(string nationality);
		bool DeleteByRegistered(bool registered);
		IEnumerable<PersonDto> Search(
			string name = null,
			int? age = null,
			string nationality = null,
			bool? registered = null);
		IEnumerable<PersonDto> FindByName(string name);
		IEnumerable<PersonDto> FindByName(FindComparison comparison, string name);
		IEnumerable<PersonDto> FindByAge(int age);
		IEnumerable<PersonDto> FindByAge(FindComparison comparison, int age);
		IEnumerable<PersonDto> FindByNationality(string nationality);
		IEnumerable<PersonDto> FindByNationality(FindComparison comparison, string nationality);
		IEnumerable<PersonDto> FindByRegistered(bool registered);
		IEnumerable<PersonDto> FindByRegistered(FindComparison comparison, bool registered);
	}
	public sealed partial class PersonRepository : BaseRepository<PersonDto>, IPersonRepository
	{
		partial void InitializeExtension();
		public PersonRepository(string connectionString) : this(connectionString, exception => { }) { }
		public PersonRepository(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => { }, useCache, cacheDurationInSeconds) { }
		public PersonRepository(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) { }
		public PersonRepository(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,
			PersonDto.Schema, PersonDto.TableName, PersonDto.Columns, useCache, cacheDurationInSeconds)
		{
			InitializeExtension();
		}

		public PersonDto Get(int id)
		{
			return Where("Id", Comparison.Equals, id).Results().FirstOrDefault();
		}

		public IEnumerable<PersonDto> Get(List<int> ids)
		{
			return Get(ids.ToArray());
		}

		public IEnumerable<PersonDto> Get(params int[] ids)
		{
			return Where("Id", Comparison.In, ids).Results();
		}

		public int GetMaxId()
		{
			using (var cn = new SqlConnection(ConnectionString))
			{
				using (var cmd = CreateCommand(cn, "SELECT ISNULL(MAX(Id), 0) FROM Person"))
				{
					cn.Open();
					return (int)cmd.ExecuteScalar();
				}
			}
		}
		public override bool Create(PersonDto item)
		{
			//Validation
			if (item == null)
				return false;

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var createdKeys = BaseCreate(item.Id, item.Name, item.Age, item.Nationality, item.Registered);
			if (createdKeys.Count != PersonDto.Columns.Count(x => x.PrimaryKey))
				return false;

			item.Id = (int)createdKeys[nameof(PersonDto.Id)];
			item.ResetDirty();

				if (CacheEnabled)
				{
					SaveToCache(item);
				}
			return true;
		}

		public override bool BulkCreate(params PersonDto[] items)
		{
			if (!items.Any())
				return false;

			var validationErrors = items.SelectMany(x => x.Validate()).ToList();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var dt = new DataTable();
			foreach (var mergeColumn in PersonDto.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))
				dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);

			foreach (var item in items)
			{
				dt.Rows.Add(item.Name, item.Age, item.Nationality, item.Registered); 
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
		public override bool BulkCreate(List<PersonDto> items)
		{
			return BulkCreate(items.ToArray());
		}

		public bool Update(PersonDto item, bool clearDirty = true)
		{
			if (item == null)
				return false;
			if (CacheEnabled)
			{
				RemoveFromCache(item.Id);
			}

			var validationErrors = item.Validate();
			if (validationErrors.Any())
				throw new ValidationException(validationErrors);

			var success = BaseUpdate(item.DirtyColumns, 
				item.Id, item.Name, item.Age, item.Nationality, item.Registered);

			if (success && clearDirty)
				item.ResetDirty();
			if (success && CacheEnabled)
			{
				SaveToCache(item);
			}

			return success;
		}
		public bool Delete(PersonDto persondto)
		{
			if (persondto == null)
				return false;

			var deleteColumn = new DeleteColumn("Id", persondto.Id, SqlDbType.Int);

			return BaseDelete(deleteColumn, out var items);
		}
		public bool Delete(IEnumerable<PersonDto> items)
		{
			if (!items.Any()) return true;
			var deleteValues = new List<object>();
			foreach (var item in items)
			{
				deleteValues.Add(item.Id);
			}

			return BaseDelete("Id", deleteValues);
		}

		public bool Delete(int id)
		{
			return Delete(new PersonDto { Id = id });
		}


		public bool Delete(IEnumerable<int> ids)
		{
			if (!ids.Any()) return true;
			var deleteValues = new List<object>();
			deleteValues.AddRange(ids.Cast<object>()); 
			return BaseDelete("Id", deleteValues);
		}

		public bool DeleteByName(string name)
		{
			if (BaseDelete(new DeleteColumn("Name", name, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByAge(int age)
		{
			if (BaseDelete(new DeleteColumn("Age", age, SqlDbType.Int), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByNationality(string nationality)
		{
			if (BaseDelete(new DeleteColumn("Nationality", nationality, SqlDbType.NVarChar), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}
		public bool DeleteByRegistered(bool registered)
		{
			if (BaseDelete(new DeleteColumn("Registered", registered, SqlDbType.Bit), out var items))
			{
				if (CacheEnabled)
				{
					foreach (var item in items)
					{
						RemoveFromCache(item.Id);
					}
				}
				return true;
			}
			return false;
		}

		public bool Merge(List<PersonDto> items)
		{
			var mergeTable = new List<object[]>();
			foreach (var item in items)
			{
				mergeTable.Add(new object[]
				{
					item.Id,
					item.Name, item.DirtyColumns.Contains("Name"),
					item.Age, item.DirtyColumns.Contains("Age"),
					item.Nationality, item.DirtyColumns.Contains("Nationality"),
					item.Registered, item.DirtyColumns.Contains("Registered")
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
				if (firstItem == "Id")
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
						Cast<int>(blocks[0]),
						Cast<string>(blocks[1]), true,
						Cast<int>(blocks[2]), true,
						Cast<string>(blocks[3]), true,
						Cast<bool>(blocks[4]), true,
					});
				} while ((line = sr.ReadLine()) != null);

				
				return BaseMerge(mergeTable);
			}
		}

		public override PersonDto ToItem(DataRow row, bool skipBase)
		{
			var item = new PersonDto
			{
				Id = GetInt32(row, "Id"),
				Name = GetString(row, "Name"),
				Age = GetInt32(row, "Age"),
				Nationality = GetString(row, "Nationality"),
				Registered = GetBoolean(row, "Registered"),
			};

			item.ResetDirty();
			return item;
		}

		public override TK ToItem<TK>(DataRow row, bool skipBase)
		{
			var item = new TK
			{
				Id = GetInt32(row, "Id"),
				Name = GetString(row, "Name"),
				Age = GetInt32(row, "Age"),
				Nationality = GetString(row, "Nationality"),
				Registered = GetBoolean(row, "Registered"),
			};

			item.ResetDirty();
			return item as TK;
		}

		public IEnumerable<PersonDto> Search(
			string name = null,
			int? age = null,
			string nationality = null,
			bool? registered = null)
		{
			var queries = new List<QueryItem>(); 

			if (name != null)
				queries.Add(new QueryItem("Name", name));
			if (age.HasValue)
				queries.Add(new QueryItem("Age", age));
			if (nationality != null)
				queries.Add(new QueryItem("Nationality", nationality));
			if (registered.HasValue)
				queries.Add(new QueryItem("Registered", registered));

			return BaseSearch(queries);
		}

		public IEnumerable<PersonDto> FindByName(string name)
		{
			return FindByName(FindComparison.Equals, name);
		}

		public IEnumerable<PersonDto> FindByName(FindComparison comparison, string name)
		{
			var items = Where("Name", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), name).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<PersonDto> FindByAge(int age)
		{
			return FindByAge(FindComparison.Equals, age);
		}

		public IEnumerable<PersonDto> FindByAge(FindComparison comparison, int age)
		{
			var items = Where("Age", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), age).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<PersonDto> FindByNationality(string nationality)
		{
			return FindByNationality(FindComparison.Equals, nationality);
		}

		public IEnumerable<PersonDto> FindByNationality(FindComparison comparison, string nationality)
		{
			var items = Where("Nationality", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), nationality).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}

		public IEnumerable<PersonDto> FindByRegistered(bool registered)
		{
			return FindByRegistered(FindComparison.Equals, registered);
		}

		public IEnumerable<PersonDto> FindByRegistered(FindComparison comparison, bool registered)
		{
			var items = Where("Registered", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), registered).Results();
			if (CacheEnabled)
			{
				foreach (var item in items)
				{
					SaveToCache(item);
				}
			}
			return items;
		}
		private void SaveToCache(PersonDto persondto)
		{
			CacheHelper.SaveToCache(PersonDto.CacheKey(persondto.Id), persondto);
		}
		private PersonDto GetFromCache(System.Int32 id)
		{
			return CacheHelper.GetFromCache<PersonDto>(PersonDto.CacheKey(id));
		}
		private void RemoveFromCache(System.Int32 id)
		{
			CacheHelper.RemoveFromCache(PersonDto.CacheKey(id));
		}
		private bool IsInCache(System.Int32 id)
		{
			return CacheHelper.IsInCache(PersonDto.CacheKey(id));
		}

	}
}
