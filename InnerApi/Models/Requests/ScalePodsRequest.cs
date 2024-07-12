namespace InnerApi.Models.Requests;

public class ScalePodsRequest
{
    public string NamespaceName { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
    public int Target { get; set; }
}