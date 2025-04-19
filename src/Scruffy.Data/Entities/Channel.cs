namespace Scruffy.Data.Entities
{
    public class Channel
    {
        public string ChannelId { get; set; }
        public string GuildId { get; set; }
        public int PurgeInterval { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}