using Newtonsoft.Json;
using Proyecto_Restful_Api.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;

namespace Proyecto_Restful_Api.Controllers
{
    public class ArchiveController : Controller
    {
        // GET: Archive
        public ActionResult Index()
        {
            if(Session["user"] == null)
            {
                return RedirectToAction("login","user");
            }

            String token = (String)Session["user"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8000/api/archives");

                var getTask = client.GetAsync("archives?token="+token);
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var responseData = result.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<List<Archive>>(responseData);
                    return View(jsonResponse.ToList());
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

                return View();
            }
        }

        // GET: Archive/Details/5
        public ActionResult Details(int id)
        {
            String token = (String)Session["user"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8000/api/show/");

                var getTask = client.GetAsync(id + "?token=" + token);
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var responseData = result.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<List<Archive>>(responseData);
                    Archive archive = new Archive();
                    archive.title = jsonResponse.FirstOrDefault().title;
                    archive.body = jsonResponse.FirstOrDefault().body;

                    return View(archive);
                }
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }


            return null;
        }

        // GET: Archive/Create
        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("login", "user");
            }
            return View();
        }

        // POST: Archive/Create
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase uploadFile)
        {
            if (uploadFile==null)
            {
                ModelState.AddModelError(string.Empty, "Error: Select a file");
                return View();
            }

            if (uploadFile.ContentLength == 0 || uploadFile.ContentLength > 50)
            {
                ModelState.AddModelError(string.Empty, "Error: The file must have at least 0 characters in its content and less than 50");
                return View();
            }
            else
            {
                Archive archive = new Archive();
                String token = (String)Session["user"];

                archive.title = Path.GetFileName(uploadFile.FileName).Replace(".txt", string.Empty);
                if (ReadFile(uploadFile) == null)
                {
                    ModelState.AddModelError(string.Empty, "The document contains non-permitted characters");
                    return View();
                }
                archive.body = ReadFile(uploadFile).ToString();

                if (archive.title.Length > 191)
                {
                    ModelState.AddModelError(string.Empty, "Error: The file must be less than 191 characters long in its contents");
                    return View();
                }
            
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:8000/api/archives?token=");
                    var postTask = client.PostAsJsonAsync<Archive>("archives?token=" + token, archive);
                    postTask.Wait();
                    var result = postTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("index", "archive");
                    }

                    ModelState.AddModelError(string.Empty, "Server Error: Please contact administrator.");
                    return View();
                }
            }
        }

        private StringBuilder ReadFile(HttpPostedFileBase uploadFile) {

            StringBuilder strbuild = new StringBuilder();

            try
            {
                var fileName = Path.GetFileName(uploadFile.FileName);
                var filePath = Path.Combine(Server.MapPath("~/Document"), fileName);
                    uploadFile.SaveAs(filePath);

                    if (!string.IsNullOrEmpty(filePath))
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(Server.MapPath("~/Document"), fileName)))
                        {
                            while (sr.Peek() >= 0)
                            {
                                strbuild.AppendFormat(sr.ReadLine());
                            }
                        }
                    }
            }
            catch (Exception)
            {
                return null;
            }

            return strbuild;
        }

        public FileStreamResult CreateFile(int id) {

            String token = (String)Session["user"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8000/api/archives/");

                var getTask = client.GetAsync(id+"?token=" + token);
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var responseData = result.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonConvert.DeserializeObject<List<Archive>>(responseData);

                    var body = jsonResponse.FirstOrDefault().body;
                    var title = jsonResponse.FirstOrDefault().title; 
                    var byteArray = Encoding.ASCII.GetBytes(body);
                    var stream = new MemoryStream(byteArray);

                    return File(stream, "text/plain", title+".txt");
                }
                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            }


            return null;

        }

        public ActionResult Delete(int id)
        {
            String token = (String)Session["user"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8000/api/archives/");

                var getTask = client.DeleteAsync(id + "?token=" + token);
                getTask.Wait();

                var result = getTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

                return RedirectToAction("Index");
            }
        }
    }
}
