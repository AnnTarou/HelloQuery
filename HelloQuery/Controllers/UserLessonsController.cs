using HelloQuery.Data;
using HelloQuery.Filter;
using HelloQuery.Method;
using HelloQuery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloQuery.Controllers
{
    // ログインしているかチェックするフィルター追加
    [SessionCheckFilter]
    public class UserLessonsController : Controller
    {
        private readonly HelloQueryContext _context;

        public UserLessonsController(HelloQueryContext context)
        {
            _context = context;
        }

        // UserLessons/Index:GETアクセスあったとき
        public async Task<IActionResult> Index()
        {
            // 現在ログインしているユーザーを取得
            User loginUser = (User)HttpContext.Items["User"];

            // ログインしていない場合はNotFoundを返す
            if (loginUser == null)
            {
                return NotFound();
            }

            // ログインしているユーザーのUserLessonを取得
            var userLessons = await _context.UserLesson
                .Include(ul => ul.Lesson)
                .Where(ul => ul.UserId == loginUser.UserId)
                .ToListAsync();

            return View(userLessons);
        }

        // 「詳細」ボタンがおされたとき　UserLessons/Details：GETアクセスあったとき
        public async Task<IActionResult> Details(int? id)
        {
            //LessonIdの引数が入ってこなかった場合エラーページ表示
            if (id == null)
            {
                return NotFound();
            }

            // ログインしているユーザーを取得
            User loginUser = (User)HttpContext.Items["User"];

            // ログインしていない場合はNotFoundを返す
            if (loginUser == null)
            {
                return NotFound();
            }

            var lesson = await _context.UserLesson
                .Include(u => u.Lesson)
                .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId && m.LessonId == id);

            // ユーザーが直接このアドレスにアクセスした場合はNotFoundを返す
            if (lesson == null)
            {
                return NotFound();
            }

            // マークダウンの内容をHTMLに変換
            MarkdownConverter.ConvertMarkdownToHtml(lesson.Lesson);

            return View(lesson);
        }

        // 「苦手リストに保存」が押されたとき　/　UserLessons/Create：POSTメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? lessonId)
        {
            // LessonIdがnullの場合はNotFoundを返す
            if (lessonId == null)
            {
                return NotFound();
            }

            // 現在ログインしているユーザーを取得
            User loginUser = (User)HttpContext.Items["User"];

            // ログインユーザーがnullの場合はログイン画面にリダイレクト
            if (loginUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // 現在のユーザーがすでにlessonIdを持っているかどうかを確認
            var existingUserLesson = await _context.UserLesson
                .FirstOrDefaultAsync(ul => ul.UserId == loginUser.UserId && ul.LessonId == lessonId);

            // 現在のユーザーがすでにlessonIdを持っている場合は何もしない
            if (existingUserLesson != null)
            {
                return RedirectToAction("AnswerPage","Lessons", new {id = lessonId});
            }

            // UserLessonのインスタンスを生成
            UserLesson userLesson = new UserLesson()
            {
                UserId = loginUser.UserId,
                LessonId = (int)lessonId
            };

            _context.Add(userLesson);

            // データベースの変更を保存
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "UserLessons");
        }

        // 苦手リストの「削除」ボタンがおされたとき
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            var userLesson = await _context.UserLesson.FindAsync(id);

            // UserLessonがDBに見つかった場合は削除
            if (userLesson != null)
            {
                _context.UserLesson.Remove(userLesson);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserLessonExists(int id)
        {
            return _context.UserLesson.Any(e => e.UserLessonId == id);
        }
    }
}
