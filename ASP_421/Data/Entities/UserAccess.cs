using System.Reflection.Metadata;

namespace ASP_421.Data.Entities
{
    public class UserAccess
    {
        public Guid     Id      { get; set; }
        public Guid     UserId  { get; set; }
        public String   RoleId  { get; set; } = null!;

        public String   Login   { get; set; } = null!;
        public String   Salt    { get; set; } = null!;
        public String   Dk  { get; set; } = null!;  //Delivered Key by RFC 2898
        
        //Navidational properties

        public User User { get; set; } = null!;
        public UserRole Role { get; set; } = null!;

    }
}
