using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeCart.Models;
using WeCart.ViewModels;

namespace WeCart.Controllers
{
    public class UserController : Controller
    {
        private UserDBEntities db = new UserDBEntities();

        // GET: User/Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password
                };

                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: User/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    Session["UserName"] = user.Name;
                    Session["UserID"] = user.Id;
                    return RedirectToAction("Home", "Home");
                }
                ModelState.AddModelError("Password", "Invalid email or password.");
            }

            return View(model);
        }

        // GET: User/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Home", "Home");
        }

        // GET: User/Profile
        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login");
            }

            int userId = (int)Session["UserID"];
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new ProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                ProfileImage = user.ProfileImage
            };

            return View(model);
        }

        // POST: User/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(ProfileViewModel model, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserID"];
                var user = db.Users.Find(userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                user.Name = model.Name;
                user.Email = model.Email;

                if (upload != null && upload.ContentLength > 0)
                {
                    var path = Server.MapPath("~/Uploads/");
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    var fileName = System.IO.Path.GetFileName(upload.FileName);
                    var filePath = System.IO.Path.Combine(path, fileName);
                    upload.SaveAs(filePath);
                    user.ProfileImage = "/Uploads/" + fileName;
                }

                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Home", "Home");
            }

            return View(model);
        }

        // GET: User/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        // POST: User/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserID"];
                var user = db.Users.Find(userId);

                if (user == null)
                {
                    return HttpNotFound();
                }

                if (user.Password != model.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                    return View(model);
                }

                user.Password = model.NewPassword;
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Profile");
            }

            return View(model);
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
