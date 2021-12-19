[树形分类博客生成器](https://github.com/ciyuanhuixing/MarkdownToTreeBlog)是我写的一个静态网站生成器，可以将一些 Markdown 格式的笔记文件转换为一个具有树形分类目录的笔记类博客，树形目录按照 Markdown 文件的存储路径来生成。

生成博客示例：[次元彗星的笔记](https://ciyuanhuixing.com/)。

我曾经用过 Hexo 这个能将 Markdown 文件转为静态博客的工具，在使用它的过程中，我经常需要维护博客的文章分类目录，比较繁琐，所以想让博客文章分类直接映射 Markdown 文件的存储路径，也就是在 Markdown 文件转为博客时，自动按照 Markdown 文件的存储路径在博客中生成一个树形文章分类目录，但我用 Hexo 无法实现。后面我又折腾过 Hugo、Vuepress、MkDocs，也都无功而返，所以就尝试自己写了这个工具。

## 特点

- **生成的博客具有树形分类目录，相当于笔记的思维导图。只需维护笔记文件目录，博客的目录会随之而变。**
- 对 Markdown 文件侵入性低，不使用 Front Matter，尽量不「污染」Markdown 文件。
- 以 Markdown 文件的形式保存笔记，让笔记管理更加方便简单。

## 使用文档链接

[https://ciyuanhuixing.com/20211019-1943/](https://ciyuanhuixing.com/20211019-1943/)
