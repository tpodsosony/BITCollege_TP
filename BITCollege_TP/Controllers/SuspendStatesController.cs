using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BITCollege_TP;
using BITCollege_TP.Data;

namespace BITCollege_TP.Controllers
{
    /// <summary>
    /// SuspendState Controller - Controls Suspend State
    /// </summary>
    public class SuspendStatesController : Controller
    {
        private BITCollege_TPContext db = new BITCollege_TPContext();

        // GET: SuspendStates
        public ActionResult Index()
        {
            //return View(db.GradePointStates.ToList());
            //return View(db.SuspendStates.ToList());
            return View(SuspendState.GetInstance());

        }

        // GET: SuspendStates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuspendState suspendState = (SuspendState)db.GradePointStates.Find(id);
            if (suspendState == null)
            {
                return HttpNotFound();
            }
            return View(suspendState);
        }

        // GET: SuspendStates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SuspendStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GradePointStateId,LowerLimit,UpperLimit,TuitionRateFactor")] SuspendState suspendState)
        {
            if (ModelState.IsValid)
            {
                db.GradePointStates.Add(suspendState);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(suspendState);
        }

        // GET: SuspendStates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuspendState suspendState = (SuspendState)db.GradePointStates.Find(id);
            if (suspendState == null)
            {
                return HttpNotFound();
            }
            return View(suspendState);
        }

        // POST: SuspendStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GradePointStateId,LowerLimit,UpperLimit,TuitionRateFactor")] SuspendState suspendState)
        {
            if (ModelState.IsValid)
            {
                db.Entry(suspendState).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(suspendState);
        }

        // GET: SuspendStates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SuspendState suspendState = (SuspendState)db.GradePointStates.Find(id);
            if (suspendState == null)
            {
                return HttpNotFound();
            }
            return View(suspendState);
        }

        // POST: SuspendStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SuspendState suspendState = (SuspendState)db.GradePointStates.Find(id);
            db.GradePointStates.Remove(suspendState);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
