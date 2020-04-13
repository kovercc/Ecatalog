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
                AskBookProperties(true, false, out Book book);
                eCatalog.AddBook(book);
                _logger.Info(LocalizedStrings.BookAdded);
            }
            else if (command == Commands.EDIT)
            {
                AskBookProperties(false, true, out Book book);
                eCatalog.EditBook(book);
                _logger.Info(LocalizedStrings.BookEdited);
            }
            else if (command == Commands.DELETE)
            {
                var isbn = AskIsbn(true);
                var book = eCatalog.GetBooks(b => b.ISBN == isbn).FirstOrDefault();
                eCatalog.DeleteBook(book.Id);
                _logger.Info(LocalizedStrings.BookDeleted);
            }
            else if (command == Commands.GET)
            {
                AskBookProperties(false, false, out Book book);
                var books = eCatalog.GetBooks(s =>
                    (book.ISBN != null && s.ISBN == book.ISBN) ||
                    (book.Name != null && s.Name.Contains(book.Name)) ||
                    (book.Language != null && s.Language == book.Language) ||
                    (book.ProgrammingLanguage != null && s.ProgrammingLanguage.Contains(book.ProgrammingLanguage)) ||
                    (book.YearOfPublication != null && s.YearOfPublication == book.YearOfPublication) ||
                    (book.ReaderLevel != null && s.ReaderLevel == book.ReaderLevel),

                    a => book.Authors.Any(b => !string.IsNullOrEmpty(b.FirstName) || !string.IsNullOrEmpty(b.LastName) || !string.IsNullOrEmpty(b.Note)) 
                    ? (
                        (book.Authors.Select(f => f.FirstName).Where(w => !string.IsNullOrEmpty(w)).Contains(a.FirstName)) ||
                        (book.Authors.Select(f => f.LastName).Where(w => !string.IsNullOrEmpty(w)).Contains(a.LastName)) ||
                        (book.Authors.Select(f => f.Note).Where(w => !string.IsNullOrEmpty(w)).Contains(a.Note))
                      ) 
                    : true                    
                    );
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
            Hepler.WriteColoredLine(LocalizedStrings.AskXmlFilePathAndName, ConsoleColor.Yellow);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskXmlFilePathAndName, ConsoleColor.Yellow);
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
            Hepler.WriteColoredLine(LocalizedStrings.AskJsonFilePathAndName, ConsoleColor.Yellow);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskJsonFilePathAndName, ConsoleColor.Yellow);
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
            Hepler.WriteColoredLine(LocalizedStrings.AskDatFilePathAndName, ConsoleColor.Yellow);
            var filePathAndName = Console.ReadLine().Trim();
            while (!Regex.IsMatch(filePathAndName, filePathReg))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskDatFilePathAndName, ConsoleColor.Yellow);
                filePathAndName = Console.ReadLine().Trim();
            }
            return filePathAndName;
        }

        /// <summary>
        /// Ask ISBN number from console and validate it
        /// </summary>
        /// <returns>Valid string with ISBN number</returns>
        private static string AskIsbn(bool isMandatory)
        {
            Hepler.WriteColoredLine(LocalizedStrings.AskISBN, ConsoleColor.Yellow);
            var isbn = Console.ReadLine().Trim();
            while (isMandatory && (string.IsNullOrEmpty(isbn)))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskISBN, ConsoleColor.Yellow);
                isbn = Console.ReadLine().Trim();
            }
            return isbn;
        }

        /// <summary>
        /// Ask Author data from console and validate it
        /// </summary>
        /// <returns>Valid Author object</returns>
        private static Author AskAuthors(bool isMandatory)
        {
            Hepler.WriteColoredLine(LocalizedStrings.AskAuthorFirstName, ConsoleColor.Yellow);
            var firstName = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(firstName))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskAuthorFirstName, ConsoleColor.Yellow);
                firstName = Console.ReadLine().Trim();
            }

            Hepler.WriteColoredLine(LocalizedStrings.AskAuthorLastName, ConsoleColor.Yellow);
            var lastName = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(lastName))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskAuthorLastName, ConsoleColor.Yellow);
                lastName = Console.ReadLine().Trim();
            }

            Hepler.WriteColoredLine(LocalizedStrings.AskAuthorNote, ConsoleColor.Yellow);
            var note = Console.ReadLine().Trim();
            while (isMandatory && string.IsNullOrEmpty(note))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskAuthorNote, ConsoleColor.Yellow);
                note = Console.ReadLine().Trim();
            }
            return new Author { FirstName = firstName, LastName = lastName, Note = note };
        }

        /// <summary>
        /// Ask Book parameters from console and validate it
        /// </summary>
        /// <param name="isAdd">Is it ADD command</param>
        /// <param name="isEdit">Is it EDIT command</param>
        /// <param name="Book">Book object with properties</param>
        private static void AskBookProperties(bool isAdd, bool isEdit, out Book book)
        {
            if (isAdd)
            {
                Hepler.WriteColoredLine(LocalizedStrings.AddMandatory, ConsoleColor.Yellow);
            }
            if (isEdit)
            {
                Hepler.WriteColoredLine(LocalizedStrings.EditMandatory, ConsoleColor.Yellow);
            }
            book = new Book();

            // ISBN
            book.ISBN = AskIsbn(isAdd || isEdit);

            // Name
            Hepler.WriteColoredLine(LocalizedStrings.AskName, ConsoleColor.Yellow);
            book.Name = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(book.Name))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskName, ConsoleColor.Yellow);
                book.Name = Console.ReadLine().Trim();
            }

            // Authors
            book.Authors.Add(AskAuthors(isAdd));
            Hepler.WriteColoredLine(LocalizedStrings.AskForAddAuthor, ConsoleColor.Yellow);
            var answer = Console.ReadLine().Trim().ToUpper();
            while (answer == LocalizedStrings.AnswerYes.ToUpper())
            {
                book.Authors.Add(AskAuthors(isAdd));
                Hepler.WriteColoredLine(LocalizedStrings.AskForAddAuthor, ConsoleColor.Yellow);
                answer = Console.ReadLine().Trim();
            }

            // Year of publication
            Hepler.WriteColoredLine(LocalizedStrings.AskYearOfPublication, ConsoleColor.Yellow);
            var yearOfPublication = Console.ReadLine().Trim();
            int yearOfPublicationInt = int.MinValue;

            while ((isAdd && (string.IsNullOrEmpty(yearOfPublication) || !int.TryParse(yearOfPublication, out yearOfPublicationInt))
                || (!isAdd && (!string.IsNullOrEmpty(yearOfPublication) && !int.TryParse(yearOfPublication, out yearOfPublicationInt)))))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskYearOfPublication, ConsoleColor.Yellow);
                yearOfPublication = Console.ReadLine().Trim();
            }
            book.YearOfPublication = !string.IsNullOrEmpty(yearOfPublication) ? (int?)yearOfPublicationInt : null;

            // Programming Language
            Hepler.WriteColoredLine(LocalizedStrings.AskProgrammingLanguage, ConsoleColor.Yellow);
            book.ProgrammingLanguage = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(book.ProgrammingLanguage))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskProgrammingLanguage, ConsoleColor.Yellow);
                book.ProgrammingLanguage = Console.ReadLine().Trim();
            }

            // Reader level
            Hepler.WriteColoredLine(LocalizedStrings.AskReaderLevel, ConsoleColor.Yellow);
            var readerLevel = Console.ReadLine().Trim();
            int readerLevelInt = int.MinValue;

            while ((isAdd && (string.IsNullOrEmpty(readerLevel)
                    || !int.TryParse(readerLevel, out readerLevelInt)
                    || !Enum.IsDefined(typeof(ReaderLevel), readerLevelInt))
                || (!isAdd && (!string.IsNullOrEmpty(readerLevel)
                    && (!int.TryParse(readerLevel, out readerLevelInt)
                    || !Enum.IsDefined(typeof(ReaderLevel), readerLevel)))))
                  )
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskReaderLevel, ConsoleColor.Yellow);
                readerLevel = Console.ReadLine().Trim();
            }
            book.ReaderLevel = !string.IsNullOrEmpty(readerLevel) ? (ReaderLevel?)readerLevelInt : null;

            // Language
            Hepler.WriteColoredLine(LocalizedStrings.AskLanguage, ConsoleColor.Yellow);
            book.Language = Console.ReadLine().Trim();
            while (isAdd && string.IsNullOrEmpty(book.Language))
            {
                Hepler.WriteColoredLine(LocalizedStrings.AskLanguage, ConsoleColor.Yellow);
                book.Language = Console.ReadLine().Trim();
            }
            book.Language = !string.IsNullOrEmpty(book.Language) ? book.Language : null;

            //// Book Rating
            //Console.WriteLine(LocalizedStrings.AskBookRating);
            //var bookRating = Console.ReadLine().Trim();
            //var bookRatingInt = int.MinValue;

            //while ((isAdd && (string.IsNullOrEmpty(bookRating) || !int.TryParse(bookRating, out bookRatingInt) || !Enum.IsDefined(typeof(BookRating), bookRatingInt))
            //    || (!isAdd && (!string.IsNullOrEmpty(bookRating) && (!int.TryParse(bookRating, out bookRatingInt) || !Enum.IsDefined(typeof(BookRating), bookRatingInt))))))
            //{
            //    Console.WriteLine(LocalizedStrings.AskBookRating);
            //    bookRating = Console.ReadLine().Trim();
            //}
            //book. = !string.IsNullOrEmpty(bookRating) ? (BookRating?)bookRatingInt : null;
        }
    }
}
