using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GestionProyectosB.Modelo;
using GestionProyectosB.Service;

namespace GestionProyectosF.Controllers
{
    public class UsuariosController : Controller
    {
        DataRepository<Usuarios> user = new DataRepository<Usuarios>();

        // GET: Usuarios
        public async Task<ActionResult> Index()
        {
            var usuario = Session["usuario"] as Usuarios;
            if (usuario.Rol == "Administrador")
            {
                var usuarios = await user.getAll();
                return View(usuarios);
            }
            else {
                TempData["Message"] = "No es administrador";
                TempData["MessageType"] = "danger";
            }
            return RedirectToAction("Index", "Proyectos");

        }

        // GET: Usuarios/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = await user.getById(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            return  View();
        }

        // POST: Usuarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> Create([Bind(Include = "UsuarioId,Nombre,Email,Rol,Contraseña,FechaRegistro")] Usuarios usuario)
        {
            try
            {
                var usuarios = await user.getAll();
                bool userExist = usuarios.Any(u => u.Email == usuario.Email);
                if (!userExist)
                {
                    if (ModelState.IsValid)
                    {
                        await user.Create(usuario);
                        TempData["Message"] = "Usuario creado exitosamente.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("index");
                    }
                }
                else
                {
                    TempData["Message"] = "Usuario ya existe.";
                    TempData["MessageType"] = "danger";
                }
            }
            catch (Exception ex) {
                TempData["Message"] = "Error al registrar usuario"+ex;
                TempData["MessageType"] = "danger";
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async  Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = await user.getById(id);
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // POST: Usuarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "UsuarioId,Nombre,Email,Rol,Contraseña,FechaRegistro")] Usuarios usuarios)
        {
            if (ModelState.IsValid)
            {
                await user.Update(usuarios);
                return RedirectToAction("Index");
            }
            return View(usuarios);
        }

        // GET: Usuarios/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuarios usuarios = await user.getById(id) ;
            if (usuarios == null)
            {
                return HttpNotFound();
            }
            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Usuarios usuarios = await user.getById(id);
            await user.Delete(usuarios);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                user.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
