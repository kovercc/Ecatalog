using Ecatalog.CoreApi.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecatalog.CoreApi.Models
{
    /// <summary>
    /// Book class
    /// </summary>
    [Serializable]
    public class Book
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Book()
        {
            Authors = new List<Author>();
        }

        /// <summary>
        /// Guid of the Book in store system
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// ISBN number
        /// </summary>
        [Required]
        public string ISBN { get; set; }

        /// <summary>
        /// Name of the book
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Year of book publication
        /// </summary>
        [Required]
        [CorrectYearRange(ErrorMessage = "Year of the publication must be in range between 1900 and current year")]
        public int? YearOfPublication { get; set; }

        /// <summary>
        /// Programming language in the book
        /// </summary>
        [Required]
        public string ProgrammingLanguage { get; set; }

        /// <summary>
        /// Book Reader level
        /// </summary>
        [Required]
        public ReaderLevel? ReaderLevel { get; set; }

        /// <summary>
        /// Language of the book
        /// </summary>
        [Required]
        public string Language { get; set; }

        ///// <summary>
        ///// Book rating
        ///// </summary>
        //public BookRating? BookRating { get;}

        /// <summary>
        /// Coolection of the Authors
        /// </summary>
        [NotEmptyCollection(ErrorMessage = "You need to specify authors for your book")]
        public virtual ICollection<Author> Authors { get; set; }
    }
}
