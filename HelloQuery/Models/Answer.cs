namespace HelloQuery.Models
{
    public class LessonAnswer
    {
        public int LessonAnswerId { get; set; }

        // Lessonの外部キー
        public int LessonId { get; set; }

        // 有効な解答
        public string ValidAnswer { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
