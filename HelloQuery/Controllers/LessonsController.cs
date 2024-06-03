using HelloQuery.Data;
using HelloQuery.Filter;
using HelloQuery.Method;
using HelloQuery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HelloQuery.Controllers
{
    [SessionCheckFilter]
    public class LessonsController : Controller
    {
        private readonly HelloQueryContext _context;

        public LessonsController(HelloQueryContext context)
        {
            _context = context;
        }

        //  Lessons/Index:GETアクセスあったとき
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            // 全Lessonレコードを取得
            var lessons = await _context.Lesson.ToListAsync();

            // 選択されたLessonを入れる変数
            Lesson selectedLesson;

            // ViewModelのインスタンス生成
            LessonViewModel viewModel = new LessonViewModel();

            // idがnullとき最初のLessonを選択
            if (id == null)
            {
                selectedLesson = lessons.FirstOrDefault(m => m.LessonId == 1);

                // マークダウンの内容をHTMLに変換
                MarkdownConverter.ConvertMarkdownToHtml(selectedLesson);
            }
            // idがnullでない場合、選択されたLessonを取得
            else
            {
                selectedLesson = lessons.FirstOrDefault(m => m.LessonId == id);

                // マークダウンの内容をHTMLに変換
                MarkdownConverter.ConvertMarkdownToHtml(selectedLesson);
            }

            // ViewModelに選択されたLessonと全Lessonをセット
            viewModel.SelectedLesson = selectedLesson;
            viewModel.AllLessons = lessons;

            return View(viewModel);
        }

        // 問題が選択されたときの部分ビュー作成メソッド
        public async Task<IActionResult> SelectLesson(int id)
        {
            // JSから渡されたidをもとに、選択されたLessonを取得
            var selectedLesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == id);

            if (selectedLesson == null)
            {
                return NotFound();
            }

            // マークダウンの内容をHTMLに変換
            MarkdownConverter.ConvertMarkdownToHtml(selectedLesson);

            LessonViewModel viewModel = new LessonViewModel();
            viewModel.SelectedLesson = selectedLesson;
            viewModel.AllLessons = null;

            return PartialView("_LessonsPartial", viewModel);
        }

        // 回答するがクリックされたとき: Lessons/Index
        [HttpPost]
        public async Task<IActionResult> Answer(string answer, int lessonId)
        {
            // LessonIdをもとに解答リストを取得
            var validAnswers = _context.Answer
                                       .Where(a => a.LessonId == lessonId)
                                       .Select(a => a.ValidAnswers)
                                       .ToList();

            // 文字が入力されていなかったらIndexにリダイレクト
            if (string.IsNullOrWhiteSpace(answer))
            {
                return RedirectToAction("Index", new { id = lessonId });
            }

            // ユーザーの入力クエリにシングルクォートがある場合はNプレフィックスを追加
            string userAnswer = AddUnicodePrefixToLiterals(answer);

            // ユーザーの入力内容と整形後のSQL文から改行文字を取り除く
            string formattedUserAnswer = Regex.Replace(userAnswer, @"\s+", " ");

            // 整形後の文字列を出力ウィンドウに表示 ※※※確認用※※※
            Debug.WriteLine("User Formatted SQL query:");
            Debug.WriteLine(formattedUserAnswer);

            try
            {
                // 大文字小文字を無視して比較する
                bool isCorrect = validAnswers.Any(answer =>
                    string.Equals(formattedUserAnswer, answer, StringComparison.OrdinalIgnoreCase));

                if (isCorrect)
                {
                    // 正解の場合の処理
                    return RedirectToAction("AnswerPage", new { id = lessonId });
                }
                else
                {
                    // 不正解の場合の処理
                    return RedirectToAction("Index", new { id = lessonId });
                }
            }
            catch (SqlException ex)
            {
                // SQL文として成立しない場合はIndexにリダイレクト
                return RedirectToAction("Index", new { id = lessonId });
            }
        }

        // Nプレフィックス追加するメソッド
        private string AddUnicodePrefixToLiterals(string query)
        {
            // 正規表現パターン: '...' で囲まれた文字列リテラルを見つける
            string pattern = @"'([^']*)'";
            // 置換パターン: N'...' として置換
            string replacement = "N'$1'";

            // 正規表現で置換
            string result = Regex.Replace(query, pattern, replacement);
            return result;
        }

        // あきらめるがクリックされたとき: Lessons/Index
        [HttpPost]
        public async Task<IActionResult> GiveUp(int lessonId)
        {
            // idをもとにLessonを取得
            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == lessonId);

            if (lesson == null)
            {
                return NotFound();
            }

            return RedirectToAction("AnswerPage", new { id = lessonId });
        }

        //  Lessons/AnswerPageにGETアクセスがあったとき
        [HttpGet]
        public async Task<IActionResult> AnswerPage(int? id)
        {
            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            // ViewModelのインスタンス生成
            var viewModel = new LessonViewModel();

            // idをもとにLessonを取得
            viewModel.SelectedLesson = await _context.Lesson.FirstOrDefaultAsync(m => m.LessonId == id);

            // Lessonがnullの場合はNotFoundを返す
            if (viewModel.SelectedLesson == null)
            {
                return NotFound();
            }

            // マークダウンの内容をHTMLに変換
            MarkdownConverter.ConvertMarkdownToHtml(viewModel.SelectedLesson);

            // 全Lessonを取得
            viewModel.AllLessons = await _context.Lesson.ToListAsync();

            return View(viewModel);
        }

        // 回答ページの部分ビュー作成メソッド
        public async Task<IActionResult> GetAnswer(int id)
        {
            // JSから渡されたidをもとに、選択されたLessonを取得
            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == id);

            // ViewModelのインスタンス生成
            var viewModel = new LessonViewModel();

            if (lesson == null)
            {
                return NotFound();
            }

            // マークダウンの内容をHTMLに変換
            MarkdownConverter.ConvertMarkdownToHtml(lesson);

            viewModel.SelectedLesson = lesson;
            viewModel.AllLessons = null;

            return PartialView("_AnswerPartial", viewModel);
        }

        // 「ランダム練習問題」がクリックされたとき
        [HttpGet]
        public async Task<IActionResult> Practice()
        {
            // セッションから前回のLessonIdを取得
            int lastLessonId = HttpContext.Session.GetInt32("LastLessonId") ?? 0;
            int last2LessonId = HttpContext.Session.GetInt32("Last2LessonId") ?? 0;

            // レッスンidをランダム生成
            Random randomPractice = new Random();
            int randomLessonId = randomPractice.Next(1, 5);

            // 前回のLessonIdと同じ場合は再度生成
            while (randomLessonId == lastLessonId || randomLessonId == last2LessonId)
            {
                randomLessonId = randomPractice.Next(1, 5);
            }

            // 生成したLessonIdをセッションに保存
            HttpContext.Session.SetInt32("LastLessonId", randomLessonId);
            HttpContext.Session.SetInt32("Last2LessonId", lastLessonId);

            // IndexのGetメソッドへidを送付
            return RedirectToAction("Index", new { id = randomLessonId });
        }
    }
}


