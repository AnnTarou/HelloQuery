using HelloQuery.Data;
using HelloQuery.Models;
using HelloQuery.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            return View(lesson);
        }

        // 「苦手リストに追加」が押されたとき　/　UserLessons/Create：POSTメソッド
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

            if (loginUser == null)
            {
                return NotFound();
            }

            // UserLessonのインスタンスを生成
            UserLesson userLesson = new UserLesson()
            {
                UserId = loginUser.UserId,
                LessonId = (int)lessonId
            };

            // UserLessonをコンテキストに追加
            _context.Add(userLesson);

            await _context.SaveChangesAsync();

            // UserLessonのIndexにリダイレクト
            return RedirectToAction("Index");
        }

        // 苦手リストの「削除」ボタンがおされたとき　UserLessons/Delete：POSTメソッド
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
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
