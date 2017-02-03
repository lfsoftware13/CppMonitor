﻿using NanjingUniversity.CppMonitor.Common;
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
        private string[] tableNameList;

        public BuildLoggerImpl()
        {
            tableNameList = new string[2];
            tableNameList[0] = "build_info";
            tableNameList[1] = "build_project_info";
        }

        public Boolean LogInfo(string target, List<KeyValuePair<String, Object>> list)
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
                string sql = "insert into build_info (time,buildstarttime,buildendtime,content) values (@time,@buildstarttime,@buildendtime,@content)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                //加时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));

                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
                        case "buildstarttime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildstarttime", paramPair.Value.ToString()));
                            break;
                        case "buildendtime":
                            cmd.Parameters.Add(new SQLiteParameter("@buildendtime", paramPair.Value.ToString()));
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
                string sql = "insert into build_project_info (time,buildid,buildstarttime,buildendtime,projectname,configurationname,configurationtype,runcommand,commandarguments,buildlogfile,buildlogcontent,compilercommand,linkcommand) values (@time,@buildid,@buildstarttime,@buildendtime,@projectname,@configurationname,@configurationtype,@runcommand,@commandarguments,@buildlogfile,@buildlogcontent,@compilercommand,@linkcommand)";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);

                //时间戳
                string current = DateTime.Now.ToString();
                cmd.Parameters.Add(new SQLiteParameter("@time", current));

                foreach (KeyValuePair<string, object> paramPair in list)
                {
                    switch (paramPair.Key)
                    {
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

        public void ensureTableExist()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + AddressCommon.DBFilePath);
            conn.Open();
            //建立build_info
            string sql = "create table if not exists build_info (id INTEGER PRIMARY KEY autoincrement, time char[22],buildstarttime TEXT,buildendtime TEXT,content TEXT)";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            //建立build_project_info
            sql = "create table if not exists build_project_info (id INTEGER PRIMARY KEY autoincrement, time char[22],buildid TEXT,buildstarttime TEXT,buildendtime TEXT,projectname TEXT,configurationname TEXT,configurationtype TEXT,runcommand TEXT,commandarguments TEXT,buildlogfile TEXT,buildlogcontent TEXT,compilercommand TEXT,linkcommand TEXT)";
            cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        public void clearLog()
        {
            DBHelper dbHelper = DBHelper.getInstance();
            SQLiteConnection conn = dbHelper.getConnection();
            foreach (string tableName in tableNameList)
            {
                string sql = "delete from " + tableName + ";";
                string sql2 = "update sqlite_sequence SET seq = 0 where name ='" + tableName + "';";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
                SQLiteCommand command2 = new SQLiteCommand(sql2, conn);
                command2.ExecuteNonQuery();
            }
            dbHelper.returnConnection();
        }
    }
}
