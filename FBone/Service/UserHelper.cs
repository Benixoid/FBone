using DocumentFormat.OpenXml.Spreadsheet;
using FBone.Database;
using FBone.Database.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace FBone.Service
{
    public class UserHelper
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataManager _dataManager;
                
        //public UserHelper(IHttpContextAccessor httpContextAccessor, DataManager dataManager)
        public UserHelper(DataManager dataManager)
        {
            // _httpContextAccessor = httpContextAccessor;
            _dataManager = dataManager;
        }

        public void SaveRequestLog(RequestLog log)
        {
            _dataManager.RequestLog.SaveRequestLog(log);
        }
        public Boolean IsHasRole(string userId, Enums.Roles role)
        {
            bool res = false;            
            if (role == Enums.Roles.isAktCreatorOrEditor)
                res = _dataManager.tUser.CanCreateAkt(userId);
            else if (role == Enums.Roles.isTranslator)
                res = _dataManager.tUser.CanTranslateAkt(userId);
            else if (role == Enums.Roles.isShiftEngineer)
                res = _dataManager.tUser.IsShiftEngineer(userId);
            else if (role == Enums.Roles.IsNModeAdministrator)
                res = _dataManager.tUser.IsNModeAdministrator(userId);
            else if (role == Enums.Roles.IsNModeEditor)
                res = _dataManager.tUser.IsNModeEditor(userId);
            else if (role == Enums.Roles.IsNModeUser)
                res = _dataManager.tUser.IsNModeUser(userId);
            else if (role == Enums.Roles.IsAuditCreatorOrEditor)
                res = _dataManager.tUser.CanCreateAudit(userId);
            return res;
        }
        public static Boolean IsUserAdmin(string userId, string path)
        {
            bool res = false;
            string sql = "select * from tUser where cai=@userId and isAdmin=1 and isactive=1";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userId });
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    if (dr.Read() == true)
                    {
                        res = true;
                    }
                    dr.Close();
                }
            }
            AddRequestLogRecord(userId, path, "As admin");
            return res;
        }

        internal static void ChangeLanguage(string userId, string lang)
        {
            //string lang = "ru"; //default language
            if (!UserHelper.IsUserExist(userId, "ChangeLang"))
            {
                userId = "fbone_temp";
            }
            string sql = "update tUser set lang = @lang where cai=@userId and isactive=1";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userId });                
                cmd.CommandType = System.Data.CommandType.Text;
                var langArr = LanguageHelper.GetAvailableLanguages();
                if (!langArr.Contains(lang))
                {
                    lang = "ru";                    
                }
                cmd.Parameters.Add(new SqlParameter("@lang", System.Data.SqlDbType.VarChar) { Value = lang });
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static Boolean IsUserExist(string userId, string path)
        {
            bool res = false;
            
            string sql = "select * from tUser where cai=@userId and isactive=1";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userId });
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    if (dr.Read() == true)
                    {
                        res = true;
                    }
                    dr.Close();
                }
            }            
            AddRequestLogRecord(userId, path, "As user");
            return res;
        }
        public static string GetUserName(string userCAI)
        {
            string username = userCAI + " - Unauthorized user";
            string userlang = GetUserLanguage(userCAI);
            string sql = "";
            if (userlang == "RU")
            {
                sql = "select name_ru as name from tUser where cai=@userId and isactive=1";
            } else if (userlang == "EN")
            {
                sql = "select name_en as name from tUser where cai=@userId and isactive=1";
            } else
            {
                sql = "select name_kk as name from tUser where cai=@userId and isactive=1";
            }            
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userCAI });
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    if (dr.Read() == true)
                    {
                        username = dr.GetString(0);
                    }
                    dr.Close();
                }
            }
            return username;
        }
        public static void AddRequestLogRecord(string userCAI, string URL, string comment)
        {
            if (URL.Equals("Change lang"))
                return;
            string sql = "";
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                
                sql = "insert into RequestLogs (cai, requesttime, requesturl,comment) values (@user_id,@req_date, @url, @comment)";
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@user_id", System.Data.SqlDbType.VarChar) { Value = userCAI});
                cmd.Parameters.Add(new SqlParameter("@req_date", System.Data.SqlDbType.DateTime2) { Value = DateTime.Now});
                cmd.Parameters.Add(new SqlParameter("@url", System.Data.SqlDbType.VarChar) { Value = URL});
                cmd.Parameters.Add(new SqlParameter("@comment", System.Data.SqlDbType.VarChar) { Value = comment });
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();                
            }
        }

        public static string GetUserLanguage(string userId)
        {
            string lang = "ru"; //default language
            string sql = "";            
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(Config.GetConnectionString());
                if (!UserHelper.IsUserExist(userId,"Change lang"))
                {
                    userId = "fbone_temp";
                    if (!UserHelper.IsUserExist(userId, "Change lang"))
                    {
                        
                        sql = "insert into tUser (cai, name_en,name_ru,name_kk, isadmin,isactive,lang,positionid,areaid,facilityid) values ('fbone_temp','temp','temp','temp',0,1,'ru',1,6,1)";
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Connection.Open();
                        cmd.ExecuteNonQuery();
                        cmd.Connection.Close();
                    }
                }
                sql = "select lang from tUser where cai=@userId and isactive=1";
                
                cmd.CommandText = sql;
                cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userId });
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Connection.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection))
                {
                    if (dr.Read() == true)
                    {
                        lang = dr.GetString(0);
                        var langArr = LanguageHelper.GetAvailableLanguages();
                        if (!langArr.Contains(lang))
                        {
                            lang = "ru";
                            //need to update to default lang                            
                            sql = "update tUser set lang = @lang where cai=@userId and isactive=1";
                            cmd.Connection.Close();
                            cmd.CommandText = sql;
                            //cmd.Parameters.Add(new SqlParameter("@userId", System.Data.SqlDbType.VarChar) { Value = userId });
                            cmd.Parameters.Add(new SqlParameter("@lang", System.Data.SqlDbType.VarChar) { Value = lang });
                            cmd.CommandType = System.Data.CommandType.Text;
                            cmd.Connection.Open();
                            cmd.ExecuteNonQuery();                            
                        }
                    }                    
                    dr.Close();
                }
            }
            return lang.ToUpper();
        }

        internal bool IsUserOrB2b()
        {
            throw new NotImplementedException();
        }
    }
}
