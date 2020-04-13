using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Ecatalog.CoreApi.Models
{
    /// <summary>
    /// Author data class
    /// </summary>
    [Serializable]
    public class Author
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Author()
        {
            Books = new List<Book>();
        }
        /// <summary>
        /// Author's Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

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
        /// Coolection of the Authors
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Book> Books { get; }
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
