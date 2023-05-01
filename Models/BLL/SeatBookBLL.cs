using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Formatters;
using SMSApp.Models.DAL;
using SMSApp.Models.SC;

namespace WebTemplate.Models.BLL
{
    public class SeatBookBLL
    {
        public void SaveFloor(ImageSC vImageSC, FloorSC vFloorSC, IConfiguration _configuration)
        {
            FloorDAL mFloorDAL = null;
            ImageDAL mImageDAL = null;

            string mFloorId = string.Empty;
            string mOrgFilePath = string.Empty;
            string mFilePath = string.Empty;

            mFloorDAL = new FloorDAL(_configuration);
            mImageDAL = new ImageDAL(_configuration);

            vFloorSC.FloorImageId = 0;
            mFloorId = mFloorDAL.SaveFloor(vFloorSC);

            if (vFloorSC.FloorImage != null)
            {
                vImageSC.PId = Convert.ToInt32(mFloorId);
                mOrgFilePath = vImageSC.PId + Path.GetExtension(vFloorSC.FloorImage.FileName);
                mFilePath = _configuration.GetValue<string>("AppSettings:FilePath").ToString() + mOrgFilePath;

                using (var stream = System.IO.File.Create(mFilePath))
                {
                    vFloorSC.FloorImage.CopyTo(stream);
                }

                vImageSC.OrgImageName = mOrgFilePath;
                vImageSC.ImagePath = "~/Uploads/FloorImage/" + mOrgFilePath;
                mImageDAL.SaveImage(vImageSC);
            }
        }

        public void SaveFloorMap(FloorMapSC vFloorMapSC, IConfiguration _configuration)
        {
            FloorDAL mFloorDAL = null;
            string mFloorId = string.Empty;

            mFloorDAL = new FloorDAL(_configuration);

            mFloorDAL.SaveFloorMap(vFloorMapSC);
        }

        public DataSet ViewSeatList(String vCurrUserId, IConfiguration _configuration)
        {
            SeatBookDAL mSeatBookDAL = null;
            DataSet mDset = null;

            mSeatBookDAL = new SeatBookDAL(_configuration);

            mDset = mSeatBookDAL.ViewSeatList(vCurrUserId);

            return mDset;
        }

        //public FloorSC FloorGetById(String vFloorId, IConfiguration _configuration)
        //{
        //    FloorDAL mFloorDAL = null;
        //    FloorSC mFloorSC = null;

        //    mFloorDAL = new FloorDAL(_configuration);
        //    mFloorSC = new FloorSC();

        //    mFloorSC = mFloorDAL.FloorGetById(vFloorId);

        //    return mFloorSC;
        //}

        //public IList<FloorMapSC> FloorMapGetById(string? vFloorId, string vId, IConfiguration _configuration)
        //{
        //    FloorDAL mFloorDAL = null;
        //    IList<FloorMapSC> mFloorMapSCList = null;

        //    mFloorDAL = new FloorDAL(_configuration);
        //    mFloorMapSCList = new List<FloorMapSC>();

        //    mFloorMapSCList = mFloorDAL.FloorMapGetById(vFloorId, vId);

        //    return mFloorMapSCList;
        //}

        public DataSet GetAllFloor(IConfiguration _configuration)
        {
            FloorDAL mFloorDAL = null;
            DataSet mDset = null;

            mFloorDAL = new FloorDAL(_configuration);

            mDset = mFloorDAL.GetAllFloorList();

            return mDset;
        }

        public DataSet GetAllFloorAdminList(IConfiguration _configuration)
        {
            FloorDAL mFloorDAL = null;
            DataSet mDset = null;

            mFloorDAL = new FloorDAL(_configuration);

            mDset = mFloorDAL.GetAllFloorAdminList();

            return mDset;
        }
    }
}