using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GestionProyectosB.Modelo;
using GestionProyectosB.Repository;
using GestionProyectosB.Service;

namespace GestionProyectosB.Service
{
    public class ProyectosUser : IProyectoUser<Proyectos>
    {
        ProyectosEntities db = new ProyectosEntities();
        public void Dispose()
        {
            db.Dispose();
        }



        public async Task<Proyectos> GetProyecto(int id)
        {
            return await db.Proyectos.FirstOrDefaultAsync(p => p.ProyectoId == id);
        }

        public async Task<IEnumerable<Proyectos>> ListPro(Usuarios user)
        {
            return await  (from p in db.Proyectos
                             join pu in db.ProyectoUsuarios on p.ProyectoId equals pu.ProyectoId
                             join u in db.Usuarios on pu.UsuarioId equals u.UsuarioId
                             where u.UsuarioId == user.UsuarioId
                             select p).ToListAsync();
            
        }
    }
}