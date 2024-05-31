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

            // ユーザーの入力クエリにシングルクォートがある場合はNプレフィックスを追加
            string userAnswer = AddUnicodePrefixToLiterals(answer);

            // 整形後の文字列を出力ウィンドウに表示 ※※※確認用※※※
            Debug.WriteLine("Formatted SQL query:");
            Debug.WriteLine(lessonAnswer);
            Debug.WriteLine("User Formatted SQL query:");
            Debug.WriteLine(userAnswer);

            // ユーザーの入力内容と整形後のSQL文から改行文字を取り除く
            string formattedUserAnswer = Regex.Replace(userAnswer, @"\s+", " ");
            string formattedLessonAnswer = Regex.Replace(lessonAnswer, @"\s+", " ");

            try
            {
                // 改行を取り除いた文字列を使ってデータを取得し、DataTableに格納
                DataTable lessonDataTable = await GetDataTableAsync(formattedLessonAnswer);
                DataTable userDataTable = await GetDataTableAsync(formattedUserAnswer);

                // DataTableの内容を比較
                bool isCorrect = CompareDataTables(lessonDataTable, userDataTable);

                // 比較結果が正解の場合はAnswerページへ遷移、不正解の場合は同じページに留まる
                if (isCorrect)
                {
                    return RedirectToAction("AnswerPage", new { id = lessonId });
                }
                else
                {
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

        // Answerカラムとユーザーが入力したSQL文をそれぞれ実行し、結果をDataTableに格納
        private async Task<DataTable> GetDataTableAsync(string sql)
        {
            // データベースに接続（usingにすることでリソースを自動的に開放）
            using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;"))
            {
                // 非同期的にデータベース接続をオープン
                await connection.OpenAsync();
                // 指定されたSQLクエリを実行
                using (var command = new SqlCommand(sql, connection))
                {
                    // SQLクエリの結果をDataTableに格納
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        // データベースから取得したデータをDataTableに読み込む
                        adapter.Fill(dataTable);

                        // DataTableの内容を出力ウィンドウに表示 ※※※確認用※※※
                        Debug.WriteLine("DataTable Contents:");
                        foreach (DataRow row in dataTable.Rows)
                        {
                            var fields = row.ItemArray.Select(field => field.ToString());
                            Debug.WriteLine(string.Join(", ", fields));
                        }

                        return dataTable;
                    }
                }
            }
        }

        // 2つのDataTableの内容を比較し、一致していればtrueを返す
        private bool CompareDataTables(DataTable dt1, DataTable dt2)
        {
            // 両方のDataTableの行数と列数が一致していなければ処理を抜ける
            if (dt1.Rows.Count != dt2.Rows.Count || dt1.Columns.Count != dt2.Columns.Count)
                return false;

            // カラム名を比較して一致していなければ処理を抜ける
            for (int i = 0; i < dt1.Columns.Count; i++)
            {
                if (dt1.Columns[i].ColumnName != dt2.Columns[i].ColumnName)
                    return false;
            }

            // 一致している場合は各セルの値を比較
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    if (!Equals(dt1.Rows[i][j], dt2.Rows[i][j]))
                        return false;
                }
            }
            return true;
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


