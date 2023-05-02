using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SMSApp.Models;
using SMSApp.Models.SC;
using System.Diagnostics;
using System.Security.Claims;
using WebTemplate.Models.BLL;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using SMSApp.Models.DAL;

namespace SMSApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private IConfiguration Configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(ILogger<UserController> logger, IConfiguration _configuration, IWebHostEnvironment? webHostEnvironment)
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

            return View("UsersList");
        }

        [HttpGet]
        public ActionResult New()
        {
            UserSC mUserSC = new UserSC();
            mUserSC.Status = "1";
            mUserSC.PageType = "User";

            return View("AddUser", mUserSC);
        }

        [HttpPost]
        public IActionResult SaveUser(UserSC vUserSC)
        {
            UserBLL mUserBLL = null;
            ImageSC mImageSC = null;
            ImageDAL mImageDAL = null;

            mUserBLL = new UserBLL();
            mImageSC = new ImageSC();

            mImageDAL = new ImageDAL(Configuration);

            vUserSC.IsEdit = vUserSC.UserId == null ? "N" : "Y";

            vUserSC.CurrUserId = User.GetUserId();
            vUserSC.UserDisplayName = String.IsNullOrEmpty(vUserSC.UserDisplayName) ? vUserSC.LNKanji : vUserSC.UserDisplayName;

            vUserSC.RoleId = "2";

            if (vUserSC.ProfilePhoto != null)
            {
                mImageSC.IsEdit = vUserSC.IsEdit;
                mImageSC.ImageName = Path.GetFileName(vUserSC.ProfilePhoto.FileName);
                mImageSC.CurrUserId = User.GetUserId();
                mImageSC.OrgImagepath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "ProfileImage");
                mImageSC.ImgId = vUserSC.ImgId != null ? Convert.ToInt32(vUserSC.ImgId) : null;
            }

            mUserBLL.SaveUser(mImageSC, vUserSC, Configuration);

            return Json(new
            {
                IsSuccess = "Y",
                PageType = vUserSC.PageType
            });
        }

        [HttpPost]
        public IActionResult ViewUserList()
        {
            UserBLL mUserBLL = null;
            DataSet mDset = null;
            string vUserId = string.Empty;

            mUserBLL = new UserBLL();
            vUserId = User.GetUserId();

            mDset = mUserBLL.ViewUserList(vUserId, Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpGet]
        public ActionResult Edit(String id, string PageType)
        {
            UserBLL mUserBLL = null;
            UserSC mUserSC = null;

            mUserBLL = new UserBLL();
            mUserSC = new UserSC();

            mUserSC = mUserBLL.UserGetById(id, Configuration);
            mUserSC.PageType = PageType;

            return View("AddUser", mUserSC);
        }

        [HttpPost]
        public IActionResult GetAllRoles()
        {
            UserBLL mUserBLL = null;
            DataSet mDset = null;

            mUserBLL = new UserBLL();

            mDset = mUserBLL.GetAllRoles(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllDepartment()
        {
            UserBLL mUserBLL = null;
            DataSet mDset = null;

            mUserBLL = new UserBLL();

            mDset = mUserBLL.GetAllDepartment(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllFloor()
        {
            FloorBLL mFloorBLL = null;
            DataSet mDset = null;
            string vCurrUsrId = string.Empty;

            mFloorBLL = new FloorBLL();
            vCurrUsrId = User.GetUserId();

            mDset = mFloorBLL.GetAllFloor(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllMapFloor()
        {
            UserBLL mUserBLL = null;
            DataSet mDset = null;
            string vCurrUsrId = string.Empty;

            mUserBLL = new UserBLL();
            vCurrUsrId = User.GetUserId();

            mDset = mUserBLL.GetAllMapFloor(vCurrUsrId, Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public ActionResult Test()
        {
            return View("Test");
        }
    }
}