[Markdown To Tree Blog](https://github.com/ciyuanhuixing/MarkdownToTreeBlog) 是一个静态网站生成器，可以将一些 Markdown 格式的笔记文件转换为一个具有树形目录的笔记类博客，树形目录与 Markdown 文件所在的目录相对应。

生成博客示例：[次元彗星的笔记](https://ciyuanhuixing.com/)

文档：[Markdown To Tree Blog 使用文档](https://ciyuanhuixing.com/20211019-1943/)

我曾经使用过的支持 Markdown 文件的静态网站生成器包括 Hexo、Hugo、Vuepress、MkDocs，使用这些工具时，我很难实现让博客文章目录自动与 Markdown 文件所在目录相对应，所以就自己写了这个工具。

**Markdown To Tree Blog 的特点：**

- **生成的博客具有树形目录，相当于笔记的思维导图。只需维护笔记文件目录，博客目录会随之而变。**
- 对 Markdown 文件侵入性低，不使用 Front Matter，尽量不「污染」Markdown 文件。
- 以 Markdown 文件的形式保存笔记，让笔记管理更加方便简单。
