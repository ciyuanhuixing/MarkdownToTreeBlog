using HtmlAgilityPack;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MarkdownToTreeBlog
{
    public class Article : Doc
    {
        private static readonly string imgUrlPrefix = ConfigurationManager.AppSettings["ImgUrlPrefix"];
        private static readonly string imgUrlFileSuffix = ConfigurationManager.AppSettings["ImgUrlFileSuffix"];

        public string Content
        {
            get { return base.Content; }
            set
            {
                base.Content = value;

                HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(string.Format("<html><body>{0}</body></html>", base.Content));

                var h1 = doc.DocumentNode.SelectSingleNode("//h1");
                if (h1 != null)
                {
                    this.Title = h1.InnerText;
                    h1.Remove();
                }
                else if (!string.IsNullOrWhiteSpace(this.Name))
                {
                    var nameSplitArr = this.Name.Split('-');
                    this.Title = nameSplitArr[nameSplitArr.Length - 1];
                }
                this.TOC = GetTOC(doc.DocumentNode);

                var imgNodes = doc.DocumentNode.Descendants("img").ToList();
                if (imgNodes.Any())
                {
                    this.ImageUrl = imgNodes[0].Attributes["src"].Value;
                }
                foreach (var img in imgNodes)
                {
                    string src = img.Attributes["src"].Value;
                    if (!string.IsNullOrWhiteSpace(imgUrlPrefix))
                    {
                        src = src.Replace("%5C", "/");
                        int index = src.LastIndexOfAny(new char[] { '/', '\\' });
                        if (index > -1)
                        {
                            src = imgUrlPrefix + src.Substring(index + 1);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(imgUrlFileSuffix))
                    {
                        int dotIndex = src.LastIndexOf('.');
                        if (dotIndex > 0)
                        {
                            src = src.Substring(0, dotIndex) + imgUrlFileSuffix;
                        }
                    }
                    string alt = img.Attributes["alt"].Value;
                    string figcaption = string.IsNullOrWhiteSpace(alt) ? string.Empty : $"<figcaption>{alt}</figcaption>";
                    var newNodeStr = $"<figure class=\"fig-img\"><img src=\"{src}\" alt=\"{alt}\">{figcaption}</figure>";
                    var newNode = HtmlNode.CreateNode(newNodeStr);
                    img.ParentNode.ReplaceChild(newNode, img);
                    if (newNode.ParentNode.Name == "p")
                    {
                        newNode.ParentNode.ParentNode.ReplaceChild(newNode, newNode.ParentNode);
                    }
                }

                base.Content = doc.DocumentNode.FirstChild.FirstChild.InnerHtml.Trim('\n');

                // summary
                var textArray = GetHtmlText(doc.DocumentNode).ToArray();
                int len = 97;
                StringBuilder sb = new StringBuilder(len + 3);
                foreach (var t in textArray)
                {
                    if (sb.Length <= len && (sb.Length + t.Length) <= len)
                    {
                        sb.Append(t);
                    }
                    else
                    {
                        // let's make sure this isn't just a really long text node
                        if (t.Length > len)
                        {
                            int charsLeft = len - sb.Length;
                            sb.Append(t.Substring(0, charsLeft));
                        }
                        sb.Append("...");
                        break;
                    }
                }
                this.BodySummary = sb.ToString();
            }
        }
        public string ImageUrl { get; private set; }
        public string BodySummary { get; private set; }

        public List<TOCH2> TOC { get; set; }

        private IEnumerable<string> GetHtmlText(HtmlNode node)
        {
            if (node.Name == "#text")
            {
                yield return node.InnerText.Trim();
            }

            foreach (var child in node.ChildNodes)
            {
                foreach (var childText in GetHtmlText(child))
                {
                    yield return childText;
                }
            }
        }

        private List<TOCH2> GetTOC(HtmlNode node)
        {
            var hNodes = node.Descendants().Where(n => n.Name == "h2" || n.Name == "h3").ToList();
            if (!hNodes.Any()) return null;
            List<TOCH2> toc = new List<TOCH2>();
            TOCH2 tocH2 = null;
            int h2Count = 0;
            int h3Count = 0;
            for (int i = 0; i < hNodes.Count; i++)
            {
                var hNode = hNodes[i];
                if (hNode.Name == "h2")
                {
                    h2Count++;
                    h3Count = 0;
                    hNode.InnerHtml = $"{NumToChinese(h2Count)}、{hNode.InnerHtml}";
                    hNode.Id = h2Count.ToString();
                    tocH2 = new TOCH2() { Title = hNode.InnerText, Level = 1, Anchor = hNode.Id };
                    toc.Add(tocH2);
                }
                else
                {
                    h3Count++;

                    if (tocH2 != null)
                    {
                        hNode.InnerHtml = $"{h2Count}.{h3Count} {hNode.InnerHtml}";
                        hNode.Id = $"{h2Count}-{h3Count}";
                        if (tocH2.H3s == null) tocH2.H3s = new List<TOCItem>();
                        tocH2.H3s.Add(new TOCItem() { Title = hNode.InnerText, Level = 2, Anchor = hNode.Id });
                    }
                    else
                    {
                        hNode.InnerHtml = $"{h3Count}. {hNode.InnerHtml}";
                        hNode.Id = $"0-{h3Count}";
                    }
                }
            }
            //return toc;
            return null;
        }
        private string NumToChinese(int num)
        {
            if (num >= 100 || num < 1) return num.ToString();

            string chNum = "十一二三四五六七八九";
            StringBuilder ch = new StringBuilder();
            if (num >= 20 && num < 100)
            {
                ch.Append(chNum[num / 10]);
                ch.Append(chNum[0]);
            }
            else if (num >= 10)
            {
                ch.Append(chNum[0]);
            }
            int lastDigit = num % 10;
            if (lastDigit == 0) return ch.ToString();
            ch.Append(chNum[lastDigit]);
            return ch.ToString();
        }
    }

    public class TOCH2 : TOCItem
    {
        public List<TOCItem> H3s { get; set; }
    }
    public class TOCItem
    {
        public string Title { get; set; }
        public string Anchor { get; set; }
        public byte Level { get; set; }
    }
}


