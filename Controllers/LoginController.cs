using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SMSApp.Models;
using SMSApp.Models.SC;
using System.Diagnostics;
using System.Security.Claims;
using WebTemplate.Models.BLL;
using System.Configuration;
using System.Data;

namespace SMSApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private IConfiguration Configuration;

        public LoginController(ILogger<LoginController> logger, IConfiguration _configuration)
        {
            _logger = logger;
            Configuration = _configuration;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (Configuration.GetSection("AppSettings:IsSSO").Value.ToString() == "Y")
            {
                LoginBLL mLoginBLL = null;
                DataSet mDset = new DataSet();
                mLoginBLL = new LoginBLL();
                string IsUserExists = string.Empty;

                string? userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();

                userName = userName.Split("\\")[1];

                mDset = mLoginBLL.UserAuthenticate(userName, "SSO", Configuration);

                if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
                {
                    IsUserExists = mDset.Tables[0].Rows[0]["IsUserExists"].ToString();

                    if (IsUserExists == "Y")
                    {
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, mDset.Tables[0].Rows[0]["Name"].ToString()),
                        new Claim(ClaimTypes.NameIdentifier, mDset.Tables[0].Rows[0]["UserId"].ToString()),
                        new Claim(ClaimTypes.PrimarySid, mDset.Tables[0].Rows[0]["RoleCode"].ToString()),
                        new Claim(ClaimTypes.Role, mDset.Tables[0].Rows[0]["RoleName"].ToString()),
                        new Claim(ClaimTypes.UserData, mDset.Tables[0].Rows[0]["ProfPic"].ToString())
                    };

                        var claimsIdentity = new ClaimsIdentity(claims, "Login");

                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View("Login");
        }

        // Login user with Username and password
        [HttpPost]
        public ActionResult Submit(LoginSC vLoginSC)
        {
            LoginBLL mLoginBLL = null;
            DataSet mDset = new DataSet();
            mLoginBLL = new LoginBLL();
            string IsUserExists = string.Empty;

            vLoginSC.password = PwdHelper.Encrypt(vLoginSC.password);

            mDset = mLoginBLL.UserAuthenticate(vLoginSC.username, vLoginSC.password, Configuration);

            if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
            {
                IsUserExists = mDset.Tables[0].Rows[0]["IsUserExists"].ToString();

                if (IsUserExists == "Y")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, mDset.Tables[0].Rows[0]["Name"].ToString()),
                        new Claim(ClaimTypes.NameIdentifier, mDset.Tables[0].Rows[0]["UserId"].ToString()),
                        new Claim(ClaimTypes.PrimarySid, mDset.Tables[0].Rows[0]["RoleCode"].ToString()),
                        new Claim(ClaimTypes.Role, mDset.Tables[0].Rows[0]["RoleName"].ToString()),
                        new Claim(ClaimTypes.UserData, mDset.Tables[0].Rows[0]["ProfPic"].ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Login");

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                }
            }

            return Json(new
            {
                IsUserExists = IsUserExists
            });
        }

        public ActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}