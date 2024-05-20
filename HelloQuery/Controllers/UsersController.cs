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

        // Users/DetailsにGETアクセスがあったとき
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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
            // すでにDBに同一のメールアドレスが存在するとき、エラー
            if (await _context.User.FindAsync(user.Email) != null)
            {
                ModelState.AddModelError("", "このメールアドレスはすでに存在しています。別のメールアドレスを入力してください。");
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
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, User user)
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];
            var loginUserId = loginUser.UserId;

            if (loginUserId != user.UserId)
            {
                return NotFound();
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

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
