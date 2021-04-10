using Common.Utils;
using HtmlAgilityPack;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Utils;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using WuxiaWorldToPDF.Utils;
using Path = System.IO.Path;

namespace WuxiaWorldToPDF.Business
{
    public class WuxiaWorldNovelHtmlToPdf
    {
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<string, string> _novelCodeToName;
        private readonly string _password;
        private readonly string _prefix;
        private readonly bool _saveBook;
        private readonly bool _saveChapters;
        private readonly string _saveLocal;
        private readonly bool _saveVolumes;
        private readonly string _sufix;
        private readonly string _urlBase;
        private readonly string _urlLogin;
        private readonly string _urlNovelBase;
        private readonly string _userEmail;
        private readonly string _xPathBook;
        private readonly string _xPathBookData;
        private readonly string _xPathBookImage;
        private readonly string _xPathBookTitle;
        private readonly string _xPathChapterContent;
        private readonly string _xPathChapters;
        private readonly string _xPathRequestVerificationToken;

        public WuxiaWorldNovelHtmlToPdf(Dictionary<string, string> novelCodeToCodeToName)
        {
            var configuration = Configuration.GetInstance();

            _saveBook = bool.Parse(configuration.GetNode("saveBook"));
            _saveVolumes = bool.Parse(configuration.GetNode("saveVolumes"));
            _saveChapters = bool.Parse(configuration.GetNode("saveChapters"));
            _saveLocal = configuration.GetNode("saveLocal");
            _prefix = configuration.GetNode("prefix");
            _sufix = configuration.GetNode("sufix");

            Console.Write("Wuxiaworld login: ");
            _userEmail = Console.ReadLine();
            Console.Write("Wuxiaworld password: ");
            _password = Console.ReadLine();

            _httpClient = new HttpClient { DefaultRequestVersion = HttpVersion.Version20 };
            _novelCodeToName = new ConcurrentDictionary<string, string>(novelCodeToCodeToName);

            _urlLogin = "https://www.wuxiaworld.com/account/login";
            _xPathRequestVerificationToken = ".//input[@name='__RequestVerificationToken']";

            _urlBase = "https://www.wuxiaworld.com/";
            _urlNovelBase = "https://www.wuxiaworld.com/novel/";
            _xPathBookData = ".//h4[@class='panel-title']/child::span";
            _xPathBook = ".//div[@id='chapters']/child::*/child::div[@class='panel panel-default']";
            _xPathChapters = ".//li[@class='chapter-item']/a";
            _xPathChapterContent = ".//div[@id='chapter-content']";
            _xPathBookImage = ".//div[@class='novel-left']/child::a/child::img";
            _xPathBookTitle = ".//div[@class='novel-body']/child::h2";
        }

        public bool Login()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var responseGet = _httpClient.GetStringAsync(_urlLogin).Result;

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(responseGet);

            var requestVerificationToken = htmlDocument.DocumentNode.SelectSingleNode(_xPathRequestVerificationToken)
                .GetAttributeValue("value", null);

            var form = new Dictionary<string, string>
            {
                {"Email", _userEmail},
                {"Password", _password},
                {"RememberMe", "false"},
                {"__RequestVerificationToken", requestVerificationToken}
            };
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type",
                "application/x-www-form-urlencoded");
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Host = "www.wuxiaworld.com";
            _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.wuxiaworld.com/account/login");


            var response = _httpClient.PostAsync(_urlLogin, new FormUrlEncodedContent(form)).Result;

