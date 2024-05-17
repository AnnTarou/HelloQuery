using System.ComponentModel.DataAnnotations;

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
        [DataType(DataType.Password)]
        [Range(6, 10)]
        public string Password { get; set; }

        public List<UserLesson> UserLesson { get; set; }
    }
}
