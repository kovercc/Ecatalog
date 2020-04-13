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
        /// Add new author to e-catalog
        /// </summary>
        /// <param name="author">Author object with data</param>
        /// <returns>Author's guid in e-catalog</returns>
        Guid AddAuthor(Author author);

        /// <summary>
        /// Edit author data
        /// </summary>
        /// <param name="updatedAuthor">updates for author in object</param>
        void EditAuthor(Author updatedAuthor);

        /// <summary>
        /// Delete author from catalog
        /// </summary>
        /// <param name="id">Author Id</param>
        void DeleteAuthor(Guid id);

        /// <summary>
        /// Add Book to e-catalog
        /// </summary>
        /// <param name="book">Book object with data</param>
        /// <returns>Book's guid in e-catalog</returns>
        Guid AddBook(Book book);

        /// <summary>
        /// Update Book properties
        /// </summary>
        /// <param name="updatedBook">Book object with updated properties</param>
        void EditBook(Book updatedBook);

        /// <summary>
        /// Delete book from e-catalog
        /// </summary>
        /// <param name="bookId">Guid of the Book in e-Catalog</param>
        public void DeleteBook(Guid bookId);


        /// <summary>
        /// Get books from e-catalog by book and author filters
        /// </summary>
        /// <param name="bookFilter">Book filter function</param>
        /// <param name="authorFilter">Author filter function</param>
        /// <returns>Collection of the books from e-catalog</returns>
        IEnumerable<Book> GetBooks(Func<Book, bool> bookFilter = null, Func<Author, bool> authorFilter = null);

        /// <summary>
        /// Get authors from e-catalog by author and book filters
        /// </summary>
        /// <param name="authorFilter">Author filter function</param>
        /// <param name="bookFilter">Book filter function</param>
        /// <returns>Collection of the authors from e-catalog</returns>
        IEnumerable<Author> GetAuthors(Func<Author, bool> authorFilter = null, Func<Book, bool> bookFilter = null);

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
