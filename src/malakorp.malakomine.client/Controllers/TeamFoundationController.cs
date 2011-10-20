using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Malakorp.TeamFoundation.Client.Controllers
{
    public class TeamFoundationController : Controller
    {
        //
        // GET: /TeamFoundation/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /TeamFoundation/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /TeamFoundation/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /TeamFoundation/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /TeamFoundation/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /TeamFoundation/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /TeamFoundation/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /TeamFoundation/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
