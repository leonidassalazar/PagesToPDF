using HtmlAgilityPack;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using Document = iText.Layout.Document;

namespace Common.Utils
{
    public class PdfToHtmlUtils
    {
        public static float ParagraphFontSize { get; } = 25f;
        public static float ChapterTitleFontSize { get; } = 33f;
        public static float BookTitleFontSize { get; } = 37f;

        private static readonly List<Tuple<string, string, string>> HtmlChars = new()
        {
            new Tuple<string, string, string>(" ", "&#32;", ""),
            new Tuple<string, string, string>("!", "&#33;", ""),
            new Tuple<string, string, string>("\"", "&#34;", "&quot;"),
            new Tuple<string, string, string>("#", "&#35;", ""),
            new Tuple<string, string, string>("$", "&#36;", ""),
            new Tuple<string, string, string>("%", "&#37;", ""),
            new Tuple<string, string, string>("&", "&#38;", "&amp;"),
            new Tuple<string, string, string>("'", "&#39;", ""),
            new Tuple<string, string, string>("(", "&#40;", ""),
            new Tuple<string, string, string>(")", "&#41;", ""),
            new Tuple<string, string, string>("*", "&#42;", ""),
            new Tuple<string, string, string>("+", "&#43;", ""),
            new Tuple<string, string, string>(",", "&#44;", ""),
            new Tuple<string, string, string>("-", "&#45;", ""),
            new Tuple<string, string, string>(".", "&#46;", ""),
            new Tuple<string, string, string>("/", "&#47;", ""),
            new Tuple<string, string, string>("0", "&#48;", ""),
            new Tuple<string, string, string>("1", "&#49;", ""),
            new Tuple<string, string, string>("2", "&#50;", ""),
            new Tuple<string, string, string>("3", "&#51;", ""),
            new Tuple<string, string, string>("4", "&#52;", ""),
            new Tuple<string, string, string>("5", "&#53;", ""),
            new Tuple<string, string, string>("6", "&#54;", ""),
            new Tuple<string, string, string>("7", "&#55;", ""),
            new Tuple<string, string, string>("8", "&#56;", ""),
            new Tuple<string, string, string>("9", "&#57;", ""),
            new Tuple<string, string, string>(":", "&#58;", ""),
            new Tuple<string, string, string>(";", "&#59;", ""),
            new Tuple<string, string, string>("<", "&#60;", "&lt;"),
            new Tuple<string, string, string>("=", "&#61;", ""),
            new Tuple<string, string, string>(">", "&#62;", "&gt;"),
            new Tuple<string, string, string>("?", "&#63;", ""),
            new Tuple<string, string, string>("@", "&#64;", ""),
            new Tuple<string, string, string>("A", "&#65;", ""),
            new Tuple<string, string, string>("B", "&#66;", ""),
            new Tuple<string, string, string>("C", "&#67;", ""),
            new Tuple<string, string, string>("D", "&#68;", ""),
            new Tuple<string, string, string>("E", "&#69;", ""),
            new Tuple<string, string, string>("F", "&#70;", ""),
            new Tuple<string, string, string>("G", "&#71;", ""),
            new Tuple<string, string, string>("H", "&#72;", ""),
            new Tuple<string, string, string>("I", "&#73;", ""),
            new Tuple<string, string, string>("J", "&#74;", ""),
            new Tuple<string, string, string>("K", "&#75;", ""),
            new Tuple<string, string, string>("L", "&#76;", ""),
            new Tuple<string, string, string>("M", "&#77;", ""),
            new Tuple<string, string, string>("N", "&#78;", ""),
            new Tuple<string, string, string>("O", "&#79;", ""),
            new Tuple<string, string, string>("P", "&#80;", ""),
            new Tuple<string, string, string>("Q", "&#81;", ""),
            new Tuple<string, string, string>("R", "&#82;", ""),
            new Tuple<string, string, string>("S", "&#83;", ""),
            new Tuple<string, string, string>("T", "&#84;", ""),
            new Tuple<string, string, string>("U", "&#85;", ""),
            new Tuple<string, string, string>("V", "&#86;", ""),
            new Tuple<string, string, string>("W", "&#87;", ""),
            new Tuple<string, string, string>("X", "&#88;", ""),
            new Tuple<string, string, string>("Y", "&#89;", ""),
            new Tuple<string, string, string>("Z", "&#90;", ""),
            new Tuple<string, string, string>("[", "&#91;", ""),
            new Tuple<string, string, string>("\\", "&#92;", ""),
            new Tuple<string, string, string>("]", "&#93;", ""),
            new Tuple<string, string, string>("^", "&#94;", ""),
            new Tuple<string, string, string>("_", "&#95;", ""),
            new Tuple<string, string, string>("`", "&#96;", ""),
            new Tuple<string, string, string>("a", "&#97;", ""),
            new Tuple<string, string, string>("b", "&#98;", ""),
            new Tuple<string, string, string>("c", "&#99;", ""),
            new Tuple<string, string, string>("d", "&#100;", ""),
            new Tuple<string, string, string>("e", "&#101;", ""),
            new Tuple<string, string, string>("f", "&#102;", ""),
            new Tuple<string, string, string>("g", "&#103;", ""),
            new Tuple<string, string, string>("h", "&#104;", ""),
            new Tuple<string, string, string>("i", "&#105;", ""),
            new Tuple<string, string, string>("j", "&#106;", ""),
            new Tuple<string, string, string>("k", "&#107;", ""),
            new Tuple<string, string, string>("l", "&#108;", ""),
            new Tuple<string, string, string>("m", "&#109;", ""),
            new Tuple<string, string, string>("n", "&#110;", ""),
            new Tuple<string, string, string>("o", "&#111;", ""),
            new Tuple<string, string, string>("p", "&#112;", ""),
            new Tuple<string, string, string>("q", "&#113;", ""),
            new Tuple<string, string, string>("r", "&#114;", ""),
            new Tuple<string, string, string>("s", "&#115;", ""),
            new Tuple<string, string, string>("t", "&#116;", ""),
            new Tuple<string, string, string>("u", "&#117;", ""),
            new Tuple<string, string, string>("v", "&#118;", ""),
            new Tuple<string, string, string>("w", "&#119;", ""),
            new Tuple<string, string, string>("x", "&#120;", ""),
            new Tuple<string, string, string>("y", "&#121;", ""),
            new Tuple<string, string, string>("z", "&#122;", ""),
            new Tuple<string, string, string>("{", "&#123;", ""),
            new Tuple<string, string, string>("|", "&#124;", ""),
            new Tuple<string, string, string>("}", "&#125;", ""),
            new Tuple<string, string, string>("~", "&#126;", ""),
            new Tuple<string, string, string>("", "&#160;", "&nbsp;"),
            new Tuple<string, string, string>("¡", "&#161;", "&iexcl;"),
            new Tuple<string, string, string>("¢", "&#162;", "&cent;"),
            new Tuple<string, string, string>("£", "&#163;", "&pound;"),
            new Tuple<string, string, string>("¤", "&#164;", "&curren;"),
            new Tuple<string, string, string>("¥", "&#165;", "&yen;"),
            new Tuple<string, string, string>("¦", "&#166;", "&brvbar;"),
            new Tuple<string, string, string>("§", "&#167;", "&sect;"),
            new Tuple<string, string, string>("¨", "&#168;", "&uml;"),
            new Tuple<string, string, string>("©", "&#169;", "&copy;"),
            new Tuple<string, string, string>("ª", "&#170;", "&ordf;"),
            new Tuple<string, string, string>("«", "&#171;", "&laquo;"),
            new Tuple<string, string, string>("¬", "&#172;", "&not;"),
            new Tuple<string, string, string>("­", "&#173;", "&shy;"),
            new Tuple<string, string, string>("®", "&#174;", "&reg;"),
            new Tuple<string, string, string>("¯", "&#175;", "&macr;"),
            new Tuple<string, string, string>("°", "&#176;", "&deg;"),
            new Tuple<string, string, string>("±", "&#177;", "&plusmn;"),
            new Tuple<string, string, string>("²", "&#178;", "&sup2;"),
            new Tuple<string, string, string>("³", "&#179;", "&sup3;"),
            new Tuple<string, string, string>("´", "&#180;", "&acute;"),
            new Tuple<string, string, string>("µ", "&#181;", "&micro;"),
            new Tuple<string, string, string>("¶", "&#182;", "&para;"),
            new Tuple<string, string, string>("·", "&#183;", "&middot;"),
            new Tuple<string, string, string>("¸", "&#184;", "&cedil;"),
            new Tuple<string, string, string>("¹", "&#185;", "&sup1;"),
            new Tuple<string, string, string>("º", "&#186;", "&ordm;"),
            new Tuple<string, string, string>("»", "&#187;", "&raquo;"),
            new Tuple<string, string, string>("¼", "&#188;", "&frac14;"),
            new Tuple<string, string, string>("½", "&#189;", "&frac12;"),
            new Tuple<string, string, string>("¾", "&#190;", "&frac34;"),
            new Tuple<string, string, string>("¿", "&#191;", "&iquest;"),
            new Tuple<string, string, string>("À", "&#192;", "&Agrave;"),
            new Tuple<string, string, string>("Á", "&#193;", "&Aacute;"),
            new Tuple<string, string, string>("Â", "&#194;", "&Acirc;"),
            new Tuple<string, string, string>("Ã", "&#195;", "&Atilde;"),
            new Tuple<string, string, string>("Ä", "&#196;", "&Auml;"),
            new Tuple<string, string, string>("Å", "&#197;", "&Aring;"),
            new Tuple<string, string, string>("Æ", "&#198;", "&AElig;"),
            new Tuple<string, string, string>("Ç", "&#199;", "&Ccedil;"),
            new Tuple<string, string, string>("È", "&#200;", "&Egrave;"),
            new Tuple<string, string, string>("É", "&#201;", "&Eacute;"),
            new Tuple<string, string, string>("Ê", "&#202;", "&Ecirc;"),
            new Tuple<string, string, string>("Ë", "&#203;", "&Euml;"),
            new Tuple<string, string, string>("Ì", "&#204;", "&Igrave;"),
            new Tuple<string, string, string>("Í", "&#205;", "&Iacute;"),
            new Tuple<string, string, string>("Î", "&#206;", "&Icirc;"),
            new Tuple<string, string, string>("Ï", "&#207;", "&Iuml;"),
            new Tuple<string, string, string>("Ð", "&#208;", "&ETH;"),
            new Tuple<string, string, string>("Ñ", "&#209;", "&Ntilde;"),
            new Tuple<string, string, string>("Ò", "&#210;", "&Ograve;"),
            new Tuple<string, string, string>("Ó", "&#211;", "&Oacute;"),
            new Tuple<string, string, string>("Ô", "&#212;", "&Ocirc;"),
            new Tuple<string, string, string>("Õ", "&#213;", "&Otilde;"),
            new Tuple<string, string, string>("Ö", "&#214;", "&Ouml;"),
            new Tuple<string, string, string>("×", "&#215;", "&times;"),
            new Tuple<string, string, string>("Ø", "&#216;", "&Oslash;"),
            new Tuple<string, string, string>("Ù", "&#217;", "&Ugrave;"),
            new Tuple<string, string, string>("Ú", "&#218;", "&Uacute;"),
            new Tuple<string, string, string>("Û", "&#219;", "&Ucirc;"),
            new Tuple<string, string, string>("Ü", "&#220;", "&Uuml;"),
            new Tuple<string, string, string>("Ý", "&#221;", "&Yacute;"),
            new Tuple<string, string, string>("Þ", "&#222;", "&THORN;"),
            new Tuple<string, string, string>("ß", "&#223;", "&szlig;"),
            new Tuple<string, string, string>("à", "&#224;", "&agrave;"),
            new Tuple<string, string, string>("á", "&#225;", "&aacute;"),
            new Tuple<string, string, string>("â", "&#226;", "&acirc;"),
            new Tuple<string, string, string>("ã", "&#227;", "&atilde;"),
            new Tuple<string, string, string>("ä", "&#228;", "&auml;"),
            new Tuple<string, string, string>("å", "&#229;", "&aring;"),
            new Tuple<string, string, string>("æ", "&#230;", "&aelig;"),
            new Tuple<string, string, string>("ç", "&#231;", "&ccedil;"),
            new Tuple<string, string, string>("è", "&#232;", "&egrave;"),
            new Tuple<string, string, string>("é", "&#233;", "&eacute;"),
            new Tuple<string, string, string>("ê", "&#234;", "&ecirc;"),
            new Tuple<string, string, string>("ë", "&#235;", "&euml;"),
            new Tuple<string, string, string>("ì", "&#236;", "&igrave;"),
            new Tuple<string, string, string>("í", "&#237;", "&iacute;"),
            new Tuple<string, string, string>("î", "&#238;", "&icirc;"),
            new Tuple<string, string, string>("ï", "&#239;", "&iuml;"),
            new Tuple<string, string, string>("ð", "&#240;", "&eth;"),
            new Tuple<string, string, string>("ñ", "&#241;", "&ntilde;"),
            new Tuple<string, string, string>("ò", "&#242;", "&ograve;"),
            new Tuple<string, string, string>("ó", "&#243;", "&oacute;"),
            new Tuple<string, string, string>("ô", "&#244;", "&ocirc;"),
            new Tuple<string, string, string>("õ", "&#245;", "&otilde;"),
            new Tuple<string, string, string>("ö", "&#246;", "&ouml;"),
            new Tuple<string, string, string>("÷", "&#247;", "&divide;"),
            new Tuple<string, string, string>("ø", "&#248;", "&oslash;"),
            new Tuple<string, string, string>("ù", "&#249;", "&ugrave;"),
            new Tuple<string, string, string>("ú", "&#250;", "&uacute;"),
            new Tuple<string, string, string>("û", "&#251;", "&ucirc;"),
            new Tuple<string, string, string>("ü", "&#252;", "&uuml;"),
            new Tuple<string, string, string>("ý", "&#253;", "&yacute;"),
            new Tuple<string, string, string>("þ", "&#254;", "&thorn;"),
            new Tuple<string, string, string>("ÿ", "&#255;", "&yuml;"),
            new Tuple<string, string, string>("Œ", "&#338;", ""),
            new Tuple<string, string, string>("œ", "&#339;", ""),
            new Tuple<string, string, string>("Š", "&#352;", ""),
            new Tuple<string, string, string>("š", "&#353;", ""),
            new Tuple<string, string, string>("Ÿ", "&#376;", ""),
            new Tuple<string, string, string>("ƒ", "&#402;", "&fnof;"),
            new Tuple<string, string, string>("–", "&#8211;", "&ndash;"),
            new Tuple<string, string, string>("—", "&#8212;", "&mdash;"),
            new Tuple<string, string, string>("‘", "&#8216;", "&lsquo;"),
            new Tuple<string, string, string>("’", "&#8217;", "&rsquo;"),
            new Tuple<string, string, string>("‚", "&#8218;", ""),
            new Tuple<string, string, string>("“", "&#8220;", "&ldquo;"),
            new Tuple<string, string, string>("”", "&#8221;", "&rdquo;"),
            new Tuple<string, string, string>("„", "&#8222;", "&bdquo;"),
            new Tuple<string, string, string>("†", "&#8224;", "&dagger;"),
            new Tuple<string, string, string>("‡", "&#8225;", "&Dagger;"),
            new Tuple<string, string, string>("•", "&#8226;", "&bull;"),
            new Tuple<string, string, string>("‥", "&#8229;", "&nldr;"),
            new Tuple<string, string, string>("…", "&#8230;", "&hellip;"),
            new Tuple<string, string, string>("‰", "&#8240;", "&permil;"),
            new Tuple<string, string, string>("‹", "&#8249;", "&lsaquo;"),
            new Tuple<string, string, string>("›", "&#8250;", "&rsaquo;"),
            new Tuple<string, string, string>("€", "&#8364;", "&euro;"),
            new Tuple<string, string, string>("™", "&#8482;", ""),
            new Tuple<string, string, string>("♡", "&#9825;", "")
        };

