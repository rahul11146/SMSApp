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
using SMSApp.Models.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace SMSApp.Controllers
{
    [Authorize]
    public class FloorController : Controller
    {
        private readonly ILogger<FloorController> _logger;
        private IConfiguration Configuration;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FloorController(ILogger<FloorController> logger, IConfiguration _configuration, IWebHostEnvironment? webHostEnvironment)
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
            return View("FloorList");
        }

        // New Floor Addition
        [HttpGet]
        public ActionResult New()
        {
            FloorSC mFloorSC = new FloorSC();
            mFloorSC.UsernameFontsize = "11";
            mFloorSC.Status = "1";

            return View("FloorForm", mFloorSC);
        }

        // Save Floor
        [HttpPost]
        public IActionResult SaveFloorData(FloorSC vFloorSC)
        {
            try
            {
                if (vFloorSC.FloorId == null && vFloorSC.FloorImage == null)
                {
                    return Json(new
                    {
                        IsSuccess = "N",
                        ErrorMessage = "The '詳細' Field is required"
                    });
                }

                FloorBLL mFloorBLL = null;
                ImageSC mImageSC = null;
                ImageDAL mImageDAL = null;

                string mFilePath = string.Empty;
                string mOrgPath = string.Empty;

                mFloorBLL = new FloorBLL();
                mImageSC = new ImageSC();

                mImageDAL = new ImageDAL(Configuration);

                if (vFloorSC.FloorId == null)
                    vFloorSC.IsEdit = "N";
                else if (vFloorSC.FloorImage != null)
                    vFloorSC.IsEdit = "N";
                else if (vFloorSC.FloorId != null)
                    vFloorSC.IsEdit = "Y";

                vFloorSC.ControllerId = !string.IsNullOrEmpty(vFloorSC.ControllerId) ? vFloorSC.ControllerId : "0";
                vFloorSC.CurrUserId = User.GetUserId();

                if (vFloorSC.FloorImage != null)
                {
                    mImageSC.IsEdit = vFloorSC.IsEdit;
                    mImageSC.ImageName = Path.GetFileName(vFloorSC.FloorImage.FileName);
                    mImageSC.CurrUserId = User.GetUserId();
                    mImageSC.OrgImagepath = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", "FloorImage");
                    mImageSC.TestPath = _webHostEnvironment.WebRootPath;
                }

                mFloorBLL.SaveFloor(mImageSC, vFloorSC, Configuration);

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\ErrorLog.txt", ex.Message.ToString());
                System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\ErrorLog.txt", Environment.NewLine);
                System.IO.File.AppendAllText(_webHostEnvironment.WebRootPath + "\\ErrorLog.txt", ex.StackTrace.ToString());
            }

            return Json(new
            {
                IsSuccess = "Y"
            });
        }

        //Save Floor Mapping
        [HttpPost]
        public IActionResult SaveMapFloor(FloorMapSC vFloorMapSC)
        {
            FloorBLL mFloorBLL = null;

            mFloorBLL = new FloorBLL();

            vFloorMapSC.IsEdit = vFloorMapSC.Id == null ? "N" : "Y";
            vFloorMapSC.CurrUserId = User.GetUserId();

            vFloorMapSC.SeatDetails = !string.IsNullOrEmpty(vFloorMapSC.SeatDetails) ? vFloorMapSC.SeatDetails : "";

            mFloorBLL.SaveFloorMap(vFloorMapSC, Configuration);

            return Json(new
            {
                IsSuccess = "Y"
            });
        }

        //View Floor List
        [HttpPost]
        public IActionResult ViewFloorList(string vIsActive)
        {
            FloorBLL mFloorBLL = null;
            DataSet mDset = null;

            mFloorBLL = new FloorBLL();

            mDset = mFloorBLL.ViewFloorList(User.GetUserId().ToString(), vIsActive, Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        //Update Floor List
        [HttpGet]
        public ActionResult Edit(String id)
        {
            UserBLL mUserBLL = null;
            UserSC mUserSC = null;

            mUserBLL = new UserBLL();
            mUserSC = new UserSC();

            mUserSC = mUserBLL.UserGetById(id, Configuration);

            return View("AddUser", mUserSC);
        }

        //Update Floor List
        [HttpGet]
        public ActionResult EditFloor(String id)
        {
            FloorBLL mFloorBLL = null;
            FloorSC mFloorSC = null;
            string mUserId = string.Empty;

            mFloorBLL = new FloorBLL();
            mFloorSC = new FloorSC();

            mUserId = User.GetUserId();

            mFloorSC = mFloorBLL.FloorGetById(id, mUserId, Configuration);

            return View("FloorForm", mFloorSC);
        }

        //Edit Floor Map List
        [HttpGet]
        public ActionResult EditMapFloor(String id, string FloorMapId, string vType)
        {
            FloorBLL mFloorBLL = null;
            FloorMapSC mFloorMapSC = null;
            FloorSC mFloorSC = null;
            string mUserId = string.Empty;

            mFloorBLL = new FloorBLL();
            mFloorMapSC = new FloorMapSC();
            mFloorSC = new FloorSC();

            mUserId = User.GetUserId();

            mFloorSC = mFloorBLL.FloorGetById(id, mUserId, Configuration);

            mFloorMapSC.FloorId = Convert.ToInt32(id);
            mFloorMapSC.ImagePath = mFloorSC.ImagePath;

            mFloorMapSC.FloorMapLst = mFloorBLL.FloorMapGetById(mFloorMapSC.FloorId.ToString(), FloorMapId, vType, Configuration);
            mFloorMapSC.IsActive = "1";
            mFloorMapSC.FloorName = mFloorSC.FloorName;

            if (mFloorMapSC.FloorMapLst != null && mFloorMapSC.FloorMapLst.Count > 0)
                mFloorMapSC.FloorMapJSON = JsonConvert.SerializeObject(mFloorMapSC.FloorMapLst);

            ViewBag.UserId = User.GetUserId();

            return View("FloorMap", mFloorMapSC);
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
        public IActionResult GetAllWFHUserUnderHM(string vFloorId)
        {
            FloorBLL mFloorBLL = null;
            DataSet mDset = null;
            string mUserId = string.Empty;

            mFloorBLL = new FloorBLL();
            mUserId = User.GetUserId();

            mDset = mFloorBLL.ShowAllWFHUserUnderHM(vFloorId, mUserId, Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllFloorAdminList()
        {
            FloorBLL mFloorBLL = null;
            DataSet mDset = null;

            mFloorBLL = new FloorBLL();

            mDset = mFloorBLL.GetAllFloorAdminList(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult GetAllControllerList()
        {
            FloorBLL mFloorBLL = null;
            DataSet mDset = null;

            mFloorBLL = new FloorBLL();

            mDset = mFloorBLL.GetAllControllerList(Configuration);

            return Json(JsonConvert.SerializeObject(mDset));
        }

        [HttpPost]
        public IActionResult BookSeat(FloorMapSC vFloorMapSC, string btnAction)
        {
            FloorBLL mFloorBLL = null;

            mFloorBLL = new FloorBLL();

            vFloorMapSC.IsEdit = vFloorMapSC.Id == null ? "N" : "Y";
            vFloorMapSC.CurrUserId = User.GetUserId();

            vFloorMapSC.UserId = Convert.ToInt32(User.GetUserId());
            vFloorMapSC.IsBook = "1";

            if (btnAction == "BookSeat")
                mFloorBLL.BookSeat(vFloorMapSC, Configuration);
            else if (btnAction == "Release")
                mFloorBLL.ReleaseSeat(vFloorMapSC, Configuration);

            return Json(new
            {
                IsSuccess = "Y"
            });
        }

        [HttpPost]
        public IActionResult ReleaseSeat(FloorMapSC vFloorMapSC)
        {
            FloorBLL mFloorBLL = null;

            mFloorBLL = new FloorBLL();

            vFloorMapSC.CurrUserId = User.GetUserId();
            vFloorMapSC.UserId = Convert.ToInt32(User.GetUserId());

            mFloorBLL.ReleaseSeat(vFloorMapSC, Configuration);

            return Json(new
            {
                IsSuccess = "Y"
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

    public class Filter
    {
        public string Type { get; set; }
    }
}