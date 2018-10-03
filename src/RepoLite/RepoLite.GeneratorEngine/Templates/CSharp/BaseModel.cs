using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MODELNAMESPACE.Base
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }

        public ValidationError(string property, string error)
        {
            PropertyName = property;
            Error = error;
        }
    }

    public abstract class BaseModel
    {
        public abstract List<ValidationError> Validate();
        public readonly List<string> DirtyColumns = new List<string>();

        public void ResetDirty()
        {
            DirtyColumns.Clear();
        }

        protected void SetValue<T>(ref T prop, T value, [CallerMemberName] string propName = "")
        {
            if (!DirtyColumns.Contains(propName))
            {
                DirtyColumns.Add(propName);
            }
            prop = value;
        }

        public static int GetDecimalPlaces(decimal n)
        {
            n = Math.Abs(n); //make sure it is positive.
            n -= (int)n;     //remove the integer part of the number.
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }
    }
}