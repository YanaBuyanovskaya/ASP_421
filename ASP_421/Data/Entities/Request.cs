using Microsoft.EntityFrameworkCore;

namespace ASP_421.Data.Entities
{
    public class Request
    {
        public Guid Id { get; set; }
        public DateTime Time {  get; set; }
        public String Path { get; set; }=null!;

        public String? Login { get; set; }
        public int Answer { get; set; }
    }

   
}
