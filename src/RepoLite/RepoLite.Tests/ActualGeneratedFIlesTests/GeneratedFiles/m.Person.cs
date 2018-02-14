using System;
using System.Collections.Generic;
using System.Xml;
using NS.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NS.Models
{
	[Table("Person", Schema="dbo")]
	public class Person : BaseModel
	{
		private Int32 _id;
		private String _name;
		private Int32 _age;
		private String _nationality;
		private Boolean _registered;

		[Key]
		public virtual Int32 Id
		{
			get => _id;
			set => SetValue(ref _id, value);
		}
		public virtual String Name
		{
			get => _name;
			set => SetValue(ref _name, value);
		}
		public virtual Int32 Age
		{
			get => _age;
			set => SetValue(ref _age, value);
		}
		public virtual String Nationality
		{
			get => _nationality;
			set => SetValue(ref _nationality, value);
		}
		public virtual Boolean Registered
		{
			get => _registered;
			set => SetValue(ref _registered, value);
		}
		public override List<ValidationError> Validate()
		{
			var validationErrors = new List<ValidationError>();
			
			if (string.IsNullOrEmpty(Name))
			validationErrors.Add(new ValidationError(nameof(Name), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Name) && Name.Length > 50)
			validationErrors.Add(new ValidationError(nameof(Name), "Max length is 50"));
			if (string.IsNullOrEmpty(Nationality))
			validationErrors.Add(new ValidationError(nameof(Nationality), "Value cannot be null"));
			if (!string.IsNullOrEmpty(Nationality) && Nationality.Length > 50)
			validationErrors.Add(new ValidationError(nameof(Nationality), "Max length is 50"));
			
			return validationErrors;
		}
	}
}

