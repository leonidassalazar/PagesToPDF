using Common.Utils;
using HtmlAgilityPack;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace NovelFullToPDF.Business
{
    public class ArifuretaNovelToPdf
    {
        private static readonly HttpClient HttpClient = new() { DefaultRequestVersion = HttpVersion.Version20 };
        private const string BaseUrl = @"https://novelfull.com/index.php/arifureta-shokugyou-de-sekai-saikyou-wn.html?page={0}&per-page=50";
        private const string XPathChapLinks = @".//ul[@class='list-chapter']/li/a";
        private const string XPathTablePages = @".//ul[contains(@class, 'pagination')]/li/a[contains(text(), 'Last')]";
        private const string XPathChapterContent = @".//div[@id='chapter-content']";

        public static void DownloadNovel()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using var stream = new MemoryStream();
            var pdfWriter = new PdfWriter(stream);
            var pdfDocument = new PdfDocument(pdfWriter);
            pdfDocument.InitializeOutlines();

            PdfReader pdfReader = null;
            PdfDocument chapterPdfDocument = null;
            PdfMerger merger = null;

            var chapterNum = 1;
            for (var i = 1; i <= GetNumberOfPages(); i++)
            {
                Console.WriteLine($"Page: {i}");
                HttpClient.DefaultRequestHeaders.Clear();
                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ToPDF");

                var response = HttpClient.GetStringAsync(string.Format(BaseUrl, i)).Result;

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                var chapterLinks = htmlDocument.DocumentNode.SelectNodes(XPathChapLinks);


                foreach (var linkRaw in chapterLinks.ToList())
                {
                    Console.WriteLine($"Page: {i} - Chapter: {chapterNum}");

                    pdfReader = new PdfReader(new MemoryStream(buffer: DownloadChapter(linkRaw)));
                    chapterPdfDocument = new PdfDocument(pdfReader);

                    merger = new PdfMerger(pdfDocument, true, true)
                        .SetCloseSourceDocuments(false);
                    merger = merger.Merge(chapterPdfDocument, 1, chapterPdfDocument.GetNumberOfPages());

                    chapterNum++;
                }
            }

            chapterPdfDocument?.Close();
            pdfReader?.Close();

            Console.WriteLine();
            Console.WriteLine();

            if (pdfDocument.GetNumberOfPages() > 0)
            {
                pdfDocument.Close();
                pdfWriter.Close();
            }

            merger?.Close();

            if (File.Exists("Arifureta Shokugyou de Sekai Saikyou.pdf"))
            {
                File.Delete("Arifureta Shokugyou de Sekai Saikyou.pdf");
            }
            File.WriteAllBytes("Arifureta Shokugyou de Sekai Saikyou.pdf", stream.ToArray());
        }

        private static int GetNumberOfPages()
        {
            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ToPDF");

            var response = HttpClient.GetStringAsync(string.Format(BaseUrl, 1)).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);

            var pagesLinks = htmlDocument.DocumentNode.SelectNodes(XPathTablePages);

            var numberOfPages = pagesLinks.Select(q =>
            {
                var match = Regex.Matches(q.Attributes["href"].Value, @"page=(\d+)&");

                if (match.Count <= 0) return 0;

                var page = int.Parse(match[0].Groups[1].Value);
                return page;

            }).Max();

            return numberOfPages;
        }

        private static byte[] DownloadChapter(HtmlNode linkRaw)
        {
            var href = linkRaw.Attributes["href"].Value;
            var title = linkRaw.Attributes["title"].Value;

            var url = @"https://novelfull.com";

            HttpClient.DefaultRequestHeaders.Clear();
            HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ToPDF");

            var responseChapter = HttpClient.GetStringAsync($"{url}{href}").Result;

            HtmlDocument chapterDocument = new();
            chapterDocument.LoadHtml(responseChapter);

            var chapterContent = chapterDocument.DocumentNode.SelectSingleNode(XPathChapterContent).InnerHtml;
            chapterContent = PdfToHtmlUtils.ReplaceHtmlCharactersAndSymbols(chapterContent);

            using var pdfStream = new MemoryStream();
            var writer = new PdfWriter(pdfStream);
            var pdfDoc = new PdfDocument(writer);
            pdfDoc.GetCatalog().SetPageMode(PdfName.UseOutlines);

            var document = new Document(pdfDoc, PageSize.A4);
            document.SetMargins(30, 40, 20, 40);

            PdfToHtmlUtils.AddChapterTitle(title, document);
            PdfToHtmlUtils.AddParagraphs(chapterContent, document);

            var outline = pdfDoc.GetOutlines(false);
            var firstPage = pdfDoc.GetFirstPage();

            var chapterOutline = outline.AddOutline(title: PdfToHtmlUtils.ReplaceHtmlCharactersAndSymbols(title));
            chapterOutline.AddAction(PdfAction.CreateGoTo(
                PdfExplicitDestination.CreateFit(firstPage)));

            document.Close();
            pdfDoc.Close();

            return pdfStream.ToArray();
        }
    }
}
