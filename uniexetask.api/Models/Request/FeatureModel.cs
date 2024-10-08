namespace uniexetask.api.Models.Request
{
    public class FeatureModel
    {
        public int FeatureId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
