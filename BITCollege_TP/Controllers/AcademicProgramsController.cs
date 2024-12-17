﻿using System;
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
    /// Academic Program Controller - Controls Academic Program
    /// </summary>
    public class AcademicProgramsController : Controller
    {
        private BITCollege_TPContext db = new BITCollege_TPContext();

        // GET: AcademicPrograms
        public ActionResult Index()
        {
            return View(db.AcademicPrograms.ToList());
        }

        // GET: AcademicPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // GET: AcademicPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AcademicPrograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AcademicProgramId,ProgramAcronym,Description")] AcademicProgram academicProgram)
        {
            if (ModelState.IsValid)
            {
                db.AcademicPrograms.Add(academicProgram);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(academicProgram);
        }

        // GET: AcademicPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // POST: AcademicPrograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AcademicProgramId,ProgramAcronym,Description")] AcademicProgram academicProgram)
        {
            if (ModelState.IsValid)
            {
                db.Entry(academicProgram).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(academicProgram);
        }

        // GET: AcademicPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);
            if (academicProgram == null)
            {
                return HttpNotFound();
            }
            return View(academicProgram);
        }

        // POST: AcademicPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AcademicProgram academicProgram = db.AcademicPrograms.Find(id);

            //Clears the relationship between the objects to be deleted
            //and the corresponing Student and Course records
            academicProgram.Student.Clear();
            academicProgram.Course.Clear();

            db.AcademicPrograms.Remove(academicProgram);
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
