namespace ASP_421.Models.User
{
    public class UserSignUpViewModel
    {
        public UserSignUpFormModel? FormModel { get; set; }

        public Dictionary<String, String>? ValidationErrors { get; set; }
    }
}
