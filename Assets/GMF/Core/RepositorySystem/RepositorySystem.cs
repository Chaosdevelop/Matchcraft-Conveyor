using System.Collections.Generic;
using System.Threading.Tasks;

namespace GMF.Data
{
    public interface IData
    {
        int Id { get; set; }
    }

    public interface IDataRepository<T> where T : IData
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        T GetSingleton();
        Task<T> GetSingletonAsync();
    }


    public static class Data<T> where T : IData
    {
        static IDataRepository<T> Repository => Core.Services.GetRequiredService<IDataRepository<T>>();

        public static T GetById(int id) => Repository.GetById(id);

        public static IEnumerable<T> GetAll() => Repository.GetAll();

        public static Task<T> GetByIdAsync(int id) => Repository.GetByIdAsync(id);

        public static Task<IEnumerable<T>> GetAllAsync() => Repository.GetAllAsync();

        public static T GetSingleton => Repository.GetSingleton();

        public static Task<T> GetSingletonAsync() => Repository.GetSingletonAsync();

    }

}