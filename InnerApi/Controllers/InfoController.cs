using InnerApi.Services;
using k8s;
using k8s.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnerApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class InfoController(ILogger<InfoController> logger, IK8SClientService k8SClientService)
    : ControllerBase
{
    private readonly ILogger<InfoController> _logger = logger;

    // get namespaces
    [HttpGet("namespaces")]
    public IEnumerable<string> GetNamespaces()
    {
        var client = k8SClientService.GetClient();
        Console.WriteLine("Starting Request!");

        var list = client.CoreV1.ListNamespace();
        foreach (var item in list.Items)
        {
            Console.WriteLine(item.Metadata.Name);
        }

        if (list.Items.Count == 0)
        {
            Console.WriteLine("Empty!");
        }

        return list.Items.Select(p => p.Metadata.Name);
    }

    // get deployments by namespace
    [HttpGet("deployments")]
    public IEnumerable<string> GetDeploymentsByNamespace(string namespaceName)
    {
        var client = k8SClientService.GetClient();
        Console.WriteLine("Starting Request!");

        var list = client.AppsV1.ListNamespacedDeployment(namespaceName);
        foreach (var item in list.Items)
        {
            Console.WriteLine(item.Metadata.Name);
        }

        if (list.Items.Count == 0)
        {
            Console.WriteLine("Empty!");
        }

        return list.Items.Select(p => p.Metadata.Name);
    }

    [HttpGet("pods")]
    public IEnumerable<string> GetPodsByNamespace(string namespaceName)
    {
        var client = k8SClientService.GetClient();
        Console.WriteLine("Starting Request!");

        var list = client.CoreV1.ListNamespacedPod(namespaceName);
        foreach (var item in list.Items)
        {
            Console.WriteLine(item.Metadata.Name);
        }

        if (list.Items.Count == 0)
        {
            Console.WriteLine("Empty!");
        }

        return list.Items.Select(p => p.Metadata.Name);
    }

    // get services by namespace
    [HttpGet("services")]
    public IEnumerable<string> GetServicesByNamespace(string namespaceName)
    {
        var client = k8SClientService.GetClient();
        Console.WriteLine("Starting Request!");

        var list = client.CoreV1.ListNamespacedService(namespaceName);
        foreach (var item in list.Items)
        {
            Console.WriteLine(item.Metadata.Name);
        }

        if (list.Items.Count == 0)
        {
            Console.WriteLine("Empty!");
        }

        return list.Items.Select(p => p.Metadata.Name);
    }

    // see logs of a pod

    [HttpGet("logs")]
    public async Task<string> GetLogsByPod(string namespaceName, string podName)
    {
        var client = k8SClientService.GetClient();

        var list = await client.CoreV1.ListNamespacedPodAsync(namespaceName);
        if (list.Items.Count == 0)
        {
            return "No pods!";
        }

        var pod = list.Items.FirstOrDefault(p => p.Metadata.Name == podName);
        if (pod == null)
        {
            return "No pod found!";
        }

        var response = await client.CoreV1.ReadNamespacedPodLogWithHttpMessagesAsync(
            pod.Metadata.Name,
            pod.Metadata.NamespaceProperty, container: pod.Spec.Containers[0].Name,
            follow: false).ConfigureAwait(false);
        var stream = response.Body;

        using var reader = new StreamReader(stream);
        var logs = await reader.ReadToEndAsync();

        return logs;
    }

    // get metrics
    [HttpGet("metrics")]
    public async Task<object> GetMetrics()
    {
        var client = k8SClientService.GetClient();

        var nodeMetrics = await NodesMetrics(client).ConfigureAwait(false);

        return nodeMetrics;
    }


    private static async Task<List<object>> NodesMetrics(IKubernetes client)
    {
        var nodesMetrics = await client.GetKubernetesNodesMetricsAsync().ConfigureAwait(false);
        var result = new List<object>();

        foreach (var item in nodesMetrics.Items)
        {
            var node = await client.CoreV1.ReadNodeAsync(item.Metadata.Name).ConfigureAwait(false);
            var totalCpu = node.Status.Capacity["cpu"];
            var totalMemory = node.Status.Capacity["memory"];

            var cpuUsage = item.Usage["cpu"];
            var memoryUsage = item.Usage["memory"];

            var cpuUsagePercentage = CalculatePercentage(cpuUsage.ToDecimal(), totalCpu.ToDecimal());
            var memoryUsagePercentage = CalculatePercentage(memoryUsage.ToDecimal(), totalMemory.ToDecimal());

            result.Add(new
            {
                NodeName = item.Metadata.Name,
                CpuUsage = $"{cpuUsagePercentage}%",
                MemoryUsage = $"{memoryUsagePercentage}%"
            });
        }

        return result;
    }
    
    private static double CalculatePercentage(decimal usage, decimal total)
    {
        var percentage = (double)(usage / total) * 100;
        return Math.Round(percentage);
    }
}