using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Authorization_Authentication.Account.RoleManager;
using Authorization_Authentication.Account.UserManager;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Authorization_Authentication.Account.ClaimManager;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Authorization_Authentication.Controllers
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
            bool onLockout = false;
            int totalFailedCount = 3;
            int remainingAttempt = 3;

            //retriving last login attempt time and adding time (after which we nee to reset failattempt) to it and then comparing it with current time , accordingly reseting the time.
            string serializedLastTime = HttpContext.Session.GetString("LastActivity");
            DateTime lastActivity;
            if (!string.IsNullOrEmpty(serializedLastTime))
            {
                lastActivity = JsonConvert.DeserializeObject<DateTime>(serializedLastTime);
                //giving 1 min for testing purpose
                DateTime lastLoginAttempt =lastActivity.AddMinutes(1);
                if(lastLoginAttempt < DateTime.UtcNow)
                {
                    _userManager.resetFailCount(User.Username);
                }
            }
           

            //checking if the account is lock or not
            bool isAccountLock = _userManager.getLockAccountdate(User.Username);

            if (!isAccountLock)
            {

                bool toAllowLogin = _userManager.LoginUser(User.Username, User.Password);
                User.isloggin = toAllowLogin;
                string role = _role.GetRole(User.Username);
                string roleId = _role.GetRoleId(role);
                List<Claim> roleClaims = _claimAction.GetRoleClaims(roleId) ?? new List<Claim>();

                //checking that if the user is authorized or not
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
                    bool reset = _userManager.resetFailCount(User.Username);


                    HttpContext.Session.SetString("Username", User.Username);
                    HttpContext.Session.SetString("Role", roleName);

                    if (ReturnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(ReturnUrl);
                    }
                }
                else
                {
                    remainingAttempt = _userManager.accessFailedCount(User.Username, totalFailedCount);

                    //storing last active time of user 
                    DateTime now = DateTime.UtcNow;
                    string serializedTime = JsonConvert.SerializeObject(now);
                    HttpContext.Session.SetString("LastActivity", serializedTime);

                    if (remainingAttempt <= 0)
                    {
                        bool setlock = _userManager.setLockAccountdate(User.Username);
                        //if no attempt remaining then lock the account and set lockout to 1

                    }
                    else
                    {
                        
                        ViewData["remainingAttempt"] = remainingAttempt;
                    }
                    return View();
                }
            }
            else
            {
                onLockout = true;
                ViewData["onLockout"] = onLockout;
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
