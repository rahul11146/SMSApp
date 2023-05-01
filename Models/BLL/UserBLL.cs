using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using System.Web;
using Newtonsoft.Json;
using SMSApp.Models;
using SMSApp.Models.DAL;
using SMSApp.Models.SC;

namespace WebTemplate.Models.BLL
{
    public class UserBLL
    {
        public void SaveUser(ImageSC vImageSC, UserSC vUserSC, IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            ImageDAL mImageDAL = null;
            TransactionScope mTransactionScope = null;

            string vUserId = string.Empty;
            string mOrgFilePath = string.Empty;
            string mFilePath = string.Empty;

            mTransactionScope = new TransactionScope();
            mUserDAL = new UserDAL(_configuration);
            mImageDAL = new ImageDAL(_configuration);

            using (mTransactionScope)
            {
                if (vUserSC.Password != null)
                    vUserSC.Password = PwdHelper.Encrypt(vUserSC.Password);

                vUserId = mUserDAL.SaveUser(vUserSC);

                if (vUserSC.ProfilePhoto != null)
                {
                    vImageSC.PId = Convert.ToInt32(vUserId);
                    mOrgFilePath = vImageSC.PId + Path.GetExtension(vUserSC.ProfilePhoto.FileName);
                    mFilePath = vImageSC.OrgImagepath + "\\" + mOrgFilePath;

                    using (var stream = System.IO.File.Create(mFilePath))
                    {
                        vUserSC.ProfilePhoto.CopyTo(stream);
                    }

                    vImageSC.OrgImageName = mOrgFilePath;
                    vImageSC.ImagePath = "~/Uploads/ProfileImage/" + mOrgFilePath;
                    mImageDAL.ProfPicImageSave(vImageSC);
                }

                mTransactionScope.Complete();
            }

            mTransactionScope.Dispose();
        }

        public DataSet ViewUserList(String vCurrUserId, IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            DataSet mDset = null;

            mUserDAL = new UserDAL(_configuration);

            mDset = mUserDAL.ViewUserList(vCurrUserId);

            return mDset;
        }

        public UserSC UserGetById(String vRoleId, IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            UserSC mUserSC = null;

            mUserDAL = new UserDAL(_configuration);
            mUserSC = new UserSC();

            mUserSC = mUserDAL.UserGetById(vRoleId);

            return mUserSC;
        }

        public DataSet GetAllRoles(IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            DataSet mDset = null;

            mUserDAL = new UserDAL(_configuration);

            mDset = mUserDAL.GetAllRoles();

            return mDset;
        }

        public DataSet GetAllDepartment(IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            DataSet mDset = null;

            mUserDAL = new UserDAL(_configuration);

            mDset = mUserDAL.GetAllDepartment();

            return mDset;
        }

        public DataSet GetAllMapFloor(string vCurrUsrId,IConfiguration _configuration)
        {
            UserDAL mUserDAL = null;
            DataSet mDset = null;

            mUserDAL = new UserDAL(_configuration);

            mDset = mUserDAL.GetAllMapFloor(vCurrUsrId);

            return mDset;
        }

    }
}
