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

    /*	public virtual async IAsyncEnumerable<Result> Save(IEnumerable<EditStudyLoadDisciplineCommand?> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            async Task<Result> Handle(EditStudyLoadDisciplineCommand command)
            {
                HttpRequestException? exception = await Save(command).ConfigureAwait(false);
                return new Result(command, exception);
            }

            List<Task<Result>> tasks = new List<Task<Result>>(source.TryGetNonEnumeratedCount(out Int32 count) ? count : 16);
            tasks.AddRange(source.WhereNotNull().Select(Handle));

            while (tasks.Count > 0)
            {
                Task<Result> task = await Task.WhenAny(tasks).ConfigureAwait(false);
                tasks.Remove(task);
                yield return await task.ConfigureAwait(false);
            }
        }*/

    public async Task LoadAsync(ISaveMetaData saveSlot)
    {
        var state = new PlayerProgress();
        CurrentSave = await serializer.DeserializeAsync(state, storageProvider, saveSlot) as PlayerProgress;

        OnSaveLoaded?.Invoke(CurrentSave, saveSlot);

    }

    public Task<IEnumerable<ISaveMetaData>> GetSaves()
    {
        return Task.FromResult<IEnumerable<ISaveMetaData>>(saves);
    }


    public async Task AutoLoadAsync()
    {
        Services.GetService<IGameStateManager>().ChangeState(new GameLoadingState());
        await LoadAsync(new SimpleSaveMetaData { Path = "", Name = "Main", Date = DateTime.Now, Version = "0" });
        Debug.Log($"AutoLoadAsync {CurrentSave}");

    }

    public async Task DeleteAsync(ISaveMetaData saveSlot)
    {
        await storageProvider.DeleteAsync(saveSlot.Path);
    }


}
