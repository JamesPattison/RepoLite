using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;

namespace NS.Models
{
	public partial class NullableTable : BaseModel
	{
		private Int32 _id;
		private Int32? _age;
		private DateTime? _doB;
		private Guid? _lolVal;

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
			get => _doB;
			set => SetValue(ref _doB, value);
		}
		public virtual Guid? LolVal
		{
			get => _lolVal;
			set => SetValue(ref _lolVal, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();

			if (DoB == DateTime.MinValue)
				validationErrors.Add(new ValidationError(nameof(DoB), "Value cannot be default."));
			if (LolVal == Guid.Empty)
				validationErrors.Add(new ValidationError(nameof(LolVal), "Value cannot be default."));

			return validationErrors;
		}
	}
}

