using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MarkdownToTreeBlog
{
    public class PathNode
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Title { get; protected set; }
        public byte level { get; set; }
        public PathNode Parent { get; set; }
        public List<PathNode> Children { get; set; }
        public int OrderInSibling { get; set; }
        public string Anchor { get; set; }
    }

    public class Doc : PathNode
    {
        public string Content { get; set; }
        public new string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                Match match = Regex.Match(base.Name, @"\d{8}-\d{4}");
                if (!match.Success) throw new Exception($"「{base.Name}」这个文件命名不符合规则。");
                string year = match.Value.Substring(0, 4);
                string month = match.Value.Substring(4, 2);
                string day = match.Value.Substring(6, 2);
                string hour = match.Value.Substring(9,2);
                string minute = match.Value.Substring(11,2);
                string dateTime = $"{year}-{month}-{day} {hour}:{minute}:00";
                DateTime dt;
                if (!DateTime.TryParse(dateTime, out dt)) throw new Exception($"「{Name}」这个文件命名中的时间格式不符合规则。");
                CreatedTime = dt;
            }
        }
        private DateTime _createdTime;
        public DateTime CreatedTime
        {
            get { return _createdTime; }
            private set
            {
                _createdTime = value;
                URL = $"/{_createdTime.ToString("yyyyMMdd-HHmm")}/";
            }
        }
        public DateTime? ModifiedTime { get; set; }
        public Article Previous { get; set; }
        public Article Next { get; set; }
        public string URL { get; set; }
    }

    public class DirNode : PathNode
    {
        public new string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                var arr = base.Name.Split('-');
                this.Title = arr[arr.Length - 1];
            }
        }
    }
}
