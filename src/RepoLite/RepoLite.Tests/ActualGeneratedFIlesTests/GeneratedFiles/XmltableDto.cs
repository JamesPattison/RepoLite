using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public partial class Xmltable : BaseModel
	{
		private String _name;
		private XmlDocument _data;

		public virtual String Name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public virtual XmlDocument Data
		{
			get => _data;
			set => SetValue(ref _data, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (string.IsNullOrEmpty(Name))
				validationErrors.Add(new ValidationError(nameof(Name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Name) && Name.Length > 12)
				validationErrors.Add(new ValidationError(nameof(Name), "Max length is 12"));

			return validationErrors;
		}
	}
}

