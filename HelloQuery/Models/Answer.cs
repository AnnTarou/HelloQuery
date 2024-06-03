namespace HelloQuery.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }

        // Lessonの外部キー
        public int LessonId { get; set; }

        // 有効な解答
        public string ValidAnswers { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
