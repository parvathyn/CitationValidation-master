
namespace DataAccess.Model
{
   public class ZoneSpace
    {
       public string Zone { get; set; }
       public string Space { get; set; }
       public int CustomerId { get; set; }
       public string MeterName { get; set; }
    }

    public class ZoneIdName
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
