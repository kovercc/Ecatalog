using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Ecatalog.CoreApi.Common
{
    /// <summary>
    /// Extension methods class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Validate object by class model with annotations
        /// </summary>
        /// <typeparam name="T">Generic object Type</typeparam>
        /// <param name="checkedObject">Validated object</param>
        public static void ValidateModel<T>(this T checkedObject) where T : class
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(checkedObject);
            if (!Validator.TryValidateObject(checkedObject, context, results, true))
            {
                throw new InvalidDataException(string.Join(", ", results.Select(s => s.ErrorMessage)));
            }
        }
    }
}
