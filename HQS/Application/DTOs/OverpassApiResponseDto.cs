namespace HQS.Infrastructure.DTOs
{
    public class OverpassResponse
    {
        public List<Element> Elements { get; set; } = new();
    }

    public class Element
    {
        public string Type { get; set; } = string.Empty; //node, way, relation
        public long Id { get; set; }

        // Nodes have lat/lon here
        public double Lat { get; set; } = 0.0;
        public double Lon { get; set; } = 0.0;

        // Ways/Relations have lat/lon inside 'center'
        public Center Center { get; set; } = new();

        public Dictionary<string, string> Tags { get; set; } = new();
    }

    public class Center
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}