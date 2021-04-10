using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Core.Enums;
using Core.Events.WuxiaWorld;
using Core.Interfaces.Services;
using Core.Utils;
using HtmlAgilityPack;
using Model.WuxiaWorldModel;
using PdfSharpCore.Pdf.IO;
using SelectPdf;

namespace WuxiaWorldService
{
    public class WuxiaWorldService : IWuxiaWorldService
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly string _urlLogin;
        private readonly string _xPathRequestVerificationToken;
        private readonly string _urlBase;
        private readonly string _urlNovelBase;
        private readonly string _xPathBook;
        private readonly string _xPathBookData;
        private readonly string _xPathChapters;
        private readonly string _xPathChapterContent;
        private readonly string _xPathBookImage;
        private readonly string _xPathBookTitle;

        #endregion

        #region Events

        public event EventHandler<NovelChangedEventArgs> NovelEventDownload;
        public event EventHandler<VolumeChangedEventArgs> VolumeEventDownload;
        public event EventHandler<ChapterChangedEventArgs> ChapterEventDownload;

        #endregion

        #region Properties

        public string Password { get; set; }
        public string UserEmail { get; set; }
        public Dictionary<string, string> NovelCodeToName { get; set; }
        public bool SaveBook { get; set; }
        public bool SaveVolumes { get; set; }
        public bool SaveChapters { get; set; }
        public string SaveLocal { get; set; }

        #endregion

        #region Contructor

        public WuxiaWorldService()
        {
            _httpClient = new HttpClient { DefaultRequestVersion = HttpVersion.Version20 };

            _urlLogin = "https://www.wuxiaworld.com/account/login";
            _xPathRequestVerificationToken = ".//input[@name='__RequestVerificationToken']";

            _urlBase = "https://www.wuxiaworld.com/";
            _urlNovelBase = "https://www.wuxiaworld.com/novel/";
            _xPathBookData = ".//h4[@class='panel-title']/child::span";
            _xPathBook = ".//div[@id='chapters']/child::*/child::div[@class='panel panel-default']";
            _xPathChapters = ".//li[@class='chapter-item']/a";
            _xPathChapterContent = ".//div[@id='chapter-outer']";
            _xPathBookImage = ".//div[@class='novel-left']/child::a/child::img";
            _xPathBookTitle = ".//div[@class='novel-body']/child::h2";
        }

        #endregion

        #region Scraping Public Methods

