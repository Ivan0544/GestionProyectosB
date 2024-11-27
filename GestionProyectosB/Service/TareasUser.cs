using GestionProyectosB.Repository;
using GestionProyectosB.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity;

namespace GestionProyectosB.Service
{
    public class TareasUser : ITareasUser<Tareas>
    {
        ProyectosEntities db = new ProyectosEntities();
        public void Dispose()
        {
            db.Dispose();
        }

        public async Task<IEnumerable<Tareas>> GetTareas(int id)
        {
            return await db.Tareas.Where(t => t.ProyectoId == id).ToListAsync();
        }
    }
}