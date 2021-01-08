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
using ClosedXML.Excel;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using DB2SQlClass.DB2;
using DB2SQlClass.Model;

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
                try
                {
                    if ((bool)TempData.Peek("GlobalSearch"))
                    {
                        GetLayoutSessionClass.GetLayoutModel(Common.StaticClass.GetClientFromSSN(SSN));
                    }
                }
                catch (Exception)
                {
                    throw;
                }
               
                

                List<DEDMET_OOP_Model> dEDMET_OOP_CurrentYear_Models = GetDEDMETOOP(SSN,DateTime.Now.Year);
                claimDetailDashBoardModel.dEDMETModelCurentYear = dEDMET_OOP_CurrentYear_Models;

                List<DEDMET_OOP_Model> dEDMET_OOP_Previous_Models = GetDEDMETOOP(SSN,DateTime.Now.Year-1);
                claimDetailDashBoardModel.dEDMETModelPreviousYear = dEDMET_OOP_Previous_Models;

                List<DependentDetailModel> dependentDetailModels = GetDependentListModel(SSN);
                claimDetailDashBoardModel.dependentDetailModels = dependentDetailModels;

                List<ClaimDetailModel> claimDetailModels = new List<ClaimDetailModel>();
                GetClaimDetailsModel(SSN, "0", "", null, null,"", "ClaimDate", "DESC",1,10, claimDetailModels);
                claimDetailDashBoardModel.claimDetailModels = claimDetailModels;

                EMPdetails eMPdetails = GetEMployDetailsModelWIthSSN(SSN);
                claimDetailDashBoardModel.eMPdetails = eMPdetails;

                ViewBag.TotalCount_claimDetails = GetTotalCount_ClaimDetailTable(SSN,"0", "", null, null,"");


            }
            catch (Exception ex)
            {
                throw;
            }
            return View(claimDetailDashBoardModel);
        }
        public JsonResult UpdateAddress(EMPdetails emp)
        {
            return Json(new { viewContent = "" }, JsonRequestBehavior.AllowGet);
        }
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

        public ActionResult RequestIDCardHtml(string SSN,string Name,string Gender)
        {
            IDCardRequest iDCardRequest = new IDCardRequest();
            string viewContent = "";
            try
            {
                iDCardRequest.EMSSN = SSN;
                iDCardRequest.Gender = Gender;
                iDCardRequest.Name = Name;
                viewContent = ConvertViewToString("_RequestIdCardPartialView", iDCardRequest);
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


                    int start = s.ToLower().IndexOf("<img");
                    int end = s.ToLower().IndexOf("\"/>", start) + 2;
                    string result = s.Substring(start + 1, end - start - 1);

                    // Replace current client logo instead of /amo_logo.png
                    s = s.Replace(result, "IMG style=\"height: 100%; width: 100%; object-fit: contain\" SRC=\"http://10.68.5.91/CSR/Content/img/logo/"+layoutModel.Headerlogo+"\"");
                }

            }
            catch (Exception ex)
            {
                throw;
            }

            //string viewContent = ConvertViewToString("_MemeberDetailsPartialView", claimDetailModels);
            return Json(new { viewContent = s }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCliamDetailTable(int Page,int PageSize,string SSN, string DependentSeq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate,string Dependent,string SortingColumn, string Orderby)
        {
            List<ClaimDetailModel> claimDetailModels = new List<ClaimDetailModel>();
            int TotalCount = 0;
            try
            {
                GetClaimDetailsModel(SSN, DependentSeq, ClaimNumber, Fromdate, Todate, Dependent,SortingColumn, Orderby, Page, PageSize, claimDetailModels);
                TotalCount = GetTotalCount_ClaimDetailTable(SSN, DependentSeq, ClaimNumber, Fromdate, Todate, Dependent);
            }
            catch (Exception ex)
            {
                throw;
            }

            string viewContent = ConvertViewToString("_MemeberDetailsPartialView", claimDetailModels);
            return Json(new { Page = Page, TotalCount = TotalCount, viewContent = viewContent }, JsonRequestBehavior.AllowGet);
        }
        public int GetTotalCount_ClaimDetailTable(string SSN, string DependentSeq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate,string Dependent)
        {

            int TotalCount = 0;


            DataTable dt = new DataTable();
            try
            {
                dt = Db2Connnect.GetDataTable(GetSqlQuery.GetTotalCountClaimDetailTable(SSN, DependentSeq, ClaimNumber, Fromdate, Todate, Dependent), CommandType.Text);
                if (dt.Rows.Count>0)
                {
                    TotalCount = (int)dt.Rows[0]["TotalCount"];
                }
            }
            catch (Exception ex)
            {

                throw;
            }
         

            return TotalCount;
               
        }
        public JsonResult IdCardRequest(IDCardRequest iDCardRequest)
        {
            try
            {
                DBManager db = new DBManager("CustomerServicePortal");
                foreach (var item in iDCardRequest.iDCardType_Quantities)
                {
                    string Commandtext = "Fill_IDCardRequestDetails";
                var parameters = new List<IDbDataParameter>();
                parameters.Add(db.CreateParameter("@SSN", iDCardRequest.EMSSN, DbType.String));
                parameters.Add(db.CreateParameter("@RequestNotes", iDCardRequest.RquestNotes, DbType.String));
                parameters.Add(db.CreateParameter("@Name", iDCardRequest.Name, DbType.String));
                parameters.Add(db.CreateParameter("@Gender", iDCardRequest.Gender, DbType.String));
               
                parameters.Add(db.CreateParameter("@Requester", User.Identity.Name, DbType.String));

               
                    parameters.Add(db.CreateParameter("@IDCardType",item.IDCardType, DbType.String));
                    parameters.Add(db.CreateParameter("@IDCardQuantity",item.IDCardQuantity, DbType.String));
                    db.Insert(Commandtext, CommandType.StoredProcedure, parameters.ToArray());
                }
                

            }
            catch (Exception EX)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
              
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult ExportExcel(string SSN = "", string DEPSEQ = "", string claim = "", string Dependent = "", string SortingColumn = "", string Orderby = "", DateTime? Fromdate = null, DateTime? Todate = null)
        {

            DataTable ExcelDatatable = new DataTable();
            var fileName = "ClaimDetails" + ".xlsx";
            string fullPath = Path.Combine(Server.MapPath("~/ExcelFile"), fileName);

            try
            {
                DataTable dt = new DataTable();
                if (DEPSEQ == null || DEPSEQ == "0")
                {
                    dt = Db2Connnect.GetDataTable(GetSqlQuery.GeTMemberClaims(SSN, claim, Fromdate, Todate, Dependent, SortingColumn, Orderby, 0, 0), CommandType.Text);
                }
                else
                {
                    dt = Db2Connnect.GetDataTable(GetSqlQuery.GeTDependentClaims(SSN, DEPSEQ, claim, Fromdate, Todate, Dependent, SortingColumn, Orderby, 0, 0), CommandType.Text);
                }
                ExcelDatatable = GetClaimDetailExportDatatable(dt);

                using (SpreadsheetDocument myDoc = SpreadsheetDocument.Create(fullPath, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookpart = myDoc.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                    Worksheet ws = new Worksheet();
                    SheetData sheetData = new SheetData();



                    WorkbookStylesPart wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
                    wbsp.Stylesheet = GenerateStylesheet();
                    wbsp.Stylesheet.Save();

                    Columns columns = new Columns();
                    columns.Append(CreateColumnData(1, 1, 11));
                    columns.Append(CreateColumnData(2, 3, 16));
                    columns.Append(CreateColumnData(4, 4, 20));
                    columns.Append(CreateColumnData(5, 5, 11));
                    columns.Append(CreateColumnData(6, 6, 28));
                    columns.Append(CreateColumnData(7, 9, 11));



                    ws.Append(columns);
                    //add a row
                    Row firstRow = new Row();
                    firstRow.RowIndex = (UInt32)1;
                    Cell dataCell = new Cell();
                    CellValue cellValue = new CellValue();



                    firstRow = new Row();
                    dataCell = new Cell();
                    dataCell.DataType = CellValues.String;
                    dataCell.CellReference = "A1";
                    dataCell.StyleIndex = 2;
                    dataCell.CellValue = new CellValue("Filtered By");
                    firstRow.Append(dataCell);
                    sheetData.Append(firstRow);



                    firstRow = new Row();
                    dataCell = new Cell();
                    dataCell.DataType = CellValues.String;
                    dataCell.CellReference = "A2";
                    dataCell.StyleIndex = 0;
                    dataCell.CellValue = new CellValue("From Date : " + ((Fromdate == null) ? "ALL" : Fromdate?.ToString("MM/dd/yyyy")));
                    firstRow.Append(dataCell);
                    sheetData.Append(firstRow);

                    firstRow = new Row();
                    dataCell = new Cell();
                    dataCell.DataType = CellValues.String;
                    dataCell.CellReference = "A3";
                    dataCell.StyleIndex = 0;
                    dataCell.CellValue = new CellValue("To Date : " + ((Todate == null) ? "ALL" : Todate?.ToString("MM/dd/yyyy")));
                    firstRow.Append(dataCell);
                    sheetData.Append(firstRow);

                    firstRow = new Row();
                    dataCell = new Cell();
                    dataCell.DataType = CellValues.String;
                    dataCell.CellReference = "A4";
                    dataCell.StyleIndex = 0;
                    dataCell.CellValue = new CellValue("DEPN# : " + ((DEPSEQ == "") ? "ALL" : DEPSEQ));
                    firstRow.Append(dataCell);
                    sheetData.Append(firstRow);


                    firstRow = new Row();
                    dataCell = new Cell();
                    dataCell.DataType = CellValues.String;
                    dataCell.CellReference = "A5";
                    dataCell.StyleIndex = 0;
                    dataCell.CellValue = new CellValue("CLAIM# : " + ((claim == "") ? "ALL" : claim));
                    firstRow.Append(dataCell);
                    sheetData.Append(firstRow);




                    firstRow = new Row();

                    List<String> columns1 = new List<string>();
                    foreach (System.Data.DataColumn column in ExcelDatatable.Columns)
                    {
                        columns1.Add(column.ColumnName);
                        dataCell = new Cell();

                        dataCell.DataType = CellValues.String;
                        dataCell.StyleIndex = 2;
                        dataCell.CellValue = new CellValue(column.ColumnName);
                        firstRow.Append(dataCell);
                    }

                    sheetData.Append(firstRow);



                    foreach (DataRow dsrow in ExcelDatatable.Rows)
                    {
                        firstRow = new Row();
                        foreach (String col in columns1)
                        {
                            dataCell = new Cell();
                            dataCell.DataType = CellValues.String;
                            //c.CellReference = "A2";
                            dataCell.StyleIndex = 1;
                            dataCell.CellValue = new CellValue(dsrow[col].ToString());
                            firstRow.Append(dataCell);
                        }
                        sheetData.Append(firstRow);
                    }



                    ws.Append(sheetData);

                    worksheetPart.Worksheet = ws;
                    MergeCells mergeCells = new MergeCells();

                    //append a MergeCell to the mergeCells for each set of merged cells
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:I1") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A2:I2") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A3:I3") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A4:I4") });
                    mergeCells.Append(new MergeCell() { Reference = new StringValue("A5:I5") });

                    worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());

                    //this is the part that was missing from your code
                    Sheets sheets = myDoc.WorkbookPart.Workbook.AppendChild(new Sheets());
                    sheets.AppendChild(new Sheet()
                    {
                        Id = myDoc.WorkbookPart.GetIdOfPart(myDoc.WorkbookPart.WorksheetParts.First()),
                        SheetId = 1,
                        Name = "Sheet1"
                    });
                    //myDoc.WorkbookPart.Workbook = workbookpart.Workbook;
                    myDoc.WorkbookPart.Workbook.Save();
                    myDoc.Close();
                }
            }
            catch (Exception)
            {

                throw;
            }



            return Json(new { fileName = fileName });
        }

        private static Column CreateColumnData(UInt32 StartColumnIndex, UInt32 EndColumnIndex, double ColumnWidth)
        {
            Column column;
            column = new Column();
            column.Min = StartColumnIndex;
            column.Max = EndColumnIndex;
            column.Width = ColumnWidth;
            column.CustomWidth = true;
            return column;
        }


        private Stylesheet GenerateStylesheet()
        {
            Stylesheet styleSheet = null;

            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 12 }

                ),
                new Font( // Index 1 - header
                    new FontSize() { Val = 15 },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" }

                ));

            Fills fills = new Fills(
                    new Fill(new PatternFill() { PatternType = PatternValues.None }), // Index 0 - default
                    new Fill(new PatternFill() { PatternType = PatternValues.Gray125 }), // Index 1 - default
                    new Fill(new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "DAA520" } }, new BackgroundColor { Rgb = "#f7931e" })
                    { PatternType = PatternValues.Solid }) // Index 2 - header
                );

            Borders borders = new Borders(
                    new Border(), // index 0 default
                    new Border( // index 1 black border
                        new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                );

            CellFormats cellFormats = new CellFormats(
                    new CellFormat(), // default
                    new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // body
                    new CellFormat { FontId = 1, FillId = 2, BorderId = 1, ApplyFill = true } // header
                );

            styleSheet = new Stylesheet(fonts, fills, borders, cellFormats);

            return styleSheet;
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
            List<DEDMET_OOP_Model> dEDMET_OOP_Models = new List<DEDMET_OOP_Model>();


            DataTable dtDedScheduled = new DataTable();
            DataTable dtOOPScheduled = new DataTable();
            DataTable dtOOPMetFamily = new DataTable();
            DataTable dtOOPMetIndividual = new DataTable();
            DataTable dtDedMetIndividual = new DataTable();
            DataTable dtDedMetFamily = new DataTable();

            // This code will change based on client , MED is currently hardcoded for AMO
            string DeductibleCode = "MED";
            string FamilyCode = "F";
            string IndividualCode = "I";
            dtDedScheduled = Db2Connnect.GetDataTable(GetSqlQuery.GetDeductibleMax(DeductibleCode, Year.ToString()), CommandType.Text);
            dtOOPScheduled = Db2Connnect.GetDataTable(GetSqlQuery.GetOOPMax(DeductibleCode, Year.ToString()), CommandType.Text);
            dtOOPMetIndividual = Db2Connnect.GetDataTable(GetSqlQuery.GetOOPMet(DeductibleCode, Year.ToString(), IndividualCode, Convert.ToInt32(SSN)), CommandType.Text);
            dtOOPMetFamily = Db2Connnect.GetDataTable(GetSqlQuery.GetOOPMet(DeductibleCode, Year.ToString(), FamilyCode, Convert.ToInt32(SSN)), CommandType.Text);
            dtDedMetIndividual = Db2Connnect.GetDataTable(GetSqlQuery.GetDeductibleMet(DeductibleCode, Year.ToString(), IndividualCode, Convert.ToInt32(SSN)), CommandType.Text);
            dtDedMetFamily = Db2Connnect.GetDataTable(GetSqlQuery.GetDeductibleMet(DeductibleCode, Year.ToString(), FamilyCode, Convert.ToInt32(SSN)), CommandType.Text);

            return AccumullatorClass.GetAccumullatorLogic(Year,dEDMET_OOP_Models, dtDedScheduled, dtOOPScheduled, dtOOPMetFamily, dtOOPMetIndividual, dtDedMetIndividual, dtDedMetFamily);
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
                dependentDetailModel.DependenetName = item["NAME"].ToString().Replace("*", ",");
                //(item["NAME"].ToString().Split('*')[1] + "*" + item["NAME"].ToString().Split('*')[0]).Replace("*", ",").Replace(" ", "()").Replace(")(", "").Replace("()", " ");
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

                dependentDetailModel.ADDRESS1 = item["ADDRESS1"].ToString();
                dependentDetailModel.ADDRESS2 = item["ADDRESS2"].ToString();
                dependentDetailModel.ADDRESS3 = item["ADDRESS3"].ToString();
                dependentDetailModel.STATE = item["STATE"].ToString();
                dependentDetailModel.CITY = item["CITY"].ToString();
                dependentDetailModel.ZIP4 = item["ZIP4"].ToString();
                dependentDetailModel.ZIP5 = item["ZIP5"].ToString();

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
                dependentDetailModel.DependenetName = item["Name"].ToString().Replace("*", ","); 
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
                eMPdetails.Name = item["Name"].ToString().Replace("*", ",");
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
            string Commadtext = @"SELECT   CASE 
                                   WHEN COUNT(CASE WHEN complete = 0 then 1 ELSE NULL END)> 0
                                    THEN 0 ELSE  1 end from [IDCardRequestDetails] where EMSSN = @SSN";
            var parameters = new List<IDbDataParameter>();
            parameters.Add(db.CreateParameter("@SSN", SSN, DbType.String));
            eMPdetails.ShowRequestId = ((int)db.GetScalarValue(Commadtext, CommandType.Text, parameters.ToArray())==1)? true:false;
            return eMPdetails;
        }

        private static void GetClaimDetailsModel(string SSN, string DependentSeq, string ClaimNumber, DateTime? Fromdate, DateTime? Todate,string Dependent,string SortingColumn, string Orderby, int page, int size, List<ClaimDetailModel> claimDetailModels)
        {
            DataTable MemberClaimTable = new DataTable();
            if (DependentSeq == null || DependentSeq == "0")
            {
                MemberClaimTable = Db2Connnect.GetDataTable(GetSqlQuery.GeTMemberClaims(SSN, ClaimNumber, Fromdate, Todate, Dependent,SortingColumn, Orderby,page,size), CommandType.Text);
            }
            else
            {
                MemberClaimTable = Db2Connnect.GetDataTable(GetSqlQuery.GeTDependentClaims(SSN, DependentSeq, ClaimNumber, Fromdate, Todate, Dependent,SortingColumn, Orderby,page,size), CommandType.Text);
            }

            foreach (DataRow item in MemberClaimTable.Rows)
            {
                ClaimDetailModel claimDetailModel = new ClaimDetailModel();
                claimDetailModel.EOBNO = item["EOBNo"].ToString();
                claimDetailModel.SSN = SSN;
                claimDetailModel.ClaimNo = item["ClaimNumber"].ToString();
                claimDetailModel.For = item["ForPerson"].ToString().Replace("*",","); 
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
        private DataTable GetClaimDetailExportDatatable(DataTable Claimdetails)
        {
            DataTable dt = new DataTable("TEST");
            dt.Columns.Add("EOB No", typeof(string));
            dt.Columns.Add("Claim No", typeof(string));
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("For", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("Provider", typeof(string));
            dt.Columns.Add("Total", typeof(string));
            dt.Columns.Add("Plan Paid", typeof(string));
            dt.Columns.Add("Member Resp", typeof(string));
            for (int i = 0; i < Claimdetails.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["EOB No"] = Claimdetails.Rows[i]["EOBNo"].ToString();
                dr["Claim No"] = Claimdetails.Rows[i]["ClaimNumber"].ToString();
                if (Claimdetails.Rows[i]["ClaimYear"].ToString() !="0" && Claimdetails.Rows[i]["ClaimMonth"].ToString() != "0" && Claimdetails.Rows[i]["ClaimDate"].ToString() != "0")
                {
                    dr["Date"] = (new DateTime(Int16.Parse(Claimdetails.Rows[i]["ClaimYear"].ToString()), Int16.Parse(Claimdetails.Rows[i]["ClaimMonth"].ToString()), Int16.Parse(Claimdetails.Rows[i]["ClaimDate"].ToString()))).ToString("MM/dd/yyyy");
                }
                else
                {
                    dr["Date"] = "N/A";
                }
               
                dr["For"] = Claimdetails.Rows[i]["ForPerson"].ToString().Replace("*", ",");
                dr["Type"] = Claimdetails.Rows[i]["ClaimType"].ToString();
                dr["Provider"] = Claimdetails.Rows[i]["PROVIDER"].ToString();
                dr["Total"] = ((decimal)Claimdetails.Rows[i]["ClaimAmount"]).ToString("C", CultureInfo.CurrentCulture);
                dr["Plan Paid"] = ((decimal)Claimdetails.Rows[i]["Paid"]).ToString("C", CultureInfo.CurrentCulture); 
                dr["Member Resp"] = ((decimal)Claimdetails.Rows[i]["MemberPaid"]).ToString("C", CultureInfo.CurrentCulture);
                dt.Rows.Add(dr);
            }

            
            return dt;
        }
    }
}