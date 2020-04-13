using Ecatalog.CoreApi.Abstract;
using Ecatalog.CoreApi.Common;
using Ecatalog.CoreApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ecatalog.CoreApi.Concrete
{
    internal class EcatalogInDb : IEcatalogCoreApi
    {
        /// <summary>
        /// Add new author to e-catalog
        /// </summary>
        /// <param name="author">Author object with data</param>
        /// <returns>author's guid in e-catalog</returns>
        public Guid AddAuthor(Author author)
        {
            author.ValidateModel();
            using var db = new EcatalogContext();
            var addedAuthor = db.Authors.Add(author);
            db.SaveChanges();
            return addedAuthor.Id;
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
            using var db = new EcatalogContext();
            var author = db.Authors.Single(a => a.Id == updatedAuthor.Id);
            if (author == null)
            {
                throw new InvalidOperationException($"The author with Id = '{updatedAuthor.Id}' didn't find. Check the Id and try again.");
            }
            UpdateChangedProperties(author, updatedAuthor);
            author.ValidateModel();
            db.SaveChanges();
        }

        /// <summary>
        /// Delete author from catalog
        /// </summary>
        /// <param name="id">Author Id</param>
        public void DeleteAuthor(Guid id)
        {
            using var db = new EcatalogContext();
            var author = db.Authors.SingleOrDefault(s => s.Id == id);
            if (author == null)
            {
                throw new InvalidOperationException($"The author with Id = '{id}' didn't find. Check the Id and try again.");
            }
            if (author.Books.Any())
            {
                throw new InvalidOperationException($"You coudn't delete author, because it has {author.Books.Count} related books.");
            }
            db.Authors.Remove(author);
            db.SaveChanges();
        }

        /// <summary>
        /// Add Book to e-catalog
        /// </summary>
        /// <param name="book">Book object with data</param>
        public Guid AddBook(Book book)
        {
            using var db = new EcatalogContext();
            CheckIsbn(db, book.ISBN);
            book.ValidateModel();
            // Add book to catalog
            var newBook = db.Books.Add(book);
            db.SaveChanges();
            return newBook.Id;
            //return db.Books.First(a => a.ISBN == book.ISBN).Id;
        }

        /// <summary>
        /// Update Book properties
        /// </summary>
        /// <param name="updatedBook">Book object with updated properties</param>
        public void EditBook(Book updatedBook)
        {
            if (updatedBook.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Id is required for updating book.");
            }
            using var db = new EcatalogContext();
            var book = db.Books.Find(updatedBook.Id);
            if (book == null)
            {
                throw new InvalidOperationException($"The book with Id = '{updatedBook.Id}' didn't find. Check the Id and try again.");
            }
            UpdateChangedProperties(book, updatedBook);
            book.ValidateModel();
            db.SaveChanges();
        }

        /// <summary>
        /// Delete book from e-catalog
        /// </summary>
        /// <param name="bookId">Guid of the Book in e-Catalog</param>
        public void DeleteBook(Guid bookId)
        {
            using var db = new EcatalogContext();
            var book = db.Books.Find(bookId);
            if (book == null)
            {
                throw new InvalidOperationException($"The book with Id = '{bookId}' didn't find. Check the Id and try again.");
            }
            db.Books.Remove(book);
            // find book authors
            var bookAuthors = db.Authors.Where(w => w.Books.Contains(book)).ToList();
            // remove book from authors
            bookAuthors.ForEach(a => a.Books.Remove(book));
            db.SaveChanges();
        }

        /// <summary>
        /// Get books from e-catalog by book and author filters
        /// </summary>
        /// <param name="bookFilter">Book filter function</param>
        /// <param name="authorFilter">Author filter function</param>
        /// <returns>Collection of the books from e-catalog</returns>
        public IEnumerable<Book> GetBooks(Func<Book, bool> bookFilter = null, Func<Author, bool> authorFilter = null)
        {
            using var db = new EcatalogContext();
            var filteredBooks = db.Books.Include(i => i.Authors).Where(bookFilter ?? (w => true));

            if (authorFilter != null)
            {
                filteredBooks = filteredBooks.Where(w => w.Authors.Where(authorFilter).Any());
            }
            return filteredBooks.ToArray();
        }

        /// <summary>
        /// Get authors from e-catalog by author and book filters
        /// </summary>
        /// <param name="authorFilter">Author filter function</param>
        /// <param name="bookFilter">Book filter function</param>
        /// <returns>Collection of the authors from e-catalog</returns>
        public IEnumerable<Author> GetAuthors(Func<Author, bool> authorFilter = null, Func<Book, bool> bookFilter = null)
        {
            using var db = new EcatalogContext();
            var filteredAuthors = db.Authors.Include(i => i.Books).Where(authorFilter ?? (w => true));
            if (bookFilter != null)
            {
                filteredAuthors = filteredAuthors.Where(w => w.Books.Where(bookFilter).Any());
            }
            return filteredAuthors.ToArray();
        }

        /// <summary>
        /// Export e-catalog to xml file (.xml)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        public void ExportCatalogToXml(string filePathAndName)
        {
            var serializer = new CommonXmlSerializer();
            using var db = new EcatalogContext();
            File.WriteAllText(filePathAndName,
                serializer.ObjectToString(db.Books));
        }

        /// <summary>
        /// Export e-catalog to json file (.json)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        public void ExportCatalogToJson(string filePathAndName)
        {
            using var db = new EcatalogContext();
            File.WriteAllText(filePathAndName, JsonConvert.SerializeObject(db.Books));
        }

        /// <summary>
        /// Export e-catalog to binary file (.dat)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        public void ExportCatalogToBinary(string filePathAndName)
        {
            var serializer = new CommonBinarySerializer();
            using var db = new EcatalogContext();
            File.WriteAllText(filePathAndName,
                System.Text.Encoding.Default.GetString(serializer.Serialize(db.Books)));
        }

        /// <summary>
        /// Import books from xml file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        public void ImportCatalogFromXml(string filePathAndName)
        {
            var serializer = new CommonXmlSerializer();
            var importBooks = serializer.StringToObject<List<Book>>(filePathAndName);
            using var db = new EcatalogContext();
            importBooks.ForEach(f => CheckIsbn(db, f.ISBN));
            db.Books.AddRange(importBooks);
            db.SaveChanges();
        }

        /// <summary>
        /// Import books from json file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.json")</param>
        public void ImportCatalogFromJson(string filePathAndName)
        {
            if (!File.Exists(filePathAndName))
                return;
            var importBooks = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(filePathAndName));
            if (importBooks != null && importBooks.Any())
            {
                using var db = new EcatalogContext();
                importBooks.ForEach(f => CheckIsbn(db, f.ISBN));
                db.Books.AddRange(importBooks);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Import books from binary file to e-catalog
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.dat")</param>
        public void ImportCatalogFromBinary(string filePathAndName)
        {
            using var db = new EcatalogContext();
            var serializer = new CommonBinarySerializer();
            var importBooks = serializer.Deserialize<List<Book>>(
                System.Text.Encoding.Default.GetBytes(
                    File.ReadAllText(filePathAndName)));
            importBooks.ForEach(f => CheckIsbn(db, f.ISBN));
            db.Books.AddRange(importBooks);
            db.SaveChanges();
        }

        /// <summary>
        /// Check ISBN number for duplicate
        /// </summary>
        /// <param name="db">Db context</param>
        /// <param name="isbn">ISBN number of the book</param>
        private void CheckIsbn(EcatalogContext db, string isbn)
        {
            if (db.Books.Any(a => a.ISBN == isbn))
            {
                throw new InvalidOperationException($"The book with ISBN = '{isbn}' is already exist. You cannot add duplicate book.");
            }
        }

        /// <summary>
        /// Update only changed properties in object
        /// </summary>
        /// <typeparam name="T">Class of object</typeparam>
        /// <param name="original">Original object</param>
        /// <param name="updated">Object with updated properties</param>
        /// <param name="ignoreList">Array of ignored properties (original properties will NOT updated)</param>
        private static void UpdateChangedProperties<T>(T original, T updated, params string[] ignoreList) where T : class
        {
            if (original != null && updated != null)
            {
                Type type = typeof(T);
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object existValue = type.GetProperty(pi.Name).GetValue(original, null);
                        object updatedValue = type.GetProperty(pi.Name).GetValue(updated, null);

                        if (existValue != updatedValue && (existValue == null || !existValue.Equals(updatedValue)))
                        {
                            type.GetProperty(pi.Name).SetValue(original, updatedValue);
                        }
                    }
                }
            }
        }
    }
}
