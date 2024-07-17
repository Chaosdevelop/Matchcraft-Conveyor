using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMF.Data;
using GMF.Tags;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

public abstract class DataRepositoryScriptableObject<T> : InstallableScriptableObject, IDataRepository<T> where T : IData
{

    public override ServiceDescriptor GetServiceDescriptor()
    {
        return new ServiceDescriptor(typeof(IDataRepository<T>), this);
    }
    void OnValidate()
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            if (dataList[i] != null)
            {
                dataList[i].Id = i;
            }

        }
    }

    void OnEnable()
    {
        foreach (var data in dataList)
        {
            (data as ITaggedContainerRegistrator)?.Register();
        }
    }
    void OnDisable()
    {
        foreach (var data in dataList)
        {
            (data as ITaggedContainerRegistrator)?.Unregister();
        }
    }

    [SerializeField]
    private List<T> dataList = new List<T>();

    public T GetById(int id)
    {
        return dataList.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<T> GetAll()
    {
        return dataList;
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await Task.Run(() => GetById(id));
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Task.Run(() => GetAll());
    }

    public T GetSingleton()
    {
        return dataList.FirstOrDefault();
    }

    public async Task<T> GetSingletonAsync()
    {
        return await Task.Run(() => GetSingleton());
    }

}