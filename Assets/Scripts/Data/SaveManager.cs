using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GMF;
using GMF.Saving;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
/// <summary>
/// Manages saving and loading game data.
/// </summary>
/// 
[ServiceDescriptor(ServiceLifetime.Singleton)]
public class SaveLoadManager : ISaveLoadManager
{
    IDataSerializer serializer;
    IStorageProvider storageProvider;

    List<ISaveMetaData> saves = new List<ISaveMetaData>();


    public Action<ISaveData, ISaveMetaData> OnSaveLoaded { get; set; }

    public static PlayerProgress CurrentSave { get; private set; }


    public void Initialize()
    {
        serializer = new UnityJsonDataSerializer();
        storageProvider = new UnityLocalStorageProvider();
        SaveSystem.Initialize(this);
    }

    public async Task SaveAsync(ISaveMetaData saveSlot)
    {

        await serializer.SerializeAsync(CurrentSave, storageProvider, saveSlot);
        Debug.Log("SaveAsync");

        saves.Add(saveSlot);

    }


    public async Task LoadAsync(ISaveMetaData saveSlot)
    {
        var state = new PlayerProgress();
        CurrentSave = await serializer.DeserializeAsync(state, storageProvider, saveSlot) as PlayerProgress;

        OnSaveLoaded?.Invoke(CurrentSave, saveSlot);

    }

    public Task<IEnumerable<ISaveMetaData>> GetAvailableSaves()
    {
        return Task.FromResult<IEnumerable<ISaveMetaData>>(saves);
    }


    public async Task AutoLoadAsync()
    {
        Core.Services.GetRequiredService<IGameStateManager>().ChangeState(new GameLoadingState());
        await LoadAsync(new SimpleSaveMetaData { Path = "", Name = "Main", Date = DateTime.Now, Version = "0" });
        Debug.Log($"AutoLoadAsync {CurrentSave}");

    }

    public async Task DeleteSaveSlotAsync(ISaveMetaData saveSlot)
    {
        await storageProvider.DeleteAsync(saveSlot.Path);
    }


}
