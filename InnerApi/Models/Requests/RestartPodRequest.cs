namespace InnerApi.Models.Requests;

public class RestartPodRequest
{
    public string NamespaceName { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}