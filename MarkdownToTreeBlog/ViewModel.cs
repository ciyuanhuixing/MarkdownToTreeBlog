using System;
using System.Collections.Generic;

namespace MarkdownToTreeBlog
{
    public class CommonInfo
    {
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? PublishedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string URL { get; set; }

        private Dictionary<string, string> _meta;
        public Dictionary<string, string> Meta
        {
            get
            {
                _meta = new Dictionary<string, string>();
                _meta["description"] = this.Description;

                _meta["og:site_name"] = this.SiteName;
                _meta["og:type"] = "article";
                _meta["og:title"] = this.Title;
                _meta["og:description"] = this.Description;
                _meta["og:url"] = this.URL;

                if (!string.IsNullOrWhiteSpace(this.ImageUrl))
                {
                    _meta["og:image"] = this.ImageUrl;
                }

                const string utcFormat = "yyyy-MM-ddTHH:mm:ss+08:00";
                if (this.PublishedTime.HasValue) _meta["article:published_time"] = this.PublishedTime?.ToString(utcFormat);
                if (this.ModifiedTime.HasValue) _meta["article:modified_time"] = this.ModifiedTime?.ToString(utcFormat);

                return _meta;
            }
        }
    }

    public class ViewModel
    {
        public CommonInfo CommonInfo { get; set; }
    }

    public class DocViewModel : ViewModel
    {
        public Article Doc { get; set; }
    }
    public class IndexViewModel : ViewModel
    {
        public PathNode PathRoot { get; set; }
    }
}
