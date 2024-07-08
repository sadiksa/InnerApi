using InnerApi.Services;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace InnerApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ChangeController(IK8SClientService k8SClientService)
{
    // Scale pods to target number
    [HttpPost("scale")]
    public async Task<string> ScalePods(string namespaceName, string deploymentName, int target)
    {
        try
        {
            // Load the Kubernetes client configuration from the config file
            var client = k8SClientService.GetClient();
            Console.WriteLine("Starting Request!");

            // Fetch the deployment
            var deployment = await client.AppsV1.ReadNamespacedDeploymentAsync(deploymentName, namespaceName);

            // Update the number of replicas
            deployment.Spec.Replicas = target;

            // Apply the changes
            var updatedDeployment =
                await client.AppsV1.ReplaceNamespacedDeploymentAsync(deployment, deploymentName, namespaceName);

            return $"Scaled deployment {deploymentName} to {target} replicas.";
        }
        catch (Exception ex)
        {
            return $"Error scaling deployment: {ex.Message}";
        }
    }

    // restart pod
    [HttpPost("restart")]
    public async Task<string> RestartPod(string namespaceName, string deploymentName)
    {
        try
        {
            var client = k8SClientService.GetClient();
            Console.WriteLine("Starting Request!");

            var pods = await client.CoreV1.ListNamespacedPodAsync(namespaceName);
            var deploymentPods = pods.Items.Where(p => p.Metadata.Labels["app"] == deploymentName);

            foreach (var pod in deploymentPods)
            {
                await client.CoreV1.DeleteNamespacedPodAsync(pod.Metadata.Name, namespaceName);
            }

            return $"Restarted deployment {deploymentName}.";
        }
        catch (Exception ex)
        {
            return $"Error restarting deployment: {ex.Message}";
        }
    }

    // add ingress. Path, service, port, namespace
    [HttpPost("ingress")]
    public async Task<string> AddIngress(string path, string serviceName, int servicePort, string namespaceName)
    {
        try
        {
            var client = k8SClientService.GetClient();
            Console.WriteLine("Starting Request!");

            var ingress = new V1Ingress
            {
                ApiVersion = "networking.k8s.io/v1",
                Kind = "Ingress",
                Metadata = new V1ObjectMeta
                {
                    Name = $"{serviceName}-ingress",
                    NamespaceProperty = namespaceName,
                    Annotations = new Dictionary<string, string>
                    {
                        //kubernetes.io/ingress.class: nginx 
                        {"kubernetes.io/ingress.class", "nginx"},
                        {"nginx.ingress.kubernetes.io/rewrite-target", "/"}
                    }
                },
                Spec = new V1IngressSpec
                {
                    Rules = new[]
                    {
                        new V1IngressRule
                        {
                            Http = new V1HTTPIngressRuleValue
                            {
                                Paths = new[]
                                {
                                    new V1HTTPIngressPath
                                    {
                                        Path = path,
                                        PathType = "Prefix",
                                        Backend = new V1IngressBackend
                                        {
                                            Service = new V1IngressServiceBackend
                                            {
                                                Name = serviceName,
                                                Port = new V1ServiceBackendPort
                                                {
                                                    Number = servicePort
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var createdIngress = await client.NetworkingV1.CreateNamespacedIngressAsync(ingress, namespaceName);
            
            return $"Created Ingress {createdIngress.Metadata.Name}.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating Ingress: {ex.Message}");
            return $"Error creating Ingress: {ex.Message}";
        }
    }
}