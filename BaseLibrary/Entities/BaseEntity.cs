using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }
}
