using GestionProyectosB.Modelo;
using GestionProyectosB.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace GestionProyectosB.Service
{
    public class LoginUser : ILogin<Usuarios>
    {
        ProyectosEntities db = new ProyectosEntities();

        public void Dispose()
        {
            db.Dispose();
        }

        public async Task<Usuarios> GetUser(string data)
        {
          return await db.Usuarios.FirstOrDefaultAsync(u => u.Email == data);

        }
    }
}