using System.Text.Json.Serialization;

namespace HQS.Domain.Entities
{
    public class AddressSuggestion
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = "";
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}