        public bool Login()
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var responseGet = _httpClient.GetStringAsync(requestUri: _urlLogin).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html: responseGet);

            var requestVerificationToken = htmlDocument.DocumentNode.SelectSingleNode(_xPathRequestVerificationToken).GetAttributeValue(name: "value", def: null);

            var form = new Dictionary<string, string>()
            {
                { "Email", UserEmail},
                { "Password", Password},
                { "RememberMe", "false"},
                { "__RequestVerificationToken", requestVerificationToken}
            };
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Host = "www.wuxiaworld.com";
            _httpClient.DefaultRequestHeaders.Referrer = new Uri("https://www.wuxiaworld.com/account/login");


            var response = _httpClient.PostAsync(requestUri: _urlLogin, content: new FormUrlEncodedContent(form)).Result;

            return response.StatusCode == HttpStatusCode.OK;
        }

        public void CreateAndSaveBookAllNovels()
        {
            if (this.Login())
            {
                foreach (var novelCode in NovelCodeToName.Keys)
                {
                    try
                    {
                        Console.WriteLine($"Start novel {NovelCodeToName[novelCode]}");
                        Console.WriteLine(CreateAndSaveBook(novelCode)
                            ? $"Book {NovelCodeToName[novelCode]} created with success"
                            : $"Fail on create book {NovelCodeToName[novelCode]}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Novel {novelCode} failed");
                        Console.WriteLine(e);
                    }
                }

            }
        }

        public bool CreateAndSaveBook(string novelCode)
        {
            var novel = this.CreateChapters(novelCode);
            CreateVolumes(novel);
            CreateBook(novel);

            if (novel.NovelBytes != null && novel.NovelBytes.Length > 0)
            {
                return true;
            }
            return false;
        }

        public void CreateBook(Novel book)
        {
            OnNovelDownload(book, DataOperationType.DownloadStart);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var pdfDoc = new PdfSharpCore.Pdf.PdfDocument();

            var coverBytes = CreateCoverBook(book.NovelCode);

            var pdf = PdfReader.Open(new MemoryStream(coverBytes), PdfDocumentOpenMode.Import);
            foreach (var page in pdf.Pages)
            {
                pdfDoc.AddPage(page);
            }

            foreach (var volumeBytes in book.Volumes)
            {
                pdf = PdfReader.Open(new MemoryStream(volumeBytes.VolumeBytes), PdfDocumentOpenMode.Import);
                foreach (var page in pdf.Pages)
                {
                    pdfDoc.AddPage(page);
                }
            }

            var stream = new MemoryStream();
            pdfDoc.Save(stream, false);
            book.NovelBytes = stream.ToArray();

            if (SaveBook)
            {
                var directory = SaveLocal;
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file = Path.Combine(directory, $"{NovelCodeToName[book.NovelCode].Replace(":", "-")}.pdf");
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                pdfDoc.Save(file);
            }

            OnNovelDownload(book, DataOperationType.DownloadCompleted);
        }

        public void CreateVolumes(Novel book)
        {
            foreach (var volume in book.Volumes)
            {
                OnVolumeDownload(volume, DataOperationType.DownloadStart);

                var chapters = volume.Chapters;

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var pdfDoc = new PdfSharpCore.Pdf.PdfDocument();

                foreach (var chapter in chapters)
                {
                    chapter.ChapterBytes = DownloadChapter(chapter);

                    var pdf = PdfReader.Open(new MemoryStream(chapter.ChapterBytes), PdfDocumentOpenMode.Import);
                    foreach (var page in pdf.Pages)
                    {
                        pdfDoc.AddPage(page);
                    }
                }

                using var stream = new MemoryStream();
                pdfDoc.Save(stream, false);
                if (SaveVolumes)
                {
                    var directory = Path.Combine(SaveLocal,
                        book.Title ?? string.Empty);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var file = Path.Combine(directory, $"{volume.Title.Replace(":", "-")}.pdf");
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }

                    pdfDoc.Save(file);
                }

                OnVolumeDownload(volume, DataOperationType.DownloadCompleted);

                volume.VolumeBytes = stream.ToArray();
            }
        }

        public Novel CreateChapters(string novelCode)
        {
            var addedChapter = new SortedDictionary<string, bool>();
            _httpClient.DefaultRequestHeaders.Clear();
            var urlNovel = Util.ConcatUrlEndpoint(urlBase: _urlNovelBase, endpoint: novelCode);
            var responseGet = _httpClient.GetStringAsync(urlNovel).Result;

            var htmlDocument = new HtmlDocument {OptionDefaultStreamEncoding = Encoding.UTF8};
            htmlDocument.LoadHtml(html: responseGet);

            var novel = new Novel
            {
                Title = NovelCodeToName[novelCode],
                NovelCode = novelCode
            };

            var volumeElement = htmlDocument.DocumentNode.SelectNodes(_xPathBook).ToList();
            var volumeCount = 1;
            foreach (var volumeNode in volumeElement)
            {
                var bookData = volumeNode.SelectNodes(_xPathBookData).ToList();
                var volumeText = $"{bookData[0].InnerText}: {bookData[1].SelectNodes(".//a").FirstOrDefault()?.InnerText}";
                var chaptersElement = volumeNode.SelectNodes(_xPathChapters)?.ToList();

                var volume = new Volume
                {
                    Novel = novel,
                    Title = volumeText.Replace("\n", string.Empty),
                    VolumeNumber = volumeCount.ToString("0000")
                };

                var chapters = new List<Chapter>();
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
                                var chapter = new Chapter
                                {
                                    Volume = volume,
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
                                var chapter = new Chapter
                                {
                                    Volume = volume,
                                    Title = q.InnerText.Replace("\n", string.Empty),
                                    ChapterNumber = newValue,
                                    ChapEndpoint = q.GetAttributeValue("href", string.Empty)
                                };
                                chapters.Add(chapter);
                            }

                        }
                        else
                        {
                            var chapter = new Chapter
                            {
                                Volume = volume,
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

                if (chapters.Count > 0)
                {
                    volume.Chapters.AddRange(chapters);
                }
                novel.Volumes.Add(volume);
                volumeCount++;
            }

            //return novelChapters.DistinctBy(q => new { q.NovelCode, q.ChapterNumber }).ToList();
            return novel;
        }

        public byte[] DownloadChapter(Chapter chapter)
        {
            OnChapterDownload(chapter, DataOperationType.DownloadStart);
            _httpClient.DefaultRequestHeaders.Clear();
            var responseGet = _httpClient.GetStringAsync(Util.ConcatUrlEndpoint(urlBase: _urlBase, endpoint: chapter.ChapEndpoint)).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html: responseGet);

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

            using var pdfStream = new MemoryStream();

            var converter = new HtmlToPdf(720);
            converter.Options.PageBreaksEnhancedAlgorithm = true;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.MarginBottom = 20;
            converter.Options.MarginTop = 30;
            converter.Options.MarginLeft = 40;
            converter.Options.MarginRight = 40;
            var docPdf = converter.ConvertHtmlString(html);
            docPdf.Save(pdfStream);

            docPdf.Close();

            if (SaveChapters)
            {
                var directory = Path.Combine(SaveLocal,
                    chapter.Volume.Novel.Title ?? string.Empty, chapter.Volume.Title.Replace(":", "-"));
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var file = Path.Combine(directory, $"{chapter.ChapterNumber}.pdf");
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                File.WriteAllBytes(file, pdfStream.ToArray());
            }

            OnChapterDownload(chapter, DataOperationType.DownloadCompleted);

            return pdfStream.ToArray();
        }

        #endregion

        #region Scraping Private Methods

        private byte[] CreateCoverBook(string novelCode)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            var urlNovel = Util.ConcatUrlEndpoint(urlBase: _urlNovelBase, endpoint: novelCode);
            var responseGet = _httpClient.GetStringAsync(urlNovel).Result;

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html: responseGet);
            var imageElement = htmlDocument.DocumentNode.SelectSingleNode(_xPathBookImage);
            var imageUrl = imageElement.GetAttributeValue("src", string.Empty);
            var title = htmlDocument.DocumentNode.SelectSingleNode(_xPathBookTitle).InnerText;
            htmlDocument = new HtmlDocument();
            htmlDocument.Load("CoverTemplate.html");
            var html = htmlDocument.DocumentNode.OuterHtml;
            html = html.Replace("{TITLE}", title);
            html = html.Replace("{IMAGE}", imageUrl);

            using var pdfStream = new MemoryStream();

            var converter = new HtmlToPdf(720);
            converter.Options.PageBreaksEnhancedAlgorithm = true;
            converter.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            converter.Options.MarginBottom = 10;
            converter.Options.MarginTop = 10;
            converter.Options.MarginLeft = 40;
            converter.Options.MarginRight = 40;
            converter.Options.KeepImagesTogether = true;
            var docPdf = converter.ConvertHtmlString(html);
            docPdf.Save(pdfStream);

            docPdf.Close();

            return pdfStream.ToArray();
        }

        #endregion

        #region Events Methods

        protected virtual void OnChapterDownload(Chapter entity, DataOperationType operationType)
        {
            ChapterEventDownload?.Invoke(this, new ChapterChangedEventArgs(entity, operationType));
        }

        protected virtual void OnVolumeDownload(Volume entity, DataOperationType operationType)
        {
            VolumeEventDownload?.Invoke(this, new VolumeChangedEventArgs(entity, operationType));
        }

        protected virtual void OnNovelDownload(Novel entity, DataOperationType operationType)
        {
            NovelEventDownload?.Invoke(this, new NovelChangedEventArgs(entity, operationType));
        }
        
        #endregion

    }
}
