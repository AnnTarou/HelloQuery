using HelloQuery.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Newtonsoft.Json;

namespace HelloQuery.Filter
{
    // フィルタークラスを継承したセッションチェックフィルター
    // 属性を付与することで、アクションメソッド実行前にセッションチェックを行う
    public class SessionCheckFilter : ActionFilterAttribute
    {
        // アクションメソッド実行前に実行されるメソッド
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // セッションからセッションIDを取得
            var sessionId = context.HttpContext.Session.GetString("SessionId");

            // セッションIDが存在しない場合はCookieをチェック
            if (sessionId == null)
            {
                context.HttpContext.Request.Cookies.TryGetValue("SessionId", out sessionId);
            }

            User user = null;

            // セッションIDが取得できた場合、Jsonを変換してユーザーを取得
            if (sessionId != null)
            {
                var userJson = context.HttpContext.Session.GetString(sessionId);
                if (userJson != null)
                {
                    user = JsonConvert.DeserializeObject<User>(userJson);
                }
            }

            // ユーザー情報が取得できなかった場合、ログインページにリダイレクト
            if (user == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            // ユーザー情報が取得できた場合ViewDataに保存
            else
            {
                context.HttpContext.Items["User"] = user;
            }
        }
    }
}
