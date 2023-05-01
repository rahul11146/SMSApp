using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using SMSApp.Models.DAL;
using SMSApp.Models.SC;

namespace WebTemplate.Models.BLL
{
    public class RoleBLL
	{
        public void SaveRoles(RolesSC vRolesSC,IConfiguration _configuration)
        {
            RoleDAL mRoleDAL = null;

			mRoleDAL = new RoleDAL(_configuration);

			mRoleDAL.SaveRoles(vRolesSC);
        }

		public DataSet RoleViewList(String vCurrUserId, IConfiguration _configuration)
		{
			RoleDAL mRoleDAL = null;
			DataSet mDset = null;

			mRoleDAL = new RoleDAL(_configuration);

			mDset = mRoleDAL.RoleViewList(vCurrUserId);

			return mDset;
		}

		public RolesSC RoleGetById(String vRoleId, IConfiguration _configuration)
		{
			RoleDAL mRoleDAL = null;
			RolesSC mRolesSC = null;

			mRoleDAL = new RoleDAL(_configuration);
			mRolesSC = new RolesSC();

			mRolesSC = mRoleDAL.RoleGetById(vRoleId);

			return mRolesSC;
		}
	}
}
