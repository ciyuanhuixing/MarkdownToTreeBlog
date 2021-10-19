let gulp = require('gulp')
let cleanCSS = require('gulp-clean-css')
let htmlmin = require('gulp-htmlmin')
let htmlclean = require('gulp-htmlclean')
// let babel = require('gulp-babel') /* 转换为es2015 */
// let uglify = require('gulp-uglify')
// let imagemin = require('gulp-imagemin')
// let uncss = require('gulp-uncss')

// 设置根目录
const root = './site'

// 匹配模式， **/*代表匹配所有目录下的所有文件
const pattern = '**/*'

// 压缩html
gulp.task('minify-html', function() {
  return gulp
    // 匹配所有 .html结尾的文件
    .src(`${root}/${pattern}.html`)
    .pipe(htmlclean())
    .pipe(
      htmlmin({
        removeComments: true,
        minifyJS: true,
        minifyCSS: true,
        minifyURLs: true
      })
    )
    .pipe(gulp.dest('./site'))
})

// // 去除未使用的css
// gulp.task('uncss', function() {
//   return gulp
//     // 匹配所有 .css结尾的文件
//     .src(`${root}/${pattern}.css`)
//     .pipe(
//       uncss({
//         html: [`${root}/${pattern}.html`]  //使用css的html页面，可多个
//       })
//     )
//     .pipe(gulp.dest('./site'))
// })

// 压缩css
gulp.task('minify-css', function() {
  return gulp
    // 匹配所有 .css结尾的文件
    .src(`${root}/${pattern}.css`)
    .pipe(
      cleanCSS({
        compatibility: 'ie8'
      })
    )
    .pipe(gulp.dest('./site'))
})

// // 压缩js
// gulp.task('minify-js', function() {
//   return gulp
//     // 匹配所有 .js结尾的文件
//     .src(`${root}/${pattern}.js`)
//     .pipe(
//       babel({
//         presets: ['@babel/preset-env']
//       })
//     )
//     .pipe(uglify())
//     .pipe(gulp.dest('./site'))
// })

// 压缩图片
// gulp.task('minify-images', function() {
//   return gulp
//     // 匹配site/images目录下的所有文件
//     .src(`${root}/images/${pattern}`)
//     .pipe(
//       imagemin(
//         [
//           imagemin.gifsicle({ optimizationLevel: 3 }),
//           imagemin.jpegtran({ progressive: true }),
//           imagemin.optipng({ optimizationLevel: 7 }),
//           imagemin.svgo()
//         ],
//         { verbose: true }
//       )
//     )
//     .pipe(gulp.dest('./site/images'))
// })

gulp.task('default', gulp.series('minify-html', 'minify-css'))