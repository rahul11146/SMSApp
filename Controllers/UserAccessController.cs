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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMSApp.Controllers
{
    [Authorize]
    public class UserAccessController : Controller
    {
        private readonly ILogger<UserAccessController> _logger;
        private IConfiguration Configuration;

        public UserAccessController(ILogger<UserAccessController> logger, IConfiguration _configuration)
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
            return View("UserAccessList");
        }

        [HttpGet]
        public ActionResult New()
        {
            this.FillUsersList();

            UserAccessSC mUserAccessSC = new UserAccessSC();
            mUserAccessSC.IsActive = "1";

            return View("UserAccessForm", mUserAccessSC);
        }

        public void FillUsersList()
        {
            SelectList mSelectList = null;
            Select2DropDown mSelect2DropDown = null;
            IList<Select2DropDown> mSelect2DropDownLst = null;

            mSelect2DropDownLst = new List<Select2DropDown>();
            mSelect2DropDown = new Select2DropDown();

            mSelectList = new SelectList(mSelect2DropDownLst.ToArray(), "Value", "Text");

            ViewData["UsersList"] = mSelectList;
        }

        [HttpPost]
        public IActionResult SaveUserAccess(UserAccessSC vUserAccessSC)
        {
            if (vUserAccessSC.FloorId == null)
            {
                return Json(new
                {
                    IsSuccess = "N",
                    ErrorMessage = "The 'フロア' Field is required"
                });
            }

            UserAccessBLL mUserAccessBLL = null;

            mUserAccessBLL = new UserAccessBLL();

            vUserAccessSC.IsEdit = vUserAccessSC.UserAccessId == null ? "N" : "Y";
            vUserAccessSC.CurrUserId = User.GetUserId();

            mUserAccessBLL.SaveUserAccess(vUserAccessSC, Configuration);

            return Json(new
            {
                IsSuccess = "Y"
            });
        }

        [HttpPost]
        public IActionResult ViewUserAccessList()
        {
            UserAccessBLL mUserAccessBLL = null;
            DataSet mDset = null;

            mUserAccessBLL = new UserAccessBLL();

            mDset = mUserAccessBLL.ViewUserAccessList(User.GetUserId().ToString(), Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpGet]
        public ActionResult Edit(String id)
        {
            UserAccessBLL mUserAccessBLL = null;
            UserAccessSC mUserAccessSC = null;

            mUserAccessBLL = new UserAccessBLL();
            mUserAccessSC = new UserAccessSC();

            mUserAccessSC = mUserAccessBLL.UserAccessGetById(id, Configuration);

            return View("UserAccessForm", mUserAccessSC);
        }

        [HttpPost]
        public IActionResult GetAllUsersList()
        {
            UserAccessBLL mUserAccessBLL = null;
            DataSet mDset = null;

            mUserAccessBLL = new UserAccessBLL();

            mDset = mUserAccessBLL.GetAllUsersList(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllFloorList(string vType)
        {
            UserAccessBLL mUserAccessBLL = null;
            DataSet mDset = null;
            string mUserId = string.Empty;

            mUserAccessBLL = new UserAccessBLL();

            mUserId = User.GetUserId();

            mDset = mUserAccessBLL.GetAllFloorList(mUserId, vType, Configuration);

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}