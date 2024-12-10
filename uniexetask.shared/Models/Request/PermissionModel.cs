namespace uniexetask.shared.Models.Request
{
    public class PermissionModel
    {
        public int PermissionId { get; set; }

        public int FeatureId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
