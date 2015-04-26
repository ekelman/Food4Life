using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Food4Life.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using PagedList;

namespace Food4Life.Controllers
{
    public class RecipesController : Controller
    {
        private superhealthyeatsEntities db = new superhealthyeatsEntities();
        int pageSize = 10;

        // GET: /Recipe/
        //[Route("{category:maxlength(50)?}")]
        public ActionResult Index(string category, int? page)
        {
            if (category == null)
            {
                category = "";
            }
            return GetRecipes(page, category);
        }

        private ActionResult GetRecipes(int? page, string category)
        {
            int pageNumber = (page ?? 1);
            ViewBag.slider = db.Recipes.OrderByDescending(m => m.id).Where(s => s.show_slider == true).Take(3).ToList();
            ViewBag.categoryName = category;
            ViewBag.page = page;
            var recipes = db.Recipes.Include(r => r.Category).OrderByDescending(m => m.id);
            if (category != String.Empty)
            {
                recipes = recipes.Where(c => c.Category.name == category).OrderByDescending(m => m.id);
            }
            GetCategories();

            if (recipes.ToPagedList(pageNumber, pageSize).Count == 0)
            {
                if (pageNumber == 1)
                    return View(recipes.ToPagedList(pageNumber, pageSize));
                else
                    return View(recipes.ToPagedList(pageNumber - 1, pageSize));
            }
            else
            {
                return View(recipes.ToPagedList(pageNumber, pageSize));
            }
            //return View(recipes.ToList());
        }

        private void GetCategories()
        {
            ViewBag.category = db.Categories.ToList().OrderBy(m => m.name);
        }

        // GET: /Recipe/Details/5
        //[Route("{description:maxlength(50)}/{category:maxlength(50)?}/{page:int?}")]
        public ActionResult Details(string description, string category, int? page)
        {
            ViewBag.categoryName = category;
            ViewBag.page = page;

            if (description == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index", "Recipes");
            }
            //Recipe recipe = db.Recipes.Find(description);
            var recipe = db.Recipes.Where(d => d.description == description);
            if (recipe == null)
            {
                return RedirectToAction("Index", "Recipe");
            }
            GetCategories();
            var materializedreRecipe = recipe.SingleOrDefault();
            ViewBag.total_cook_time = materializedreRecipe.cook_time + materializedreRecipe.prep_time;
            return View(materializedreRecipe);
        }

