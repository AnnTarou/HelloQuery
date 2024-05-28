using HelloQuery.Data;
using HelloQuery.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloQuery.Controllers
{
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
            var lessons = await _context.Lesson.ToListAsync();
            Lesson selectedLesson;
            LessonViewModel viewModel = new LessonViewModel();

            // idがnullとき最初のLessonを選択
            if (id == null)
            {
                selectedLesson = lessons.FirstOrDefault(m => m.LessonId == 1);

                // DiscriptionをHTMLに変換
                selectedLesson.Description = Markdown.ToHtml(selectedLesson.Description);
            }
            // idがnullでない場合、選択されたLessonを取得
            else
            {
                selectedLesson = lessons.FirstOrDefault(m => m.LessonId == id);

                // DiscriptionをHTMLに変換
                selectedLesson.Description = Markdown.ToHtml(selectedLesson.Description);
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
            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            // DiscriptionをHTMLに変換
            lesson.Description = Markdown.ToHtml(lesson.Description);

            LessonViewModel viewModel = new LessonViewModel();
            viewModel.SelectedLesson = lesson;
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

            if (lesson == null)
            {
                return NotFound();
            }

            // 入力文字を大文字に変換
            if (answer == null || answer == "")
            {
                return RedirectToAction("Index", new { id = lessonId });
            }
            string conversionAnswer = answer.ToUpper();

            if (conversionAnswer == lesson.Answer.ToUpper())
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

        // GETメソッド： Lessons/AnswerPage
        [HttpGet]
        public async Task<IActionResult> AnswerPage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // idをもとにLessonを取得
            var selectedLesson = await _context.Lesson.FirstOrDefaultAsync(m => m.LessonId == id);

            // 全Lessonを取得
            var allLessons = await _context.Lesson.ToListAsync();

            if (selectedLesson == null)
            {
                return NotFound();
            }

            var viewModel = new LessonViewModel
            {
                SelectedLesson = selectedLesson,
                AllLessons = allLessons
            };

            return View(viewModel);
        }

        // 回答ページの部分ビュー作成メソッド
        public async Task<IActionResult> GetAnswer(int id)
        {
            // JSから渡されたidをもとに、選択されたLessonを取得
            var lesson = await _context.Lesson
                .FirstOrDefaultAsync(m => m.LessonId == id);

            if (lesson == null)
            {
                return NotFound();
            }

            return PartialView("_AnswerPartial", lesson);
        }
    }
}


