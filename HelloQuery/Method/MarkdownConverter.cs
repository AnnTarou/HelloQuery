using HelloQuery.Models;
using Markdig;

namespace HelloQuery.Method
{
    static public class MarkdownConverter
    {
        public static void ConvertMarkdownToHtml(Lesson selectedLesson)
        {
            selectedLesson.Description = Markdown.ToHtml(selectedLesson.Description);
            selectedLesson.Question = Markdown.ToHtml(selectedLesson.Question);
            selectedLesson.Hint = Markdown.ToHtml(selectedLesson.Hint);
            selectedLesson.Answer = Markdown.ToHtml(selectedLesson.Answer);
            selectedLesson.Reference = Markdown.ToHtml(selectedLesson.Reference);
        }
    }
}
