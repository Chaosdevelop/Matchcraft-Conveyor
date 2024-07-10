using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public class DependencyInjectionSetup : MonoBehaviour
{
    public static ServiceProvider ServiceProvider { get; private set; }

    void Awake()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Регистрация зависимостей
        services.AddTransient<ICloneService, CloneService>();
    }
}

public interface ICloneService
{
    GameObject Clone(GameObject prefab, Vector3 position, Quaternion rotation);
}

public class CloneService : ICloneService
{
    public GameObject Clone(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Object.Instantiate(prefab, position, rotation);
    }
}