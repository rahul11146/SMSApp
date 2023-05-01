using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using SMSApp.Models.DAL;
using SMSApp.Models.SC;

namespace WebTemplate.Models.BLL
{
    public class ControllerMapBLL
    {
        public void SaveControllerMap(ControllerMapSC vControllerMapSC, IConfiguration _configuration)
        {
            ControllerMapDAL mControllerMapDAL = null;

            mControllerMapDAL = new ControllerMapDAL(_configuration);

            if (vControllerMapSC.FloorListJson != null)
                vControllerMapSC.FloorList = JsonConvert.DeserializeObject<DataTable>(vControllerMapSC.FloorListJson);

            if (vControllerMapSC.FloorList != null && vControllerMapSC.FloorList.Columns.Count > 0)
            {
                if (vControllerMapSC.FloorList.Columns.Contains("RowId"))
                    vControllerMapSC.FloorList.Columns.Remove("RowId");

                if (vControllerMapSC.FloorList.Columns.Contains("$$hashkey"))
                    vControllerMapSC.FloorList.Columns.Remove("$$hashkey");

                if (vControllerMapSC.FloorList.Columns.Contains("RoleCode"))
                    vControllerMapSC.FloorList.Columns.Remove("RoleCode");
            }

            mControllerMapDAL.SaveControllerMap(vControllerMapSC);
        }

        public DataSet ViewControllerMapList(String vCurrUserId, IConfiguration _configuration)
        {
            ControllerMapDAL mControllerMapDAL = null;
            DataSet mDset = null;

            mControllerMapDAL = new ControllerMapDAL(_configuration);

            mDset = mControllerMapDAL.ViewControllerMapList(vCurrUserId);

            return mDset;
        }

        public ControllerMapSC ControllerMapGetById(String vControllerMapId, IConfiguration _configuration)
        {
            ControllerMapDAL? mControllerMapDAL = null;
            ControllerMapSC? mControllerMapSC = null;

            mControllerMapDAL = new ControllerMapDAL(_configuration);
            mControllerMapSC = new ControllerMapSC();

            mControllerMapSC = mControllerMapDAL.ControllerMapGetById(vControllerMapId);

            return mControllerMapSC;
        }

        public DataSet GetAllFloor(string vCurrUsrId, IConfiguration _configuration)
        {
            ControllerMapDAL mControllerMapDAL = null;
            DataSet mDset = null;

            mControllerMapDAL = new ControllerMapDAL(_configuration);

            mDset = mControllerMapDAL.GetAllFloor(vCurrUsrId);

            return mDset;
        }
    }
}
