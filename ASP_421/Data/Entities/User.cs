using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace ASP_421.Data.Entities
{
    public class User
    {
        public Guid       Id              { get; set; }
        public String     Name            { get; set; } = null!;
        public String     Email           { get; set; } = null!;
        public DateTime?  BirthDate       { get; set; }
        public DateTime   RegisteredAt    { get; set; }
        public DateTime?  DeletedAt       { get; set; }

        //Inverse Navigation properties

        //public IEnumerable<UserAccess> Accesses { get; set; } = [];
        [JsonIgnore]
        public ICollection<UserAccess> Accesses { get; set; } = new List<UserAccess>();

    }
}
