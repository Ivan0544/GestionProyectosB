using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GestionProyectosB.Modelo;
using GestionProyectosB.Service;

namespace GestionProyectosF.Controllers
{
    public class TareasController : Controller
    {
        private ProyectosEntities db = new ProyectosEntities();
        DataRepository<Tareas> Tareas = new DataRepository<Tareas>();
        TareasUser TareasUser = new TareasUser();
        ProyectosUser ProyectosUser = new ProyectosUser();
        Usuarios logUser = new Usuarios();


        // GET: Tareas
        public async Task<ActionResult> Index(int proyectoId)
        {
            try
            {
                var usuario = Session["usuario"] as Usuarios;
                logUser = usuario;
                ViewBag.NombreUsuario = logUser.Nombre;
                var proyecto = await ProyectosUser.GetProyecto(proyectoId);
                ViewBag.NombreP = proyecto.Nombre;
                ViewBag.Id = proyectoId;
                var tareas = await TareasUser.GetTareas(proyectoId);
                return View(tareas);
            }
            catch (Exception ex) { 
            
            }
            return View();
        }

        // GET: Tareas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tareas tareas = await Tareas.getById(id);
            if (tareas == null)
            {
                return HttpNotFound();
            }
            return View(tareas);
        }

        // GET: Tareas/Create
        public ActionResult Create(int proyectoId)
        {
            var usuario = Session["usuario"] as Usuarios;
            ViewBag.Id = proyectoId;
            ViewBag.ProyectoId = new SelectList(db.Proyectos.Where(p=> p.ProyectoId == proyectoId), "ProyectoId", "Nombre");
            ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios.Where(u=> u.UsuarioId == usuario.UsuarioId), "UsuarioId", "Nombre");
            return View();
        }

        // POST: Tareas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TareaId,Nombre,Descripcion,FechaLimite,Prioridad,Estado,UsuarioAsignadoId,ProyectoId")] Tareas tareas)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TempData["Message"] = "Tarea creada con Exito.";
                    TempData["MessageType"] = "success";
                    await Tareas.Create(tareas);
                    return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
                }

                ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
            }
            catch (Exception ex) {
                TempData["Message"] = "Error en al creacion."+ex;
                TempData["MessageType"] = "danger";
            }
            return View();
        }

        // GET: Tareas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var usuario = Session["usuario"] as Usuarios;
            Tareas tareas = await Tareas.getById(id);
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (tareas == null)
            {
                return HttpNotFound();
            }
            if (usuario.UsuarioId == tareas.UsuarioAsignadoId)
            {
                ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                return View(tareas);
            }
            else {

                if (usuario.Rol == "Administrador")
                {
                    ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                    ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                    return View(tareas);
                }
                else {
                    TempData["Message"] = "No puede modificar tareas de otro colaborador.";
                    TempData["MessageType"] = "danger";
                    //ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                    //ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                }
            }
            return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });

        }

        // POST: Tareas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TareaId,Nombre,Descripcion,FechaLimite,Prioridad,Estado,UsuarioAsignadoId,ProyectoId")] Tareas tareas)
        {
            var usuario = Session["usuario"] as Usuarios;
            try
            {
                if (usuario.UsuarioId == tareas.UsuarioAsignadoId)
                {
                    if (ModelState.IsValid)
                    {
                        await Tareas.Update(tareas);
                        TempData["Message"] = "Tarea modificada con Exito.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
                    }
                    ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                    ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                }
                else
                {
                    if (usuario.Rol == "Administrador") {
                        if (ModelState.IsValid)
                        {
                            await Tareas.Update(tareas);
                            TempData["Message"] = "Tarea modificada con Exito.";
                            TempData["MessageType"] = "success";
                            return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
                        }
                        ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                        ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                    }
                    else {
                        TempData["Message"] = "No puede modificar tareas de otro colaborador.";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            catch (Exception ex) {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
        }

        // GET: Tareas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var usuario = Session["usuario"] as Usuarios;
            Tareas tareas = await Tareas.getById(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (tareas == null)
            {
                return HttpNotFound();
            }
            if (usuario.UsuarioId == tareas.UsuarioAsignadoId)
            {
                return View(tareas);
            }
            else {
                if (usuario.Rol == "Administrador")
                {
                    return View(tareas);
                }
                else {
                    TempData["Message"] = "No puede eliminar tareas de otro colaborador.";
                    TempData["MessageType"] = "danger";
                }
            }

            return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var usuario = Session["usuario"] as Usuarios;
            Tareas tareas = await Tareas.getById(id);
            try
            {     
                if (usuario.UsuarioId == tareas.UsuarioAsignadoId)
                {
                    if (ModelState.IsValid)
                    {
                        await Tareas.Delete(tareas);
                        TempData["Message"] = "Tarea modificada con Exito.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
                    }
                    ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                    ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);
                }
                else
                {
                    if (usuario.Rol == "Administrador")
                    {
                        if (ModelState.IsValid)
                        {
                            await Tareas.Update(tareas);
                            TempData["Message"] = "Tarea modificada con Exito.";
                            TempData["MessageType"] = "success";
                            return RedirectToAction("Index", "Tareas", new { proyectoId = tareas.ProyectoId });
                        }
                        ViewBag.ProyectoId = new SelectList(db.Proyectos, "ProyectoId", "Nombre", tareas.ProyectoId);
                        ViewBag.UsuarioAsignadoId = new SelectList(db.Usuarios, "UsuarioId", "Nombre", tareas.UsuarioAsignadoId);

                    }
                    else {
                        TempData["Message"] = "No puede eliminar tareas de otro colaborador.";
                        TempData["MessageType"] = "danger";
                    }
                }
            }
            catch (Exception ex) {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "success";
            }
            return View(tareas);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tareas.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
