using HelloQuery.Data;
using HelloQuery.Filter;
using HelloQuery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelloQuery.Controllers
{
    public class UsersController : Controller
    {
        private readonly HelloQueryContext _context;

        public UsersController(HelloQueryContext context)
        {
            _context = context;
        }

        // Users/CreateにGETアクセスがあったとき
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Users/CreateのPOSTメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {

            // ニックネームが25文字以上のとき、エラー
            if (user.UserName.Length > 25)
            {
                ModelState.AddModelError("", "ニックネームは25文字以内で入力してください。");
                return View(user);
            }

            // すでにDBに同一のメールアドレスが存在するとき、エラー
            if (await _context.User.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("", "このメールアドレスはすでに存在しています。別のメールアドレスを入力してください。");
                return View(user);
            }

            // パスワードの文字数が6文字以上でないとき、エラー
            if (user.Password.Length < 6)
            {
                ModelState.AddModelError("", "パスワードは6文字以上で入力してください。");
                return View(user);
            }

            // パスワードと確認用パスワードが一致しないとき、エラー
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("", "パスワードが一致しません");
                return View(user);
            }

            // ModelState.IsValidがfalseのときにあえて設定している
            if (!ModelState.IsValid)
            {
                // パスワードハッシュ化クラスのインスタンス生成
                var hasher = new PasswordHasher<IdentityUser>();

                // ハッシュ化されたパスワードの生成
                var hashedPassword = hasher.HashPassword(null, user.Password);

                // ハッシュ化されたパスワードをパスワードに設定
                user.Password = hashedPassword;

                // コンテキストに入力されたuserを登録
                _context.Add(user);

                // データベースの更新
                await _context.SaveChangesAsync();

                // DB更新成功したらログインページへリダイレクト
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(user);
            }
        }

        // Users/EditにGETアクセスがあったとき
        [SessionCheckFilter]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Useridがnullのとき、NotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);

            // Userがnullのとき、NotFoundを返す
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Users/EditのPOSTメソッド
        [HttpPost]
        [SessionCheckFilter]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            if (loginUser.UserId != user.UserId)
            {
                return NotFound();
            }

            // ニックネームが25文字以上のとき、エラー
            if (user.UserName.Length > 25)
            {
                ModelState.AddModelError("", "ニックネームは25文字以内で入力してください。");
                return View(user);
            }

            // パスワードの文字数が6文字以上でないとき、エラー
            if (user.Password.Length < 6)
            {
                ModelState.AddModelError("", "パスワードは6文字以上で入力してください。");
                return View(user);
            }

            // パスワードと確認用パスワードが一致しないとき、エラー
            if (user.Password != user.ConfirmPassword)
            {
                ModelState.AddModelError("", "パスワードが一致しません");
                return View(user);
            }

            // ModelState.IsValidがfalseのときにあえて設定している
            if (!ModelState.IsValid)
            {
                try
                {
                    // パスワードハッシュ化クラスのインスタンス生成
                    var hasher = new PasswordHasher<IdentityUser>();

                    // ハッシュ化されたパスワードの生成
                    var hashedPassword = hasher.HashPassword(null, user.Password);

                    // ハッシュ化されたパスワードをパスワードに設定
                    user.Password = hashedPassword;

                    // コンテキストに変更を登録
                    _context.Update(user);

                    // データベースの更新
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // DB更新成功したらアカウント詳細ページにリダイレクト
                return RedirectToAction("Details", "Users");
            }

            return View(user);
        }

        // Users/DeleteにGETアクセスがあったとき
        [SessionCheckFilter]
        public async Task<IActionResult> Delete()
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            // idがnullのとき、NotFoundを返す
            if (loginUser == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId);

            // Userがnullのとき、NotFoundを返す
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Users/Delete:POSTメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheckFilter]
        public async Task<IActionResult> Delete(int id)
        {
            // Includeメソッドを使用してこのユーザーに関連するUserLessonを取得
            var user = await _context.User
                .Include(u => u.UserLesson)
                .FirstOrDefaultAsync(u => u.UserId == id);

            // Userがnullでないとき
            if (user != null)
            {
                // このユーザーのUserLessonを削除
                _context.UserLesson.RemoveRange(user.UserLesson);

                // このユーザーをUserから削除
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }

        // Users/DetailsにGETアクセスがあったとき
        [HttpGet]
        [SessionCheckFilter]
        public async Task<IActionResult> Details()
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            if (loginUser == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
