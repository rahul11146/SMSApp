using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using SMSApp.Models.SC;
using Microsoft.Extensions.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;
using Newtonsoft.Json;
//using Microsoft.EntityFrameworkCore.Storage;

namespace SMSApp.Models.DAL
{
    public class UserAccessDAL
    {
        static Database CurrentDataBase = null;

        public UserAccessDAL(IConfiguration _configuration)
        {
            CurrentDataBase = new SqlDatabase(_configuration.GetConnectionString("DBConn"));
        }

        public string SaveUserAccess(UserAccessSC vUserAccessSC)
        {
            string? vUserId = string.Empty;
            DbCommand? mDbCommand = null;

            try
            {
                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_Save);

                CurrentDataBase.AddInParameter(mDbCommand, "@vUserAccessId", DbType.String, vUserAccessSC.UserAccessId);
                CurrentDataBase.AddInParameter(mDbCommand, "@vFloorId", DbType.String, vUserAccessSC.FloorId);
                CurrentDataBase.AddInParameter(mDbCommand, "@vFloorDesc", DbType.String, vUserAccessSC.FloorDesc);
                CurrentDataBase.AddInParameter(mDbCommand, "@vIsActive", DbType.String, vUserAccessSC.IsActive);
                CurrentDataBase.AddInParameter(mDbCommand, "@vCurrUserId", DbType.String, vUserAccessSC.CurrUserId);
                CurrentDataBase.AddInParameter(mDbCommand, "@vIsEdit", DbType.String, vUserAccessSC.IsEdit);

                vUserId = CurrentDataBase.ExecuteScalar(mDbCommand).ToString();
            }
            catch (Exception ex)
            {
                throw;
            }

            return vUserId;
        }

        public void SaveUserAccessUsers(UserAccessSC vUserAccessSC)
        {
            DbCommand? mDbCommand = null;

            try
            {
                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_SaveUsers);

                SqlParameter param = new SqlParameter("@vUserAccessListUT", vUserAccessSC.UserDT);
                param.SqlDbType = SqlDbType.Structured;

                mDbCommand.Parameters.Add(param);

                CurrentDataBase.AddInParameter(mDbCommand, "@vUserAccessId", DbType.String, vUserAccessSC.UserAccessId);
                CurrentDataBase.ExecuteNonQuery(mDbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet ViewUserAccessList(String vCurrUserId)
        {
            DataSet mDset = null;

            try
            {
                DbCommand mDbCommand = null;

                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_GetList);
                CurrentDataBase.AddInParameter(mDbCommand, "@vCurrUserId", DbType.String, vCurrUserId);

                mDset = CurrentDataBase.ExecuteDataSet(mDbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }

            return mDset;
        }

        public UserAccessSC UserAccessGetById(string vUserAccessId)
        {
            DataSet mDset = null;
            UserAccessSC? mUserAccessSC = null;

            try
            {
                DbCommand mDbCommand = null;

                mUserAccessSC = new UserAccessSC();
                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_GetById);

                CurrentDataBase.AddInParameter(mDbCommand, "@vUserAccessId", DbType.String, vUserAccessId);

                mDset = CurrentDataBase.ExecuteDataSet(mDbCommand);

                if (mDset != null && mDset.Tables.Count > 0 && mDset.Tables[0].Rows.Count > 0)
                {
                    mUserAccessSC.UserAccessId = Convert.ToInt32(mDset.Tables[0].Rows[0]["UserAccessId"]);
                    mUserAccessSC.FloorId = mDset.Tables[0].Rows[0]["UserAccessId"].ToString();
                    mUserAccessSC.FloorName = mDset.Tables[0].Rows[0]["FloorName"].ToString();
                    mUserAccessSC.FloorDesc = mDset.Tables[0].Rows[0]["FloorDesc"].ToString();
                    mUserAccessSC.IsActive = mDset.Tables[0].Rows[0]["IsActive"].ToString();

                    mUserAccessSC.UserJSON = JsonConvert.SerializeObject(mDset.Tables[1]);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return mUserAccessSC;
        }

        public DataSet GetAllUsersList()
        {
            DataSet mDset = null;

            try
            {
                DbCommand mDbCommand = null;

                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_GetAllUsers);

                mDset = CurrentDataBase.ExecuteDataSet(mDbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }

            return mDset;
        }

        public DataSet GetAllFloorList(string vUserId,string vType)
        {
            DataSet? mDset = null;

            try
            {
                DbCommand? mDbCommand = null;

                mDbCommand = CurrentDataBase.GetStoredProcCommand(StoredProcedures.spr_UserAccess_GetAllFloor);
                CurrentDataBase.AddInParameter(mDbCommand, "@vUserId", DbType.String, vUserId);
                CurrentDataBase.AddInParameter(mDbCommand, "@vType", DbType.String, vType);

                mDset = CurrentDataBase.ExecuteDataSet(mDbCommand);
            }
            catch (Exception ex)
            {
                throw;
            }

            return mDset;
        }
    }
}