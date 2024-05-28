using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloQuery.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必須項目です")]
        [EmailAddress(ErrorMessage = "無効なメールアドレスです")]
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }

        [Required(ErrorMessage = "パスワードを入力してください")]
        [DataType(DataType.Password)]
        [Range(6, 10)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Display(Name = "ログイン状態を保持する")]
        public bool RememberMe { get; set; }
    }
}
