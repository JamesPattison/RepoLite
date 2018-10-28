using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using NS.Base;
using NS.Models.Base;

namespace NS.Models
{
	public partial class xmltable : BaseModel
	{
		public override string EntityName => "xmltable";
		private String _name;
		private XmlDocument _data;

		public virtual String name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public virtual XmlDocument data
		{
			get => _data;
			set => SetValue(ref _data, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(name))
				validationErrors.Add(new ValidationError(nameof(name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(name) && name.Length > 12)
				validationErrors.Add(new ValidationError(nameof(name), "Max length is 12"));

			return validationErrors;
		}
		internal static List<ColumnDefinition> Columns => new List<ColumnDefinition>
		{
			new ColumnDefinition("name", typeof(System.String), "[VARCHAR](12)", SqlDbType.VarChar, false, false, false),
			new ColumnDefinition("data", typeof(System.Xml.XmlDocument), "[XML]", SqlDbType.Xml, false, false, false),
		};
	}
}

