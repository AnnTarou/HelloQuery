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

            try
            {
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

                    // 登録成功メッセージを設定
                    TempData["SuccessMessage"] = "アカウント登録が成功しました";

                    // DB更新成功したらログインページへリダイレクト
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    return View(user);
                }
            }
            catch(Exception ex)
            {
                TempData["Message"] = "E-010:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        // Users/EditにGETアクセスがあったとき
        [SessionCheckFilter]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Useridがnullのとき
            if (id == null)
            {
                TempData["Message"] = "E-011:ユーザーが見つかりません";
                return RedirectToAction("error", "Error"); 
            }

            // データベースへ接続
            try
            {
                var user = await _context.User.FindAsync(id);

                // データベースからnullが戻ってきたとき
                if (user == null)
                {
                    TempData["Message"] = "E-012:操作をやり直してください";
                    return RedirectToAction("error", "Error");
                }

                return View(user);
            }
            catch(Exception ex)
            {
                TempData["Message"] = "E-013:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        // Users/EditのPOSTメソッド
        [HttpPost]
        [SessionCheckFilter]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            // ログインユーザーと引数のユーザーが一致しないとき
            if (loginUser.UserId != user.UserId)
            {
                TempData["Message"] = "E-014:ユーザー情報が一致しません";
                return RedirectToAction("error", "Error");
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

                    // DB更新成功したらアカウント詳細ページにリダイレクト
                    return RedirectToAction("Details", "Users");
                }
                catch (Exception ex)
                {
                    if (!UserExists(user.UserId))
                    {
                        TempData["Message"] = "E-015:上書き保存に失敗しました";
                        return RedirectToAction("error", "Error");
                    }
                    else
                    {
                        TempData["Message"] = "E-016:ex.Message";
                        return RedirectToAction("error", "Error");
                    }
                }
            }
            else
            {
                return View(user);
            }
        }

        // Users/DeleteにGETアクセスがあったとき
        [SessionCheckFilter]
        public async Task<IActionResult> Delete()
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            try
            {
                var user = await _context.User
               .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId);

                // userがnullのとき
                if (user == null)
                {
                    TempData["Message"] = "E-017:ユーザーが見つかりませんでした。";
                    return RedirectToAction("error", "Error");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-018:" + ex.Message;
                return RedirectToAction("error", "Error");
            }   
        }

        // Users/Delete:POSTメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionCheckFilter]
        public async Task<IActionResult> Delete(int id)
        {
            try
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

                    await _context.SaveChangesAsync();

                    // セッションをクリア
                    HttpContext.Session.Clear();

                    // クッキーを削除
                    Response.Cookies.Delete("SessionId");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "E-019:情報が見つかりませんでした";
                    return RedirectToAction("error", "Error");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-020:" + ex.Message;
                return RedirectToAction("error", "Error");
            }
        }

        // Users/DetailsにGETアクセスがあったとき
        [HttpGet]
        [SessionCheckFilter]
        public async Task<IActionResult> Details()
        {
            // セッションからユーザーIDを取得
            var loginUser = (User)HttpContext.Items["User"];

            try
            {
                var user = await _context.User
               .FirstOrDefaultAsync(m => m.UserId == loginUser.UserId);

                if (user == null)
                {
                    TempData["Message"] = "E-021:ログインをもう一度やり直してください";
                    return RedirectToAction("error", "Error");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Message"] = "E-022:ログインをもう一度やり直してください";
                return RedirectToAction("error", "Error");
            }           
        }
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
