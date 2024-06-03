namespace HelloQuery.Models
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public int LessonId { get; set; }
        public string ValidAnswers { get; set; }

        public virtual Lesson Lesson { get; set; }
    }
}
