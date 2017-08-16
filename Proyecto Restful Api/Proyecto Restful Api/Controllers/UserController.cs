using Newtonsoft.Json;
using Proyecto_Restful_Api.Models;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace Proyecto_Restful_Api.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8000/api/auth/register");

                var postTask = client.PostAsJsonAsync<User>("register", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "User");
                }

                ModelState.AddModelError(string.Empty, "Error: Data invalid");

                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            using (var client = new HttpClient()) {
                client.BaseAddress = new Uri("http://localhost:8000/api/auth/login");

                var postTask = client.PostAsJsonAsync<User>("login", user);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var responseData = result.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<Token>(responseData);
                    Session["user"] = jsonResponse.token;
                    return RedirectToAction("index", "archive");
                }

                ModelState.AddModelError(string.Empty, "Error: Email or password is incorrect");

                return View();
            }
        }

        public void Logout()
        {
            Session.Abandon();
            Response.Redirect("~/User/Login");
        }
    }
}
