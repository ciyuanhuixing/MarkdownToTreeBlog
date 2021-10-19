using Markdig;
using RazorLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownToTreeBlog
{
    class Program
    {
        private const string _siteURL = "https://ciyuanhuixing.com";
        private const string _siteName = "次元彗星的笔记";
        private const string _author = "次元彗星";
        private const string _siteDescription = "这里记录了一些我对编程的学习、经验总结，还有对生活的一点思考。";
        private const string _theme = "comet";
        private const string _siteDir = "site";

        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            BuildSite().Wait();
            sw.Stop();
            Console.WriteLine($"Total time: {sw.Elapsed.TotalSeconds.ToString("0")}s");
        }

        private static async Task BuildSite()
        {
            string docsPath = ConfigurationManager.AppSettings["DocsDir"];
            DirNode root = new DirNode() { Name = _siteName, Path = docsPath, Children = new List<PathNode>() };
            List<Article> docs = await BuildMDFilePathTreeAndGetArticles(root);
            RazorLightEngine engine = new RazorLightEngineBuilder()
                .UseFileSystemProject($"{Directory.GetCurrentDirectory().Replace('\\', '/')}/themes/{_theme}")
                .UseMemoryCachingProvider()
                .Build();
            if (!Directory.Exists(_siteDir)) Directory.CreateDirectory(_siteDir);

            CommonInfo commonInfo = new CommonInfo()
            {
                SiteName = _siteName,
                SiteURL = _siteURL,
                Author = _author,
                Title = _siteName,
                Description = _siteDescription,
                PublishedTime = DateTime.UtcNow.AddHours(8),
                URL = _siteURL,
            };
            commonInfo.ModifiedTime = commonInfo.PublishedTime;
            string razorResult = await engine.CompileRenderAsync("index.cshtml", new IndexViewModel { CommonInfo = commonInfo, PathRoot = root });
            await SaveFile(razorResult, "index.html");
            commonInfo.URL = _siteURL + "404.html";
            razorResult = await engine.CompileRenderAsync("404.cshtml", new DocViewModel { CommonInfo = commonInfo });
            await SaveFile(razorResult, "404.html");

            foreach (var doc in docs)
            {
                commonInfo.Title = $"{doc.Title} - {commonInfo.SiteName}";
                commonInfo.Description = doc.BodySummary;
                commonInfo.PublishedTime = doc.CreatedTime;
                commonInfo.ModifiedTime = doc.ModifiedTime;
                commonInfo.URL = _siteURL + doc.URL;
                commonInfo.ImageUrl = doc.ImageUrl;
                razorResult = await engine.CompileRenderAsync("doc.cshtml", new DocViewModel() { CommonInfo = commonInfo, Doc = doc });
                await SaveFile(razorResult, doc.URL);
            }

            string staticDir = $"themes\\{_theme}\\static";
            if (Directory.Exists(staticDir)) FileHelper.CopyDir(staticDir, _siteDir);
        }

        private static async Task<List<Article>> BuildMDFilePathTreeAndGetArticles(DirNode root)
        {
            var mdFiles = Directory.EnumerateFiles(root.Path, "*.md", SearchOption.AllDirectories);
            if (!mdFiles.Any()) throw new Exception($"{root.Path}目录中没有.md文件。");
            mdFiles = mdFiles.OrderBy(x => x);
            List<Article> articles = new List<Article>();

            Dictionary<string, DirNode> keyNodes = new Dictionary<string, DirNode>();
            Article preDoc = null;
            foreach (var file in mdFiles)
            {
                string[] nodeNames = file.Substring(root.Path.Length + 1).Split(Path.DirectorySeparatorChar);

                Article docNode = new Article()
                {
                    Path = file,
                    Name = Path.GetFileName(file),
                    level = (byte)nodeNames.Length,
                    Previous = preDoc,
                };
                articles.Add(docNode);
                if (preDoc != null) preDoc.Next = docNode;
                preDoc = docNode;
                if (nodeNames.Length == 1)
                {
                    root.Children.Add(docNode);
                    docNode.Parent = root;
                    docNode.OrderInSibling = root.Children.Count;
                    continue;
                }

                PathNode preNode = docNode;
                for (int i = nodeNames.Length - 2; i > -1; i--)
                {
                    string currentName = nodeNames[i];
                    string key = $"{i}/{currentName}";

                    DirNode parentNode;
                    if (keyNodes.ContainsKey(key))
                    {
                        parentNode = keyNodes[key];
                        AddChildPathNode(parentNode, preNode);
                        break;
                    }
                    else
                    {
                        parentNode = new DirNode()
                        {
                            Name = currentName,
                            level = (byte)(i + 1),
                            Children = new List<PathNode>(),
                        };
                        keyNodes.Add(key, parentNode);

                        AddChildPathNode(parentNode, preNode);
                        preNode = parentNode;

                        if (i == 0)
                        {
                            root.Children.Add(parentNode);
                            parentNode.Parent = root;
                            parentNode.OrderInSibling = root.Children.Count;
                        }
                    }
                }
            }

            SetChildrenAnchor(root);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            //using (var repoInfo = RepositoryInformation.GetRepositoryInformationForPath(docsPath))
            //{
            foreach (var doc in articles)
            {
                //doc.ModifiedTime = repoInfo.GetFileLastCommitTime(doc.Path.Replace(Path.DirectorySeparatorChar, '/')
                //.Substring($"{docsPath}/".Length));

                string text = await File.ReadAllTextAsync(doc.Path);
                doc.Content = Markdown.ToHtml(text, pipeline);
            }
            //}

            return articles;
        }

        private static void SetChildrenAnchor(PathNode parent)
        {
            foreach (var child in parent.Children)
            {
                child.Anchor = parent.Anchor == null ? child.OrderInSibling.ToString() : $"{parent.Anchor}-{child.OrderInSibling}";
                if (child.Children != null) SetChildrenAnchor(child);
            }
        }

        private static void AddChildPathNode(PathNode parent, PathNode child)
        {
            parent.Children.Add(child);
            child.Parent = parent;
            child.OrderInSibling = parent.Children.Count;
        }

        private static async Task SaveFile(string content, string path)
        {
            if (path.First() == '/') path = path.Substring(1, path.Length - 1);
            if (path.Last() == '/') path += "index.html";
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = Path.Combine(_siteDir, path);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            Console.WriteLine("生成：{0}", path);
            using (StreamWriter writer = new StreamWriter(path))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
