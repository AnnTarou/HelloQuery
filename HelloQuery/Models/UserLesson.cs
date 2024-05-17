namespace HelloQuery.Models
{
    // 中間テーブル
    public class UserLesson
    {
        public int UserLessonId { get; set; }

        // Drillの外部キー
        public int LessonId { get; set; }

        // Userの外部キー
        public int UserId { get; set; }

        public Lesson Lesson { get; set; }

        public User User { get; set; }
    }
}
