using Microsoft.AspNetCore.Mvc;

namespace ASP_421.Models.User
{
    public class UserEditFormModel
    {
        [FromForm(Name = "user-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "user-email")]
        public String Email { get; set; } = null!;

        [FromForm(Name ="user-birthday")]
        public DateTime? Birthday { get; set; }
    }
}
