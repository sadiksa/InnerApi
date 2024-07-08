using k8s;

namespace InnerApi.Services;

public interface IK8SClientService
{
    public IKubernetes GetClient();
}