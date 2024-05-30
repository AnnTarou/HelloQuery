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

                // Booksテーブルのシード追加 ※データベースにレコードがある場合、シード初期化子が返されレコードが追加されない
                if (!context.Book.Any())
                {
                    var books = new Book[]
                    {
                        new Book{ Title = "こころ",
                                  Author =  "夏目漱石",
                                  PublicationDate =  new DateTime(1914, 9, 20),
                                  Pages =  308,
                                  Price  = 408 },

                        new Book{ Title = "人間失格",
                                  Author =  "太宰治",
                                  PublicationDate =  new DateTime(1948, 7, 25),
                                  Pages =  192,
                                  Price  = 308 },

                        new Book{ Title = "吾輩は猫である",
                                  Author =  "夏目漱石",
                                  PublicationDate =  new DateTime(1905, 10, 6),
                                  Pages =  592,
                                  Price  = 660 },

                        new Book{ Title = "ドグラ・マグラ",
                                  Author =  "夢野久作",
                                  PublicationDate =  new DateTime(1935, 1, 15),
                                  Pages =  679,
                                  Price  = 1276 },

                        new Book{ Title = "羅生門",
                                  Author =  "芥川龍之介",
                                  PublicationDate =  new DateTime(1917, 5, 23),
                                  Pages =  24,
                                  Price  = 440 },

                        new Book{ Title = "銀河鉄道の夜",
                                  Author =  "宮沢賢治",
                                  PublicationDate =  new DateTime(1941, 12, 20),
                                  Pages =  102,
                                  Price  = 1800 },

                        new Book{ Title = "走れメロス",
                                  Author =  "太宰治",
                                  PublicationDate =  new DateTime(1940, 6, 15),
                                  Pages =  216,
                                  Price  = 660 },

                        new Book{ Title = "坊っちゃん",
                                  Author =  "夏目漱石",
                                  PublicationDate =  new DateTime(1907, 1, 1),
                                  Pages =  256,
                                  Price  = 770 },

                        new Book{ Title = "蜘蛛の糸",
                                  Author =  "芥川龍之介",
                                  PublicationDate =  new DateTime(1919, 1, 15),
                                  Pages =  36,
                                  Price  = 1600 },

                        new Book{ Title = "斜陽",
                                  Author =  "太宰治",
                                  PublicationDate =  new DateTime(1947, 12, 15),
                                  Pages =  256,
                                  Price  = 374 }
                    };
                    context.Book.AddRange(books);
                    context.SaveChanges();
                }

                // Lessonテーブルのシード追加
                if (!context.Lesson.Any())
                {
                    var lessons = new Lesson[]
                    {
                        new Lesson{ Category = "基本的な検索",
                                    Title =  "SELECT文とFROM句",
                                    Description = "このページでは、SQLにおける基本中の基本であるSELECT文とFROM句について学びます。\r\n### SELECT文とFROM句について\r\nSELECT文は、データベースから取得したい列（カラム）を指定するための命令です。\r\nFROM句は、データを取得するテーブルの名前を指定します。\r\nカラムが複数ある場合は、カンマ（`,`）を使用して列挙します。\r\n\r\n構文\r\n```\r\nSELECT カラム1, カラム2, ... FROM テーブル名;\r\n```\r\n上記の例では、`テーブル名`から`カラム1`、`カラム2`などの列を取得します。\r\n",
                                    Question = "本のタイトルと著者の一覧を取得してください。\r\n",
                                    Hint = "- タイトルと著者のカラムを取得するには `SELECT` を使用します。\r\n- データを取得するテーブルを指定するには `FROM` を使用します。\r\n",
                                    Answer = "```\r\nSELECT title, author\r\n  FROM books;\r\n```\r\n",
                                    Reference = "- `SELECT title, author`: タイトルと著者を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "WHERE句 / AS",
                                    Description = "このページでは、データを絞り込むためのWHERE句と、カラムやテーブルに別名を付けるためのASについて学びます。\r\n### WHERE句について\r\nWHERE句は、SELECT文などで取得するデータから、特定の条件に一致する行を選択するために使用します。\r\n具体的な条件を指定することで、取得したいデータを絞り込むことができます。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 WHERE 条件式;\r\n```\r\n上記の例では、`テーブル名`から`条件式`を満たす`カラム`のデータを取得します。\r\n例えば、`WHERE price > 100`のようにして、価格が100より大きい商品を取得します。\r\n\r\n### ASについて\r\nASは、SQLクエリ内でカラムやテーブルに一時的な別名を付けるために使用します。\r\nこの別名を使用することで、取得したデータの列に対して分かりやすい名前を付けることができます。\r\n\r\n構文\r\n```\r\nSELECT カラム AS 別名 FROM テーブル名;\r\n```\r\n上記の例では、`カラム`に`別名`を付けて、取得されたデータを表現します。\r\n",
                                    Question = "著者が「夏目漱石」の作品のタイトルと出版日を取得してください。\r\n※出力する項目名は「タイトル」と「出版日」\r\n",
                                    Hint = "- タイトルと出版日を取得するには `SELECT` を使用します。\r\n- 別名を付けるには `AS` を使用します。\r\n- 著者を指定するには、 `WHERE` を使用します。\r\n",
                                    Answer = "```\r\nSELECT title AS \"タイトル\", publication_date AS \"出版日\"\r\n  FROM books\r\n WHERE author = '夏目漱石';\r\n```\r\n",
                                    Reference = "- `SELECT title AS \"タイトル\", publication_date AS \"出版日\"`: タイトルと出版日を取得し、それぞれに「タイトル」と「出版日」という別名を付けます。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `WHERE author = '夏目漱石'`: 著者が「夏目漱石」の作品を指定します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "DISTINCT",
                                    Description = "このページでは、重複行を除外するためのDISTINCTについて学びます。\r\n### DISTINCTについて\r\nDISTINCTは、取得するデータから重複行を除外し、一意の値のみを取得するために使用されます。\r\n- 重複行の削除：特定のカラムの組み合わせで重複する行を一つにまとめます。\r\n- 一意な値の取得：特定のカラムに存在する一意な値を取得するのに使われます。\r\n\r\n構文\r\n```\r\nSELECT DISTINCT カラム FROM テーブル名;\r\n```\r\n上記の例では、`テーブル名`から重複を除外した`カラム`のデータを取得します。\r\n",
                                    Question = "著者の一覧を、重複行を削除して取得してください。\r\n",
                                    Hint = "- 著者を取得するには `SELECT` を使用します。\r\n- 重複を削除するには `DISTINCT` を使用します。\r\n",
                                    Answer = "```\r\nSELECT DISTINCT author\r\n  FROM books;\r\n```\r\n",
                                    Reference = "- `SELECT DISTINCT author`: 著者の重複を削除して取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "ORDER BY句 / ASC・DESC",
                                    Description = "このページでは、取得したデータを特定の条件で並べ替えるためのORDER BY句と、並べ替えの順序を指定するASC・DESCについて学びます。\r\n### ORDER BY句について\r\nORDER BY句は、SELECT文で取得したデータを、指定したカラムの値に基づいて並べ替えるために使用します。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 ORDER BY カラム;\r\n```\r\n上記の例では、`テーブル名`から取得した`カラム`のデータを、`カラム`の値に基づいて昇順に並べ替えます。\r\n\r\n### ASC・DESCについて\r\nASCは昇順、DESCは降順を表します。ORDER BY句と共に使用し、並べ替えの順序を指定します。\r\n- ASC（昇順）：値が小さいものから大きいもの、またはアルファベット順に並べます。これはデフォルトの並べ替え方法です。\r\n- DESC（降順）：値が大きいものから小さいもの、または逆アルファベット順に並べます。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 ORDER BY カラム ASC;\r\n```\r\n上記の式では、`テーブル名`から取得した`カラム`のデータを、`カラム`の値に基づいて昇順に並べ替えます。\r\n",
                                    Question = "タイトル、著者、出版日の一覧を出版日が新しい順（降順）に並べてください。\r\n",
                                    Hint = "- タイトル、著者、出版日を取得するには `SELECT` を使用します。\r\n- データを並べ替えるには `ORDER BY` を使用します。\r\n- 出版日を新しい順（降順）にするには `DESC` を使用します。",
                                    Answer = "```\r\nSELECT title, author, publication_date\r\n  FROM books\r\n ORDER BY publication_date DESC;\r\n```\r\n",
                                    Reference = "- `SELECT title, author, publication_date`: タイトル、著者、出版日を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `ORDER BY publication_date DESC`: 出版日が新しい順（降順）に並べ替えます。\r\n" }
                    };
                    context.Lesson.AddRange(lessons);
                    context.SaveChanges();
                }
            }
        }
    }
}