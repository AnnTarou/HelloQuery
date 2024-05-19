namespace HelloQuery.Models
{
    public class LessonViewModel
    {
        // 選択されたLesson
        public Lesson SelectedLesson { get; set; }

        // 全Lessonを保持するリスト
        public List<Lesson> AllLessons { get; set; }
    }
}
