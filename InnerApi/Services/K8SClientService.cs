using k8s;

namespace InnerApi.Services;

public class K8SClientService(IWebHostEnvironment env) : IK8SClientService
{
    public IKubernetes GetClient()
    {
        if (env.IsProduction())
        {
            var configForInCluster = KubernetesClientConfiguration.InClusterConfig();
            return new Kubernetes(configForInCluster);
        }
        var configForConfigFile = KubernetesClientConfiguration.BuildConfigFromConfigFile("./config");
        configForConfigFile.Host = "https://13.51.79.228:6443";
        // master node ip. it changing cause of aws has not elastic ip
        return new Kubernetes(configForConfigFile);
    }
}