using ASP_421.Data.Entities;
namespace ASP_421.Models.User
{
    public class UserCabinetViewModel
    {
        public Data.Entities.User User { get; set; } = null!;

        public Dictionary<string,string>? ValidationErrors { get; set; }

        public bool IsEditing { get; set; }
    }
}
