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
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LostStuffs.Controllers
{
    public class LostStuffsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        List<LostStuff> lostStuffs = new List<LostStuff>();
        LostStuff lost = new LostStuff();

        public async Task<ActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var items = from s in db.LostStuffs
                        select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString)
                                       || s.CreatedAt.ToString().Contains(searchString));
            }
            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        items = items.OrderByDescending(s => s.Description);
            //        break;
            //    case "Date":
            //        items = items.OrderBy(s => s.CreatedAt);
            //        break;
            //    case "date_desc":
            //        items = items.OrderByDescending(s => s.UpdatedAt);
            //        break;
            //    default:
            //        items = items.OrderBy(s => s.Description);
            //        break;
            //}

            var lastPosted = db.LostStuffs.OrderByDescending(x => x.CreatedAt).Take(4).ToList();
            ViewData["MyData"] = lastPosted; // Send this list to the view

            return View(await items.OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync());
        }
        
        public ActionResult Details(int? id)
        {
            string directoryPath =Server.MapPath("~/Content/UploadedFiles/" + id + "/");

            List<string> imageNamesList = new List<string>();

            var imagePathList = Directory.GetFiles(directoryPath).ToList();
            foreach (var image in imagePathList)
            {
                var imageName = Path.GetFileName(image);
                imageNamesList.Add("~/Content/UploadedFiles/" + id + "/"+imageName);        
            }
            ViewData["ImagePathList"] = imageNamesList;

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                LostStuff lostStuff = db.LostStuffs.Find(id);

                if (lostStuff == null)
                {
                    return HttpNotFound();
                }

                return View(lostStuff);           
        }

        // GET: LostStuffs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LostStuffs/Create       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LostStuff lostStuff, IEnumerable<HttpPostedFileBase> files, HttpPostedFileBase mainImage)
        {          
            if (ModelState.IsValid)
            {
                //create entity
                lostStuff.CreatedAt = DateTime.Now;
                lostStuff.UpdatedAt = DateTime.Now;
                db.LostStuffs.Add(lostStuff);
                db.SaveChanges();
               
                //create directory for entity
                string directoryPath = Server.MapPath(string.Format("~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/"));
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //save main image
                lostStuff.ImageName = mainImage.FileName;
                lostStuff.ImagePath = "~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/" + mainImage.FileName;
                db.SaveChanges();
                var mainFileName = Path.GetFileName(mainImage.FileName);
                var path = "~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/";
                mainImage.SaveAs(Server.MapPath(path + mainFileName));

                //save other images
                foreach (var file in files)
                {   
                    //lostStuff.ImageName = file.FileName;
                    //lostStuff.ImagePath = "~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/" + file.FileName;
                    //db.SaveChanges();
                    var fileName = Path.GetFileName(file.FileName);
                    //var path = "~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/";
                    file.SaveAs(Server.MapPath(path + fileName));
                }

                return RedirectToAction("Index");
            }

            return View(lostStuff);
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {

            LostStuff entity = db.LostStuffs.Find(id);

            //get all images for entity and send to view
            string directoryPath = Server.MapPath("~/Content/UploadedFiles/" + id + "/");

            List<string> imageFullPathList = new List<string>();
            List<string> imageNameList = new List<string>();

            var imagePathList = Directory.GetFiles(directoryPath).ToList();
            foreach (var image in imagePathList)
            {
                var imageName = Path.GetFileName(image);
                imageFullPathList.Add("~/Content/UploadedFiles/" + id + "/" + imageName);
                imageNameList.Add(imageName);
               
            }
            ViewData["ImagePathList"] = imageFullPathList;
            ViewData["ImageNameList"] = imageNameList;
            
            return View(entity);
        }

        [HttpPost]
        public ActionResult Edit(LostStuff entity)
        {
           
            //get all images for entity and send to view
            string directoryPath = Server.MapPath("~/Content/UploadedFiles/" + entity.Id + "/");

            List<string> imageNamesList = new List<string>();

            var imagePathList = Directory.GetFiles(directoryPath).ToList();
            foreach (var image in imagePathList)
            {
                var imageName = Path.GetFileName(image);
                imageNamesList.Add("~/Content/UploadedFiles/" + entity.Id + "/" + imageName);
            }
            ViewData["ImagePathList"] = imageNamesList;
          

            //update entity
            entity.UpdatedAt = DateTime.Now;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
       
        // GET: LostStuffs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LostStuff lostStuff = db.LostStuffs.Find(id);
            if (lostStuff == null)
            {
                return HttpNotFound();
            }

            return View(lostStuff);
        }

        // POST: LostStuffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            LostStuff lostStuff = db.LostStuffs.Find(id);
            string directoryPath = Server.MapPath(string.Format("~/Content/UploadedFiles/" + lostStuff.Id.ToString() + "/"));
              
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string pathFile in files)
            {
                FileInfo file = new FileInfo(pathFile);
                if (file.Exists)//check file exsit or not
                {
                    file.Delete();         
                }
            }
            Directory.Delete(directoryPath);

            db.LostStuffs.Remove(lostStuff);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UploadImage( HttpPostedFileBase file, int id)
        {
            string directoryPath = Server.MapPath(string.Format("~/Content/UploadedFiles/" + id + "/"));

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.GetFileName(file.FileName);
            var path = "~/Content/UploadedFiles/" + id + "/";
            file.SaveAs(Server.MapPath(path + fileName));

            return RedirectToAction("Edit/" + id);
        }

        public ActionResult DeleteImage(string fileName, int id)
        {
            System.IO.File.Delete(Request.PhysicalApplicationPath + "Content/UploadedFiles/" + id + "/" + fileName);
            return RedirectToAction("Edit/" + id);
        }

        public ActionResult ChangeMainImage(string imageName, int id)
        {
            LostStuff entity = db.LostStuffs.Find(id);

            entity.ImageName = imageName;
            entity.ImagePath = "~/Content/UploadedFiles/" + entity.Id + "/" + imageName;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Edit/" + id);
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