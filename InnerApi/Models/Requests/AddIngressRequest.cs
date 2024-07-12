namespace InnerApi.Models.Requests;

public class AddIngressRequest
{
    public string Path { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public int ServicePort { get; set; }
    public string NamespaceName { get; set; } = string.Empty;
}