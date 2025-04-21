namespace Scruffy.Data.Entities;

public class Role
{
    public string GuildId { get; set; }
 public string ChannelId { get; set; }
    public string MessageId { get; set; }
    public string Emote { get; set; }
    public string RoleId { get; set; }
    public DateTime CreatedDate { get; set; }
}