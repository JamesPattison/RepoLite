using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class NullableTableDto : BaseModel
	{
		public override string EntityName => "NullableTable";
		public static string CacheKey(object identifier) => $"NullableTable_{identifier}";
		protected int _id;
		protected int? _age;
		protected DateTime? _dob;
		protected Guid? _lolval;

		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public int? Age
		{
			get => _age;
			set => SetValue(ref _age, value);
		}
		public DateTime? DoB
		{
			get => _dob;
			set => SetValue(ref _dob, value);
		}
		public Guid? lolVal
		{
			get => _lolval;
			set => SetValue(ref _lolval, value);
		}
		public NullableTableDto() { }
		public NullableTableDto(
			int id,
			int age,
			DateTime dob,
			Guid lolval)
		{
			_id = id;
			_age = age;
			_dob = dob;
			_lolval = lolval;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_id = row.GetValue<int>($"{propertyPrefix}Id") ?? default(int); 
			_age = row.GetValue<int>($"{propertyPrefix}Age"); 
			_dob = row.GetValue<DateTime>($"{propertyPrefix}DoB"); 
			_lolval = row.GetValue<Guid>($"{propertyPrefix}lolVal"); 
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (DoB == DateTime.MinValue)
				validationErrors.Add(new ValidationError(nameof(DoB), "Value cannot be default."));
			if (lolVal == Guid.Empty)
				validationErrors.Add(new ValidationError(nameof(lolVal), "Value cannot be default."));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "NullableTable";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(int), "[INT]", SqlDbType.Int, false, true, true),
			new ColumnDefinition("Age", typeof(int), "[INT]", SqlDbType.Int, true, false, false),
			new ColumnDefinition("DoB", typeof(DateTime), "[DATETIME]", SqlDbType.DateTime, true, false, false),
			new ColumnDefinition("lolVal", typeof(Guid), "[UNIQUEIDENTIFIER]", SqlDbType.UniqueIdentifier, true, false, false),
		};
	}
}

