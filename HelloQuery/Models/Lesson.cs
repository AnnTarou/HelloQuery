namespace HelloQuery.Models
{
    public class Lesson
    {
        public int LessonId { get; set; }

        // メインカテゴリ
        public string Category { get; set; }

        // 問題の題名
        public string Title { get; set; }

        // 説明
        public string Description { get; set; }

        // 問題
        public string Question { get; set; }

        // ヒント
        public string Hint { get; set; }

        // 解答
        public string Answer { get; set; }

        // 解説
        public string Reference { get; set; }

        // ナビゲーションプロパティ
        public List<UserLesson> UserLesson { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
