using GestionProyectosB.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectosB.Repository
{
    internal interface ITareasUser<Tareas> : IDisposable
    {
        Task<IEnumerable<Tareas>> GetTareas(int id);
    }
}
