using HelloQuery.Data;
using HelloQuery.Filter;
using HelloQuery.Method;
using HelloQuery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
            // idをもとにLessonを取得
            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == lessonId);

            // 一致するlessonがない場合はNotFoundを返す
            if (lesson == null)
            {
                return NotFound();
            }

            // 文字が入力されていなかったらIndexにリダイレクト
            if (string.IsNullOrWhiteSpace(answer))
            {
                return RedirectToAction("Index", new { id = lessonId });
            }

            // 正規表現パターンを定義
            string pattern = @"```\s*\r?\n([\s\S]*?)\r?\n```";

            // 正規表現を使ってAnswerカラムからSQL文を抽出
            string lessonAnswer = Regex.Match(lesson.Answer, pattern).Groups[1].Value;

            // 整形後の文字列を出力ウィンドウに表示 ※※※確認用※※※
            Debug.WriteLine("Formatted SQL query:");
            Debug.WriteLine(lessonAnswer);

            // ユーザーの入力内容と整形後のSQL文から改行文字を取り除く
            string userAnswer = Regex.Replace(answer, @"\s+", " ");
            string formattedLessonAnswer = Regex.Replace(lessonAnswer, @"\s+", " ");

            // 改行を取り除いた文字列を大文字に変換して比較
            if (userAnswer.ToUpper() == formattedLessonAnswer.ToUpper())
            {
                return RedirectToAction("AnswerPage", new { id = lessonId });
            }
            else
            {
                return RedirectToAction("Index", new { id = lessonId });
            }
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


