using System.ComponentModel.DataAnnotations;

namespace HelloQuery.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必須項目です")]
        [EmailAddress(ErrorMessage = "無効なメールアドレスです")]
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }

        [Required(ErrorMessage = "必須項目です")]
        [DataType(DataType.Password)]
        [Range(6, 10)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Display(Name = "ログイン状態を保持する")]
        public bool RememberMe { get; set; }
    }
}
