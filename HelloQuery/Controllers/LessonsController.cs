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
            try
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

                // Booksテーブルのデータを取得
                var books = await _context.Book.ToListAsync();

                // ViewModelに選択されたLessonと全Lesson、Booksのデータをセット
                viewModel.SelectedLesson = selectedLesson;
                viewModel.AllLessons = lessons;
                viewModel.Books = books;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-023:" + ex.Message;
                return RedirectToAction("error", "Error");
            }           
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
            var validAnswers = await _context.LessonAnswer
                                     .Where(a => a.LessonId == lessonId)
                                     .Select(a => a.ValidAnswer)
                                     .AsNoTracking()
                                     .ToListAsync();

            // リスト取得に失敗した場合
            if (validAnswers == null)
            {
                TempData["Message"] = "E-026:データの取得に失敗しました";
                return RedirectToAction("error", "Error");
            }

            // 文字が入力されていなかったらIndexにリダイレクト
            if (string.IsNullOrWhiteSpace(answer))
            {
                // リダイレクト時のメッセージを設定
                TempData["AnswerMessage"] = "解答を入力してください";

                return RedirectToAction("Index", new { id = lessonId });
            }

            // 入力文字列を整形
            string formattedUserAnswer = FormatUserInput(answer);

            // 整形後の文字列を出力ウィンドウに表示 ※※※確認用※※※
            Debug.WriteLine("User Formatted SQL query:");
            Debug.WriteLine(formattedUserAnswer);

            try
            {
                // 大文字小文字を無視して比較する
                bool isCorrect = validAnswers.Any(answer =>
                    string.Equals(formattedUserAnswer, answer, StringComparison.OrdinalIgnoreCase));

                // 正解の場合の処理
                if (isCorrect)
                {
                    // 正解の場合のメッセージを設定
                    TempData["AnswerMessage"] = "おめでとう！正解です！";
                    
                    return RedirectToAction("AnswerPage", new { id = lessonId });
                }
                // 不正解の場合の処理
                else
                {
                    // 不正解の場合のメッセージを設定
                    TempData["AnswerMessage"] = "残念。違います。";
                    
                    return RedirectToAction("Index", new { id = lessonId });
                }
            }
            // SQL文として成立しない場合はIndexにリダイレクト
            catch (SqlException ex)
            {
                // 不正解の場合のメッセージを設定
                TempData["AnswerMessage"] = "残念。違います。";

                return RedirectToAction("Index", new { id = lessonId });
            }
        }

        // ユーザーの入力文字列を整形するメソッド
        private string FormatUserInput(string userInput)
        {
            // Nプレフィックスを追加
            string formattedInput = AddUnicodePrefixToLiterals(userInput);

            // カンマの後ろにスペースを追加
            formattedInput = AddSpaceAfterComma(formattedInput);

            // 改行を取り除く
            formattedInput = RemoveNewLines(formattedInput);

            return formattedInput;
        }

        // 文字リテラルにNプレフィックス追加するメソッド
        private string AddUnicodePrefixToLiterals(string query)
        {
            string pattern = @"'([^']*)'";
            string replacement = "N'$1'";
            string result = Regex.Replace(query, pattern, replacement);
            return result;
        }

        // カンマの後ろにスペースを追加するメソッド
        private string AddSpaceAfterComma(string query)
        {
            string pattern = @",(\S)";
            string replacement = @", $1";
            string result = Regex.Replace(query, pattern, replacement);
            return result;
        }

        // 改行を取り除くメソッド
        private string RemoveNewLines(string query)
        {
            return Regex.Replace(query, @"\s+", " ");
        }

        // LINQを使用してSQL文を実行し、結果をDataTableに格納
        private async Task<DataTable> GetDataTableAsync(string sql)
        {
            var dataTable = new DataTable();

            try
            {
                // ROUND関数の検出と表示桁数の調整
                string pattern = @"ROUND\((.+?),\s*(\d+)\)";
                sql = Regex.Replace(sql, pattern, match =>
                {
                    int digits = int.Parse(match.Groups[2].Value);
                    return $"CAST(ROUND({match.Groups[1].Value}, {digits}) AS DECIMAL(18, {digits}))";
                });

                // データベースコマンドを実行し、その結果をDataTableに読み込む
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    await _context.Database.OpenConnectionAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
                // DataTableの内容を出力ウィンドウに表示 ※※※確認用※※※
                Debug.WriteLine("DataTable Contents:");
                foreach (DataRow row in dataTable.Rows)
                {
                    var fields = row.ItemArray.Select(field => field.ToString());
                    Debug.WriteLine(string.Join(", ", fields));
                }
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // あきらめるがクリックされたとき: Lessons/Index
        [HttpPost]
        public async Task<IActionResult> GiveUp(int lessonId)
        {
            try
            {
                // idをもとにLessonを取得
                var lesson = await _context.Lesson
                    .FirstOrDefaultAsync(m => m.LessonId == lessonId);

                // データベースからlessonを取得できなかったとき
                if (lesson == null)
                {
                    TempData["Message"] = "E-027:データの取得に失敗しました";
                    return RedirectToAction("error", "Error");
                }

                return RedirectToAction("AnswerPage", new { id = lessonId });
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-028:データの取得に失敗しました";
                return RedirectToAction("error", "Error");
            }
        }

        //  Lessons/AnswerPageにGETアクセスがあったとき
        [HttpGet]
        public async Task<IActionResult> AnswerPage(int? id)
        {
            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                TempData["Message"] = "E-029:データの取得に失敗しました";
                return RedirectToAction("error", "Error");
            }

            // ViewModelのインスタンス生成
            var viewModel = new LessonViewModel();

            try
            {
                // idをもとにLessonを取得
                viewModel.SelectedLesson = await _context.Lesson.FirstOrDefaultAsync(m => m.LessonId == id);

                // Lessonがnullの場合はNotFoundを返す
                if (viewModel.SelectedLesson == null)
                {
                    TempData["Message"] = "E-030:データの取得に失敗しました";
                    return RedirectToAction("error", "Error");
                }

                // LessonのAnswerカラムからSQL文を抽出
                string pattern = @"```\s*\r?\n([\s\S]*?)\r?\n```";
                string extractionAnswer = Regex.Match(viewModel.SelectedLesson.Answer, pattern).Groups[1].Value;
                string lessonAnswer = AddUnicodePrefixToLiterals(extractionAnswer);
                string formattedLessonAnswer = Regex.Replace(lessonAnswer, @"\s+", " ");

                // LINQを使用してSQL文を実行し、結果をDataTableに格納
                DataTable lessonDataTable = await GetDataTableAsync(formattedLessonAnswer);
                viewModel.LessonDataTable = lessonDataTable;

                // マークダウンの内容をHTMLに変換
                MarkdownConverter.ConvertMarkdownToHtml(viewModel.SelectedLesson);

                // 全Lessonを取得
                viewModel.AllLessons = await _context.Lesson.ToListAsync();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-031:" + ex.Message;
                return RedirectToAction("error", "Error");
            }            
        }

        // 回答ページの部分ビュー作成メソッド
        public async Task<IActionResult> GetAnswer(int id)
        {
            try
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
            catch (Exception ex) 
            {
                return NotFound();
            }            
        }

        // 「ランダム練習問題」がクリックされたとき
        [HttpGet]
        public IActionResult Practice()
        {
            // セッションから前回のLessonIdを取得
            int lastLessonId = HttpContext.Session.GetInt32("LastLessonId") ?? 0;
            int last2LessonId = HttpContext.Session.GetInt32("Last2LessonId") ?? 0;

            // 全Lessonの数＋1
            int lessonCount = 14;

            // レッスンidをランダム生成
            Random randomPractice = new Random();
            int randomLessonId = randomPractice.Next(1, lessonCount);

            // 前回のLessonIdと同じ場合は再度生成
            while (randomLessonId == lastLessonId || randomLessonId == last2LessonId)
            {
                randomLessonId = randomPractice.Next(1, lessonCount);
            }

            // 生成したLessonIdをセッションに保存
            HttpContext.Session.SetInt32("LastLessonId", randomLessonId);
            HttpContext.Session.SetInt32("Last2LessonId", lastLessonId);

            // IndexのGetメソッドへidを送付
            return RedirectToAction("Index", new { id = randomLessonId });
        }
    }
}


