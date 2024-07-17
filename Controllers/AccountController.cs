using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Test3.Account.RoleManager;
using Test3.Account.UserManager;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Test3.Account.ClaimManager;

namespace Test3.Controllers
{
    public class AccountController : Controller
    {
        private string _userName = "";
        private readonly IUserAction _userManager;
        private readonly IRoleAction _role;
        private readonly UserModel _user;
        private readonly IClaimAction _claimAction;

        public AccountController(IUserAction users,IRoleAction role,UserModel User,IClaimAction claimAction)
        {
            this._userManager = users;
            this._role = role;
            _user = User;
            this._claimAction = claimAction;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register( UserModel Users, RoleModel Roles)
        {

            bool isregister = _userManager.createUser(Users.Username, Users.Password , Roles.Role, Users.Email);
            if (isregister)
            {
                return RedirectToAction("RegisterSuccessfully");

            }
            else
            {
                return View();
            }



        }

        public IActionResult RegisterSuccessfully()
        {
            return View();
        }

        public IActionResult Login(string ReturnUrl)
        {
            ViewData["ReturnedUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel User,string ReturnUrl)
        {
            bool toAllowLogin = _userManager.LoginUser(User.Username, User.Password);
            User.isloggin = toAllowLogin;
            string role = _role.GetRole(User.Username);
            string roleId = _role.GetRoleId(role);
            List<Claim> roleClaims = _claimAction.GetRoleClaims(roleId);
            if (toAllowLogin)
            {
                _userName = User.Username;
                string roleName = _role.GetRole(User.Username);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, User.Username));
                claims.Add(new Claim(ClaimTypes.Name, User.Username));
                claims.Add(new Claim(ClaimTypes.Role, roleName));
                claims.AddRange(roleClaims);
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);


                HttpContext.Session.SetString("Username", User.Username);
                HttpContext.Session.SetString("Role", roleName);

                if (ReturnUrl == null)
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return Redirect(ReturnUrl);
                }
                


            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Logout(UserModel Users)
        {
            Users.isloggin = false;
            await HttpContext.SignOutAsync();
            HttpContext.Session.Remove("Username");
            HttpContext.Session.Remove("Role");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Manage()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            string email = _userManager.getEmail(username);
            ViewData["Username"] = username;
            ViewData["Role"] = role;
            ViewData["Email"] = email;
            
            return View();
        }

        public ActionResult EditData()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");
            string email = _userManager.getEmail(username);
            ViewData["Username"] = username;
            ViewData["Role"] = role;
            ViewData["Email"] = email;

            return View();
        }

        [HttpPost]
        public ActionResult EditData(UserModel user)
        {
            var username = HttpContext.Session.GetString("Username");
            string Id = _userManager.getIdByName(username);
            bool isupdated = _userManager.updateUser(Id, user.Username, user.Email, user.Role);
            if (isupdated == true)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("Email", user.Email);
                return RedirectToAction("Manage");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public   IActionResult ChangePassword(UserModel user)
        {
            var username = HttpContext.Session.GetString("Username");
            bool isupdated =_userManager.UpdatePassword(username, user.CurrentPassword, user.Password);
            if (isupdated == true)
            {
                return RedirectToAction("Manage");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult DeleteAccount()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewData["Username"] = username;
            return View();
        }

        [HttpPost]
        public IActionResult DeleteAccount(UserModel user)
        {
            var username = HttpContext.Session.GetString("Username");
            bool userDelete =_userManager.deleteUser(username, user.Password);

            if(userDelete == true)
            {
                return RedirectToAction("Logout");
            }
            
            return View();
        }
    }

    
}
