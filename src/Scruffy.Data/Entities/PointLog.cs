using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Scruffy.Data.Entities;

public class PointLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string GrantorId { get; set; }
    public string GranteeId { get; set; }
    public string GuildId { get; set; }
    public DateTime GrantedDate { get; set; }
}