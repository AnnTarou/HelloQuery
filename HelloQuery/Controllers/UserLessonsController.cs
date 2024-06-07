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

            // データベースへ接続
            try
            {
                // ログインしているユーザーのUserLessonを取得
                var userLessons = await _context.UserLesson
                    .Include(ul => ul.Lesson)
                    .Where(ul => ul.UserId == loginUser.UserId)
                    .ToListAsync();

                return View(userLessons);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-002:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        // 「詳細」ボタンがおされたとき　UserLessons/Details：GETアクセスあったとき
        public async Task<IActionResult> Details(int? id)
        {
            //LessonIdの引数が入ってこなかった場合エラーページ表示
            if (id == null)
            {
                TempData["Message"] = "E-003:もう一度ログインしてください";
                return RedirectToAction("error", "Error");
            }

            // ログインしているユーザーを取得
            User loginUser = (User)HttpContext.Items["User"];

            // データベースにアクセス
            try
            {
                var lesson = await _context.UserLesson
               .Include(u => u.Lesson)
               .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId && m.LessonId == id);

                // lessonがnullとなった場合
                if (lesson == null)
                {
                    TempData["Message"] = "E-004:もう一度ログインしてください";
                    return RedirectToAction("error", "Error");
                }

                // マークダウンの内容をHTMLに変換
                MarkdownConverter.ConvertMarkdownToHtml(lesson.Lesson);

                return View(lesson);
            }
            catch(Exception ex) 
            {
                TempData["Message"] = "E-005:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        // 「苦手リストに保存」が押されたとき　/　UserLessons/Create：POSTメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? lessonId)
        {
            // LessonIdがnullの場合
            if (lessonId == null)
            {
                TempData["Message"] = "E-006:もう一度ログインしてください:";
                return RedirectToAction("error", "Error");
            }

            // 現在ログインしているユーザーを取得
            User loginUser = (User)HttpContext.Items["User"];

            // データベースに接続
            try
            {
                // 現在のユーザーがすでにlessonIdを持っているかどうかを確認
                var existingUserLesson = await _context.UserLesson
                    .FirstOrDefaultAsync(ul => ul.UserId == loginUser.UserId && ul.LessonId == lessonId);

                // 現在のユーザーがすでにlessonIdを持っている場合は何もしない
                if (existingUserLesson != null)
                {
                    return RedirectToAction("AnswerPage", "Lessons", new { id = lessonId });
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
            catch (Exception ex)
            {
                TempData["Message"] = "E-007:"  + ex.Message;
                return RedirectToAction("error", "Error");
            }            
        }

        // 苦手リストの「削除」ボタンがおされたとき
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            // idが入ってこなかった場合
            if (id == null)
            {
                TempData["Message"] = "E-008:もう一度ログインしてください:";
                return RedirectToAction("error", "Error");
            }
            // データベースに接続
            try
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
            catch(Exception ex) 
            {
                TempData["Message"] = "E-009:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        private bool UserLessonExists(int id)
        {
            return _context.UserLesson.Any(e => e.UserLessonId == id);
        }
    }
}