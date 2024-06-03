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
                                    Description = "このページでは、SQLにおける基本中の基本であるSELECT文とFROM句について学びます。\r\n### SELECT文とFROM句について\r\nSELECT文は、データベースから取得したい列（カラム）を指定するための命令です。\r\nFROM句は、データを取得するテーブルの名前を指定します。\r\nカラムが複数ある場合は、カンマ（`,`）を使用して列挙します。\r\n\r\n構文\r\n```\r\nSELECT カラム1, カラム2, ... FROM テーブル名;\r\n```\r\n上記の例では、`テーブル名`から`カラム1`、`カラム2`などの列を取得します。",
                                    Question = "本のタイトルと著者の一覧を取得してください。",
                                    Hint = "- タイトルと著者のカラムを取得するには `SELECT` を使用します。\r\n- データを取得するテーブルを指定するには `FROM` を使用します。",
                                    Answer = "```\r\nSELECT title, author\r\n  FROM books;\r\n```",
                                    Reference = "- `SELECT title, author`: タイトルと著者を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "WHERE句 / AS",
                                    Description = "このページでは、データを絞り込むためのWHERE句と、カラムやテーブルに別名を付けるためのASについて学びます。\r\n### WHERE句について\r\nWHERE句は、SELECT文などで取得するデータから、特定の条件に一致する行を選択するために使用します。\r\n具体的な条件を指定することで、取得したいデータを絞り込むことができます。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 WHERE 条件式;\r\n```\r\n上記の例では、`テーブル名`から`条件式`を満たす`カラム`のデータを取得します。\r\n例えば、`WHERE price > 100`のようにして、価格が100より大きい商品を取得します。\r\n\r\n### ASについて\r\nASは、SQLクエリ内でカラムやテーブルに一時的な別名を付けるために使用します。\r\nこの別名を使用することで、取得したデータの列に対して分かりやすい名前を付けることができます。\r\n\r\n構文\r\n```\r\nSELECT カラム AS 別名 FROM テーブル名;\r\n```\r\n上記の例では、`カラム`に`別名`を付けて、取得されたデータを表現します。",
                                    Question = "著者が「夏目漱石」の作品のタイトルと出版日を取得してください。\r\n※出力する項目名は「タイトル」と「出版日」",
                                    Hint = "- タイトルと出版日を取得するには `SELECT` を使用します。\r\n- 別名を付けるには `AS` を使用します。\r\n- 著者を指定するには、 `WHERE` を使用します。",
                                    Answer = "```\r\nSELECT title AS \"タイトル\", publication_date AS \"出版日\"\r\n  FROM books\r\n WHERE author = '夏目漱石';\r\n```",
                                    Reference = "- `SELECT title AS \"タイトル\", publication_date AS \"出版日\"`: タイトルと出版日を取得し、それぞれに「タイトル」と「出版日」という別名を付けます。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `WHERE author = '夏目漱石'`: 著者が「夏目漱石」の作品を指定します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "DISTINCT",
                                    Description = "このページでは、重複行を除外するためのDISTINCTについて学びます。\r\n### DISTINCTについて\r\nDISTINCTは、取得するデータから重複行を除外し、一意の値のみを取得するために使用されます。\r\n- 重複行の削除：特定のカラムの組み合わせで重複する行を一つにまとめます。\r\n- 一意な値の取得：特定のカラムに存在する一意な値を取得するのに使われます。\r\n\r\n構文\r\n```\r\nSELECT DISTINCT カラム FROM テーブル名;\r\n```\r\n上記の例では、`テーブル名`から重複を除外した`カラム`のデータを取得します。",
                                    Question = "著者の一覧を、重複行を削除して取得してください。",
                                    Hint = "- 著者を取得するには `SELECT` を使用します。\r\n- 重複を削除するには `DISTINCT` を使用します。",
                                    Answer = "```\r\nSELECT DISTINCT author\r\n  FROM books;\r\n```",
                                    Reference = "- `SELECT DISTINCT author`: 著者の重複を削除して取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n" },

                        new Lesson{ Category = "基本的な検索",
                                    Title =  "ORDER BY句 / ASC・DESC",
                                    Description = "このページでは、取得したデータを特定の条件で並べ替えるためのORDER BY句と、並べ替えの順序を指定するASC・DESCについて学びます。\r\n### ORDER BY句について\r\nORDER BY句は、SELECT文で取得したデータを、指定したカラムの値に基づいて並べ替えるために使用します。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 ORDER BY カラム;\r\n```\r\n上記の例では、`テーブル名`から取得した`カラム`のデータを、`カラム`の値に基づいて昇順に並べ替えます。\r\n\r\n### ASC・DESCについて\r\nASCは昇順、DESCは降順を表します。ORDER BY句と共に使用し、並べ替えの順序を指定します。\r\n- ASC（昇順）：値が小さいものから大きいもの、またはアルファベット順に並べます。これはデフォルトの並べ替え方法です。\r\n- DESC（降順）：値が大きいものから小さいもの、または逆アルファベット順に並べます。\r\n\r\n構文\r\n```\r\nSELECT カラム FROM テーブル名 ORDER BY カラム ASC;\r\n```\r\n上記の式では、`テーブル名`から取得した`カラム`のデータを、`カラム`の値に基づいて昇順に並べ替えます。",
                                    Question = "タイトル、著者、出版日の一覧を出版日が新しい順（降順）に並べてください。",
                                    Hint = "- タイトル、著者、出版日を取得するには `SELECT` を使用します。\r\n- データを並べ替えるには `ORDER BY` を使用します。\r\n- 出版日を新しい順（降順）にするには `DESC` を使用します。",
                                    Answer = "```\r\nSELECT title, author, publication_date\r\n  FROM books\r\n ORDER BY publication_date DESC;\r\n```",
                                    Reference = "- `SELECT title, author, publication_date`: タイトル、著者、出版日を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `ORDER BY publication_date DESC`: 出版日が新しい順（降順）に並べ替えます。\r\n" },

                        new Lesson{ Category = "基本的な演算",
                                    Title =  "算術演算子（四則演算） / ROUND関数",
                                    Description = "このページでは、SQLクエリで幅広く活用されている四則演算と、四捨五入の関数について学びます。\r\n### 算術演算子（四則演算）について\r\n算術演算子である四則演算（加算：`+`,減算：`-`,乗算：`*`,除算：`/`)は、データベース内の数値型の列に対して使用されます。\r\n具体的には、売上データの集計や数量と単価の掛け算など、様々な場面で活用されます。\r\n演算子の優先順位は数学的な概念に従い、括弧で囲まれた部分が最初に計算されます。\r\n\r\n構文\r\n```sql\r\nSELECT カラム1 + カラム2 FROM テーブル名;\r\n```\r\n上記の式では、`カラム1`と`カラム2`の値を足し合わせた結果を返します。\r\n### ROUND関数について\r\nROUNDは、数値を指定された桁数に丸めるための関数です。主に小数点以下の桁数を調整するために使用されますが、整数も丸めることができます。この関数は様々なプログラミング言語やデータベースクエリ言語で利用されており、一般的な数学的な丸め操作を行います。\r\n\r\n構文\r\n```sql\r\nROUND (丸め対象となる数値, 丸めの桁数)\r\n```\r\n例えば、`ROUND(3.14159, 2)`は、3.14159を小数点以下2桁に丸め、3.14を返します。\r\n注意：丸めの基準（5を繰り上げるか、切り捨てるかなど）は使用する言語や型、データベースにより異なる場合があるため、公式のドキュメントを参照してください。",
                                    Question = "タイトル、著者名、本の税抜き価格（10％）を取得してください。\r\n※税抜き価格は整数丸め\r\n※出力する項目名は「タイトル」、「著者」、「税抜き価格」",
                                    Hint = "- タイトルと著者名を取得するには `SELECT` を使用します。\r\n- 項目名を変更するためには `AS` を使用します。\r\n- 税抜き価格を計算するために、価格を 1.1 で割り、`ROUND` を使用して四捨五入します。",
                                    Answer = "```\r\nSELECT title AS \"タイトル\", author AS \"著者\",\r\n\t\t\t ROUND(price / 1.1, 0) AS \"税抜き価格\"\r\n  FROM books;\r\n```",
                                    Reference = "- `SELECT title AS \"タイトル\", author AS \"著者\",`: タイトルと著者名を取得し、それぞれに「タイトル」、「著者」という別名を付けます。\r\n- `ROUND(price / 1.1, 0) AS \"税抜き価格\"`: 価格を 1.1 で割り、四捨五入して税抜き価格を計算し、それに「税抜き価格」という別名を付けます。\r\n- `FROM books`: 書籍テーブルからデータを取得します。" },

                        new Lesson{ Category = "基本的な演算",
                                    Title =  "比較演算子",
                                    Description = "このページでは、条件式を記述する際に使用する比較演算子について学びます。\r\n### 比較演算子について\r\n比較演算子は、WHERE句で使用され、値を比較するために使用します。基本的な比較演算子には、等しい（`=`）、等しくない（`<>`）、より大きい（`>`）、未満（`<`）、以上（`>=`）、以下（`<=`）があります。\r\n\r\n構文\r\n```sql\r\nSELECT カラム FROM テーブル名 WHERE カラム > 10;\r\n```\r\n上記の式では、`テーブル名`から`カラム`の値が10より大きいデータを取得します。",
                                    Question = "出版日が1940年1月1日以前のタイトル、著者、出版日を取得してください。",
                                    Hint = "- タイトル、著者名、出版日を取得するには `SELECT` を使用します。\r\n- 出版日が特定の日付の前か後かを比較するためには、比較演算子を使用します。",
                                    Answer = "```\r\nSELECT title, author, publication_date\r\n  FROM books\r\n WHERE publication_date <= '1907-01-01';\r\n```",
                                    Reference = "- `SELECT title, author, publication_date`: タイトル、著者名、出版日を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `WHERE publication_date <= '1970-01-01'`: 出版日が1907年1月1日以前の条件を指定します。" },

                        new Lesson{ Category = "基本的な演算",
                                    Title =  "論理演算子",
                                    Description = "このページでは、複数の条件式を組み合わせるための論理演算子について学びます。\r\n### 論理演算子について\r\n論理演算子は、複数の条件式を組み合わせるために使用します。基本的な論理演算子には、ANDとORがあります。\r\n- AND演算子：AND 演算子は、2つの条件が True のときだけ結果が True になり、それ以外（片方または両方が False）のときは、結果が False になります。\r\n- OR演算子：どちらか一方または両方の条件が True のとき結果が True になり、両方の条件が False のときだけ結果が False になります。\r\n- NOT演算子：入力の真理値を反転させます（True が False になり、False が True になります）。\r\n\r\n構文\r\n```sql\r\nSELECT カラム1 FROM テーブル名 WHERE 条件式1 AND 条件式2;\r\n```\r\n上記の式では、`テーブル名`から`条件式1`と`条件式2`の両方を満たすデータを取得します。",
                                    Question = "500円以下の「太宰治」の作品のタイトル、ページ数、価格を取得してください。",
                                    Hint = "- 複数の条件を結合するには `AND` を使用します。\r\n- 特定の著者の名前を検索するには `=` を使用します。",
                                    Answer = "```\r\nSELECT title, pages, price\r\n  FROM books\r\n WHERE author = '太宰治'\r\n   AND price <= 500;\r\n```",
                                    Reference = "- `SELECT title, pages, price`: タイトル、ページ数、価格を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `WHERE author = '太宰治' AND price <= 500`: 著者が「太宰治」であり、価格が500円以下の条件を指定します。" },

                        new Lesson{ Category = "基本的な演算",
                                    Title =  "BETWEEN述語",
                                    Description = "このページでは、ある範囲内の値を検索するためのBETWEENについて学びます。\r\n### BETWEENについて\r\nBETWEENは、ある値が2つの値の範囲内にあるデータを検索するために使用します。\r\n主に数値や日付の範囲指定などに用いられます。\r\n\r\n構文\r\n```sql\r\nSELECT カラム FROM テーブル名 WHERE カラム BETWEEN 10 AND 100;\r\n```\r\n上記の式では、`テーブル名`から`カラム`の値が`10`以上かつ`100`以下であるデータを取得します。\r\n補足：検索範囲に自身の値を含めない場合、例えば10より大きく100より小さい範囲を取得する際は、比較演算子を使用し、`WHERE カラム > 10 AND カラム < 100;` のように記述します。",
                                    Question = "500円以上、1,000円以下の本のタイトル、価格を取得してください。",
                                    Hint = "- 範囲内の値を検索するために `BETWEEN` を使用します。",
                                    Answer = "```\r\nSELECT title, price\r\n  FROM books\r\n WHERE price BETWEEN 500 AND 1000;\r\n```",
                                    Reference = "- `SELECT title, price`: タイトルと価格を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `WHERE price BETWEEN 500 AND 1000`: 価格が500円以上1000円以下の条件を指定します。" },

                        new Lesson{ Category = "基本的な関数",
                                    Title =  "COUNT関数",
                                    Description = "このページでは、データの件数を取得するためのCOUNT関数について学びます。\r\n### COUNT関数について\r\nCOUNT関数は、指定したカラムやテーブルのデータ件数を取得するために使用します。\r\n通常、グループごとの行数を計算するために使用されます。\r\n\r\n構文\r\n```sql\r\nSELECT COUNT(カラム) FROM テーブル名;\r\n```\r\n上記の式では、`テーブル名`の`カラム`に含まれる値の数を取得します。",
                                    Question = "重複を除外して、著者の数を取得してください。",
                                    Hint = "- 著者の数を取得するためには `COUNT` 関数を使用します。\r\n- 重複を除外するために `DISTINCT` を使用します。",
                                    Answer = "```\r\nSELECT COUNT(DISTINCT author)\r\n  FROM books;\r\n```",
                                    Reference = "- `COUNT(DISTINCT author)`: 著者の数をカウントします。`DISTINCT` を使用することで、重複する著者が複数回数えられるのを防ぎます。\r\n- `FROM books`: 書籍テーブルからデータを取得します。" },

                        new Lesson{ Category = "基本的な関数",
                                    Title =  "SUM関数 / AVG関数",
                                    Description = "このページでは、データの合計値と平均値を取得するためのSUM関数とAVG関数について学びます。\r\n### SUM関数について\r\nSUM関数は、指定したカラムの値の合計を取得するために使用します。\r\n\r\n構文\r\n```sql\r\nSELECT SUM(カラム) FROM テーブル名;\r\n```\r\n上記の式では、`テーブル名`の`カラム`に含まれる値の合計を取得します。\r\n### AVG関数について\r\nAVG関数は、指定したカラムの値の平均を取得するために使用します。\r\n\r\n構文\r\n```sql\r\nSELECT AVG(カラム) FROM テーブル名;\r\n```\r\n上記の式では、`テーブル名`の`カラム`に含まれる値の平均を取得します。",
                                    Question = "全ての本の合計金額と平均金額を取得してください。",
                                    Hint = "- 本の合計金額を取得するためには `SUM` 関数を使用します。\r\n- 本の平均金額を取得するためには`AVG`関数を使用します。",
                                    Answer = "```\r\nSELECT SUM(price), AVG(price)\r\n  FROM books;\r\n```",
                                    Reference = "- `SUM(price)`: 全ての本の価格の合計を計算します。\r\n- `AVG(price)`: 全ての本の価格の平均を計算します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。" },

                        new Lesson{ Category = "基本的な関数",
                                    Title =  "MAX関数 / MIN関数",
                                    Description = "このページでは、データの最大値と最小値を取得するためのMAX関数とMIN関数について学びます。\r\n### MAX関数について\r\nMAX関数は、指定したカラムの最大値を取得するために使用します。\r\n\r\n構文\r\n```sql\r\nSELECT MAX(カラム) FROM テーブル名;\r\n```\r\n上記の式では、`テーブル名`の`カラム`に含まれる値の最大値を取得します。\r\n### MIN関数について\r\nMIN関数は、指定したカラムの最小値を取得するために使用します。\r\n\r\n構文\r\n```sql\r\nSELECT MIN(カラム) FROM テーブル名;\r\n```\r\n上記の式では、`テーブル名`の`カラム`に含まれる値の最小値を取得します。",
                                    Question = "一番長いページ数、一番古い出版日を取得してください。",
                                    Hint = "- ページ数の最大値と最小値、出版日の最古値を取得するために `MAX` と `MIN` 関数を使用します。",
                                    Answer = "```\r\nSELECT MAX(pages), MIN(publication_date)\r\n  FROM books;\r\n```",
                                    Reference = "- `MAX(pages)`: 書籍の中で最もページ数が多いものを取得します。\r\n- `MIN(publication_date)`: 書籍の中で最も古い出版日を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。" },

                        new Lesson{ Category = "基本的な関数",
                                    Title =  "GROUP BY句",
                                    Description = "このページでは、データをグループ化するためのGROUP BY句について学びます。\r\n### GROUP BY句について\r\nGROUP BY句は、SELECT文で取得したデータを指定したカラムの値に基づいて、結果セットをグループ化するために使用します。\r\n\r\n構文\r\n```sql\r\nSELECT カラム, COUNT(*) FROM テーブル名 GROUP BY カラム;\r\n```\r\n上記の式では、`テーブル名`から取得した`カラム`の値ごとにデータをグループ化し、各グループの件数を取得します。",
                                    Question = "それぞれの著者ごとの、本の数を取得してください。",
                                    Hint = "- 著者ごとに本の数を取得するために `GROUP BY` を使用します。\r\n- 著者の本の数を数えるために `COUNT(*)` を使用します。",
                                    Answer = "```\r\nSELECT author, COUNT(*)\r\n  FROM books\r\n GROUP BY author;\r\n```",
                                    Reference = "- `COUNT(*)`: 各著者の書籍の数を数えます。\r\n- `GROUP BY author`: 著者ごとにグループ化します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n" },

                        new Lesson{ Category = "基本的な関数",
                                    Title =  "HAVING句",
                                    Description = "このページでは、分類したデータに対して条件を指定するためのHAVING句について学びます。\r\n### HAVING句について\r\nHAVING句は、GROUP BY句で分類したデータに対して、集計関数を使用した条件を指定し、集計結果をフィルタリングするために使用します。WHERE句との違いは、集計関数を条件に使用できる点です。\r\n\r\n構文\r\n```sql\r\nSELECT カラム1, AVG(カラム2) FROM テーブル名 GROUP BY カラム1 HAVING AVG(カラム2) > 10;\r\n```\r\n上記の式では、`テーブル名`から取得した`カラム1`の値ごとにデータをグループ化し、`カラム2`の平均値が10より大きいグループのみを取得します。",
                                    Question = "各著者の合計価格が1500円を超える著者の名前と、その合計価格を取得してください。",
                                    Hint = "- 著者ごとの合計価格が1500円を超える著者を取得するために `HAVING` を使用します。\r\n- 合計価格を取得するために `SUM(price)`を使用します。",
                                    Answer = "```\r\nSELECT author, SUM(price) AS total_price\r\nFROM books\r\nGROUP BY author\r\nHAVING SUM(price) > 1500;\r\n```",
                                    Reference = "- `SUM(price) AS total_price`: 各著者の合計価格を取得します。\r\n- `FROM books`: 書籍テーブルからデータを取得します。\r\n- `GROUP BY author`: 著者ごとにグループ化します。\r\n- `HAVING SUM(price) > 1500`: 合計価格が1500円を超える条件を適用します。" }
                    };
                    context.Lesson.AddRange(lessons);
                    context.SaveChanges();
                }

                // LessonAnswerテーブルのシード追加
                if (!context.LessonAnswer.Any())
                {
                    var lessonAnswers = new LessonAnswer[]
                    {
                        new LessonAnswer{ LessonId = 1,
                                          ValidAnswer = "SELECT title, author FROM books;" },

                        new LessonAnswer{ LessonId = 2,
                                          ValidAnswer = "SELECT title AS \"タイトル\", publication_date AS \"出版日\" FROM books WHERE author = N'夏目漱石';" },

                        new LessonAnswer{ LessonId = 2,
                                          ValidAnswer = "SELECT title \"タイトル\", publication_date \"出版日\" FROM books WHERE author = N'夏目漱石';" },

                        new LessonAnswer{ LessonId = 3,
                                          ValidAnswer = "SELECT DISTINCT author FROM books;" },

                        new LessonAnswer{ LessonId = 4,
                                          ValidAnswer = "SELECT title, author, publication_date FROM books ORDER BY publication_date DESC;" },

                        new LessonAnswer{ LessonId = 5,
                                          ValidAnswer = "SELECT title AS \"タイトル\", author AS \"著者\", ROUND(price / 1.1, 0) AS \"税抜き価格\" FROM books;" },

                        new LessonAnswer{ LessonId = 6,
                                          ValidAnswer = "SELECT title, author, publication_date FROM books WHERE publication_date <= N'1907-01-01';" },

                        new LessonAnswer{ LessonId = 7,
                                          ValidAnswer = "SELECT title, pages, price FROM books WHERE author = N'太宰治' AND price <= 500;" },

                        new LessonAnswer{ LessonId = 8,
                                          ValidAnswer = "SELECT title, price FROM books WHERE price BETWEEN 500 AND 1000;" },

                        new LessonAnswer{ LessonId = 9,
                                          ValidAnswer = "SELECT COUNT(DISTINCT author) FROM books;" },

                        new LessonAnswer{ LessonId = 10,
                                          ValidAnswer = "SELECT SUM(price), AVG(price) FROM books;" },

                        new LessonAnswer{ LessonId = 11,
                                          ValidAnswer = "SELECT MAX(pages), MIN(publication_date) FROM books;" },

                        new LessonAnswer{ LessonId = 12,
                                          ValidAnswer = "SELECT author, COUNT(*) FROM books GROUP BY author;" },

                        new LessonAnswer{ LessonId = 13,
                                          ValidAnswer = "SELECT author, SUM(price) AS total_price FROM books GROUP BY author HAVING SUM(price) > 1500;" }
                    };
                    context.LessonAnswer.AddRange(lessonAnswers);
                    context.SaveChanges();
                }
            }
        }
    }
}