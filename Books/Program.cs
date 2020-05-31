using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

using BooksTools;

namespace Books
{
    class Program
    {
        // Путь к папке с книгами.
        const string booksDirectoryPath = "../../../../Книги";
        // Путь к папке с локализованными книгами.
        const string russianBooksDirectoryPath = "../../../../Русские Книги";
        // URI книги.
        const string uri = "https://www.gutenberg.org/files/1342/1342-0.txt";

        /// <summary>
        /// Возвращает строковое представление прошедшего времени.
        /// </summary>
        /// <param name="stopwatch">Таймер</param>
        /// <returns></returns>
        static string GetTimePassed(Stopwatch stopwatch)
        {
            TimeSpan ts = stopwatch.Elapsed;
            return String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
        }

        static async Task Main(string[] args)
        {
            // Получение всех путей к файлам с книгами из папки "Книги".
            IEnumerable<string> tempFileNames = Directory.EnumerateFiles(booksDirectoryPath);

            // Создание списка с путями к файлам с книгами из папки "Книги".
            List<string> fileNames = new List<string>();

            foreach (var fileName in tempFileNames)
                fileNames.Add(fileName);

            // Создание папки "Русские книги"
            Directory.CreateDirectory(russianBooksDirectoryPath);

            // Не async.
            Console.WriteLine("Не async:");

            // Обновление и запуск таймера.
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (string fileName in fileNames)
            {
                // Инициализация объекта с инструментами для работы с книгами.
                BooksTools.BooksTools book = new BooksTools.BooksTools();
                // Локализация книги из папки "Книги" и сохранение локализованной версии книги в папку "Русские книги" с приставкой "_new".
                // Вывод информации о книге и времени, за которое была переведена книга.
                book.TranslateBook($"{fileName}", $"{russianBooksDirectoryPath}/new_{fileName.Split('/')[fileName.Split('/').Length - 1]}", stopwatch);
            }

            // Остановка таймера.
            stopwatch.Stop();

            // Вывод информации об общем затраченном времени.
            Console.WriteLine($"Общее время выполнения не async: {GetTimePassed(stopwatch)}");

            Console.WriteLine();

            // async.
            Console.WriteLine("Async:");

            // Обновление и запуск таймера.
            stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.ForEach<string>(fileNames, (fileName) =>
            {
                BooksTools.BooksTools book = new BooksTools.BooksTools();
                book.TranslateBook($"{fileName}", $"{russianBooksDirectoryPath}/new_{fileName.Split('/')[fileName.Split('/').Length - 1]}", stopwatch);
            });

            // Остановка таймера.
            stopwatch.Stop();

            // Вывод информации об общем затраченном времени.
            Console.WriteLine($"Общее время выполнения async: {GetTimePassed(stopwatch)}");

            Console.WriteLine();

            // Загрузка через web ссылку.
            Console.WriteLine("Книга по uri ссылке:");

            // Обновление и запуск таймера.
            stopwatch = new Stopwatch();

            {
                // Инициализация объекта с инструментами для работы с книгами.
                BooksTools.BooksTools book = new BooksTools.BooksTools();
                // Асинхронная локализация книги из URI и сохранение локализованной версии книги в папку "Русские книги" с приставкой "_new".
                // Вывод информации о книге и времени, за которое была переведена книга.
                await book.TranslateBookURI(uri, $"{russianBooksDirectoryPath}/new_book_from_web.txt", stopwatch);
            }

            // Остановка таймера.
            stopwatch.Stop();

            // Вывод информации об общем затраченном времени.
            Console.WriteLine($"Общее время выполнения при использовании uri: {GetTimePassed(stopwatch)}");
        }
    }
}
