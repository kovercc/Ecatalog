using Ecatalog.CoreApi.Concrete;
using Ecatalog.CoreApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Ecatalog.CoreApi.Tests.Concrete
{
    [TestClass()]
    public class ECatalogInMemoryTests
    {
        /// <summary>
        /// Check if one book added
        /// </summary>
        [TestMethod()]
        public void AddBook_OneBookAdded()
        {
            // Arrange
            var book = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book);
            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book.ISBN);
            Assert.AreEqual(eCatBook.Name, book.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, book.Authors[0].FirstName);
            Assert.AreEqual(eCatBook.Authors[0].LastName, book.Authors[0].LastName);
            Assert.AreEqual(eCatBook.Authors[0].Note, book.Authors[0].Note);
            Assert.AreEqual(eCatBook.YearOfPublication, book.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, book.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, book.Language);
            Assert.AreEqual(eCatBook.BookRating, book.BookRating);
            Assert.AreEqual(eCatBook.ReaderLevel, book.ReaderLevel);
        }

        /// <summary>
        /// Check if add book with duplicate isbn
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddBook_IsbnIsDuplicate()
        {
            var isbn = Guid.NewGuid();
            // Arrange
            var book1 = new Book()
            {
                ISBN = isbn,
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = isbn,
                Name = "BookName2",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book1);
            manager.AddBook(book2);

            // Assert
        }

        /// <summary>
        /// Check if book edited
        /// </summary>
        [TestMethod()]
        public void AddBook_BookEdited()
        {
            // Arrange
            var book = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book);
            manager.EditBook(book.ISBN, "EditBookName",
                new Author[] { new Author { FirstName = "EditAuthorFirstName", LastName = "EditAuthorLastName", Note = "EditAuthorNote" },
                               new Author { FirstName = "Edit2AuthorFirstName", LastName = "Edit2AuthorLastName", Note = "Edit2AuthorNote" } },
                2010, "F#", ReaderLevel.Middle, "Russian", BookRating.Bad);
            var eCatBooks = manager.GetBooks();

            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book.ISBN);
            Assert.AreEqual(eCatBook.Name, "EditBookName");
            Assert.AreEqual(eCatBook.Authors.Count(), 2);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, "EditAuthorFirstName");
            Assert.AreEqual(eCatBook.Authors[0].LastName, "EditAuthorLastName");
            Assert.AreEqual(eCatBook.Authors[0].Note, "EditAuthorNote");
            Assert.AreEqual(eCatBook.Authors[1].FirstName, "Edit2AuthorFirstName");
            Assert.AreEqual(eCatBook.Authors[1].LastName, "Edit2AuthorLastName");
            Assert.AreEqual(eCatBook.Authors[1].Note, "Edit2AuthorNote");
            Assert.AreEqual(eCatBook.YearOfPublication, 2010);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, "F#");
            Assert.AreEqual(eCatBook.Language, "Russian");
            Assert.AreEqual(eCatBook.BookRating, BookRating.Bad);
            Assert.AreEqual(eCatBook.ReaderLevel, ReaderLevel.Middle);
        }

        /// <summary>
        /// Check if book deleted
        /// </summary>
        [TestMethod()]
        public void DeleteBook_OneBookDeleted()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName2",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" } },
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                BookRating = BookRating.Bad,
                ReaderLevel = ReaderLevel.Elementary
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book1);
            manager.AddBook(book2);
            manager.DeleteBook(book2.ISBN);
            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book1.ISBN);
            Assert.AreEqual(eCatBook.Name, book1.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, book1.Authors[0].FirstName);
            Assert.AreEqual(eCatBook.Authors[0].LastName, book1.Authors[0].LastName);
            Assert.AreEqual(eCatBook.Authors[0].Note, book1.Authors[0].Note);
            Assert.AreEqual(eCatBook.YearOfPublication, book1.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, book1.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, book1.Language);
            Assert.AreEqual(eCatBook.BookRating, book1.BookRating);
            Assert.AreEqual(eCatBook.ReaderLevel, book1.ReaderLevel);
        }

        /// <summary>
        /// Check if book deleted from empty catalog
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteBook_DeletedFromEmptyCatalog()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };          
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.DeleteBook(book1.ISBN);

            // Assert
        }

        /// <summary>
        /// Check GetBooks method
        /// </summary>
        [TestMethod()]
        public void GetBooks_AllReturns()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName2",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" } },
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                BookRating = BookRating.Bad,
                ReaderLevel = ReaderLevel.Elementary
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks().ToArray();

            var eCatBook1 = eCatBooks[0];
            var eCatBook2 = eCatBooks[1];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 2);

            Assert.AreEqual(eCatBook1.ISBN, book1.ISBN);
            Assert.AreEqual(eCatBook1.Name, book1.Name);
            Assert.AreEqual(eCatBook1.Authors.Count(), 1);
            Assert.AreEqual(eCatBook1.Authors[0].FirstName, book1.Authors[0].FirstName);
            Assert.AreEqual(eCatBook1.Authors[0].LastName, book1.Authors[0].LastName);
            Assert.AreEqual(eCatBook1.Authors[0].Note, book1.Authors[0].Note);
            Assert.AreEqual(eCatBook1.YearOfPublication, book1.YearOfPublication);
            Assert.AreEqual(eCatBook1.ProgrammingLanguage, book1.ProgrammingLanguage);
            Assert.AreEqual(eCatBook1.Language, book1.Language);
            Assert.AreEqual(eCatBook1.BookRating, book1.BookRating);
            Assert.AreEqual(eCatBook1.ReaderLevel, book1.ReaderLevel);

            Assert.AreEqual(eCatBook2.ISBN, book2.ISBN);
            Assert.AreEqual(eCatBook2.Name, book2.Name);
            Assert.AreEqual(eCatBook2.Authors.Count(), 1);
            Assert.AreEqual(eCatBook2.Authors[0].FirstName, book2.Authors[0].FirstName);
            Assert.AreEqual(eCatBook2.Authors[0].LastName, book2.Authors[0].LastName);
            Assert.AreEqual(eCatBook2.Authors[0].Note, book2.Authors[0].Note);
            Assert.AreEqual(eCatBook2.YearOfPublication, book2.YearOfPublication);
            Assert.AreEqual(eCatBook2.ProgrammingLanguage, book2.ProgrammingLanguage);
            Assert.AreEqual(eCatBook2.Language, book2.Language);
            Assert.AreEqual(eCatBook2.BookRating, book2.BookRating);
            Assert.AreEqual(eCatBook2.ReaderLevel, book2.ReaderLevel);
        }

        /// <summary>
        /// Check GetBooks method (Author Filter)
        /// </summary>
        [TestMethod()]
        public void GetBooks_WithAuthorFilter()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName2",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" },
                                         new Author { FirstName = "AuthorFirstName3", LastName = "AuthorLastName3", Note = "AuthorNote3" }},
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                BookRating = BookRating.Bad,
                ReaderLevel = ReaderLevel.Elementary
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(null, null, new Author[] { new Author { FirstName = "AuthorFirstName2" }, new Author { LastName = "AuthorLastName3" } }).ToArray();
            var eCatBook2 = eCatBooks[0];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook2.ISBN, book2.ISBN);
            Assert.AreEqual(eCatBook2.Name, book2.Name);
            Assert.AreEqual(eCatBook2.Authors.Count(), 2);
            Assert.AreEqual(eCatBook2.Authors[0].FirstName, book2.Authors[0].FirstName);
            Assert.AreEqual(eCatBook2.Authors[0].LastName, book2.Authors[0].LastName);
            Assert.AreEqual(eCatBook2.Authors[0].Note, book2.Authors[0].Note);
            Assert.AreEqual(eCatBook2.Authors[1].FirstName, book2.Authors[1].FirstName);
            Assert.AreEqual(eCatBook2.Authors[1].LastName, book2.Authors[1].LastName);
            Assert.AreEqual(eCatBook2.Authors[1].Note, book2.Authors[1].Note);
            Assert.AreEqual(eCatBook2.YearOfPublication, book2.YearOfPublication);
            Assert.AreEqual(eCatBook2.ProgrammingLanguage, book2.ProgrammingLanguage);
            Assert.AreEqual(eCatBook2.Language, book2.Language);
            Assert.AreEqual(eCatBook2.BookRating, book2.BookRating);
            Assert.AreEqual(eCatBook2.ReaderLevel, book2.ReaderLevel);
        }

        /// <summary>
        /// Check GetBooks method (Name Filter)
        /// </summary>
        [TestMethod()]
        public void GetBooks_WithNameFilter()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "DifferentName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" } },
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                BookRating = BookRating.Bad,
                ReaderLevel = ReaderLevel.Elementary
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(null, "fferent").ToArray();
            var eCatBook2 = eCatBooks[0];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook2.ISBN, book2.ISBN);
            Assert.AreEqual(eCatBook2.Name, book2.Name);
            Assert.AreEqual(eCatBook2.Authors.Count(), 1);
            Assert.AreEqual(eCatBook2.Authors[0].FirstName, book2.Authors[0].FirstName);
            Assert.AreEqual(eCatBook2.Authors[0].LastName, book2.Authors[0].LastName);
            Assert.AreEqual(eCatBook2.Authors[0].Note, book2.Authors[0].Note);
            Assert.AreEqual(eCatBook2.YearOfPublication, book2.YearOfPublication);
            Assert.AreEqual(eCatBook2.ProgrammingLanguage, book2.ProgrammingLanguage);
            Assert.AreEqual(eCatBook2.Language, book2.Language);
            Assert.AreEqual(eCatBook2.BookRating, book2.BookRating);
            Assert.AreEqual(eCatBook2.ReaderLevel, book2.ReaderLevel);
        }

        /// <summary>
        /// Check GetBooks method (ReaderLevel Filter)
        /// </summary>
        [TestMethod()]
        public void GetBooks_WithReaderLevelFilter()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "DifferentName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" } },
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                BookRating = BookRating.Bad,
                ReaderLevel = ReaderLevel.Elementary
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(null, null, null, null, null, ReaderLevel.Elementary).ToArray();
            var eCatBook2 = eCatBooks[0];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook2.ISBN, book2.ISBN);
            Assert.AreEqual(eCatBook2.Name, book2.Name);
            Assert.AreEqual(eCatBook2.Authors.Count(), 1);
            Assert.AreEqual(eCatBook2.Authors[0].FirstName, book2.Authors[0].FirstName);
            Assert.AreEqual(eCatBook2.Authors[0].LastName, book2.Authors[0].LastName);
            Assert.AreEqual(eCatBook2.Authors[0].Note, book2.Authors[0].Note);
            Assert.AreEqual(eCatBook2.YearOfPublication, book2.YearOfPublication);
            Assert.AreEqual(eCatBook2.ProgrammingLanguage, book2.ProgrammingLanguage);
            Assert.AreEqual(eCatBook2.Language, book2.Language);
            Assert.AreEqual(eCatBook2.BookRating, book2.BookRating);
            Assert.AreEqual(eCatBook2.ReaderLevel, book2.ReaderLevel);
        }

        /// <summary>
        /// Check XML Serialize ad Deserialize
        /// </summary>
        [TestMethod()]
        public void ExportCatalogToXml_And_ImportCatalogFromXml_CheckFullScenario()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };
          
            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = @"c:\Test\test.xml";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToXml(filePath);
            manager.DeleteBook(book1.ISBN);
            manager.ImportCatalogFromXml(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book1.ISBN);
            Assert.AreEqual(eCatBook.Name, book1.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, book1.Authors[0].FirstName);
            Assert.AreEqual(eCatBook.Authors[0].LastName, book1.Authors[0].LastName);
            Assert.AreEqual(eCatBook.Authors[0].Note, book1.Authors[0].Note);
            Assert.AreEqual(eCatBook.YearOfPublication, book1.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, book1.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, book1.Language);
            Assert.AreEqual(eCatBook.BookRating, book1.BookRating);
            Assert.AreEqual(eCatBook.ReaderLevel, book1.ReaderLevel);
        }

        /// <summary>
        /// Check Json Serialize ad Deserialize
        /// </summary>
        [TestMethod()]
        public void ExportCatalogToJson_And_ImportCatalogFromJson_CheckFullScenario()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };

            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = @"c:\Test\test.json";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToJson(filePath);
            manager.DeleteBook(book1.ISBN);
            manager.ImportCatalogFromJson(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book1.ISBN);
            Assert.AreEqual(eCatBook.Name, book1.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, book1.Authors[0].FirstName);
            Assert.AreEqual(eCatBook.Authors[0].LastName, book1.Authors[0].LastName);
            Assert.AreEqual(eCatBook.Authors[0].Note, book1.Authors[0].Note);
            Assert.AreEqual(eCatBook.YearOfPublication, book1.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, book1.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, book1.Language);
            Assert.AreEqual(eCatBook.BookRating, book1.BookRating);
            Assert.AreEqual(eCatBook.ReaderLevel, book1.ReaderLevel);
        }

        /// <summary>
        /// Check Binary Serialize ad Deserialize
        /// </summary>
        [TestMethod()]
        public void ExportCatalogToBinary_And_ImportCatalogFromBinary_CheckFullScenario()
        {
            // Arrange
            var book1 = new Book()
            {
                ISBN = Guid.NewGuid(),
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                BookRating = BookRating.Good,
                ReaderLevel = ReaderLevel.Experienced
            };

            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = @"c:\Test\test.xml";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToBinary(filePath);
            manager.DeleteBook(book1.ISBN);
            manager.ImportCatalogFromBinary(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book1.ISBN);
            Assert.AreEqual(eCatBook.Name, book1.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            Assert.AreEqual(eCatBook.Authors[0].FirstName, book1.Authors[0].FirstName);
            Assert.AreEqual(eCatBook.Authors[0].LastName, book1.Authors[0].LastName);
            Assert.AreEqual(eCatBook.Authors[0].Note, book1.Authors[0].Note);
            Assert.AreEqual(eCatBook.YearOfPublication, book1.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, book1.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, book1.Language);
            Assert.AreEqual(eCatBook.BookRating, book1.BookRating);
            Assert.AreEqual(eCatBook.ReaderLevel, book1.ReaderLevel);
        }
    }
}
