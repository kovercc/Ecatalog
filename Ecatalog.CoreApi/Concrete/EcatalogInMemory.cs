using Ecatalog.CoreApi.Abstract;
using Ecatalog.CoreApi.Common;
using Ecatalog.CoreApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

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
        private List<Book> _books = new List<Book>();

        /// <summary>
        /// Add Book to e-catalog
        /// </summary>
        /// <param name="book">Book object with data</param>
        public void AddBook(Book book)
        {
            CheckIsbn(book.ISBN);
            var results = new List<ValidationResult>();
            var context = new ValidationContext(book);
            if (!Validator.TryValidateObject(book, context, results, true))
            {
                throw new InvalidDataException(string.Join(", ", results.Select(s => s.ErrorMessage)));
            }
            _books.Add(new Book
            {
                ISBN = book.ISBN,
                Name = book.Name,
                Authors = book.Authors,
                YearOfPublication = book.YearOfPublication,
                ProgrammingLanguage = book.ProgrammingLanguage,
                ReaderLevel = book.ReaderLevel,
                Language = book.Language,
                BookRating = book.BookRating
            });
        }

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
        public void EditBook(Guid isbn, string name = null, Author[] authors = null, int? yearOfPublication = null, string programmingLanguage = null, ReaderLevel? readerLevel = null, string language = null, BookRating? bookRating = null)
        {
            var book = _books.Single(a => a.ISBN == isbn);
            if (book == null)
            {
                throw new InvalidOperationException($"The book with ISBN = '{isbn}' didn't find. Check the ISBN code and try again.");
            }
            if (name != null)
            {
                book.Name = name;
            }
            if (authors != null)
            {
                book.Authors = authors;
            }
            if (yearOfPublication != null)
            {
                book.YearOfPublication = (int)yearOfPublication;
            }
            if (programmingLanguage != null)
            {
                book.ProgrammingLanguage = programmingLanguage;
            }
            if (readerLevel != null)
            {
                book.ReaderLevel = (ReaderLevel)readerLevel;
            }
            if (language != null)
            {
                book.Language = language;
            }
            if (bookRating != null)
            {
                book.BookRating = (BookRating)bookRating;
            }
        }

        /// <summary>
        /// Delete book from e-catalog
        /// </summary>
        /// <param name="isbn">ISBN code of the Book in e-Catalog</param>
        public void DeleteBook(Guid isbn)
        {
            if (!_books.Any(a => a.ISBN == isbn))
            {
                throw new InvalidOperationException($"The book with ISBN = '{isbn}' didn't find. Check the ISBN code and try again.");
            }
            _books.Remove(_books.Single(s => s.ISBN == isbn));
        }

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
        public IEnumerable<Book> GetBooks(Guid? isbn = null, string name = null, Author[] authors = null, int? yearOfPublication = null, string programmingLanguage = null, ReaderLevel? readerLevel = null, string language = null, BookRating? bookRating = null)
        {
            var filteredBooks = _books.Where(w =>
            (isbn == null || w.ISBN == isbn)
            && (name == null || w.Name.ToLower().Contains(name.ToLower()))
            //&& ((authors == null || !authors.Any() 
            //|| w.Authors.Any(a => authors.Any(aut => aut.LastName?.ToLower() == a.LastName.ToLower()))
            //|| w.Authors.Any(a => authors.Any(aut => aut.FirstName?.ToLower() == a.FirstName.ToLower())))
            //|| w.Authors.Any(a => authors.Any(aut => aut.Note?.ToLower() == a.Note.ToLower())))
            && (yearOfPublication == null || w.YearOfPublication == yearOfPublication)
            && (programmingLanguage == null || w.ProgrammingLanguage == programmingLanguage)
            && (readerLevel == null || w.ReaderLevel == readerLevel)
            && (language == null || w.Language == language)
            && (bookRating == null || w.BookRating == bookRating)
            );
            if (authors != null) {
                var authorFilteredBooks = new List<Book>();
                var hasAuthorFilter = false;
                foreach (var author in authors)
                {
                    if (!string.IsNullOrEmpty(author.FirstName) || !string.IsNullOrEmpty(author.LastName) || !string.IsNullOrEmpty(author.Note)) {
                        hasAuthorFilter = true;
                        authorFilteredBooks.AddRange(filteredBooks.Where(w =>
                               w.Authors.Any(a => authors.Any(aut => aut.LastName?.ToLower() == a.LastName.ToLower()))
                            || w.Authors.Any(a => authors.Any(aut => aut.FirstName?.ToLower() == a.FirstName.ToLower()))
                            || w.Authors.Any(a => authors.Any(aut => aut.Note?.ToLower() == a.Note.ToLower()))));
                    }
                }
                if (hasAuthorFilter)
                {
                    filteredBooks = authorFilteredBooks;
                }
            }
            return filteredBooks.Distinct();
        }

        /// <summary>
        /// Export e-catalog to xml file (.xml)
        /// </summary>
        /// <param name="filePathAndName">file name with path on the computer (@"c:\exmaple.xml")</param>
        public void ExportCatalogToXml(string filePathAndName)
        {
            var serializer = new CommonXmlSerializer();
            File.WriteAllText(filePathAndName, serializer.ObjectToString(_books));
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
            File.WriteAllText(filePathAndName, System.Text.Encoding.Default.GetString(serializer.Serialize(_books)));
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
        private void CheckIsbn(Guid isbn)
        {
            if (_books.Any(a => a.ISBN == isbn))
            {
                throw new InvalidOperationException($"The book with ISBN = '{isbn}' is already exist. You cannot add duplicate book.");
            }
        }
    }
}
