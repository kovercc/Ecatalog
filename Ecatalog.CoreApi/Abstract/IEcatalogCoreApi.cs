using Ecatalog.CoreApi.Models;
using System;
using System.Collections.Generic;

namespace Ecatalog.CoreApi.Abstract
{
    /// <summary>
    /// Provides methods for books e-catalog
    /// </summary>
    public interface IEcatalogCoreApi
    {
        /// <summary>
        /// Add Book to e-catalog
        /// </summary>
        /// <param name="book">Book object with data</param>
        void AddBook(Book book);

        /// <summary>
        /// Edit Book in e-catalog
        /// </summary>
        /// <param name="isbn">ISBN code of the book in e-Catalog</param>
        /// <param name="name">Name of the book</param>
        /// <param name="authors">Array of Authors of the book</param>
        /// <param name="yearOfPublication">The year of the publication</param>
        /// <param name="programmingLanguage">Programming language which describes in the book</param>
        /// <param name="readerLevel">The book reader level</param>
        /// <param name="language">The language of the book</param>
        /// <param name="bookRating">The book rating</param>
        void EditBook(Guid isbn, string name = null, Author[] authors = null, int? yearOfPublication = null, string programmingLanguage = null,
            ReaderLevel? readerLevel = null, string language = null, BookRating? bookRating = null);

        /// <summary>
        /// Delete book from e-catalog
        /// </summary>
        /// <param name="isbn">ISBN code of the Book in e-Catalog</param>
        void DeleteBook(Guid isbn);

        /// <summary>
        /// Get books from e-catalog by filters
        /// </summary>
        /// <param name="isbn">ISBN code of the book in e-Catalog</param>
        /// <param name="name">Name of the book</param>
        /// <param name="authors">Array of Authors of the book</param>
        /// <param name="yearOfPublication">The year of the publication</param>
        /// <param name="programmingLanguage">Programming language which describes in the book</param>
        /// <param name="readerLevel">The book reader level</param>
        /// <param name="language">The language of the book</param>
        /// <param name="bookRating">The book rating</param>
        /// <returns>Collection of the books from e-catalog</returns>
        IEnumerable<Book> GetBooks(Guid? isbn = null, string name = null, Author[] authors = null, int? yearOfPublication = null, string programmingLanguage = null, ReaderLevel? readerLevel = null, string language = null, BookRating? bookRating = null);

        /// <summary>
        /// Export e-catalog to xml file (.xml)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        void ExportCatalogToXml(string filePathAndName);

        /// <summary>
        /// Export e-catalog to json file (.json)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        void ExportCatalogToJson(string filePathAndName);

        /// <summary>
        /// Export e-catalog to binary file (.dat)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        void ExportCatalogToBinary(string filePathAndName);

        /// <summary>
        /// Import books from xml file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        void ImportCatalogFromXml(string filePathAndName);

        /// <summary>
        /// Import books from json file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        void ImportCatalogFromJson(string filePathAndName);

        /// <summary>
        /// Import books from binary file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        void ImportCatalogFromBinary(string filePathAndName);
    }
}
