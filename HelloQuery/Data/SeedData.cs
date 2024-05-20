using HelloQuery.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloQuery.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new HelloQueryContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<HelloQueryContext>>()))
            {
                // データベースにレコードがある場合、シード初期化子が返されレコードが追加されません。
                if (context.Lesson.Any())
                {
                    return;   // DB has been seeded
                }

                var lessons = new Lesson[]
                {
                new Lesson{ Category = "基本の検索",
                            Title =  "SELECT文とFROM句",
                            Description = "データを取得するための基本的な構文",
                            Question = "タイトルと著者の一覧を取得してください",
                            Hint = "ヒントだよ～",
                            Answer = "SELECT title, author FROM books;",
                            Reference = "解説だよ～" },

                new Lesson{ Category = "基本の検索",
                            Title =  "sdgdsfht句",
                            Description = "デsazaghktyktの基本的な構文",
                            Question = "タイトルzdg得してください",
                            Hint = "ヒンhzdfhだよ～",
                            Answer = "SELECT tdsf hjtyoks;",
                            Reference = "解説fdgだよ～" }
                };
                context.Lesson.AddRange(lessons);
                context.SaveChanges();

                var users = new User[]
                {
                new User{ UserName = "オザタロウ",
                            Email =  "zakizaki@gmail.com",
                            Password = "zakizaki0401" }
                };
                context.User.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
