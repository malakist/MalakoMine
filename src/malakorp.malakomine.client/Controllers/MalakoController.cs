using System.Linq;
using System.Web.Mvc;
using Malakorp.MalakoMine.Client.Models;
using System.Collections.Generic;
using System.Web.Caching;

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
            var reason = task.State == "Closed" ? (f["Reason"] ?? "").ToString() : "";

            repository.UpdateTask(task, reason);

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

        public void CreateBugTask(Task task)
        {
            repository.CreateBugTask(task);
        }

        public IEnumerable<string> GetUserList()
        {
            return repository.GetUsers();
        }

        [ChildActionOnly]
        public ActionResult GetReasons(int id)
        {
            return PartialView(repository.GetReasons(id));
        }

        public ActionResult Login()
        {
            return View();
        }
    }
}
