using System.Linq;
using GMF;
using GMF.Saving;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public abstract class InstallableScriptableObject : ScriptableObject
{
    public abstract ServiceDescriptor GetServiceDescriptor();
}



public class UnityGameStarter : MonoBehaviour
{
    [SerializeField]
    InstallableScriptableObject[] installableScriptableObjects;


    private async void Awake()
    {
        Core.Services.Initialize(installableScriptableObjects?.Select(arg => arg.GetServiceDescriptor()));
        Core.Services.GetRequiredService<IGameStateManager>().ChangeState(new InitializationState());
        Core.Services.GetRequiredService<ISaveLoadManager>().Initialize();
        await SaveSystem.TryAutoLoadAsync();
        Core.Services.GetRequiredService<IGameStateManager>().ChangeState(new MainPlayState());

        Debug.Log($"UnityGameStarter awaited");
    }

    /*    private void Awake()
		{
			Core.Services.Initialize(installableScriptableObjects?.Select(arg => arg.GetServiceDescriptor()));
			Core.Services.GetRequiredService<IGameStateManager>().ChangeState(new InitializationState());
			Core.Services.GetRequiredService<ISaveLoadManager>().Initialize();

			//Task.Run(() => SaveSystem.TryAutoLoadAsync()).GetAwaiter().GetResult();

			SaveSystem.TryAutoLoadAsync()

			Debug.Log($"UnityGameStarter ends");
		}*/
}
