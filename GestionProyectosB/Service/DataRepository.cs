using GestionProyectosB.Modelo;
using GestionProyectosB.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GestionProyectosB.Service
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        ProyectosEntities _dbContext = new ProyectosEntities();
        private DbSet<T> EntitySet
        {
            get
            {
                return _dbContext.Set<T>();
            }
        }
        public async Task<IEnumerable<T>> getAll()
        {
            return await EntitySet.ToListAsync();

        }

        public async Task Create(T data)
        {
            EntitySet.Add(data);
            await Save();
        }

        public async Task Delete(T data)
        {
            EntitySet.Remove(data);
            await Save();
        }

        public async Task<T> getByIdString(string id)
        {
            return await EntitySet.FindAsync(id);
        }

        public async Task<T> getById(int? id)
        {
            return await EntitySet.FindAsync(id.Value);
        }

        public async Task Update(T data)
        {
            EntitySet.AddOrUpdate(data);
            await Save();
        }

        public async Task Save() {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
     
        }
    }
}