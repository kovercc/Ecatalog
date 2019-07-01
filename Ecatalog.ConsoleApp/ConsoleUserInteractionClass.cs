using Ecatalog.CoreApi.Abstract;
using Ecatalog.CoreApi.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ecatalog.ConsoleApp
{
    /// <summary>
    /// Class of user interaction methods
    /// </summary>
    static class ConsoleUserInteractionClass
    {
        /// <summary>
        /// Regex for validate file path and name
        /// </summary>
        private const string FILE_PATH_AND_NAME_BASE = @"^(?:[\w]\:|\\)(\\[aA-zZ_аА-яЯ\-\s0-9\.]+)+\.";

        /// <summary>
        /// Handle command from console
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="eCatalog">E-catalog instance</param>
        public static void HandleCommand(string command, IEcatalogCoreApi eCatalog, ILog _logger)
        {
            if (command == Commands.ADD)
            {
                AskBookProperties(true, false, out Guid? isbnV, out string nameV, out Author[] authors, out int? yearOfPublicationV,
                            out string programmingLanguageV, out ReaderLevel? readerLevelV, out string languageV, out BookRating? bookRating);
                var book = new Book
                {
                    ISBN = (Guid)isbnV,
                    Name = nameV,
                    Authors = authors,
                    YearOfPublication = (int)yearOfPublicationV,
                    ProgrammingLanguage = programmingLanguageV,
                    ReaderLevel = (ReaderLevel)readerLevelV,
                    Language = languageV,
                    BookRating = (BookRating)bookRating
                };
                eCatalog.AddBook(book);
                _logger.Info(LocalizedStrings.BookAdded);
            }
            else if (command == Commands.EDIT)
            {
                AskBookProperties(false, true, out Guid? isbnV, out string nameV, out Author[] authors, out int? yearOfPublicationV,
                            out string programmingLanguageV, out ReaderLevel? readerLevelV, out string languageV, out BookRating? bookRating);
                eCatalog.EditBook((Guid)isbnV, nameV, authors, yearOfPublicationV, programmingLanguageV, readerLevelV, languageV, bookRating);
                _logger.Info(LocalizedStrings.BookEdited);
            }
            else if (command == Commands.DELETE)
            {
                var isbn = AskIsbn(true);
                eCatalog.DeleteBook((Guid)isbn);
                _logger.Info(LocalizedStrings.BookDeleted);
            }
            else if (command == Commands.GET)
            {
                AskBookProperties(false, false, out Guid? isbnV, out string nameV, out Author[] authors, out int? yearOfPublicationV,
                            out string programmingLanguageV, out ReaderLevel? readerLevelV, out string languageV, out BookRating? bookRating);
                var books = eCatalog.GetBooks(isbnV, nameV, authors, yearOfPublicationV, programmingLanguageV, readerLevelV, languageV, bookRating);
                _logger.Info($"{LocalizedStrings.BookRetrieved} {books.Count()}. {LocalizedStrings.ShortList}:");
                _logger.Info(books.Select(s => $"{Environment.NewLine}ISBN:'{s.ISBN}', '{s.Name}', {LocalizedStrings.PreAuthors} {string.Join($" {LocalizedStrings.And} ", s.Authors?.Select(a => a?.ToShortString()).ToArray())}"));
            }
            else if (command == Commands.EXPORT_XML)
            {
                eCatalog.ExportCatalogToXml(AskXmlFilePathAndName());
                _logger.Info(LocalizedStrings.ExportDone);
            }
            else if (command == Commands.EXPORT_JSON)
            {
                eCatalog.ExportCatalogToJson(AskJsonFilePathAndName());
                _logger.Info(LocalizedStrings.ExportDone);
            }
            else if (command == Commands.EXPORT_BINARY)
            {
                eCatalog.ExportCatalogToBinary(AskDatFilePathAndName());
                _logger.Info(LocalizedStrings.ExportDone);
            }
            else if (command == Commands.IMPORT_XML)
            {
                eCatalog.ImportCatalogFromXml(AskXmlFilePathAndName());
                _logger.Info(LocalizedStrings.ImportDone);
            }
            else if (command == Commands.IMPORT_JSON)
            {
                eCatalog.ImportCatalogFromJson(AskJsonFilePathAndName());
                _logger.Info(LocalizedStrings.ImportDone);
            }
            else if (command == Commands.IMPORT_BINARY)
            {
                eCatalog.ImportCatalogFromBinary(AskDatFilePathAndName());
                _logger.Info(LocalizedStrings.ImportDone);
            }
        }

        /// <summary>
        /// Ask file path and name from console and validate
        /// </summary>
        /// <returns>Valid string with file path and name</returns>
        private static string AskXmlFilePathAndName()
        {
            var filePathReg = $"{FILE_PATH_AND_NAME_BASE}xml$";
            Console.Write(LocalizedStrings.AskXmlFilePathAndName);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Console.WriteLine(LocalizedStrings.AskXmlFilePathAndName);
                filePathAndName = Console.ReadLine().Trim();
            }
            return filePathAndName;
        }

        /// <summary>
        /// Ask file path and name from console and validate
        /// </summary>
        /// <returns>Valid string with file path and name</returns>
        private static string AskJsonFilePathAndName()
        {
            var filePathReg = $"{FILE_PATH_AND_NAME_BASE}json$";
            Console.Write(LocalizedStrings.AskJsonFilePathAndName);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Console.WriteLine(LocalizedStrings.AskJsonFilePathAndName);
                filePathAndName = Console.ReadLine().Trim();
            }
            return filePathAndName;
        }

        /// <summary>
        /// Ask file path and name from console and validate
        /// </summary>
        /// <returns>Valid string with file path and name</returns>
        private static string AskDatFilePathAndName()
        {
            var filePathReg = $"{FILE_PATH_AND_NAME_BASE}dat$";
            Console.Write(LocalizedStrings.AskDatFilePathAndName);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Console.WriteLine(LocalizedStrings.AskDatFilePathAndName);
                filePathAndName = Console.ReadLine().Trim();
            }
            return filePathAndName;
        }

        /// <summary>
        /// Ask ISBN number from console and validate it
        /// </summary>
        /// <returns>Valid string with ISBN number</returns>
        private static Guid? AskIsbn(bool isMandatory)
        {
            Console.WriteLine(LocalizedStrings.AskISBN);
            var isbn = Console.ReadLine().Trim();
            Guid isbnV = Guid.Empty;

            while ((isMandatory && (string.IsNullOrEmpty(isbn) || !Guid.TryParse(isbn, out isbnV))
                || (!isMandatory && (!string.IsNullOrEmpty(isbn) && !Guid.TryParse(isbn, out isbnV)))))
            {
                Console.WriteLine(LocalizedStrings.AskISBN);
                isbn = Console.ReadLine().Trim();
            }
            return isbnV != Guid.Empty ? (Guid?)isbnV : null;
        }

        /// <summary>
        /// Ask Author data from console and validate it
        /// </summary>
        /// <returns>Valid Author object</returns>
        private static Author AskAuthors(bool isMandatory)
        {
            Console.WriteLine(LocalizedStrings.AskAuthorFirstName);
            var firstName = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(firstName))
            {
                Console.WriteLine(LocalizedStrings.AskAuthorFirstName);
                firstName = Console.ReadLine().Trim();
            }

            Console.WriteLine(LocalizedStrings.AskAuthorLastName);
            var lastName = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(lastName))
            {
                Console.WriteLine(LocalizedStrings.AskAuthorLastName);
                lastName = Console.ReadLine().Trim();
            }

            Console.WriteLine(LocalizedStrings.AskAuthorNote);
            var note = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(note))
            {
                Console.WriteLine(LocalizedStrings.AskAuthorNote);
                note = Console.ReadLine().Trim();
            }
            return new Author { FirstName = firstName, LastName = lastName, Note = note };
        }

        /// <summary>
        /// Ask Book parameters from console and validate it
        /// </summary>
        /// <param name="isAdd">Is it ADD command</param>
        /// <param name="isEdit">Is it EDIT command</param>
        /// <param name="isbnV">ISBN number</param>
        /// <param name="nameV">Name of the book</param>
        /// <param name="authors">Authors</param>
        /// <param name="yearOfPublicationV">Year of the book publication</param>
        /// <param name="programmingLanguageV">Programming language in the book</param>
        /// <param name="readerLevelV">Reader level</param>
        /// <param name="languageV">Language of the book</param>
        /// <param name="bookRatingV">Book rating</param>
        private static void AskBookProperties(bool isAdd, bool isEdit, out Guid? isbnV, out string nameV, out Author[] authors, out int? yearOfPublicationV,
            out string programmingLanguageV, out ReaderLevel? readerLevelV, out string languageV, out BookRating? bookRatingV)
        {
            if (isAdd)
            {
                Console.WriteLine(LocalizedStrings.AddMandatory);
            }
            if (isEdit)
            {
                Console.WriteLine(LocalizedStrings.EditMandatory);
            }

            // ISBN
            isbnV = AskIsbn(isAdd || isEdit);

            // Name
            Console.WriteLine(LocalizedStrings.AskName);
            nameV = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(nameV))
            {
                Console.WriteLine(LocalizedStrings.AskName);
                nameV = Console.ReadLine().Trim();
            }

            // Authors
            var authorsList = new List<Author>
            {
                AskAuthors(isAdd)
            };
            Console.WriteLine(LocalizedStrings.AskForAddAuthor);
            var answer = Console.ReadLine().Trim().ToUpper();
            while (answer == LocalizedStrings.AnswerYes.ToUpper())
            {
                authorsList.Add(AskAuthors(isAdd));
                Console.WriteLine(LocalizedStrings.AskForAddAuthor);
                answer = Console.ReadLine().Trim();
            }
            authors = authorsList.ToArray();

            // Year of publication
            Console.WriteLine(LocalizedStrings.AskYearOfPublication);
            var yearOfPublication = Console.ReadLine().Trim();
            int yearOfPublicationInt = int.MinValue;

            while ((isAdd && (string.IsNullOrEmpty(yearOfPublication) || !int.TryParse(yearOfPublication, out yearOfPublicationInt))
                || (!isAdd && (!string.IsNullOrEmpty(yearOfPublication) && !int.TryParse(yearOfPublication, out yearOfPublicationInt)))))
            {
                Console.WriteLine(LocalizedStrings.AskYearOfPublication);
                yearOfPublication = Console.ReadLine().Trim();
            }
            yearOfPublicationV = !string.IsNullOrEmpty(yearOfPublication) ? (int?)yearOfPublicationInt : null;

            // Programming Language
            Console.WriteLine(LocalizedStrings.AskProgrammingLanguage);
            programmingLanguageV = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(programmingLanguageV))
            {
                Console.WriteLine(LocalizedStrings.AskProgrammingLanguage);
                programmingLanguageV = Console.ReadLine().Trim();
            }
            programmingLanguageV = !string.IsNullOrEmpty(programmingLanguageV) ? programmingLanguageV : null;

            // Reader level
            Console.WriteLine(LocalizedStrings.AskReaderLevel);
            var readerLevel = Console.ReadLine().Trim();
            int readerLevelInt = int.MinValue;

            while ((isAdd && (string.IsNullOrEmpty(readerLevel) || !int.TryParse(readerLevel, out readerLevelInt) || !Enum.IsDefined(typeof(ReaderLevel), readerLevelInt))
                || (!isAdd && (!string.IsNullOrEmpty(readerLevel) && (!int.TryParse(readerLevel, out readerLevelInt) || !Enum.IsDefined(typeof(ReaderLevel), readerLevel))))))
            {
                Console.WriteLine(LocalizedStrings.AskReaderLevel);
                readerLevel = Console.ReadLine().Trim();
            }
            readerLevelV = !string.IsNullOrEmpty(readerLevel) ? (ReaderLevel?)readerLevelInt : null;

            // Language
            Console.WriteLine(LocalizedStrings.AskLanguage);
            languageV = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(languageV))
            {
                Console.WriteLine(LocalizedStrings.AskLanguage);
                languageV = Console.ReadLine().Trim();
            }
            languageV = !string.IsNullOrEmpty(languageV) ? languageV : null;

            // Book Rating
            Console.WriteLine(LocalizedStrings.AskBookRating);
            var bookRating = Console.ReadLine().Trim();
            var bookRatingInt = int.MinValue;

            while ((isAdd && (string.IsNullOrEmpty(bookRating) || !int.TryParse(bookRating, out bookRatingInt) || !Enum.IsDefined(typeof(BookRating), bookRatingInt))
                || (!isAdd && (!string.IsNullOrEmpty(bookRating) && (!int.TryParse(bookRating, out bookRatingInt) || !Enum.IsDefined(typeof(BookRating), bookRatingInt))))))
            {
                Console.WriteLine(LocalizedStrings.AskBookRating);
                bookRating = Console.ReadLine().Trim();
            }
            bookRatingV = !string.IsNullOrEmpty(bookRating) ? (BookRating?)bookRatingInt : null;
        }
    }
}