        // GET: /Recipe/Create
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.category_id_list = new SelectList(db.Categories, "id", "name");
                GetCategories();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Recipes");
            }
        }

        // POST: /Recipe/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "id,category_id,title,description,details,ratings,slider_image,thumnail_image,ingredients_thumbnail_image,show_slider,instructions,prep_time,cook_time,serves")] Recipe recipe)
        {
            if (Request.IsAuthenticated)
            {
                //Validate description
                ModelState.Remove("Description");
                if (recipe.description == null)
                {
                    ModelState.AddModelError("Description", "Description is required.");
                    ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                    GetCategories();
                    return View(recipe);
                }
                var recipeCheck = db.Recipes.Where(d => d.description == recipe.description);
                if (recipeCheck.Count() != 0)
                {
                    ModelState.AddModelError("Description", "Item alreday exists.");
                    ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                    GetCategories();
                    return View(recipe);
                }

                if (ModelState.IsValid && Request.Files[0].FileName != String.Empty)
                {
                    ProcessImageFile(recipe);
                    recipe.details = recipe.details.Replace("&nbsp;", " ");
                    db.Recipes.Add(recipe);
                    db.SaveChanges();
                    GetCategories();
                    return RedirectToAction("Index");
                }
                if (Request.Files[0].FileName == String.Empty)
                {
                    ModelState.AddModelError("ErrorImage", "Image is required.");
                }
                ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                GetCategories();
                return View(recipe);
            }
            else
            {
                return RedirectToAction("Index", "Recipes");
            }
        }

        // GET: /Recipe/Edit/5
        public ActionResult Edit(int? recipeId)
        {
            if (Request.IsAuthenticated)
            {
                if (recipeId == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return RedirectToAction("Index", "Recipes");
                }
                Recipe recipe = db.Recipes.Find(recipeId);
                if (recipe == null)
                {
                    return HttpNotFound();
                }
                ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                GetCategories();
                return View(recipe);
            }
            else
            {
                return RedirectToAction("Index", "Recipes");
            }
        }

        // POST: /Recipe/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "id,category_id,title,description,details,ratings,slider_image,thumnail_image,ingredients_thumbnail_image,show_slider,instructions,prep_time,cook_time,serves")] Recipe recipe)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    //Validate description
                    ModelState.Remove("Description");
                    if (recipe.description == null)
                    {
                        ModelState.AddModelError("Description", "Description is required.");
                        ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                        GetCategories();
                        return View(recipe);
                    }
                    var recipeCheck = db.Recipes.Find(recipe.id);
                    if (recipeCheck.description != recipe.description)
                    {
                        var recipeCheckDup = db.Recipes.Where(d => d.description == recipe.description);
                        if (recipeCheckDup.Count() != 0)
                        {
                            ModelState.AddModelError("Description", "Item alreday exists.");
                            ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                            GetCategories();
                            return View(recipe);
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        var list = db.Recipes.Find(recipe.id);
                        list.category_id = recipe.category_id;
                        list.title = recipe.title;
                        list.description = recipe.description;
                        list.details = recipe.details;
                        list.ratings = recipe.ratings;
                        list.show_slider = recipe.show_slider;
                        list.instructions = recipe.instructions;
                        list.prep_time = recipe.prep_time;
                        list.cook_time = recipe.cook_time;
                        list.serves = recipe.serves;
                        if (Request.Files[0].FileName != String.Empty)
                        {
                            ProcessImageFile(list);
                        }
                        if (Request.Files[1].FileName != String.Empty)
                        {
                            list.ingredients_thumbnail_image = ResizeSaveImage(1, 600, 400, list.ingredients_thumbnail_image);
                        }
                        db.SaveChanges();

                        GetCategories();
                        ViewBag.message = "<p class=\"text-danger\">Successfully saved item.</p>";
                        //return RedirectToAction("Index");
                    }
                    ViewBag.category_id_list = new SelectList(db.Categories, "id", "name", recipe.category_id);
                    GetCategories();
                    return View(recipe);
                }
                else
                {
                    return RedirectToAction("Index", "Recipes");
                }
            }
            catch (Exception ex)
            {
                ViewBag.message = "<p class=\"text-success\">Error, could not save.</p>";
                return View(recipe);
            }
        }

        // GET: /Recipe/Delete/5
        public ActionResult Delete(int? recipeId)
        {
            if (Request.IsAuthenticated)
            {
                if (recipeId == null)
                {
                    //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    return RedirectToAction("Index", "Recipe");
                }
                Recipe recipe = db.Recipes.Find(recipeId);
                if (recipe == null)
                {
                    return HttpNotFound();
                }
                db.Recipes.Remove(recipe);
                db.SaveChanges();
                DeleteImageFile(recipe.slider_image);
                DeleteImageFile(recipe.thumnail_image);
                if (recipe.ingredients_thumbnail_image != null)
                {
                    DeleteImageFile(recipe.ingredients_thumbnail_image);
                }
                GetCategories();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index", "Recipes");
            }
        }

        private void DeleteImageFile(string fileName)
        {
            string fileToDelete = HttpContext.Server.MapPath("~") + fileName.Replace("~", "");
            System.IO.File.Delete(fileToDelete);
        }

        private void ProcessImageFile(Recipe recipe)
        {
            recipe.thumnail_image = ResizeSaveImage(0, 600, 400, recipe.thumnail_image);
            recipe.slider_image = ResizeSaveImage(0, 1248, 460, recipe.slider_image);
            if (Request.Files[1].FileName != String.Empty) 
            {
                //recipe.ingredients_thumbnail_image = ResizeSaveImage(1, 280, 280, recipe.ingredients_thumbnail_image);
                recipe.ingredients_thumbnail_image = ResizeSaveImage(1, 600, 400, recipe.ingredients_thumbnail_image);
            }
        }

        private static string GenerateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[16];
            rng.GetBytes(buff);
            string salt = Convert.ToBase64String(buff).Replace("\\", "").Replace("/", "").Replace("+","").Replace("=","");
            return salt;
        }

        private string ResizeSaveImage(int fileIndex, int width, int height, string oldImageName)
        {
            string imageName ="";
            string salt = GenerateSalt();

            ImageFormat format = GetImageFormat(Request.Files[fileIndex].FileName);
            byte[] firstImageBytes = GetResizedImage(Request.Files[fileIndex].InputStream, format, width, height);
            string firstImagePath = Server.MapPath("~/Images/" + salt + "_" + width + "X" + height + "_" + System.IO.Path.GetFileName(Request.Files[fileIndex].FileName));
            System.IO.File.WriteAllBytes(firstImagePath, firstImageBytes);

            //delete old image file
            if (oldImageName != null)
            {
                string fileToDelete = HttpContext.Server.MapPath("~") + oldImageName.Replace("~","");
                System.IO.File.Delete(fileToDelete);
            }

            return imageName = "~\\Images\\" + salt + "_" + width + "X" + height + "_" + System.IO.Path.GetFileName(Request.Files[fileIndex].FileName);
        }

        byte[] GetResizedImage(Stream originalStream, ImageFormat imageFormat, int width, int height)
        {
            Bitmap imgIn = new Bitmap(originalStream);
            double y = imgIn.Height;
            double x = imgIn.Width;

            double factor = 1;
            if (width > 0)
            {
                factor = width / x;
            }
            else if (height > 0)
            {
                factor = height / y;
            }
            System.IO.MemoryStream outStream = new System.IO.MemoryStream();
            Bitmap imgOut = new Bitmap((int)(x * factor), (int)(y * factor));

            // Set DPI of image (xDpi, yDpi)
            imgOut.SetResolution(72, 72);

            Graphics g = Graphics.FromImage(imgOut);
            g.Clear(Color.White);
            g.DrawImage(imgIn, new Rectangle(0, 0, (int)(factor * x), (int)(factor * y)),
              new Rectangle(0, 0, (int)x, (int)y), GraphicsUnit.Pixel);

            imgOut.Save(outStream, imageFormat);
            return outStream.ToArray();
        }

        ImageFormat GetImageFormat(String path)
        {
            switch (Path.GetExtension(path))
            {
                case ".bmp": return ImageFormat.Bmp;
                case ".gif": return ImageFormat.Gif;
                case ".jpg": return ImageFormat.Jpeg;
                case ".png": return ImageFormat.Png;
                default: break;
            }
            return ImageFormat.Jpeg;
        }

        private static string GenerateSaltValue()
        {
            UnicodeEncoding utf16 = new UnicodeEncoding();

            if (utf16 != null)
            {
                // Create a random number object seeded from the value
                // of the last random seed value. This is done
                // interlocked because it is a static value and we want
                // it to roll forward safely.

                Random random = new Random(unchecked((int)DateTime.Now.Ticks));

                if (random != null)
                {
                    // Create an array of random values.

                    byte[] saltValue = new byte[16];

                    random.NextBytes(saltValue);

                    // Convert the salt value to a string. Note that the resulting string
                    // will still be an array of binary values and not a printable string. 
                    // Also it does not convert each byte to a double byte.

                    string saltValueString = utf16.GetString(saltValue);

                    // Return the salt value as a string.

                    return saltValueString;
                }
            }

            return null;
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
