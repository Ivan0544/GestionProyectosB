using GestionProyectosB.DTO;
using GestionProyectosB.Modelo;
using GestionProyectosB.Service;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace GestionProyectosB.Controllers
{
    public class LoginController : Controller
    {
        //ProyectosEntities db = new ProyectosEntities();
        DataRepository<Usuarios> user = new DataRepository<Usuarios>();
        LoginUser login = new LoginUser();

        [HttpGet]
        public async Task<ActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            var usuario = await login.GetUser(email);

            //Debug.WriteLine("usuario: " + usuario.Email);
            //Debug.WriteLine("usuario: " + usuario.Contraseña);
            if (usuario != null)
            {
                if (email != null && email == usuario.Email)
                {
                    if (password != null && password == usuario.Contraseña)
                    {
                        Session["usuario"] = usuario;

                        TempData["Message"] = "Acceso correcto.";
                        TempData["MessageType"] = "success";
                        return RedirectToAction("Index", "Proyectos");
                    }
                    else
                    {
                        Debug.WriteLine("No se encontró usuario.");
                        TempData["Message"] = "Usuario o contrasena incorrecta.";
                        TempData["MessageType"] = "danger";
                        return View();
                    }

                }
                else
                {

                    TempData["Message"] = "Usuario o contrasena incorrecta.";
                    TempData["MessageType"] = "danger";
                    return View();
                }
            }
            else
            {
                TempData["Message"] = "Usuario o contrasena incorrecta.";
                TempData["MessageType"] = "danger";

            }
             return RedirectToAction("Login", "Login");
        }

        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await login.GetUser(email);
            if (user != null)
            {
                TempData["Message"] = "Se ha enviado recuperacion de contrasena al correo registrado";
                TempData["MessageType"] = "success";
                ViewBag.Message = "Una nueva contraseña ha sido enviada a tu correo electrónico.";
            }
            else
            {
                TempData["Message"] = "No se encontro un usuario con ese correo electronico";
                TempData["MessageType"] = "danger";
                ViewBag.ErrorMessage = "No se encontró un usuario con ese correo electrónico.";
            }

            return View("Login");
        }

        public string EncriptarContraseña(string contraseña)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contraseña));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }




    }
}