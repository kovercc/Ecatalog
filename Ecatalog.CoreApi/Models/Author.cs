using System;
using System.ComponentModel.DataAnnotations;

namespace Ecatalog.CoreApi.Models
{
    /// <summary>
    /// Author data class
    /// </summary>
    [Serializable]
    public class Author
    {
        /// <summary>
        /// First name of the Author
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the Author
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// Note of te Author
        /// </summary>
        [Required]
        public string Note { get; set; }


        /// <summary>
        /// Represents author info in string format
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
