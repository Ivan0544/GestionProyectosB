using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectosB.Repository
{
    internal interface IDataRepository<T> : IDisposable where T : class
    {
        Task<T> getById(int? id);
        Task<T> getByIdString(string id);
        Task<IEnumerable<T>> getAll();
        Task Create(T data);
        Task Update(T data);
        Task Delete(T data);
        Task Save();
    }
}
