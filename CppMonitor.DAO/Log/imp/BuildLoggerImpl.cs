using NanjingUniversity.CppMonitor.Util.Common;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NanjingUniversity.CppMonitor.DAO.imp
{
    class BuildLoggerImpl : ILoggerDao
    {
        public BuildLoggerImpl()
        {
            tableNameList = new string[2];
            tableNameList[0] = "build_info";
            tableNameList[1] = "build_project_info";
        }

        public override Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
        {
            bool result = false;
            switch (target)
            {
                case "build_info":
                    result = logBuildInfo(list);
                    break;
                case "build_project_info":
                    result = logBuildProjectInfo(list);
                    break;
                default:
                    break;
            }
            return result;
        }

        private bool logBuildInfo(List<KeyValuePair<String, Object>> list)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into build_info (id,time,buildstarttime,buildendtime,solutionname,content) values (@id,@time,@buildstarttime,@buildendtime,@solutionname,@content)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));

                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "id":
                            cmd.Parameters.Add(new SQLiteParameter("@id", paramPair.Value.ToString()));
                            break;
                        case "buildstarttime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildstarttime", paramPair.Value.ToString()));
                            break;
                        case "buildendtime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildendtime", paramPair.Value.ToString()));
                            break;
                        case "solutionname":
                            cmd.Parameters.Add(new SQLiteParameter("@solutionname", paramPair.Value.ToString()));
                            break;
                        case "content":
                            cmd.Parameters.Add(new SQLiteParameter("@content", paramPair.Value.ToString()));
                            break;
                        default:
                            break;
                    }
                }
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbHelper.returnConnection();
            }
        }

        private bool logBuildProjectInfo(List<KeyValuePair<String, Object>> list)
        {

            DBHelper dbHelper = DBHelper.getInstance();
            try
            {
                SQLiteConnection conn = dbHelper.getConnection();
                string sql = "insert into build_project_info (id,time,buildid,buildstarttime,buildendtime,solutionname,projectname,configurationname,configurationtype,runcommand,commandarguments,buildlogfile,buildlogcontent,compilercommand,linkcommand) values (@id,@time,@buildid,@buildstarttime,@buildendtime,@solutionname,@projectname,@configurationname,@configurationtype,@runcommand,@commandarguments,@buildlogfile,@buildlogcontent,@compilercommand,@linkcommand)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                //时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));

                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "id":
                            cmd.Parameters.Add(new SQLiteParameter("@id", paramPair.Value.ToString()));
                            break;
                        case "buildid":
                            cmd.Parameters.Add(new SQLiteParameter("@buildid", paramPair.Value.ToString()));
                            break;
                        case "buildstarttime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildstarttime", paramPair.Value.ToString()));
                            break;
                        case "buildendtime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildendtime", paramPair.Value.ToString()));
                            break;
                        case "projectname":
                            cmd.Parameters.Add(new SQLiteParameter("@projectname", paramPair.Value.ToString()));
                            break;
                        case "solutionname":
                            cmd.Parameters.Add(new SQLiteParameter("@solutionname", paramPair.Value.ToString()));
                            break;
                        case "configurationname":
                            cmd.Parameters.Add(new SQLiteParameter("@configurationname", paramPair.Value.ToString()));
                            break;
                        case "configurationtype":
                            cmd.Parameters.Add(new SQLiteParameter("@configurationtype", paramPair.Value.ToString()));
                            break;
                        case "runcommand":
                            cmd.Parameters.Add(new SQLiteParameter("@runcommand", paramPair.Value.ToString()));
                            break;
                        case "commandarguments":
                            cmd.Parameters.Add(new SQLiteParameter("@commandarguments", paramPair.Value.ToString()));
                            break;
                        case "buildlogcontent":
                            cmd.Parameters.Add(new SQLiteParameter("@buildlogcontent", paramPair.Value.ToString()));
                            break;
                        case "buildlogfile":
                            cmd.Parameters.Add(new SQLiteParameter("@buildlogfile", paramPair.Value.ToString()));
                            break;
                        case "compilercommand":
                            cmd.Parameters.Add(new SQLiteParameter("@compilercommand", paramPair.Value.ToString()));
                            break;
                        case "linkcommand":
                            cmd.Parameters.Add(new SQLiteParameter("@linkcommand", paramPair.Value.ToString()));
                            break;
                        default:
                            break;
                    }
                }
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                dbHelper.returnConnection();
            }
        }

        public override void ensureTableExist()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + AddressCommon.DBFilePath);
            conn.Open();
            //建立build_info
            string sql = "create table if not exists build_info (id INTEGER PRIMARY KEY autoincrement, time char[22],buildstarttime TEXT, buildendtime TEXT, solutionname TEXT, content TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立build_project_info
            sql = "create table if not exists build_project_info (id INTEGER PRIMARY KEY autoincrement, time char[22],buildid TEXT,buildstarttime TEXT,buildendtime TEXT, solutionname TEXT, projectname TEXT,configurationname TEXT,configurationtype TEXT,runcommand TEXT,commandarguments TEXT,buildlogfile TEXT,buildlogcontent TEXT,compilercommand TEXT,linkcommand TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }
     
    }
}
