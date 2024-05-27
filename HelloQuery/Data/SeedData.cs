using Microsoft.EntityFrameworkCore;
using HelloQuery.Models;

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
                // ※※※このブロックは、本番環境で削除します※※※
                // Userテーブルのシード追加 ※データベースにレコードがある場合、シード初期化子が返されレコードが追加されない
                if (!context.User.Any())
                {
                    var users = new User[]
                    {
                        new User{ UserName = "アンタロウ",
                                  Email =  "Ann@gmail.com",
                                  Password = "AnnAnn0000" },

                        new User{ UserName = "オザタロウ",
                                  Email =  "zaki@gmail.com",
                                  Password = "zakizaki0000" },

                        new User{ UserName = "コベタロウ",
                                  Email =  "kobe@gmail.com",
                                  Password = "kobekobe0000" }
                    };
                    context.User.AddRange(users);
                    context.SaveChanges();
                }
                // ※※※ここまで※※※
                // Lessonテーブルのシード追加
                if (!context.Lesson.Any())
                {
                    var lessons = new Lesson[]
                    {
                        new Lesson{ Category = "基本的な検索",
                                    Title =  "SELECT文とFROM句",
                                    Description = "このページでは、SQLにおける基本中の基本であるSELECT文とFROM句について学びます。\r\n\r\n### SELECT文とFROM句について\r\nSELECT文は、データベースから取得したい列（カラム）を指定するための命令です。\r\nFROM句は、データを取得するテーブルの名前を指定します。\r\nカラムが複数ある場合は、カンマ（`,`）を使用して列挙します。\r\n\r\n構文\r\n```sql\r\nSELECT カラム1, カラム2, ... FROM テーブル名;\r\n```\r\n上記の例では、`テーブル名`から`カラム1`、`カラム2`などの列を取得します。",
                                    Question = "本のタイトルと著者の一覧を取得してください。",
                                    Hint = "- タイトルと著者のカラムを取得するには `SELECT` を使用します。\r\n- データを取得するテーブルを指定するには `FROM` を使用します。",
                                    Answer = "```SELECT title, author\r\nFROM books;\r\n```",
                                    Reference = "- `SELECT title, author`: タイトルと著者を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。" },

                        new Lesson{ Category = "基本の検索",
                                    Title =  "WHERE句",
                                    Description = "WHERE句を使用することで、特定の条件に基づいてデータを絞り込む。\r\nASでカラム名に別名を付けることができる。",
                                    Question = "著者が夏目漱石の作品の、タイトルと出版日を取得してください。\r\n※AS句を使用して表示するカラム名を「タイトル」「出版日」に変更",
                                    Hint = "ヒントだよ～",
                                    Answer = "SELECT title AS \"タイトル\", publication_date AS \"出版日\" FROM books WHERE author = '夏目漱石';",
                                    Reference = "解説だよ～" },

                        new Lesson{ Category = "基本の検索",
                                    Title =  "DISTINCT",
                                    Description = "重複を省く",
                                    Question = "著者一覧を、重複行を削除して取得してください",
                                    Hint = "ヒントだよ～",
                                    Answer = "SELECT DISTINCT author FROM books;",
                                    Reference = "解説だよ～" },

                        new Lesson{ Category = "基本の検索",
                                    Title =  "ORDER BY句",
                                    Description = "取得したデータを特定の列で並べ替える。",
                                    Question = "書籍、著者、出版日の一覧を出版日順に並べてください",
                                    Hint = "ヒントだよ～",
                                    Answer = "SELECT title, author, publication_date\r\n  FROM books\r\n ORDER BY publication_date;",
                                    Reference = "解説だよ～" }
                    };
                    context.Lesson.AddRange(lessons);
                    context.SaveChanges();
                }
            }
        }
    }
}