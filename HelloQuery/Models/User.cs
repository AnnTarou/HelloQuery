using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelloQuery.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Display(Name = "ニックネーム")]
        [Required(ErrorMessage = "必須項目です")]
        public string UserName { get; set; }

        [Display(Name = "メールアドレス")]
        [Required(ErrorMessage = "必須項目です")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "パスワード")]
        [Required(ErrorMessage = "パスワードを入力してください")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name = "パスワード確認用")]
        [Required(ErrorMessage = "パスワードを入力してください")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public List<UserLesson> UserLesson { get; set; }
    }
}
