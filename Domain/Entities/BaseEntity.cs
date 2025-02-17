using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public DateTime CreatedTime { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedTime { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; } 
    }
}
