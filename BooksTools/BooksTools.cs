using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;

namespace BooksTools
{
    public class BooksTools
    {
        // Поля.
        private string bookText;
        private string bookRussianText;
        private string fileName;

        // Свойства.
        /// <summary>
        /// Текст книги на английском языке.
        /// </summary>
        public string BookText { get => bookText; private set => bookText = value; }
        /// <summary>
        /// Текст книги на русском языке.
        /// </summary>
        public string BookRussianText { get => bookRussianText; private set => bookRussianText = value; }
        /// <summary>
        /// Имя файла с книгой.
        /// </summary>
        public string FileName { get => fileName; private set => fileName = value; }

        /// <summary>
        /// Словарь с правилами перевода текста с английского языка на русский.
        /// </summary>
        private Dictionary<char, string> changeLettersRule = new Dictionary<char, string>
        {
            { 'a', "а" },
            { 'b', "б" },
            { 'v', "в" },
            { 'g', "г" },
            { 'd', "д" },
            { 'e', "е" },
            { 'j', "ж" },
            { 'z', "з" },
            { 'i', "и" },
            { 'k', "к" },
            { 'l', "л" },
            { 'm', "м" },
            { 'n', "н" },
            { 'o', "о" },
            { 'p', "п" },
            { 'r', "р" },
            { 's', "с" },
            { 't', "т" },
            { 'u', "у" },
            { 'f', "ф" },
            { 'h', "х" },
            { 'c', "ц" },
            { 'q', "ку" },
            { 'w', "у" },
            { 'x', "кс" },
            { 'y', "ы" },
        };

        // Методы.
        /// <summary>
        /// Скачивает книгу локально.
        /// </summary>
        /// <param name="path">Путь к файлу с книгой</param>
        private void DownloadBook(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    // Чтение текста книги из файла по пути path.
                    BookText = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File Not Found Exception: файл не найден: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory Not Found Exception: директория не найдена: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Exception: ошибка ввода/вывода: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: возникла непредвиденная ошибка: {ex}");
            }
        }

        /// <summary>
        /// Скачивает книгу по заданному URI.
        /// </summary>
        /// <param name="uri">URI, где хранится книга</param>
        /// <returns></returns>
        private async Task DownloadBookURIAsync(string uri)
        {
            var client = new HttpClient();

            try
            {
                // Чтение текста книги из URI.
                BookText = await client.GetStringAsync(uri);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Http Request Exception: ошибка http заброса: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: возникла непредвиденная ошибка: {ex}");
            }
        }

        /// <summary>
        /// Изменяет язык книги с английского на русский.
        /// </summary>
        private void ChangeLanguageToRussian()
        {
            // Текст книги на русском языке.
            StringBuilder tempBookRussianText = new StringBuilder();

            // Используя Regex получается в 2-3 раза дольше, но хочу выпендреться, что знаю, как этим пользоваться.
            // Regex validCharacters = new Regex(@"[a-zA-Z\d\s_?!,.'*\/]+");

            // Проверки для каждой буквы текста.
            for (int i = 0; i < BookText.Length; i++)
            {
                // Если символ - буква.
                if (Char.IsLetter(BookText[i]))
                {
                    // Если буква - не заглавная или строчная буква английского алфавита.
                    if ((Char.IsUpper(BookText[i]) && (BookText[i] < 'A' || BookText[i] > 'Z')) ||
                        (Char.IsLower(BookText[i]) && (BookText[i] < 'a' || BookText[i] > 'z')))
                    // if (!validCharacters.IsMatch(BookText[i].ToString()))
                        // Буква не переводится и не добавляется в текст книги на русском языке.
                        continue;
                    else
                    {
                        // Буква переводится и добавляется в текст книги на русском языке, сохраняя регистр.
                        if (Char.IsUpper(BookText[i]))
                            tempBookRussianText.Append(changeLettersRule[Char.ToLower(BookText[i])]);
                        else
                            tempBookRussianText.Append(changeLettersRule[BookText[i]]);
                    }
                }
                // Если символ - не буква.
                else
                    tempBookRussianText.Append(BookText[i]);
            }

            BookRussianText = tempBookRussianText.ToString();
        }

        /// <summary>
        /// Сохраняет переведенную книгу локально по заданному пути.
        /// </summary>
        /// <param name="path"></param>
        public void SaveBookRussianText(string path)
        {
            // Переводит текст книги на русский язык.
            ChangeLanguageToRussian();

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    // Сохранение текста книги на русском языке по заданному пути.
                    sw.Write(BookRussianText);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File Not Found Exception: файл не найден: {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory Not Found Exception: директория не найдена: {ex.Message}");
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine($"Path Too Long Exception: слишком длинный путь к файлу: {ex.Message}");
            }
            catch(IOException ex)
            {
                Console.WriteLine($"IO Exception: ошибка ввода/вывода: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: возникла непредвиденная ошибка: {ex}");
            }
        }

        /// <summary>
        /// Переводит книгу, хранящуюся локально, с английского языка на русский и сохраняет переведенную книгу локально по заданному пути.
        /// Выводит в консоль информацию о книге.
        /// </summary>
        /// <param name="pathSource">Путь, где хранится книга, которую требуется перевести</param>
        /// <param name="pathResult">Путь, куда необходимо сохранить переведенную книгу</param>
        /// <param name="stopWatch">Таймер</param>
        public void TranslateBook(string pathSource, string pathResult, Stopwatch stopWatch)
        {
            // Считывает название файла, в котором хранится книга.
            string[] tempPath = pathSource.Split('/');
            FileName = tempPath[tempPath.Length - 1];

            // Считывает текст книги по заданному пути.
            DownloadBook(pathSource);

            // Сохраняет текст книги на русском языке по заданному пути.
            SaveBookRussianText(pathResult);

            // Выводит в консоль информацию о книге.
            Console.WriteLine(BookInfo(stopWatch));
        }

        /// <summary>
        /// Переводит книгу, хранящуюся по заданному URI, с английского языка на русский и сохраняет переведенную книгу локально по заданному пути.
        /// Выводит в консоль информацию о книге.
        /// </summary>
        /// <param name="uri">URI, где хранится книга, которую требуется перевести</param>
        /// <param name="pathResult">Путь, куда необходимо сохранить переведенную книгу</param>
        /// <param name="stopwatch">Таймер</param>
        /// <returns></returns>
        public async Task TranslateBookURI(string uri, string pathResult, Stopwatch stopwatch)
        {
            // Буква не переводится и не добавляется в текст книги на русском языке.
            string[] tempPath = uri.Split('/');
            FileName = tempPath[tempPath.Length - 1];

            // Считывает текст книги по заданному URI.
            await DownloadBookURIAsync(uri);

            // Запуск таймера.
            stopwatch.Start();

            // Сохраняет текст книги на русском языке по заданному пути.
            SaveBookRussianText(pathResult);

            // Выводит в консоль информацию о книге.
            Console.WriteLine(BookInfo(stopwatch));
        }

        /// <summary>
        /// Выводит информацию о книге: название файла с книгой, количество символов на английском и русском языках,
        /// время, за которое была переведена книга.
        /// </summary>
        /// <param name="stopWatch"></param>
        /// <returns></returns>
        public string BookInfo(Stopwatch stopWatch)
        {
            // Определяет, за сколько времени была переведена книга.
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds);

            return $"Файл {FileName}: символов на английском языке: {BookText.Length}, на русском: {BookRussianText.Length}, время выполнения: {elapsedTime}";
        }

        // Конструктор.
        public BooksTools()
        {

        }
    }
}
