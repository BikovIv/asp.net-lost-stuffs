using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LostStuffs.Entities;
using LostStuffs.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace LostStuffs.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

        // GET: Comments
        [AllowAnonymous]
        public ActionResult Index(int id)
        {
            ViewBag.ID = id;
            var comments = db.Comments.Where(x => x.LostStuffId == id).ToList();


            if (user != null)
            {
                ViewData["CurrentUserId"] = user.Id;
            }

            return View(comments);
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            if (user.Id == comment.UserId)
            {
                return View(comment);
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }
        
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            var entity = new Comment()
            {
                LostStuffId = id
            };

            if (user != null)
            {

                return View(entity);
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }
        }

        [HttpPost]
        public ActionResult Create(Comment entity, int id)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;

            entity.LostStuffId = id;
            entity.UserId = user.Id;

            entity.UserName = user.UserName;

            db.Comments.Add(entity);
            db.Entry(entity).State = EntityState.Added;
            db.SaveChanges();

            return RedirectToAction("Details/" + id, new { controller = "LostStuffs" });
        }



        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }

            if (user.Id == comment.UserId)
            {
                return View(comment);
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }

        }

        // POST: Comments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.UpdatedAt = DateTime.Now;

                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index/"+comment.LostStuffId);
                return RedirectToAction("Details/"+comment.LostStuffId,new {controller = "LostStuffs" });
            }

            if (user.Id == comment.UserId)
            {
                return View(comment);
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            if (user.Id == comment.UserId)
            {
                return View(comment);
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (user.Id == comment.UserId)
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                return RedirectToAction("Details/" + comment.LostStuffId, new { controller = "LostStuffs" });
             
            }
            else
            {
                return RedirectToAction("Error", new { controller = "Account" });
            }
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
