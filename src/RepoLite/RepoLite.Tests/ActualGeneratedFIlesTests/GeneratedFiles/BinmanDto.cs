using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Base;
using NS.Models.Base;

namespace NS.Models
{
	public partial class BinMan : BaseModel
	{
		public override string EntityName => "BinMan";
		private Int32 _id;
		private Byte[] _data;

		public virtual Int32 Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public virtual Byte[] Data
		{
			get => _data;
			set => SetValue(ref _data, value);
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
		internal static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("Id", typeof(System.Int32), "[INT]", SqlDbType.Int, false, false, false),
			new ColumnDefinition("Data", typeof(System.Byte[]), "[BINARY](8)", SqlDbType.Binary, false, false, false),
		};
	}
}

