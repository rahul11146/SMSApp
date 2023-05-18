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
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.NetworkInformation;

namespace SMSApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LoginController(ILogger<LoginController> logger, IConfiguration _configuration, IWebHostEnvironment? webHostEnvironment)
        {
            _logger = logger;
            Configuration = _configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log1605.txt", "Start 1" + Environment.NewLine);
            System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log1605.txt", Configuration.GetSection("AppSettings:IsSSO").Value.ToString() + Environment.NewLine);

            if (Configuration.GetSection("AppSettings:IsSSO").Value.ToString() == "Y")
            {
                LoginBLL mLoginBLL = null;
                DataSet mDset = new DataSet();
                mLoginBLL = new LoginBLL();
                string? IsUserExists = string.Empty;

                //string userName = string.Empty;
                //if (System.Web.HttpContext.Current != null &&
                //    System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                //{
                //    System.Web.Security.MembershipUser usr = Membership.GetUser();
                //    if (usr != null)
                //    {
                //        userName = usr.UserName;
                //    }
                //}
                //System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log1605.txt", userName + Environment.NewLine);

                mDset = mLoginBLL.UserAuthenticate("", "SSO", Configuration);

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

        public string GetFQDN()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();
            string fqdn;
            if (!hostName.Contains(domainName))
                fqdn = hostName + "." + domainName;
            else
                fqdn = hostName;

            return fqdn;
        }

        private IPAddress GetDnsAdress()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
                    IPAddressCollection dnsAddresses = ipProperties.DnsAddresses;

                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        return dnsAdress;
                    }
                }
            }

            throw new InvalidOperationException("Unable to find DNS Address");
        }

    }
}