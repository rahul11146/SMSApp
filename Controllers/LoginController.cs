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
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using Microsoft.AspNetCore.Authorization;

namespace SMSApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(ILogger<LoginController> logger, IConfiguration _configuration,
                               IWebHostEnvironment? webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            Configuration = _configuration;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public string? GetUser()
        {
            return _httpContextAccessor.HttpContext.User?.Identity?.Name;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("Login");
        }

        [HttpPost]
        public JsonResult LoginWithSSO()
        {
            this.DebugLog("Start 1");
            this.DebugLog("SSO Entry Check");

            LoginBLL mLoginBLL = null;
            DataSet mDset = new DataSet();
            mLoginBLL = new LoginBLL();
            string? IsUserExists = string.Empty;

            //string userName = Environment.UserName;

            string userName = Environment.UserName;

            this.DebugLog("SSO Username : " + userName);

            try
            {
                System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log\\" + "LogFile.txt", userName);
            }
            catch (Exception)
            {
            }

            mDset = mLoginBLL.UserAuthenticate(userName, "SSO", Configuration);

            if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
            {
                this.DebugLog("SSO Details Count : " + mDset.Tables[0].Rows.Count.ToString());

                IsUserExists = mDset.Tables[0].Rows[0]["IsUserExists"].ToString();

                this.DebugLog("User Exists : " + IsUserExists);

                if (IsUserExists == "Y")
                {
                    var claims = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, mDset.Tables[0].Rows[0]["Name"].ToString()),
                        new Claim(ClaimTypes.NameIdentifier, mDset.Tables[0].Rows[0]["UserId"].ToString()),
                        new Claim(ClaimTypes.PrimarySid, mDset.Tables[0].Rows[0]["RoleCode"].ToString()),
                        new Claim(ClaimTypes.Role, mDset.Tables[0].Rows[0]["RoleName"].ToString()),
                        new Claim(ClaimTypes.UserData, mDset.Tables[0].Rows[0]["ProfPic"].ToString()),
                        new Claim(ClaimTypes.Actor, mDset.Tables[0].Rows[0]["UserTitle"].ToString()),
                        new Claim(ClaimTypes.GivenName, mDset.Tables[0].Rows[0]["DeptName"].ToString())
                    };

                    this.DebugLog("SSO Username : " + mDset.Tables[0].Rows[0]["Name"].ToString());
                    this.DebugLog("SSO UserId : " + mDset.Tables[0].Rows[0]["UserId"].ToString());
                    this.DebugLog("SSO RoleCode : " + mDset.Tables[0].Rows[0]["RoleCode"].ToString());
                    this.DebugLog("SSO RoleName : " + mDset.Tables[0].Rows[0]["RoleName"].ToString());
                    this.DebugLog("SSO ProfPic : " + mDset.Tables[0].Rows[0]["ProfPic"].ToString());
                    this.DebugLog("SSO UserTitle : " + mDset.Tables[0].Rows[0]["UserTitle"].ToString());
                    this.DebugLog("SSO DeptName : " + mDset.Tables[0].Rows[0]["DeptName"].ToString());

                    var claimsIdentity = new ClaimsIdentity(claims, "Login");

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return Json(new { IsUserExists = "Y" });

                    //return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                this.DebugLog("User not exists");
                return Json(new { IsUserExists = "N" });
            }

            return Json(new { IsUserExists = "N" });
        }

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    //String UserName = Request.LogonUserIdentity.Name;
        //    //String? UserName = System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
        //    //string vUsername = this.GetUser();

        //    //WindowsIdentity microsoftIdentity = WindowsIdentity.GetCurrent();
        //    //UserPrincipal userPrincipal = UserPrincipal.Current;
        //    //string mName = userPrincipal.Name;
        //    //string mDisplayName = userPrincipal.DisplayName;

        //    //this.DebugLog("Start 1");
        //    //this.DebugLog(mName);
        //    //this.DebugLog(mDisplayName);
        //    //this.DebugLog("End");

        //    this.DebugLog("Start 1");

        //    if (Configuration.GetSection("AppSettings:IsSSO").Value.ToString() == "Y")
        //    {

        //        this.DebugLog("SSO Entry Check");

        //        LoginBLL mLoginBLL = null;
        //        DataSet mDset = new DataSet();
        //        mLoginBLL = new LoginBLL();
        //        string? IsUserExists = string.Empty;

        //        //string userName = Environment.UserName;

        //        string userName = "SeatApp";//Environment.UserName;

        //        this.DebugLog("SSO Username : " + userName);

        //        try
        //        {
        //            System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log\\" + "LogFile.txt", userName);
        //        }
        //        catch (Exception)
        //        {
        //        }

        //        mDset = mLoginBLL.UserAuthenticate(userName, "SSO", Configuration);


        //        if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
        //        {
        //            this.DebugLog("SSO Details Count : " + mDset.Tables[0].Rows.Count.ToString());

        //            IsUserExists = mDset.Tables[0].Rows[0]["IsUserExists"].ToString();

        //            this.DebugLog("User Exists : " + IsUserExists);

        //            if (IsUserExists == "Y")
        //            {
        //                var claims = new List<Claim>
        //            {
        //                 new Claim(ClaimTypes.Name, mDset.Tables[0].Rows[0]["Name"].ToString()),
        //                new Claim(ClaimTypes.NameIdentifier, mDset.Tables[0].Rows[0]["UserId"].ToString()),
        //                new Claim(ClaimTypes.PrimarySid, mDset.Tables[0].Rows[0]["RoleCode"].ToString()),
        //                new Claim(ClaimTypes.Role, mDset.Tables[0].Rows[0]["RoleName"].ToString()),
        //                new Claim(ClaimTypes.UserData, mDset.Tables[0].Rows[0]["ProfPic"].ToString()),
        //                new Claim(ClaimTypes.Actor, mDset.Tables[0].Rows[0]["UserTitle"].ToString()),
        //                new Claim(ClaimTypes.GivenName, mDset.Tables[0].Rows[0]["DeptName"].ToString())
        //            };

        //                this.DebugLog("SSO Username : " + mDset.Tables[0].Rows[0]["Name"].ToString());
        //                this.DebugLog("SSO UserId : " + mDset.Tables[0].Rows[0]["UserId"].ToString());
        //                this.DebugLog("SSO RoleCode : " + mDset.Tables[0].Rows[0]["RoleCode"].ToString());
        //                this.DebugLog("SSO RoleName : " + mDset.Tables[0].Rows[0]["RoleName"].ToString());
        //                this.DebugLog("SSO ProfPic : " + mDset.Tables[0].Rows[0]["ProfPic"].ToString());
        //                this.DebugLog("SSO UserTitle : " + mDset.Tables[0].Rows[0]["UserTitle"].ToString());
        //                this.DebugLog("SSO DeptName : " + mDset.Tables[0].Rows[0]["DeptName"].ToString());

        //                var claimsIdentity = new ClaimsIdentity(claims, "Login");

        //                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        //                return RedirectToAction("Index", "Home");
        //            }
        //        }
        //        else
        //        {
        //            this.DebugLog("User not exists");
        //        }
        //    }

        //    return View("Login");
        //}

        private string getWindowsUserId()
        {
            var identityInfo = HttpContext.User.Identity;

            var identityInfo1 = HttpContext;

            //_logger.SystemLog(Logger.LogType.Debug, string.Format("[Identity]UserId:{0},IsAuthenticated:{1},AuthenticationType:{2}",
            //    identityInfo.Name, identityInfo.IsAuthenticated, identityInfo.AuthenticationType));

            if (identityInfo.IsAuthenticated)
            {
                return identityInfo.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        // Login user with Username and password
        [HttpPost]
        public ActionResult Submit(LoginSC vLoginSC)
        {

            this.DebugLog("Login Button Check");

            LoginBLL mLoginBLL = null;
            DataSet mDset = new DataSet();
            mLoginBLL = new LoginBLL();
            string? IsUserExists = string.Empty;

            vLoginSC.password = Helper.Encrypt(vLoginSC.password);

            mDset = mLoginBLL.UserAuthenticate(vLoginSC.username, vLoginSC.password, Configuration);

            if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
            {
                this.DebugLog("SSO Details Count : " + mDset.Tables[0].Rows.Count.ToString());

                IsUserExists = mDset.Tables[0].Rows[0]["IsUserExists"].ToString();

                this.DebugLog("User Exists : " + IsUserExists);

                if (IsUserExists == "Y")
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, mDset.Tables[0].Rows[0]["Name"].ToString()),
                        new Claim(ClaimTypes.NameIdentifier, mDset.Tables[0].Rows[0]["UserId"].ToString()),
                        new Claim(ClaimTypes.PrimarySid, mDset.Tables[0].Rows[0]["RoleCode"].ToString()),
                        new Claim(ClaimTypes.Role, mDset.Tables[0].Rows[0]["RoleName"].ToString()),
                        new Claim(ClaimTypes.UserData, mDset.Tables[0].Rows[0]["ProfPic"].ToString()),
                        new Claim(ClaimTypes.Actor, mDset.Tables[0].Rows[0]["UserTitle"].ToString()),
                        new Claim(ClaimTypes.GivenName, mDset.Tables[0].Rows[0]["DeptName"].ToString())
                    };

                    this.DebugLog("SSO Username : " + mDset.Tables[0].Rows[0]["Name"].ToString());
                    this.DebugLog("SSO UserId : " + mDset.Tables[0].Rows[0]["UserId"].ToString());
                    this.DebugLog("SSO RoleCode : " + mDset.Tables[0].Rows[0]["RoleCode"].ToString());
                    this.DebugLog("SSO RoleName : " + mDset.Tables[0].Rows[0]["RoleName"].ToString());
                    this.DebugLog("SSO ProfPic : " + mDset.Tables[0].Rows[0]["ProfPic"].ToString());
                    this.DebugLog("SSO UserTitle : " + mDset.Tables[0].Rows[0]["UserTitle"].ToString());
                    this.DebugLog("SSO DeptName : " + mDset.Tables[0].Rows[0]["DeptName"].ToString());

                    var claimsIdentity = new ClaimsIdentity(claims, "Login");

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                }

            }
            else
            {
                this.DebugLog("User not exists");
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

        public void DebugLog(string vMessage)
        {
            System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log\\" + System.DateTime.Now.ToString("dd_MM_yyyy") + "_DebugLog.txt", vMessage);
            System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\Log\\" + System.DateTime.Now.ToString("dd_MM_yyyy") + "_DebugLog.txt", Environment.NewLine);
        }

    }
}