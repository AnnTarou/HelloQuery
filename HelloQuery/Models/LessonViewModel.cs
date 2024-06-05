using System.Data;

namespace HelloQuery.Models
{
    public class LessonViewModel
    {
        // 選択されたLesson
        public Lesson SelectedLesson { get; set; }

        // 全Lessonを保持するリスト
        public List<Lesson>? AllLessons { get; set; }

        // Booksテーブルから取得した本のリストを保持
        public List<Book> Books { get; set; }

        // DataTableの追加
        public DataTable LessonDataTable { get; set; }
    }
}
