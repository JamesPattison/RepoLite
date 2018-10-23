using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public partial class NullableTable : BaseModel
	{
		public override string EntityName => "NullableTable";
		private Int32 _id;
		private Int32? _age;
		private DateTime? _dob;
		private Guid? _lolval;

		public virtual Int32 Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public virtual Int32? Age
		{
			get => _age;
			set => SetValue(ref _age, value);
		}
		public virtual DateTime? DoB
		{
			get => _dob;
			set => SetValue(ref _dob, value);
		}
		public virtual Guid? lolVal
		{
			get => _lolval;
			set => SetValue(ref _lolval, value);
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
	}
}

