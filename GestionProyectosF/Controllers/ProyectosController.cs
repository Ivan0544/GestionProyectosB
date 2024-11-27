using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionProyectosB.Modelo;
using GestionProyectosB.DTO;
using GestionProyectosB;
using GestionProyectosB.Service;


namespace ProyectosF.Controllers
{
    public class ProyectosController : Controller
    {
        //private ProyectosEntities db = new ProyectosEntities();
        Usuarios logUser = new Usuarios();
        DataRepository<Proyectos> Pro = new DataRepository<Proyectos>();
        DataRepository<ProyectoUsuarios> proUser = new DataRepository<ProyectoUsuarios>();
        
        ProyectosUser consulPro = new ProyectosUser();

        // GET: Proyectos
        public async Task<ActionResult> Index()
        {
            try
            {
                var usuario = Session["usuario"] as Usuarios;
                logUser = usuario;
                ViewBag.NombreUsuario = logUser.Nombre;

                if (usuario != null)
                {
                    if (usuario.Rol != "Administrador")
                    {
                        var proyectos = await consulPro.ListPro(usuario);
                        return View(proyectos);
                    }
                    else {
                        var proyectos = await Pro.getAll();
                        return View(proyectos);
                    }
                }
            }
            catch (Exception ex) { 
            
            }

            ViewBag.ErrorMessage = "No se encontraron proyectos para este usuario.";
            return View(); 
        }

        // GET: Proyectos/Details/5
        //public async ActionResult AsignProyecto()
        //{
        //    ViewBag.Usuarios = await  // Método que devuelve la lista de usuarios
        //    return View();
        //}
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyectos proyectos = await Pro.getById(id);
            if (proyectos == null)
            {
                return HttpNotFound();
            }
            return View(proyectos);
        }

        // GET: Proyectos/Create
        public ActionResult Create()
        {
            var usuario = Session["usuario"] as Usuarios;

            if (usuario.Rol == "Administrador")
            {
                return View();
            }
            else {
                TempData["Message"] = "No es administrador.";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index");
        }

        // POST: Proyectos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProyectoId,Nombre,Descripcion,FechaInicio,FechaFin,Estado,Prioridad")] Proyectos proyectos)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProyectoUsuarios pu = new ProyectoUsuarios();
                    var usuario = Session["usuario"] as Usuarios;
                    await Pro.Create(proyectos);

                    TempData["Message"] = "Proyecto creado con Exito.";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Index");
                }   
            }
            catch (Exception ex) {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";

            }
            return View(proyectos);

        }

        // GET: Proyectos/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyectos proyectos = await Pro.getById(id);
            if (proyectos == null)
            {
                return HttpNotFound();
            }
            return View(proyectos);
        }

        // POST: Proyectos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProyectoId,Nombre,Descripcion,FechaInicio,FechaFin,Estado,Prioridad")] Proyectos proyectos)
        {
            try
            {
                var usuario = Session["usuario"] as Usuarios;
                if (usuario.Rol == "Administrador")
                {
                    if (ModelState.IsValid)
                    {
                        await Pro.Update(proyectos);
                        TempData["Message"] = "Proyecto modificado con exito.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    TempData["Message"] = "No es administrador.";
                    TempData["MessageType"] = "danger";
                }
            }
            catch (Exception ex) {

                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";
            }
            return View(proyectos);
        }

        // GET: Proyectos/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proyectos proyectos = await Pro.getById(id);
            if (proyectos == null)
            {
                return HttpNotFound();
            }
            return View(proyectos);
        }


        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var usuario = Session["usuario"] as Usuarios;
                if (usuario.Rol == "Administrador")
                {
                    Proyectos proyectos = await Pro.getById(id);
                    await Pro.Delete(proyectos);
                    TempData["Message"] = "Proyecto Eliminado con exito.";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Index");
                }
                else {
                    TempData["Message"] = "No es administrador";
                    TempData["MessageType"] = "danger";
                }
            }
            catch (Exception ex) {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index");
        }


        // GET: Proyectos/AsignarProyecto
        public async Task<ActionResult> AsignarProyecto()
        {
            try
            {
                var usuario = Session["usuario"] as Usuarios;
                if (usuario == null || usuario.Rol != "Administrador")
                {
                    TempData["Message"] = "No tiene permisos para asignar proyectos.";
                    TempData["MessageType"] = "danger";
                    return View();
                }

                // Obtener los usuarios y proyectos para la asignación
                var usuarios = await new DataRepository<Usuarios>().getAll();
                var proyectos = await Pro.getAll();

                // Pasar los datos a la vista
                ViewBag.Usuarios = usuarios;
                ViewBag.Proyectos = proyectos;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AsignarProyecto(int usuarioId, int proyectoId)
        {
            try
            {
                // Verificar si el proyecto y el usuario existen
                var proyecto = await Pro.getById(proyectoId);
                var usuario = await new DataRepository<Usuarios>().getById(usuarioId);

                if (proyecto == null || usuario == null)
                {
                    TempData["Message"] = "El proyecto o el usuario no existen.";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }

                // Crear la asignación
                ProyectoUsuarios asignacion = new ProyectoUsuarios
                {
                    UsuarioId = usuarioId,
                    ProyectoId = proyectoId
                };

                // Guardar la asignación
                await proUser.Create(asignacion);

                TempData["Message"] = "Proyecto asignado al usuario con éxito.";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Pro.Dispose();
            }
            base.Dispose(disposing);
        }
       
    }
}
