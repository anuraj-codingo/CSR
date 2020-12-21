using CustomerServicePortal.DAL;
using CustomerServicePortal.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using CustomerServicePortal.Common;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class MemberDetailsController : Controller
    {
       
        // GET: MemberDetails
        [HttpPost]
        public ActionResult Index(string SSN = "")
        {
            ClaimDetailDashBoardModel claimDetailDashBoardModel = new ClaimDetailDashBoardModel();

            try
            {
                List<DEDMET_OOP_Model> dEDMET_OOP_CurrentYear_Models = GetDEDMETOOP(SSN,DateTime.Now.Year);
                claimDetailDashBoardModel.dEDMETModelCurentYear = dEDMET_OOP_CurrentYear_Models;

                List<DEDMET_OOP_Model> dEDMET_OOP_Previous_Models = GetDEDMETOOP(SSN,DateTime.Now.Year-1);
                claimDetailDashBoardModel.dEDMETModelPreviousYear = dEDMET_OOP_Previous_Models;

                List<DependentDetailModel> dependentDetailModels = GetDependentListModel(SSN);
                claimDetailDashBoardModel.dependentDetailModels = dependentDetailModels;

                List<ClaimDetailModel> claimDetailModels = new List<ClaimDetailModel>();
                GetClaimDetailsModel(SSN, "0", "", null, null, claimDetailModels);
                claimDetailDashBoardModel.claimDetailModels = claimDetailModels;

                EMPdetails eMPdetails = GetEMployDetailsModelWIthSSN(SSN);
                claimDetailDashBoardModel.eMPdetails = eMPdetails;


            }
            catch (Exception ex)
            {
                throw;
            }
            return View(claimDetailDashBoardModel);
        }
        //public  JsonResult UpdateAddress(EMPdetails emp)
        //{
        //    return Json(new { viewContent = "" }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult EditDependentHtml(string SSN,int DependentSeq)
        {
            DependentDetailModel dependentDetailModel = new DependentDetailModel();
            string viewContent = "";
            try
            {
                if (DependentSeq !=0)
                {
                    dependentDetailModel = GetDependentWithSEQtModel(SSN,DependentSeq);
                }


                viewContent = ConvertViewToString("_Dependent_Add_Edit_PartialView", dependentDetailModel);
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        
        public  JsonResult GetAddreessParialViewHtml(string SSN)
        {
            EMPdetails eMPdetails = new EMPdetails();
            string viewContent = "";
            try
            {
                 eMPdetails = GetEMployDetailsModelWIthSSN(SSN);
                 viewContent = ConvertViewToString("_Address_Add_Edit_PartialView",eMPdetails);
            }
            catch (Exception ex)
            {

                throw;
            }
         
            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetClaimDetailExpandModelHTml(string SSN,string Claimnumber)
        {
            List<ClaimDeatilExpandModel> claimDeatilExpandModels = new List<ClaimDeatilExpandModel>();

            try
            {
                GetClaimDetailsExpandList(SSN, Claimnumber, claimDeatilExpandModels);
            }
            catch (Exception ex)
            {

                throw;
            }
          
           

            string viewContent = ConvertViewToString("_ClaimDetailExpandPartialView", claimDeatilExpandModels);
            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }

        private static void GetClaimDetailsExpandList(string SSN, string Claimnumber, List<ClaimDeatilExpandModel> claimDeatilExpandModels)
        {
            DataTable dt = new DataTable();

            dt = Db2Connnect.GetDataTable(GetSqlQuery.GetClaimDetailsWIthClaimNumber(SSN, Claimnumber), CommandType.Text);
            foreach (DataRow item in dt.Rows)
            {
                ClaimDeatilExpandModel claimDeatilExpandModel = new ClaimDeatilExpandModel();
                claimDeatilExpandModel.BenefitCode = item["BenefitCode"].ToString();
                claimDeatilExpandModel.ClaimNo = item["ClaimNo"].ToString();
                claimDeatilExpandModel.Coinsurance = ((decimal)item["Coinsurance"]).ToString("C", CultureInfo.CurrentCulture); 
                claimDeatilExpandModel.CPT = item["CPT#"].ToString();
                claimDeatilExpandModel.Dedcutible = ((decimal)item["Dedcutible"]).ToString("C", CultureInfo.CurrentCulture);
                claimDeatilExpandModel.LineNo = ((decimal)item["LineNo"]).ToString();
                claimDeatilExpandModel.OOP = ((decimal)item["OOP"]).ToString("C", CultureInfo.CurrentCulture); ;
                claimDeatilExpandModel.Paid = ((decimal)item["Paid"]).ToString("C", CultureInfo.CurrentCulture); ;
                claimDeatilExpandModel.ProviderDiscount = ((decimal)item["ProviderDiscount"]).ToString("C", CultureInfo.CurrentCulture); 
                claimDeatilExpandModel.Status = item["Status"].ToString();
                claimDeatilExpandModel.TotalCharge = ((decimal)item["TotalCharge"]).ToString("C", CultureInfo.CurrentCulture);
                claimDeatilExpandModels.Add(claimDeatilExpandModel);
            }
        }
        public JsonResult GetEOBDetailsHtml(string SSN,string Claimnumber)
        {
          
            string s = "";
            try
            {
                LayoutModel layoutModel = new LayoutModel();
                layoutModel = (LayoutModel)Session["LayoutDetails"];
                if (layoutModel.Client !=null)
                {
                    System.Net.WebClient client = new System.Net.WebClient();
                    var theURL = string.Format("http://10.68.5.64/ReportServer_SSRS/Pages/ReportViewer.aspx?%2fDental+Portal%2fProviderEOB&rs:Format=HTML4.0&Claimnum={0}&Client={1}&rc:toolbar=false", Claimnumber, layoutModel.Client);
                    client.UseDefaultCredentials = false;

                    var credCache = new System.Net.CredentialCache();
                    credCache.Add(new Uri("http://10.68.5.64"),
                                     "NTLM",
                                     new System.Net.NetworkCredential("osvsethi", "aBcP@s@2019@OneSmarter", "abc"));

                    client.Credentials = credCache;
                    s = client.DownloadString(theURL);
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            //string viewContent = ConvertViewToString("_MemeberDetailsPartialView", claimDetailModels);
            return Json(new { viewContent = s }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCliamDetailTable(string SSN, string DependentSeq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate)
        {
            List<ClaimDetailModel> claimDetailModels = new List<ClaimDetailModel>();
            try
            {
                GetClaimDetailsModel(SSN, DependentSeq, ClaimNumber, Fromdate, Todate, claimDetailModels);
            }
            catch (Exception ex)
            {
                throw;
            }

            string viewContent = ConvertViewToString("_MemeberDetailsPartialView", claimDetailModels);
            return Json(new { viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IdCardRequest(string SSN,string Notes,string Name,string Gender)
        {
            try
            {
                DBManager db = new DBManager("CustomerServicePortal");
                string Commandtext = "Fill_IDCardRequestDetails";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@SSN", SSN, DbType.String));
                parameters.Add(db.CreateParameter("@RequestNotes", Notes, DbType.String));
                parameters.Add(db.CreateParameter("@Name", Name, DbType.String));
                parameters.Add(db.CreateParameter("@Gender", Gender, DbType.String));
                parameters.Add(db.CreateParameter("@Requester", User.Identity.Name, DbType.String));
                db.Insert(Commandtext,CommandType.StoredProcedure, parameters.ToArray());

            }
            catch (Exception EX)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
              
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }    
     
        [HttpPost]
        public JsonResult ExportExcel(string SSN = "", string DEPSEQ = "", string claim = "", DateTime? Fromdate = null, DateTime? Todate = null)
        {
            var fileName = "ClaimDetails" + ".xlsx";
            string fullPath = Path.Combine(Server.MapPath("~/ExcelFile"), fileName);
            try
            {
                DataTable dt = new DataTable();
                if (DEPSEQ == null || DEPSEQ == "0")
                {
                    dt = Db2Connnect.GetDataTable(GetSqlQuery.GeTMemberClaims(SSN, claim, Fromdate, Todate), CommandType.Text);
                }
                else
                {
                    dt = Db2Connnect.GetDataTable(GetSqlQuery.GeTDependentClaims(SSN, DEPSEQ, claim, Fromdate, Todate), CommandType.Text);
                }

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(fullPath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in dt.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in dt.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                    workbookPart.Workbook.Save();
                }
            }
            catch (Exception ex)
            {
                //Log.Error(ex);
                //Log.Fatal(ex);
                //throw;
                throw;
            }
            return Json(new { fileName = fileName });
        }

        [HttpGet]
        public ActionResult Download(string fileName)
        {
            string fullPath = Path.Combine(Server.MapPath("~/ExcelFile"), fileName);
            byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
            System.IO.File.Delete(fullPath);
            return File(fileByteArray, "application/vnd.ms-excel", fileName);
        }

        private static List<DEDMET_OOP_Model> GetDEDMETOOP(string SSN, int Year)
        {
            DataTable dt = new DataTable();
            dt = Db2Connnect.GetDataTable(GetSqlQuery.GetDEDMET_OOP_Details(Year, SSN, 0), CommandType.Text);
            List<DEDMET_OOP_Model> dEDMET_OOP_Models = new List<DEDMET_OOP_Model>();
            foreach (DataRow item in dt.Rows)
            {
                DEDMET_OOP_Model dEDMET_OOP_Model = new DEDMET_OOP_Model();
                dEDMET_OOP_Model.APPLIED = ((decimal)item["APPLIED"]).ToString("C", CultureInfo.CurrentCulture);
                dEDMET_OOP_Model.MAXIMUM = ((decimal)item["MAXIMUM"]).ToString("C", CultureInfo.CurrentCulture);
                dEDMET_OOP_Model.REMAINING = ((decimal)item["REMAINING"]).ToString("C", CultureInfo.CurrentCulture);
                dEDMET_OOP_Model.DESC = item["DESC"].ToString();
                dEDMET_OOP_Model.DEEFDY = item["DEEFDY"].ToString();
                dEDMET_OOP_Model.Year = (int)Year;
                dEDMET_OOP_Models.Add(dEDMET_OOP_Model);

            }

            return dEDMET_OOP_Models;
        }

        private static DependentDetailModel GetDependentWithSEQtModel(string SSN,int DEPSEQ)
        {
            DataTable dependenttable = new DataTable();
            dependenttable = Db2Connnect.GetDataTable(GetSqlQuery.GetDependentDetailsWithSeq(SSN,DEPSEQ), CommandType.Text);
            DependentDetailModel dependentDetailModel = new DependentDetailModel();
            foreach (DataRow item in dependenttable.Rows)
            {
               
                dependentDetailModel.SSN =item["DPSSN"].ToString();
                dependentDetailModel.DependentSeq = item["SEQ"].ToString();
                dependentDetailModel.DependenetName = (item["NAME"].ToString().Split('*')[1] + "*" + item["NAME"].ToString().Split('*')[0]).Replace("*", "").Replace(" ", "()").Replace(")(", "").Replace("()", " ");
                dependentDetailModel.Relation = item["RELATION"].ToString();
                dependentDetailModel.Status = item["STATUS"].ToString();
                dependentDetailModel.BirthYear = item["DOBY"].ToString();
                dependentDetailModel.BirthMonth = item["DOBM"].ToString();
                dependentDetailModel.BirthDay = item["DOBD"].ToString();
                dependentDetailModel.Class = item["CLASS"].ToString();
                dependentDetailModel.Plan = item["PLAN"].ToString();
                dependentDetailModel.BoolStatus = item["STATUS"].ToString() == "A" ? true : false;

                dependentDetailModel.EffectiveYear = item["EFDY"].ToString();
                dependentDetailModel.EffectiveMonth = item["EFDM"].ToString();
                dependentDetailModel.EffectiveDay = item["EFDD"].ToString();

                if (item["TDTY"].ToString()!="0" && item["TDTM"].ToString() != "0" && item["TDTD"].ToString() != "0")
                {
                    dependentDetailModel.TerminationDate = new DateTime(Convert.ToInt32(item["TDTY"].ToString()), Convert.ToInt32(item["TDTM"].ToString()), Convert.ToInt32(item["TDTD"].ToString()));
                }
                if (item["EFDY"].ToString() != "0" && item["EFDM"].ToString() != "0" && item["EFDD"].ToString() != "0")
                {
                    dependentDetailModel.EffectiveDate = new DateTime(Convert.ToInt32(item["EFDY"].ToString()), Convert.ToInt32(item["EFDM"].ToString()), Convert.ToInt32(item["EFDD"].ToString()));
                }



            }

            return dependentDetailModel;
        }
        private static List<DependentDetailModel> GetDependentListModel(string SSN)
        {
            DataTable dependenttable = new DataTable();
            dependenttable = Db2Connnect.GetDataTable(GetSqlQuery.GetDependentDetails(SSN), CommandType.Text);
            List<DependentDetailModel> dependentDetailModels = new List<DependentDetailModel>();
            foreach (DataRow item in dependenttable.Rows)
            {
                DependentDetailModel dependentDetailModel = new DependentDetailModel();
                dependentDetailModel.SSN = item["DPSSN"].ToString();
                dependentDetailModel.DependentSeq = item["SEQ"].ToString();
                dependentDetailModel.DependenetName = (item["NAME"].ToString().Split('*')[1] + "*" + item["NAME"].ToString().Split('*')[0]).Replace("*", "");
                dependentDetailModel.Relation = item["RELATION"].ToString();
                dependentDetailModel.Status = item["STATUS"].ToString();
                dependentDetailModel.BirthYear = item["DOBY"].ToString();
                dependentDetailModel.BirthMonth = item["DOBM"].ToString();
                dependentDetailModel.BirthDay = item["DOBD"].ToString();
                dependentDetailModel.Class = item["CLASS"].ToString();
                dependentDetailModel.Plan = item["PLAN"].ToString();

                dependentDetailModels.Add(dependentDetailModel);
            }

            return dependentDetailModels;
        }

        private static EMPdetails GetEMployDetailsModelWIthSSN(string SSN)
        {

           
            DataTable Employdetails = new DataTable();
            Employdetails = Db2Connnect.GetDataTable(GetSqlQuery.GetMemberDetailsWIthSSN(SSN), CommandType.Text);
            EMPdetails eMPdetails = new EMPdetails();
            foreach (DataRow item in Employdetails.Rows)
            {
                eMPdetails.Id = item["Id"].ToString();
                eMPdetails.Name = (item["Name"].ToString().Split('*')[1] + "*" + item["Name"].ToString().Split('*')[0]).Replace("*", "");
                eMPdetails.Gender = item["Gender"].ToString();
                eMPdetails.DOBDay = item["DOBD"].ToString();
                eMPdetails.DOBMonth = item["DOBM"].ToString();
                eMPdetails.DOBYear = item["DOBY"].ToString();
                eMPdetails.EMSSN = item["EMSSN"].ToString();
                eMPdetails.Addr1= item["Addr1"].ToString().TrimEnd().TrimStart();
                eMPdetails.Addr2 = item["Addr2"].ToString().TrimEnd().TrimStart();
                eMPdetails.Addr3 = item["Addr3"].ToString().TrimEnd().TrimStart();
                eMPdetails.Addr4 = item["Addr4"].ToString().TrimEnd().TrimStart();
                eMPdetails.City = item["City"].ToString().TrimEnd().TrimStart();
                eMPdetails.State = item["State"].ToString().TrimEnd().TrimStart();
                eMPdetails.Zip1 = item["Zip1"].ToString().TrimEnd().TrimStart();
                eMPdetails.Zip2 = item["Zip2"].ToString().TrimEnd().TrimStart();
                eMPdetails.Zip3 = item["Zip3"].ToString().TrimEnd().TrimStart();

            }
            DBManager db = new DBManager("CustomerServicePortal");
            string Commadtext = "SELECT  CASE WHEN COUNT(Id)>0  THEN 0  ELSE 1  END FROM[IDCardRequestDetails] where EMSSN = @SSN";
            var parameters = new List<IDbDataParameter>();
            parameters.Add(db.CreateParameter("@SSN", SSN, DbType.String));
            eMPdetails.ShowRequestId = ((int)db.GetScalarValue(Commadtext, CommandType.Text, parameters.ToArray())==1)? true:false;
            return eMPdetails;
        }

        private static void GetClaimDetailsModel(string SSN, string DependentSeq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate, List<ClaimDetailModel> claimDetailModels)
        {
            DataTable MemberClaimTable = new DataTable();
            if (DependentSeq == null || DependentSeq == "0")
            {
                MemberClaimTable = Db2Connnect.GetDataTable(GetSqlQuery.GeTMemberClaims(SSN, ClaimNumber, Fromdate, Todate), CommandType.Text);
            }
            else
            {
                MemberClaimTable = Db2Connnect.GetDataTable(GetSqlQuery.GeTDependentClaims(SSN, DependentSeq, ClaimNumber, Fromdate, Todate), CommandType.Text);
            }

            foreach (DataRow item in MemberClaimTable.Rows)
            {
                ClaimDetailModel claimDetailModel = new ClaimDetailModel();
                claimDetailModel.EOBNO = item["EOBNo"].ToString();
                claimDetailModel.SSN = SSN;
                claimDetailModel.ClaimNo = item["ClaimNumber"].ToString();
                claimDetailModel.For = (item["ForPerson"].ToString().Split('*')[1] + "*" + item["ForPerson"].ToString().Split('*')[0]).Replace("*", "");
                claimDetailModel.Type = item["ClaimType"].ToString();
                claimDetailModel.Total = ((decimal)item["ClaimAmount"]).ToString("C", CultureInfo.CurrentCulture);
                claimDetailModel.PlanPaid = ((decimal)item["Paid"]).ToString("C", CultureInfo.CurrentCulture);
                claimDetailModel.MemerResp = ((decimal)item["MemberPaid"]).ToString("C", CultureInfo.CurrentCulture);
                claimDetailModel.ClaimYear = item["ClaimYear"].ToString();
                claimDetailModel.ClaimMonth = item["ClaimMonth"].ToString();
                claimDetailModel.ClaimDate = item["ClaimDate"].ToString();
                claimDetailModel.Provider = item["PROVIDER"].ToString();
                claimDetailModels.Add(claimDetailModel);
            }
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
    }
}