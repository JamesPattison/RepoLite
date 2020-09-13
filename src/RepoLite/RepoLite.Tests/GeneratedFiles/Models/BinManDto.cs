using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public sealed partial class BinManDto : BaseModel
	{
		public override string EntityName => "BinMan";
		public static string CacheKey(object identifier) => $"BinMan_{identifier}";
		protected int _id;
		protected byte[] _data;

		public int Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public byte[] Data
		{
			get => _data;
			set => SetValue(ref _data, value);
		}
		public BinManDto() { }
		public BinManDto(
			int id,
			byte[] data)
		{
			_id = id;
			_data = data;
		}
		public override IBaseModel SetValues(DataRow row, string propertyPrefix)
		{
			_id = row.GetValue<int>($"{propertyPrefix}Id") ?? default(int); 
			_data = (byte[])row[$"{propertyPrefix}Data"];
			return this;
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (Data == null)
				validationErrors.Add(new ValidationError(nameof(Data), "Value cannot be null"));
			if (Data != null && Data.Length > 8)
				validationErrors.Add(new ValidationError(nameof(Data), "Binary array values exceed database size"));

			return validationErrors;
		}
		public static string Schema = "dbo";
		public static string TableName = "BinMan";
		public static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(int), "[INT]", SqlDbType.Int, false, false, false),
			new ColumnDefinition("Data", typeof(byte[]), "[BINARY](8)", SqlDbType.Binary, false, false, false),
		};
	}
}

