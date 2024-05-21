using HelloQuery.Data;
using HelloQuery.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HelloQuery.Controllers
{
    public class UserLessonsController : Controller
    {
        private readonly HelloQueryContext _context;

        public UserLessonsController(HelloQueryContext context)
        {
            _context = context;
        }

        // GET: UserLessons
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

        // POST: UserLessons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserLessonId,LessonId,UserId")] UserLesson userLesson)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userLesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LessonId"] = new SelectList(_context.Lesson, "LessonId", "LessonId", userLesson.LessonId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", userLesson.UserId);
            return View(userLesson);
        }

        // GET: UserLessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userLesson = await _context.UserLesson.FindAsync(id);
            if (userLesson == null)
            {
                return NotFound();
            }
            ViewData["LessonId"] = new SelectList(_context.Lesson, "LessonId", "LessonId", userLesson.LessonId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", userLesson.UserId);
            return View(userLesson);
        }

        // POST: UserLessons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserLessonId,LessonId,UserId")] UserLesson userLesson)
        {
            if (id != userLesson.UserLessonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userLesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserLessonExists(userLesson.UserLessonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LessonId"] = new SelectList(_context.Lesson, "LessonId", "LessonId", userLesson.LessonId);
            ViewData["UserId"] = new SelectList(_context.User, "UserId", "Email", userLesson.UserId);
            return View(userLesson);
        }

        // GET: UserLessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: UserLessons/Delete/5
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
