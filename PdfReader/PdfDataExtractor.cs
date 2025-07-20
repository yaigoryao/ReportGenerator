using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace PdfReader
{
    public class PdfDataExtractor
    {
        public class PdfData
        {
            public string Purpose { get; set; } = string.Empty;
            public IEnumerable<string> Tasks { get; set; } = Enumerable.Empty<string>();
            public IEnumerable<string> Questions {  get; set; } = Enumerable.Empty<string>();
        }
        public PdfDataExtractor() { }
        public PdfData ExtractData(string pdfPath)
        {
            try
            {
                using var pdf = PdfDocument.Open(pdfPath);
                return new PdfData() { };
            }
            catch(Exception ex)
            {
                return new PdfData() { };
            }
        }

        //[Цц]ел[ь|ью|ями][\s\w]*((раб|Раб)\w*\s)([\w\s]*)\.
        private string GetPurpose(IEnumerable<Page> pages)
        {
            return string.Join(" ", pages.SelectMany(p => p.GetWords())
            .SkipWhile(w => !Regex.IsMatch(w.Text, "[Цц]ел[ь|ью|ями]"))
            .TakeWhile(w => !w.Text.Contains("."))
            .Select(w => w.Text));

        }
    }
}
