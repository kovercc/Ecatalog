using Ecatalog.CoreApi.Abstract;
using Ecatalog.CoreApi.Common;
using Ecatalog.CoreApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ecatalog.CoreApi.Concrete
{
    /// <summary>
    /// Implements Ecatalog.CoreApi.Abstract.IEcatalogCoreApi in the RAM of computer
    /// </summary>
    internal class EcatalogInMemory : IEcatalogCoreApi
    {
        /// <summary>
        /// Books Array in e-Catalog
        /// </summary>
        private readonly List<Book> _books = new List<Book>();

        /// <summary>
        /// Books Array in e-Catalog
        /// </summary>
        private readonly List<Author> _authors = new List<Author>();

        /// <summary>
        /// Add new author to e-catalog
        /// </summary>
        /// <param name="author">Author object with data</param>
        /// <returns>author's guid in e-catalog</returns>
        public Guid AddAuthor(Author author)
        {
            author.ValidateModel();
            author.Id = Guid.NewGuid();
            _authors.Add(author);
            return author.Id;
        }

        /// <summary>
        /// Edit author data
        /// </summary>
        /// <param name="updatedAuthor">updates for author in object</param>
        public void EditAuthor(Author updatedAuthor)
        {
            if (updatedAuthor.Id == null)
            {
                throw new InvalidOperationException("Id is required for updating author.");
            }
            var author = _authors.Single(a => a.Id == updatedAuthor.Id);
            if (author == null)
            {
                throw new InvalidOperationException($"The author with Id = '{updatedAuthor.Id}' didn't find. Check the Id and try again.");
            }
            UpdateChangedProperties(author, updatedAuthor);
            author.ValidateModel();
        }

        /// <summary>
        /// Delete author from catalog
        /// </summary>
        /// <param name="id">Author Id</param>
        public void DeleteAuthor(Guid id)
        {
            var author = _authors.SingleOrDefault(s => s.Id == id);
            if (author == null)
            {
                throw new InvalidOperationException($"The author with Id = '{id}' didn't find. Check the Id and try again.");
            }
            if (author.Books.Any())
            {
                throw new InvalidOperationException($"You coudn't delete author, because it has {author.Books.Count} related books.");
            }
            _authors.Remove(author);
        }

        /// <summary>
        /// Add Book to e-catalog
        /// </summary>
        /// <param name="book">Book object with data</param>
        public Guid AddBook(Book book)
        {
            CheckIsbn(book.ISBN);
            book.ValidateModel();
            // Add book to catalog
            book.Id = Guid.NewGuid();
            _books.Add(book);
            foreach (var author in book.Authors)
            {
                author.Books.Add(book);
                AddAuthor(author);
            }
            return book.Id;
        }

        /// <summary>
        /// Update Book properties
        /// </summary>
        /// <param name="updatedBook">Book object with updated properties</param>
        public void EditBook(Book updatedBook)
        {
            if (updatedBook.ISBN == null)
            {
                throw new InvalidOperationException("ISBN is required for updating book.");
            }
            var book = _books.SingleOrDefault(a => a.ISBN == updatedBook.ISBN);
            if (book == null)
            {
                throw new InvalidOperationException($"The book with ISBN = '{updatedBook.ISBN}' didn't find. Check the ISBN code and try again.");
            }
            UpdateChangedProperties(book, updatedBook);
            book.ValidateModel();
        }

        /// <summary>
        /// Delete book from e-catalog
        /// </summary>
        /// <param name="bookId">Guid of the Book in e-Catalog</param>
        public void DeleteBook(Guid bookId)
        {
            var book = _books.SingleOrDefault(s => s.Id == bookId);
            if (book == null)
            {
                throw new InvalidOperationException($"The book with ISBN = '{bookId}' didn't find. Check the ISBN code and try again.");
            }
            _books.Remove(book);
            // find book authors
            var bookAuthors = _authors.Where(w => w.Books.Contains(book)).ToList();
            // remove book from authors
            bookAuthors.ForEach(a => a.Books.Remove(book));
        }

        /// <summary>
        /// Get books from e-catalog by book and author filters
        /// </summary>
        /// <param name="bookFilter">Book filter function</param>
        /// <param name="authorFilter">Author filter function</param>
        /// <returns>Collection of the books from e-catalog</returns>
        public IEnumerable<Book> GetBooks(Func<Book, bool> bookFilter = null, Func<Author, bool> authorFilter = null)
        {
            var filteredBooks = _books.Where(bookFilter ?? (w => true));
            if (authorFilter != null)
            {
                var authorsBooks = new List<Guid>();
                _authors.Where(authorFilter).ToList().ForEach(a => authorsBooks.AddRange(a.Books.Select(s => s.Id)));             
                filteredBooks = filteredBooks.Where(b => authorsBooks.Contains(b.Id));
            }
            return filteredBooks;
        }

        /// <summary>
        /// Get authors from e-catalog by author and book filters
        /// </summary>
        /// <param name="authorFilter">Author filter function</param>
        /// <param name="bookFilter">Book filter function</param>
        /// <returns>Collection of the authors from e-catalog</returns>
        public IEnumerable<Author> GetAuthors(Func<Author, bool> authorFilter = null, Func<Book, bool> bookFilter = null)
        {
            var filteredAuthors = _authors.Where(authorFilter ?? (w => true));
            if (bookFilter != null)
            {
                var books = _books.Where(bookFilter).ToList();
                var booksAuthors = new List<Guid>();
                _books.Where(bookFilter).ToList().ForEach(b => booksAuthors.AddRange(b.Authors.Select(s => s.Id)));
                filteredAuthors = filteredAuthors.Where(b => booksAuthors.Contains(b.Id));
            }
            return filteredAuthors;
        } 

        /// <summary>
        /// Export e-catalog to xml file (.xml)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        public void ExportCatalogToXml(string filePathAndName)
        {
            var serializer = new CommonXmlSerializer();
            File.WriteAllText(filePathAndName,
                serializer.ObjectToString(_books));
        }

        /// <summary>
        /// Export e-catalog to json file (.json)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        public void ExportCatalogToJson(string filePathAndName)
        {
            File.WriteAllText(filePathAndName, JsonConvert.SerializeObject(_books));
        }

        /// <summary>
        /// Export e-catalog to binary file (.dat)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        public void ExportCatalogToBinary(string filePathAndName)
        {
            var serializer = new CommonBinarySerializer();
            File.WriteAllText(filePathAndName, 
                System.Text.Encoding.Default.GetString(serializer.Serialize(_books)));
        }

        /// <summary>
        /// Import books from xml file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        public void ImportCatalogFromXml(string filePathAndName)
        {
            var serializer = new CommonXmlSerializer();
            var importBooks = serializer.StringToObject<List<Book>>(filePathAndName);
            importBooks.ForEach(f => CheckIsbn(f.ISBN));
            _books.AddRange(importBooks);
        }

        /// <summary>
        /// Import books from json file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        public void ImportCatalogFromJson(string filePathAndName)
        {
            var importBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(filePathAndName));
            importBooks.ForEach(f => CheckIsbn(f.ISBN));
            _books.AddRange(importBooks);
        }

        /// <summary>
        /// Import books from binary file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        public void ImportCatalogFromBinary(string filePathAndName)
        {
            var serializer = new CommonBinarySerializer();
            var importBooks = serializer.Deserialize<List<Book>>(System.Text.Encoding.Default.GetBytes(File.ReadAllText(filePathAndName)));
            importBooks.ForEach(f => CheckIsbn(f.ISBN));
            _books.AddRange(importBooks);
        }

        /// <summary>
        /// Check ISBN number for duplicate
        /// </summary>
        /// <param name="isbn">ISBN number of the book</param>
        private void CheckIsbn(string isbn)
        {
            if (_books.Any(a => a.ISBN == isbn))
            {
                throw new InvalidOperationException($"The book with ISBN = '{isbn}' is already exist. You cannot add duplicate book.");
            }
        }     
        private static void UpdateChangedProperties<T>(T exist, T updated, params string[] ignore) where T : class
        {
            if (exist != null && updated != null)
            {
                Type type = typeof(T);
                var ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object existValue = type.GetProperty(pi.Name).GetValue(exist, null);
                        object updatedValue = type.GetProperty(pi.Name).GetValue(updated, null);

                        if (existValue != updatedValue && (existValue == null || !existValue.Equals(updatedValue)))
                        {
                            type.GetProperty(pi.Name).SetValue(exist, updatedValue);
                        }
                    }
                }             
            }
        }
    }
}
