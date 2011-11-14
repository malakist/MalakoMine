using System.Linq;
using System.Web.Mvc;
using Malakorp.MalakoMine.Client.Models;
using System.Collections.Generic;
using System.Web.Caching;
using System.Json;

namespace Malakorp.MalakoMine.Client.Controllers
{
    public class MalakoController : Controller
    {
        readonly ITaskRepository repository;

        public MalakoController(ITaskRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Edit(Task task, FormCollection f)
        {
            var taskReason = (f["Reason"] ?? "").ToString();
            var bugReason = (f["BugReason"] ?? "").ToString();

            if (ModelState.IsValid)
            {
                repository.UpdateTask(task, taskReason, bugReason);
            }

            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var tasks = repository.GetTasks().ToList();

            ViewBag.Users = HttpContext.Cache["users"] as IEnumerable<string> ?? repository.GetUsers();

            return View(tasks);
        }

        [HttpPost]
        public ActionResult Index(Task task)
        {
            repository.CreateBugTask(task);

            return RedirectToAction("Index");
        }

        public ActionResult CreateBugTask(Task task)
        {
            repository.CreateBugTask(task);

            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult BugReason(int id)
        {
            return PartialView(repository.GetReasons(id, "Closed"));
        }

        public JsonResult GetReason(int id, string state)
        {
            return Json(repository.GetReasons(id, state), JsonRequestBehavior.AllowGet);
        }
        [ChildActionOnly]
        public string GetReasons(int id)
        {
            //Uma bagunça horrível pra montar um json simples. C# coxinha...
            return new JsonObject(repository.GetReasons(id).Select(g =>
                new KeyValuePair<string, JsonValue>(g.Key, new JsonArray(g.Select(x =>
                    new JsonObject(new KeyValuePair<string, JsonValue>("val", (JsonValue) x))))))
            ).ToString();
        }
    }
}