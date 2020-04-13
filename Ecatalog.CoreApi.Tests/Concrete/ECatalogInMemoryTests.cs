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
        private readonly string _testFolderPath = @"c:\Test\";
        private readonly string _testISBN1 = "978-5-6041394-3-1";
        private readonly string _testISBN2 = "978-5-6041395-3-1";

        public ECatalogInMemoryTests()
        {
            System.IO.Directory.CreateDirectory(_testFolderPath);
        }

        /// <summary>
        /// Check if one book added
        /// </summary>
        [TestMethod()]
        public void AddBook_OneBookAdded()
        {
            // Arrange
            var book = new Book()
            {
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book);
            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book, eCatBook);
        }

        /// <summary>
        /// Check if add book with duplicate isbn
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddBook_IsbnIsDuplicate()
        {
            var isbn = Guid.NewGuid();
            var author = new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" };
            // Arrange
            var book1 = new Book()
            {
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(author);
            var book2 = new Book()
            {
                ISBN = _testISBN1,
                Name = "BookName2",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book2.Authors.Add(author);
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
        public void EditBook_BookEdited()
        {
            // Arrange
            var book = new Book()
            {
                ISBN = _testISBN1,
                Name = "BookName",
                Authors = new Author[] { new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" } },
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book);
            var addedBook = manager.GetBooks(a => a.ISBN == book.ISBN).FirstOrDefault();
            addedBook.Name = "EditBookName";
            addedBook.Authors = new Author[] {
                    new Author { FirstName = "EditAuthorFirstName", LastName = "EditAuthorLastName", Note = "EditAuthorNote" },
                    new Author { FirstName = "Edit2AuthorFirstName", LastName = "Edit2AuthorLastName", Note = "Edit2AuthorNote" }
                };
            addedBook.ProgrammingLanguage = "F#";
            addedBook.YearOfPublication = 2010;
            addedBook.Language = "Russian";
            addedBook.ReaderLevel = ReaderLevel.Middle;

            manager.EditBook(addedBook);

            var eCatBooks = manager.GetBooks();

            var eCatBook = eCatBooks.FirstOrDefault();
            var eCatBookAuthors = eCatBook.Authors.ToArray();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.ISBN, book.ISBN);
            Assert.AreEqual(eCatBook.Name, "EditBookName");
            Assert.AreEqual(eCatBook.Authors.Count(), 2);
            Assert.AreEqual(eCatBookAuthors[0].FirstName, "EditAuthorFirstName");
            Assert.AreEqual(eCatBookAuthors[0].LastName, "EditAuthorLastName");
            Assert.AreEqual(eCatBookAuthors[0].Note, "EditAuthorNote");
            Assert.AreEqual(eCatBookAuthors[1].FirstName, "Edit2AuthorFirstName");
            Assert.AreEqual(eCatBookAuthors[1].LastName, "Edit2AuthorLastName");
            Assert.AreEqual(eCatBookAuthors[1].Note, "Edit2AuthorNote");
            Assert.AreEqual(eCatBook.YearOfPublication, 2010);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, "F#");
            Assert.AreEqual(eCatBook.Language, "Russian");
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var book2 = new Book()
            {
                ISBN = _testISBN2,
                Name = "BookName2",
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                ReaderLevel = ReaderLevel.Elementary
            };
            book2.Authors.Add(new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" });
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book1);
            var addedBook2Id = manager.AddBook(book2);
            manager.DeleteBook(addedBook2Id);
            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            CompareBooks(book1, eCatBook);
        }

        /// <summary>
        /// Check if book deleted from empty catalog
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DeleteBook_DeletedFromEmptyCatalog()
        {
            // Arrange
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.DeleteBook(Guid.NewGuid());

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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var book2 = new Book()
            {
                ISBN = _testISBN2,
                Name = "BookName2",
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                ReaderLevel = ReaderLevel.Elementary
            };
            book2.Authors.Add(new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" });
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks().ToArray();

            var eCatBook1 = eCatBooks[0];
            var eCatBook2 = eCatBooks[1];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 2);

            CompareBooks(book1, eCatBook1);
            CompareBooks(book2, eCatBook2);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            var book1Author = new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" };
            book1.Authors.Add(book1Author);
            var book2 = new Book()
            {
                ISBN = _testISBN2,
                Name = "BookName2",
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                ReaderLevel = ReaderLevel.Elementary
            };
            var book2Authors = new Author[] { new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" },
                                         new Author { FirstName = "AuthorFirstName3", LastName = "AuthorLastName3", Note = "AuthorNote3" }};
            book2.Authors.Add(book2Authors[0]);
            book2.Authors.Add(book2Authors[1]);
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(null, (a => a.FirstName.Contains("AuthorFirst") && a.LastName.Contains("LastName3"))).ToArray();
            var eCatBook2 = eCatBooks.FirstOrDefault();
            var eCatBook2Authors = eCatBook2.Authors.ToArray();
            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book2, eCatBook2);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var book2 = new Book()
            {
                ISBN = _testISBN2,
                Name = "DifferentName",
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                ReaderLevel = ReaderLevel.Elementary
            };
            book2.Authors.Add(new Author { FirstName = "AuthorFirstName2", LastName = "AuthorLastName2", Note = "AuthorNote2" });
            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(b => b.Name.Contains("fferent")).ToArray();
            var eCatBook2 = eCatBooks[0];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book2, eCatBook2);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            var book2 = new Book()
            {
                ISBN = _testISBN2,
                Name = "DifferentName",
                ProgrammingLanguage = "F#",
                YearOfPublication = 2010,
                Language = "English",
                ReaderLevel = ReaderLevel.Elementary
            };

            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            book2.Authors.Add(new Author { FirstName = "Author2FirstName", LastName = "Author2LastName", Note = "Author2Note" });

            var manager = EcatalogManager.GetCatalogInMamory();

            // Act 
            manager.AddBook(book1);
            manager.AddBook(book2);
            var eCatBooks = manager.GetBooks(b => b.ReaderLevel == ReaderLevel.Elementary).ToArray();
            var eCatBook2 = eCatBooks[0];

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book2, eCatBook2);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = $@"{_testFolderPath}test.xml";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToXml(filePath);
            manager.DeleteBook(book1.Id);
            manager.ImportCatalogFromXml(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book1, eCatBook);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            var authors = new[]
            {
                new Author { FirstName = "Author1FirstName", LastName = "Author1LastName", Note = "Author1Note" },
                new Author { FirstName = "Author2FirstName", LastName = "Author2LastName", Note = "Author2Note" }
            };
            book1.Authors.Add(authors[0]);
            book1.Authors.Add(authors[1]);

            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = $@"{_testFolderPath}test.json";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToJson(filePath);
            manager.DeleteBook(book1.Id);
            manager.ImportCatalogFromJson(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();
            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            CompareBooks(book1, eCatBook);
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
                ISBN = _testISBN1,
                Name = "BookName",
                ProgrammingLanguage = "C#",
                YearOfPublication = 2009,
                Language = "English",
                ReaderLevel = ReaderLevel.Experienced
            };
            book1.Authors.Add(new Author { FirstName = "AuthorFirstName", LastName = "AuthorLastName", Note = "AuthorNote" });
            var manager = EcatalogManager.GetCatalogInMamory();
            var filePath = $@"{_testFolderPath}test.xml";

            // Act
            manager.AddBook(book1);
            manager.ExportCatalogToBinary(filePath);
            manager.DeleteBook(book1.Id);
            manager.ImportCatalogFromBinary(filePath);

            var eCatBooks = manager.GetBooks();
            var eCatBook = eCatBooks.FirstOrDefault();

            // Assert
            Assert.AreEqual(eCatBooks.Count(), 1);
            Assert.AreEqual(eCatBook.Authors.Count(), 1);
            CompareBooks(book1, eCatBook);
        }

        private void CompareBooks(Book origBook, Book eCatBook)
        {
            Assert.AreEqual(eCatBook.ISBN, origBook.ISBN);
            Assert.AreEqual(eCatBook.Name, origBook.Name);
            Assert.AreEqual(eCatBook.Authors.Count(), origBook.Authors.Count());
            var origAuthors = origBook.Authors.ToArray();
            var eCatAuthors = eCatBook.Authors.ToArray();
            for (var i = 0; i < eCatAuthors.Count(); i++)
            {
                Assert.AreEqual(eCatAuthors[i].FirstName, origAuthors[i].FirstName);
                Assert.AreEqual(eCatAuthors[i].LastName, origAuthors[i].LastName);
                Assert.AreEqual(eCatAuthors[i].Note, origAuthors[i].Note);
            }
            Assert.AreEqual(eCatBook.YearOfPublication, origBook.YearOfPublication);
            Assert.AreEqual(eCatBook.ProgrammingLanguage, origBook.ProgrammingLanguage);
            Assert.AreEqual(eCatBook.Language, origBook.Language);
            Assert.AreEqual(eCatBook.ReaderLevel, origBook.ReaderLevel);
        }
    }
}
