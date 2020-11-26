﻿using CustomerServicePortal.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace CustomerServicePortal.Controllers
{
    [Authorize]
    public class MemberDetailsController : Controller
    {
        //private DBManager db = new DBManager("CustomerServicePortal");
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

        private static List<DEDMET_OOP_Model> GetDEDMETOOP(string SSN,int Year)
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
                dependentDetailModel.DependenetName = (item["NAME"].ToString().Split('*')[1] + "*" + item["NAME"].ToString().Split('*')[0]); 
                dependentDetailModel.Relation = item["RELATION"].ToString();
                dependentDetailModel.Status = item["STATUS"].ToString();
                dependentDetailModel.Year = item["DOBY"].ToString();
                dependentDetailModel.Month = item["DOBM"].ToString();
                dependentDetailModel.Day = item["DOBD"].ToString();
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
                eMPdetails.Name =  (item["Name"].ToString().Split('*')[1] + "*" + item["Name"].ToString().Split('*')[0]);
                eMPdetails.Gender = item["Gender"].ToString();
                eMPdetails.DOBDay = item["DOBD"].ToString();
                eMPdetails.DOBMonth = item["DOBM"].ToString();
                eMPdetails.DOBYear = item["DOBY"].ToString();
                eMPdetails.EMSSN = (decimal)item["EMSSN"];
            }

            return eMPdetails;
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
                claimDetailModel.ClaimNo = item["ClaimNumber"].ToString();
                claimDetailModel.For = (item["ForPerson"].ToString().Split('*')[1] + "*" + item["ForPerson"].ToString().Split('*')[0]); 
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

        //public ActionResult DedMetOOPCurrentYear()
        //{
          
        //    return View(_DEDMET_OOP_CurentYear_PartialView);
        //}

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
    }
}