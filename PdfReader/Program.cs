using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PdfReader
{
    public static class Extensions
    {
        public static IEnumerable<T> TakeWhile<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool inclusive)
        {
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    if (inclusive) yield return item;

                    yield break;
                }
            }
        }
    }
    internal class Program
    {

        public static IList<IEnumerable<string>> GetPurposes(IEnumerable<Page> pages)
        {
            List<IEnumerable<string>> purposes = new List<IEnumerable<string>> { };

            int lastIndex = 0;
            var words = pages.SelectMany(p => p.GetWords()).ToList();
            var totalWordsCount = words.Count();

            while (lastIndex <= words.Count())
            {
                var purpose = words.Skip(lastIndex)
                .Select((w, i) => new
                {
                    Word = w,
                    Index = i + lastIndex
                })
                .SkipWhile(w => !Regex.IsMatch(w.Word.Text, "[Цц]ел(ь|ью|ями)[\\s\\w]*"))
                .TakeWhile(w => !w.Word.Text.Contains('.'), true)
                .Select(w => new
                {
                    Index = w.Index,
                    Word = w.Word.Text
                })
                .ToList();

                if (!purpose.Any()) break;
                lastIndex = purpose.Last().Index + 1;
                Console.WriteLine($"Обработано {(Convert.ToDouble(lastIndex) / totalWordsCount * 100):0.00}% (позиция {lastIndex} из {totalWordsCount})");
                purposes.Add(purpose.Select(w => w.Word));
            }
            return purposes;
        }

        public static IList<IEnumerable<string>> GetTasks(IEnumerable<Page> pages)
        {

            int lastIndex = 0;
            var words = pages.SelectMany(p => p.GetWords()).ToList();
            var totalWordsCount = words.Count();

            IList<IEnumerable<string>> workTasks = new List<IEnumerable<string>> { };

            var sentences = new List<string> { };
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < words.Count(); i++)
            {
                var word = words[i];
                if (Regex.IsMatch(word.Text, "\\d{1}\\.")) sb.Append(word.Text + " ");
                else if (!word.Text.EndsWith('.') && !word.Text.EndsWith(':') && !word.Text.EndsWith(';')) sb.Append(word.Text + " ");
                else
                {
                    sb.Append(words[i].Text);
                    sentences.Add(sb.ToString().Trim());
                    sb.Clear();
                }
            }

            while (lastIndex <= words.Count())
            {
                var tasks = sentences.Skip(lastIndex)
                .Select((s, i) => new
                {
                    Sentence = s,
                    Index = i + lastIndex,
                })
                .SkipWhile(s => !Regex.IsMatch(s.Sentence, "[Оо]сновн(ым|ыми|ой)[\\s\\w]*явл.*"))
                .Skip(1)
                .TakeWhile(s => Regex.IsMatch(s.Sentence, "\\d{1}\\s*\\..*"), true)
                .ToList();

                if (!tasks.Any()) break;

                lastIndex = tasks.Last().Index + 1;
                Console.WriteLine($"Обработано {(Convert.ToDouble(lastIndex) / totalWordsCount * 100):0.00}% (позиция {lastIndex} из {totalWordsCount})");
                workTasks.Add(tasks.Select(t => t.Sentence));
            }

            return workTasks;
        }

        static void Main(string[] args)
        {
            //var path = "D:\\projects\\study\\IUK4_62B\\ksit\\lab1\\Петроченков_ИА_ИУК4_62_Б_ЛР1_Компьютерные_сети_и_интернет_технологии.pdf";
            var path2 = "D:\\projects\\study\\IUK4_62B\\ksit\\Практикум компьютерные сети и интернет технологии+.pdf";
            using var pdf = PdfDocument.Open(path2);
            var pages = pdf.GetPages();

            var purposes = GetPurposes(pages);
            var tasks = GetTasks(pages);
            int i = 1;
            foreach(var purpose in purposes)
            {
                Console.WriteLine($"[{i++}] {string.Join(" ", purpose)}");
            }
            Console.WriteLine("==========================");

            i = 1;
            foreach (var task in tasks)
            {
                Console.WriteLine($"[{i++}] {string.Join("\n\t", task)}");
            }


            //int i = 1;
            //foreach(var purpose in purposes)
            //{
            //    Console.WriteLine($"{i++} {string.Join(" ", purpose)}");
            //    var match = Regex.Match(string.Join(" ", purpose), "[Цц]ел[ь|ью|ями][\\s\\w]*((раб|Раб)\\w*\\s)(?<conclusion>[\\w\\s]*\\.)");
            //    Console.WriteLine(match.Groups["conclusion"]);
            //}



            //var purpose = pages.SelectMany(p => p.GetWords())
            //.Select((w, i) => new 
            //{
            //    Word = w,
            //    Index = i
            //})
            //.SkipWhile(w => !Regex.IsMatch(w.Word.Text, "[Цц]ел[ь|ью|ями][\\s\\w]*"))
            //.TakeWhile(w => !w.Word.Text.Contains('.'), true)
            //.Select(w => new
            //{
            //    Index = w.Index,
            //    Word = w.Word.Text
            //});
            //Console.WriteLine(string.Join(" ", purpose));

            //Console.WriteLine("\n\n\n============================\n\n\n" +
            //    string.Join(" ", pdf.GetPage(54).GetWords().Select((w, i) => $"{i} {w.Text}")) +
            //    "\n\n\n================================\n\n\n");

            //var questions = string.Join(" ", pages.SelectMany(p => p.GetWords())
            //.SkipWhile(w => !Regex.IsMatch(w.Text, "КОНТРОЛ[\\s\\w]*"))
            //.SkipLast(1)
            //.Select(w => w.Text));
            //Console.WriteLine(questions);
            //Console.WriteLine("=====");

            //var match = Regex.Match(string.Join(" ", purpose), "[Цц]ел[ь|ью|ями][\\s\\w]*((раб|Раб)\\w*\\s)(?<conclusion>[\\w\\s]*\\.)");
            //Console.WriteLine(match.Groups["conclusion"]);
        }
    }
}
