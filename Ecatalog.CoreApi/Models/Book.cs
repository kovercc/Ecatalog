using System;
using System.ComponentModel.DataAnnotations;

namespace Ecatalog.CoreApi.Models
{
    /// <summary>
    /// Book class
    /// </summary>
    [Serializable]
    public class Book
    {
        /// <summary>
        /// ISBN number (code)
        /// </summary>
        [Required]
        public Guid ISBN { get; set; }

        /// <summary>
        /// Related authors array
        /// </summary>
        [Required]
        public Author[] Authors { get; set; }

        /// <summary>
        /// Name of the book
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Year of book publication
        /// </summary>
        [Required]
        [Range(1900, 2019)]
        public int YearOfPublication { get; set; }

        /// <summary>
        /// Programming language in the book
        /// </summary>
        [Required]
        public string ProgrammingLanguage { get; set; }

        /// <summary>
        /// Book Reader level
        /// </summary>
        [Required]
        public ReaderLevel ReaderLevel { get; set; }

        /// <summary>
        /// Language of the book
        /// </summary>
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// Book rating
        /// </summary>
        [Required]
        public BookRating BookRating { get; set; }
    }
}
