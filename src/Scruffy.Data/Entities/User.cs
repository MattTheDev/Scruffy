namespace Scruffy.Data.Entities;

public class User
{
    public string UserId { get; set; }
    public int Points { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}