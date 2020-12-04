using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CustomerServicePortal.Auth
{
    public class Transaction
    {
        //string cstr = ConfigurationManager.ConnectionStrings["BICCTimesheetsConnectionString"].ToString();

        string cstr = "";
        // string cstr = ConfigurationManager.ConnectionStrings[1].ToString();
        #region "Property"

        public string Action { get; set; }
        public decimal TotalHours { get; set; }
        public int FinalSubmitCount { get; set; }

        #region "Employee"
        public int employee_id { get; set; }
        public string user_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string Fullname { get; set; }

        #endregion
        #region "Tasks"
        public int task_id { get; set; }
        public string task { get; set; }

        #endregion
        #region "clients"
        public int client_id { get; set; }
        public string client_name { get; set; }
        public string client_location { get; set; }
        #endregion
        #region "transactions"

        public int transaction_id { get; set; }
        public decimal hours { get; set; }
        public DateTime transaction_date { get; set; }
        public string description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        #endregion

        #region "WeekDays

        public string WeekDays { get; set; }
        public DateTime WeekDate { get; set; }
        #endregion
        public DateTime CurrentDate { get; set; }
        public string strDate { get; set; }
        public string strCurrentDate { get; set; }

        #region Setting

        public int st_Id { get; set; }
        public DateTime st_StartDate { get; set; }
        public DateTime st_EndDate { get; set; }
        public int st_Active { get; set; }
        public string strst_StartDate { get; set; }
        public string strst_EndDate { get; set; }
        #endregion

        #endregion

        #region "Method"

        public List<Transaction> RetriveTasksDetails()
        {


            List<Transaction> lstTask = new List<Transaction>();
            try
            {


                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTasks", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Get_Tasks");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        lstTask.Add(
                          new Transaction
                          {
                              task_id = Convert.ToInt32(dr["task_id"]),
                              task = Convert.ToString(dr["task"]),
                          }
                      );

                    }


                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }



            return lstTask;


        }
        public List<Transaction> RetriveClientsDetails()
        {
            List<Transaction> lstClient = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveClients", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Get_Clients");
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstClient.Add(new Transaction
                        {
                            client_id = Convert.ToInt32(dr["client_id"]),
                            client_name = Convert.ToString(dr["client_name"]),

                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }

            return lstClient;
        }
        public bool InsertUpdateTransaction(Transaction tran)
        {


            SqlConnection con = new SqlConnection(cstr);
            try
            {

                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Insert_Transaction");
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                cmd.Parameters.AddWithValue("@client_id", tran.client_id);
                cmd.Parameters.AddWithValue("@task_id", tran.task_id);
                cmd.Parameters.AddWithValue("@hours", tran.hours);
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@description", tran.description);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                if (ConfigurationManager.AppSettings.Get("NotifyOnCreate").ToString().ToLower() == "true")
                {
                    System.Data.DataTable dt = GetMailDetails(tran, 0 /*Create*/ );
                    SendMail(tran, 0 /*Create*/, dt);
                }

                return true;
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

                con.Close();
                return false;

            }
        }
        public List<Transaction> RetriveTransactionDetailsByEmployeeId(Transaction tran)
        {
            List<Transaction> lstTransaction = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Retrive_TransactionByEmpId_betweenCurrentWeekStartDayandLastDay");
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstTransaction.Add(new Transaction
                        {
                            transaction_id = Convert.ToInt32(dr["tranid"]),
                            employee_id = Convert.ToInt32(dr["Empid"]),
                            client_id = Convert.ToInt32(dr["client_id"]),
                            task_id = Convert.ToInt32(dr["task_id"]),
                            description = Convert.ToString(dr["description"]),
                            hours = Convert.ToDecimal(dr["hours"]),

                            WeekDate = Convert.ToDateTime(dr["weekdate"]),
                            WeekDays = Convert.ToString(dr["WeekDays"]),
                            strDate = Convert.ToString(dr["weekdate"]),
                            client_name = Convert.ToString(dr["client_name"]),
                            task = Convert.ToString(dr["task"]),

                            CurrentDate = System.DateTime.Now,
                            strCurrentDate = Convert.ToString(CurrentDate),

                        });

                    }
                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

            }
            return lstTransaction;
        }
        public bool UpdateUpdateTransaction(Transaction tran)
        {
            SqlConnection con = new SqlConnection(cstr);
            try
            {

                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (tran.transaction_id == 0)
                {
                    tran.Action = "Insert_Transaction";

                }
                else
                {
                    tran.Action = "Update_Transaction";

                }
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                cmd.Parameters.AddWithValue("@transaction_id", tran.transaction_id);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                cmd.Parameters.AddWithValue("@client_id", tran.client_id);
                cmd.Parameters.AddWithValue("@task_id", tran.task_id);
                cmd.Parameters.AddWithValue("@hours", tran.hours);
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@description", tran.description);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                if (ConfigurationManager.AppSettings.Get("NotifyOnUpdate").ToString().ToLower() == "true")
                {
                    System.Data.DataTable dt = GetMailDetails(tran, 1 /*Update*/ );
                    SendMail(tran, 1 /*Update*/, dt);
                }

                return true;
            }
            catch (Exception ex)

            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

                con.Close();
                return false;

            }
        }
        public bool DeleteTransaction(Transaction tran)
        {
            SqlConnection con = new SqlConnection(cstr);
            System.Data.DataTable dt = null;

            try
            {
                if (ConfigurationManager.AppSettings.Get("NotifyOnDelete").ToString().ToLower() == "true")
                    dt = GetMailDetails(tran, 2 /*Delete*/ );

                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                tran.Action = "Delete_Transaction";
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                cmd.Parameters.AddWithValue("@transaction_id", tran.transaction_id);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                if (ConfigurationManager.AppSettings.Get("NotifyOnDelete").ToString().ToLower() == "true")
                {
                    tran.transaction_date = DateTime.Parse(dt.Rows[0]["TransactionDate"].ToString());
                    tran.hours = decimal.Parse(dt.Rows[0]["Hours"].ToString());
                    SendMail(tran, 2 /*Delete*/, dt);
                }

                return true;
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

                con.Close();
                return false;

            }
        }
        public List<Transaction> RetriveTotalHoursByTranDate(Transaction tran)
        {
            List<Transaction> lstTotalHours = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "TotalHours_ByDate");
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstTotalHours.Add(new Transaction
                        {
                            hours = Convert.ToDecimal(dr["Total_Hours"]),


                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }
            return lstTotalHours;
        }
        public List<Transaction> RetriveWeekTotalHours(Transaction tran)
        {
            List<Transaction> lstWeekTotalHours = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "TotalWeekHours");
                //cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstWeekTotalHours.Add(new Transaction
                        {
                            TotalHours = Convert.ToDecimal(dr["Total_Hours"]),


                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }
            return lstWeekTotalHours;
        }
        public List<Transaction> RetriveWeekTotalHoursByWeekDate(Transaction tran)
        {
            List<Transaction> lstWeekTotalHours = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_ReportTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "TotalWeekHours");
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstWeekTotalHours.Add(new Transaction
                        {
                            TotalHours = Convert.ToDecimal(dr["Total_Hours"]),


                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

            }
            return lstWeekTotalHours;
        }

        public List<Transaction> RetriveTotalHoursByTranDateAndTranID(Transaction tran)
        {
            List<Transaction> lstTotalHours = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "TotalHours_ByDate_TransactionId");
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                cmd.Parameters.AddWithValue("@transaction_id", tran.transaction_id);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstTotalHours.Add(new Transaction
                        {
                            hours = Convert.ToDecimal(dr["Total_Hours"]),


                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");

            }
            return lstTotalHours;
        }
        public bool FinalSubmitTransaction(Transaction obj_tran)
        {
            SqlConnection con = new SqlConnection(cstr);
            try
            {

                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                obj_tran.Action = "Final_Submit_By_Empid_TranID_TranDate";
                cmd.Parameters.AddWithValue("@Action", obj_tran.Action);
                cmd.Parameters.AddWithValue("@transaction_id", obj_tran.transaction_id);
                cmd.Parameters.AddWithValue("@employee_id", obj_tran.employee_id);
                //cmd.Parameters.AddWithValue("@transaction_date", obj_tran.transaction_date);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
                con.Close();
                return false;

            }
        }
        public List<Transaction> CheckFinalButtonCount(Transaction tran)
        {
            List<Transaction> lstFinalCount = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Check_FinalSubmit");

                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstFinalCount.Add(new Transaction
                        {


                            FinalSubmitCount = Convert.ToInt32(dr["final_submit_count"]),


                        });
                    }

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }
            return lstFinalCount;
        }
        public List<Transaction> ReportWeekSheet(Transaction tran)
        {
            List<Transaction> lstReport = new List<Transaction>();
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_ReportTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                cmd.Parameters.AddWithValue("@task_id", tran.task_id);
                cmd.Parameters.AddWithValue("@client_id", tran.client_id);
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstReport.Add(new Transaction
                        {
                            transaction_id = Convert.ToInt32(dr["tranid"]),
                            employee_id = Convert.ToInt32(dr["Empid"]),
                            client_id = Convert.ToInt32(dr["client_id"]),
                            task_id = Convert.ToInt32(dr["task_id"]),
                            description = Convert.ToString(dr["description"]),
                            hours = Convert.ToDecimal(dr["hours"]),

                            WeekDate = Convert.ToDateTime(dr["weekdate"]),
                            WeekDays = Convert.ToString(dr["WeekDays"]),
                            strDate = Convert.ToString(dr["weekdate"]),
                            client_name = Convert.ToString(dr["client_name"]),
                            task = Convert.ToString(dr["task"]),

                            CurrentDate = System.DateTime.Now,
                            strCurrentDate = Convert.ToString(CurrentDate),

                        });

                    }
                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }

            return lstReport;

        }
        public DataTable LoginAuth()
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_LogIn", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@user_id", user_id);
                cmd.Parameters.AddWithValue("@first_name", first_name);
                cmd.Parameters.AddWithValue("@last_name", last_name);
                cmd.Parameters.AddWithValue("@email", email);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    return dt;

                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }
            return dt;

        }
        public List<Transaction> GetEmployee(Transaction tran)
        {
            List<Transaction> lstEmployee = new List<Transaction>();
            DataTable dt = new DataTable();
            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_LogIn", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                cmd.Parameters.AddWithValue("@user_id", tran.user_id);
                cmd.Parameters.AddWithValue("@first_name", tran.first_name);
                cmd.Parameters.AddWithValue("@last_name", tran.last_name);
                cmd.Parameters.AddWithValue("@email", tran.email);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            lstEmployee.Add(new Transaction
                            {

                                employee_id = Convert.ToInt32(dr["employee_id"]),
                                Fullname = Convert.ToString(dr["Fullname"]),
                                first_name = Convert.ToString(dr["first_name"]),
                                last_name = Convert.ToString(dr["last_name"]),
                                email = Convert.ToString(dr["email"]),
                            });


                        }
                    }


                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }
            return lstEmployee;

        }
        public List<Transaction> AdminReportWeekSheet(Transaction tran)
        {
            List<Transaction> lstAdminReport = new List<Transaction>();
            DataTable dt = new DataTable();

            try
            {
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_ReportTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                cmd.Parameters.AddWithValue("@employee_id", tran.employee_id);
                cmd.Parameters.AddWithValue("@task_id", tran.task_id);
                cmd.Parameters.AddWithValue("@client_id", tran.client_id);
                cmd.Parameters.AddWithValue("@transaction_date", tran.transaction_date);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        lstAdminReport.Add(new Transaction
                        {
                            transaction_id = Convert.ToInt32(dr["tranid"]),
                            employee_id = Convert.ToInt32(dr["Empid"]),
                            client_id = Convert.ToInt32(dr["client_id"]),
                            task_id = Convert.ToInt32(dr["task_id"]),
                            description = Convert.ToString(dr["description"]),
                            hours = Convert.ToDecimal(dr["hours"]),

                            WeekDate = Convert.ToDateTime(dr["weekdate"]),
                            WeekDays = Convert.ToString(dr["WeekDays"]),
                            strDate = Convert.ToString(dr["weekdate"]),
                            client_name = Convert.ToString(dr["client_name"]),
                            task = Convert.ToString(dr["task"]),

                            CurrentDate = System.DateTime.Now,
                            strCurrentDate = Convert.ToString(CurrentDate),

                        });

                    }
                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }

            return lstAdminReport;

        }
        public bool InsertUpdateSettingTimesheet(Transaction tran)
        {


            SqlConnection con = new SqlConnection(cstr);
            try
            {

                SqlCommand cmd = new SqlCommand("USP_InsertUpdateSetting", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                if (tran.Action == "Insert")
                {
                    cmd.Parameters.AddWithValue("@st_StartDate", tran.st_StartDate);
                    cmd.Parameters.AddWithValue("@st_EndDate", tran.st_EndDate);
                    cmd.Parameters.AddWithValue("@st_Active", tran.st_Active);
                }
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
                con.Close();
                return false;

            }
        }
        public List<Transaction> CheckTimeSheetVisible(Transaction tran)
        {

            List<Transaction> lstCheckTimeSheetVisible = new List<Transaction>();
            try
            {
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(cstr);
                SqlCommand cmd = new SqlCommand("USP_InsertUpdateRetriveTransaction", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", tran.Action);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {

                            lstCheckTimeSheetVisible.Add(
                              new Transaction
                              {
                                  st_Active = Convert.ToInt32(dr["st_Active"]),
                                  strst_StartDate = Convert.ToString(dr["startdate"]),
                                  strst_EndDate = Convert.ToString(dr["enddate"]),
                              }
                          );

                        }
                    }
                    else
                    {
                        lstCheckTimeSheetVisible.Add(
                          new Transaction
                          {
                              st_Active = 0,
                          }
                      );
                    }



                }
                else
                {
                    lstCheckTimeSheetVisible.Add(
                          new Transaction
                          {
                              st_Active = 0,
                          }
                      );
                }

            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }


            return lstCheckTimeSheetVisible;


        }
        #endregion

        #region [ Send Mail ]

        /// <summary>
        /// Function to send mail on Create/Update/Delete timesheet
        /// </summary>
        /// <param name="iEmpId"></param>
        /// <param name="iTaskId"></param>
        /// <param name="iClientId"></param>
        /// <returns></returns>
        private bool SendMail(Transaction oCTRansaction, int iAction, DataTable dtMailDetals)
        {
            bool bSuccess = false;

            try
            {
                if (dtMailDetals != null && dtMailDetals.Rows.Count > 0)
                {
                    CMailCreator oMail = new CMailCreator();
                    oMail.SendMail(dtMailDetals, oCTRansaction.transaction_date, oCTRansaction.hours, iAction);
                }

                bSuccess = true;
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "Model");
            }

            return bSuccess;
        }

        /// <summary>
        /// Get emplloyee name, client name and task name for sending mail
        /// </summary>
        /// <param name="oCTransaction"></param>
        /// <param name="iAction"></param>
        /// <returns></returns>
        private DataTable GetMailDetails(Transaction oCTransaction, int iAction)
        {
            SqlConnection con = new SqlConnection(cstr);
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {

                SqlCommand cmd = new SqlCommand("USP_Mail_DetailsSelect", con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (iAction != 2)
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", oCTransaction.employee_id);
                    cmd.Parameters.AddWithValue("@ClientId", oCTransaction.client_id);
                    cmd.Parameters.AddWithValue("@TaskId", oCTransaction.task_id);
                }
                else
                    cmd.Parameters.AddWithValue("@TransactionId", oCTransaction.transaction_id);

                con.Open();

                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);

                if (dt == null) throw new Exception();

                con.Close();
            }
            catch (Exception ex)
            {
                //ExceptionLog.WriteToErrorLog(ex.Message, ex.StackTrace, "GetMailDetails");
            }

            return dt;
        }

        #endregion

    }
}