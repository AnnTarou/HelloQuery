using HelloQuery.Data;
using HelloQuery.Models;
using HelloQuery.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.FlowAnalysis;

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
            var helloQueryContext = _context.UserLesson.Include(u => u.Lesson).Include(u => u.User);
            return View(await helloQueryContext.ToListAsync());
        }

        // GET: UserLessons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLesson = await _context.UserLesson
                .Include(u => u.Lesson)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserLessonId == id);
            if (userLesson == null)
            {
                return NotFound();
            }

            return View(userLesson);
        }

        // GET: UserLessons/Create
        public IActionResult Create()
        {
            ViewData["LessonId"] = new SelectList(_context.Lesson, "LessonId", "LessonId");
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email");
            return View();
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
                LessonId = (int)lessonId,
                User = loginUser,
                Lesson = await _context.Lesson.FindAsync(lessonId)
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userLesson = await _context.UserLesson.FindAsync(id);
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
