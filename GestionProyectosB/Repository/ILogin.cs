using GestionProyectosB.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectosB.Repository
{
    internal interface ILogin<Usuarios>: IDisposable
    {
        Task<Usuarios> GetUser(string id);
    }
}
