using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Ecatalog.CoreApi.Common
{
    /// <summary>
    /// Check if collection is empty or null
    /// </summary>
    public class NotEmptyCollectionAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as ICollection;
            if (list != null)
            {
                return list.Count > 0;
            }
            return false;
        }
    }

    /// <summary>
    /// Check year range
    /// </summary>
    public class CorrectYearRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value != null && int.TryParse(value.ToString(), out int intValue))
            {
                if (intValue > 1900 && intValue <= DateTime.Now.Year)
                    return true;
            }
            return false;
        }
    }
}
