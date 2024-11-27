using GestionProyectosB.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectosB.Repository
{
    internal interface IProyectoUser<Proyectos> : IDisposable
    {
        Task<Proyectos> GetProyecto(int id);
        Task<IEnumerable<Proyectos>> ListPro(Usuarios user);
    }
}