            return response.StatusCode == HttpStatusCode.OK;
        }

        public bool CreateAndSaveBook(string novelCode)
        {
            var novel = CreateNovel(novelCode);
            CreateVolumes(novel);
            CreateBook(novel);

            if (novel.NovelBytes != null && novel.NovelBytes.Length > 0) return true;
            return false;
        }

        public void CreateAndSaveBookAllNovels()
        {
            if (Login())
                foreach (var novelCode in _novelCodeToName.Keys)
                    try
                    {
                        Console.WriteLine($"{new string('-', 40)}");
                        Console.WriteLine($"Start novel {_novelCodeToName[novelCode]}");
                        Console.WriteLine(CreateAndSaveBook(novelCode)
                            ? $"Book {_novelCodeToName[novelCode]} created with success"
                            : $"Fail on create book {_novelCodeToName[novelCode]}");
                        Console.WriteLine($"{new string('-', 40)}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Novel {novelCode} failed");
                        Console.WriteLine(e);
                    }
        }

        public Novel CreateNovel(string novelCode)
        {
            SortedDictionary<string, bool> addedChapter = new();
            _httpClient.DefaultRequestHeaders.Clear();
            var urlNovel = Util.ConcatUrlEndpoint(_urlNovelBase, novelCode);
            var responseGet = _httpClient.GetStringAsync(urlNovel).Result;

            HtmlDocument htmlDocument = new() { OptionDefaultStreamEncoding = Encoding.UTF8 };
            htmlDocument.LoadHtml(responseGet);

            Novel novel = new()
            {
                Title = _novelCodeToName[novelCode],
                NovelCode = novelCode
            };

            var volumeElement = htmlDocument.DocumentNode.SelectNodes(_xPathBook).ToList();
            var volumeCount = 1;
            foreach (var volumeNode in volumeElement)
            {
                var bookData = volumeNode.SelectNodes(_xPathBookData).ToList();
                var volumeText =
                    $"{bookData[0].InnerText}: {bookData[1].SelectNodes(".//a").FirstOrDefault()?.InnerText}";
                var chaptersElement = volumeNode.SelectNodes(_xPathChapters)?.ToList();

                Volume volume = new(novel)
                {
                    Title = volumeText.Replace("\n", string.Empty),
                    VolumeNumber = volumeCount.ToString("0000")
                };

                List<Chapter> chapters = new();
                chaptersElement?.ForEach(q =>
                {
                    try
                    {
                        var regex = Regex.Match(q.InnerText.ToLower().Replace("\n", string.Empty),
                            @"(\d+)(?=\s*-)|(\d+)(?=\s*:)|(\d+)(?=\s*–)|(\d+)(?=\s*\()|(\d+)(?=\s*\s)|chapter (\d+)");
                        var value = regex.Value.Replace("chapter ", string.Empty);

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (!addedChapter.ContainsKey(value))
                            {
                                addedChapter.Add(value, true);
                                Chapter chapter = new(volume)
                                {
                                    Title = q.InnerText.Replace("\n", string.Empty),
                                    ChapterNumber = value,
                                    ChapEndpoint = q.GetAttributeValue("href", string.Empty)
                                };
                                chapters.Add(chapter);
                            }
                            else if (addedChapter[value])
                            {
                                var count = 2;
                                var newValue = value;
                                while (addedChapter.ContainsKey(newValue))
                                {
                                    newValue = $"{value}-{count}";
                                    count++;
                                }

                                addedChapter.Add(newValue, true);
                                Chapter chapter = new(volume)
                                {
                                    Title = q.InnerText.Replace("\n", string.Empty),
                                    ChapterNumber = newValue,
                                    ChapEndpoint = q.GetAttributeValue("href", string.Empty)
                                };
                                chapters.Add(chapter);
                            }
                        }
                        else
                        {
                            Chapter chapter = new(volume)
                            {
                                Title = q.InnerText.Replace("\n", string.Empty),
                                ChapterNumber = 0.ToString(),
                                ChapEndpoint = q.GetAttributeValue("href", string.Empty)
                            };
                            chapters.Add(chapter);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });

                addedChapter.Select(q => q.Key).ToList().ForEach(q => addedChapter[q] = false);

                if (chapters.Count > 0) volume.Chapters.AddRange(chapters);
                novel.Volumes.Add(volume);
                volumeCount++;
            }

            //return novelChapters.DistinctBy(q => new { q.NovelCode, q.ChapterNumber }).ToList();
            return novel;
        }

        public byte[] DownloadChapter(Chapter chapter)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var responseGet = _httpClient.GetStringAsync(Util.ConcatUrlEndpoint(_urlBase, chapter.ChapEndpoint)).Result;

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(responseGet);

            var volumeElement = htmlDocument.DocumentNode.SelectSingleNode(_xPathChapterContent);

            var html = volumeElement.OuterHtml;
            var element = volumeElement.SelectSingleNode(".//div[@class='font-resize']");
            if (element != null)
            {
                var removeHtml = element.OuterHtml;
                html = volumeElement.InnerHtml.Replace(removeHtml, string.Empty);
            }

            element = volumeElement.SelectSingleNode(".//div[@class='visible-xs visible-sm visible-md font-resize']");
            if (element != null)
            {
                var removeHtml = element.OuterHtml;
                html = volumeElement.InnerHtml.Replace(removeHtml, string.Empty);
            }

            var elements = volumeElement.SelectNodes(".//a[@class='chapter-nav']");
            if (elements != null)
            {
                var removeHtml = elements[0].OuterHtml;
                html = html.Replace(removeHtml, string.Empty);
            }

            if (volumeElement.SelectNodes(".//a[@class='chapter-nav']").Count > 1)
            {
                var removeHtml = volumeElement.SelectNodes(".//a[@class='chapter-nav']")[1].OuterHtml;
                html = html.Replace(removeHtml, string.Empty);
            }

            using MemoryStream pdfStream = new();
            PdfWriter pdfWriter = new(pdfStream);
            PdfDocument pdfDocument = new(pdfWriter);
            pdfDocument.GetCatalog().SetPageMode(PdfName.UseOutlines);

            Document document = new(pdfDocument, PageSize.A4);
            document.SetMargins(30, 40, 20, 40);

            document.Add(new Paragraph(text: chapter.Title).SetBold()
                .SetFontSize(PdfToHtmlUtils.ChapterTitleFontSize));

            PdfToHtmlUtils.AddParagraphs(html, document);

            var outline = pdfDocument.GetOutlines(false);
            var firstPage = pdfDocument.GetFirstPage();
            //var topBookmark = document.GetRenderer().GetCurrentArea().GetBBox().GetTop();
            var chapterOutline = outline.AddOutline(title: chapter.Title);
            chapterOutline.AddAction(PdfAction.CreateGoTo(
                PdfExplicitDestination.CreateFit(firstPage)));

            document.Close();
            pdfDocument.Close();

            if (!_saveChapters) return pdfStream.ToArray();

            var directory = Path.Combine(_saveLocal,
                chapter.Volume.Novel.Title ?? string.Empty, chapter.Volume.Title.Replace(":", "-"));
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var file = Path.Combine(directory, $"{chapter.ChapterNumber}.pdf");
            if (File.Exists(file)) File.Delete(file);
            File.WriteAllBytes(file, pdfStream.ToArray());

            return pdfStream.ToArray();
        }

        public void CreateVolumes(Novel book)
        {
            var countVolume = 1;
            var count = 1;
            foreach (var volume in book.Volumes)
            {
                var chapters = volume.Chapters;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using MemoryStream stream = new();
                PdfWriter pdfWriter = new(stream);
                PdfDocument pdfDocument = new(pdfWriter);
                pdfDocument.InitializeOutlines();

                PdfReader pdfReader = null;
                PdfDocument chapterPdfDocument = null;
                PdfMerger merger = null;
                foreach (var chapter in chapters)
                {
                    chapter.ChapterBytes = DownloadChapter(chapter);

                    var text =
                        $"{book.NovelCode}: volume {countVolume} of {book.Volumes.Count} - chapter {count} of " +
                        $"{book.Volumes.Sum(selector: q => q.Chapters.Count)} downloaded.";
                    Util.WriteLineOnTop(text);

                    if (chapter.ChapterBytes != null && chapter.ChapterBytes.Length > 0)
                    {
                        pdfReader = new PdfReader(new MemoryStream(buffer: chapter.ChapterBytes));
                        chapterPdfDocument = new PdfDocument(pdfReader);

                        merger = new PdfMerger(pdfDocument, true, true)
                            .SetCloseSourceDocuments(false);
                        merger = merger.Merge(chapterPdfDocument, 1, chapterPdfDocument.GetNumberOfPages());
                    }

                    count++;
                }

                chapterPdfDocument?.Close();
                pdfReader?.Close();

                countVolume++;
                Console.WriteLine();
                Console.WriteLine();

                if (pdfDocument.GetNumberOfPages() > 0)
                {
                    pdfDocument.Close();
                    pdfWriter.Close();
                }

                merger?.Close();

                if (_saveVolumes)
                {
                    var directory = Path.Combine(_saveLocal,
                        book.Title ?? string.Empty);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    var pre = string.IsNullOrEmpty(_prefix) ? string.Empty : $"{_prefix}_";
                    var suf = string.IsNullOrEmpty(_sufix) ? string.Empty : $"_{_sufix}";
                    var file = Path.Combine(directory,
                        $"{pre}" +
                        $"{volume.Novel.Title}-" +
                        $"{volume.Title.Replace(":", "-")}" +
                        $"{suf}.pdf");
                    if (File.Exists(file)) File.Delete(file);

                    File.WriteAllBytes(file, stream.ToArray());
                }

                volume.VolumeBytes = stream.ToArray();
            }
        }

        public void CreateBook(Novel book)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            MemoryStream stream = new();
            PdfWriter pdfWriter = new(stream);
            PdfDocument bookPdfDocument = new(pdfWriter);

            var coverBytes = CreateCoverBook(book.NovelCode);

            PdfReader pdfReader = null;
            PdfDocument pdfDocument = null;
            PdfMerger pdfMerger = null;
            if (coverBytes != null && coverBytes.Length > 0)
            {
                pdfReader = new PdfReader(new MemoryStream(coverBytes));
                pdfDocument = new PdfDocument(pdfReader);

                pdfMerger = new PdfMerger(bookPdfDocument).SetCloseSourceDocuments(false);
                pdfMerger.Merge(pdfDocument, 1, pdfDocument.GetNumberOfPages());
            }

            foreach (var volume in book.Volumes.Where(volume => volume.VolumeBytes != null &&
                                                                volume.VolumeBytes.Length > 0 &&
                                                                volume.Chapters.Count > 0)
                .OrderBy(q => q.VolumeNumber).ToList())
            {
                pdfReader = new PdfReader(new MemoryStream(buffer: volume.VolumeBytes));
                pdfDocument = new PdfDocument(pdfReader);

                pdfMerger = new PdfMerger(bookPdfDocument).SetCloseSourceDocuments(false);
                pdfMerger = pdfMerger.Merge(pdfDocument, 1, pdfDocument.GetNumberOfPages());
            }

            if (bookPdfDocument.GetNumberOfPages() <= 0) return;

            pdfMerger?.Close();
            pdfDocument?.Close();
            pdfReader?.Close();
            bookPdfDocument.Close();

            if (!_saveBook) return;

            var directory = _saveLocal;
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var pre = string.IsNullOrEmpty(_prefix) ? string.Empty : $"{_prefix}_";
            var suf = string.IsNullOrEmpty(_sufix) ? string.Empty : $"_{_sufix}";
            var file = Path.Combine(directory, $"{pre}{_novelCodeToName[book.NovelCode].Replace(":", "-")}{suf}.pdf");
            if (File.Exists(file)) File.Delete(file);

            book.NovelBytes = stream.ToArray();
            File.WriteAllBytes(file, book.NovelBytes);
        }

        private byte[] CreateCoverBook(string novelCode)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var urlNovel = Util.ConcatUrlEndpoint(_urlNovelBase, novelCode);
            var responseGet = _httpClient.GetStringAsync(urlNovel).Result;

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(responseGet);
            var imageElement = htmlDocument.DocumentNode.SelectSingleNode(_xPathBookImage);
            var imageUrl = imageElement.GetAttributeValue("src", string.Empty);
            var title = htmlDocument.DocumentNode.SelectSingleNode(_xPathBookTitle).InnerText;

            var requestStream = _httpClient.GetStreamAsync(imageUrl).Result;

            MemoryStream imageStream = new();
            requestStream.CopyTo(imageStream);


            using MemoryStream pdfStream = new();

            //htmlDocument = new HtmlDocument();
            //htmlDocument.Load("CoverTemplate.html");
            //var html = htmlDocument.DocumentNode.OuterHtml;
            //html = html.Replace("{TITLE}", title);
            //html = html.Replace("{IMAGE}", imageUrl);

            //var converter = new HtmlToPdf(720);
            //converter.Options.PageBreaksEnhancedAlgorithm = true;
            //converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            //converter.Options.MarginBottom = 10;
            //converter.Options.MarginTop = 10;
            //converter.Options.MarginLeft = 40;
            //converter.Options.MarginRight = 40;
            //converter.Options.KeepImagesTogether = true;
            //var docPdf = converter.ConvertHtmlString(html);
            //docPdf.Save(pdfStream);

            //docPdf.Close();

            PdfWriter pdfWriter = new(pdfStream);
            PdfDocument pdfDocument = new(pdfWriter);
            Document document = new(pdfDocument, PageSize.A4);
            document.SetMargins(30, 40, 20, 40);

            Image image = new(new PdfImageXObject(new PdfStream(imageStream.ToArray())));

            document.Add(new Paragraph(title).SetBold().SetFontSize(PdfToHtmlUtils.BookTitleFontSize)
                .SetHorizontalAlignment(HorizontalAlignment.CENTER));
            document.Add(image);

            document.Close();
            pdfDocument.Close();
            pdfWriter.Close();

            return pdfStream.ToArray();
        }
    }
}