        public static string ReplaceHtmlCharactersAndSymbols(string html)
        {
            var newHtml = html;

            foreach (var (symbol, htmlNumber, htmlName) in HtmlChars)
            {
                newHtml = newHtml.Replace(htmlNumber, symbol);
                newHtml = newHtml.Replace(htmlNumber.Replace(";", ""), symbol);

                if (!string.IsNullOrEmpty(htmlName))
                {
                    newHtml = newHtml.Replace(htmlName, symbol);
                }
            }

            return newHtml;
        }

        public static void AddParagraphs(string html, Document document)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var paragraphs = htmlDocument.DocumentNode.ChildNodes;

            foreach (var paragraph in paragraphs)
            {
                try
                {
                    switch (paragraph.Name.ToLower())
                    {
                        case "p":
                        case "a":
                            {
                                var prg = new Paragraph(paragraph.InnerText).SetFontSize(ParagraphFontSize);

                                document.Add(prg);
                                break;
                            }
                        case "h1":
                        case "h2":
                        case "h3":
                        case "h4":
                        case "h5":
                        case "h6":
                            {
                                var prg = new Paragraph(paragraph.InnerText).SetFontSize(ParagraphFontSize + 1).SetBold();

                                document.Add(prg);
                                break;
                            }
                        case "#text":
                            {
                                var prg = new Paragraph(paragraph.InnerText).SetFontSize(ParagraphFontSize - 5).SetItalic();

                                document.Add(prg);
                                break;
                            }
                        case "table":
                            {
                                var trs = paragraph.SelectSingleNode(".//tbody") != null ?
                                    paragraph.SelectSingleNode(".//tbody").ChildNodes.Where(q => q.Name.ToLower() == "tr").ToList() :
                                    paragraph.ChildNodes.Where(q => q.Name.ToLower() == "tr").ToList();

                                var rows = trs.Select(tr => tr.ChildNodes.Where(q => q.Name == "td").ToList())
                                    .Select(tds => tds.Select(td => td.InnerText.Replace("•", "-"))
                                        .ToList())
                                    .ToList();

                                if (rows.Count <= 0) continue;

                                Table table = new(rows.Select(q => q.Count).Max());
                                foreach (var row in rows)
                                {
                                    for (var i = 0; i < table.GetNumberOfColumns(); i++)
                                    {
                                        table.AddCell(i >= row.Count
                                            ? new Cell().Add(new Paragraph(""))
                                            : new Cell().Add(new Paragraph(row[i]).SetFontSize(ParagraphFontSize - 5).SetItalic()));
                                    }
                                }

                                document.Add(table);
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void AddChapterTitle(string title, Document document)
        {
            try
            {
                var prg = new Paragraph(title).SetBold()
                                        .SetFontSize(ChapterTitleFontSize);
                document.Add(prg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}