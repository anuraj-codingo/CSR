using CustomerServicePortal.Models;
using System.Data;
using System.Web;

namespace CustomerServicePortal.DAL
{
    public class GetLayoutSessionClass
    {
        public static LayoutModel GetLayoutModel(string Client)
        {

            LayoutModel layoutModel = new LayoutModel();
            try
            {
                if (((LayoutModel)HttpContext.Current.Session["LayoutDetails"]).HeaderTitle == null)
                {
                    DBManager db = new DBManager("CustomerServicePortal");
                    string CommandText = "Select * from LayoutDetails Where CLIENT='" + Client + "'";
                    DataTable dt = new DataTable();
                    dt = db.GetDataTable(CommandText, CommandType.Text);
                    if (dt.Rows.Count>0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            layoutModel.HeaderTitle = item["HeaderTitle"].ToString();
                            layoutModel.Headerlogo = item["Headerlogo"].ToString();
                            layoutModel.FooterText = item["FooterText"].ToString();
                            layoutModel.BackgroundImg = item["BackgroundImg"].ToString();
                            layoutModel.Client = item["CLIENT"].ToString();
                            layoutModel.ShowMenuBar = true;

                        }
                    }
                    else
                    {
                         CommandText = "Select * from LayoutDetails Where CLIENT='ABC'";                       
                        dt = db.GetDataTable(CommandText, CommandType.Text);
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                layoutModel.HeaderTitle = item["HeaderTitle"].ToString();
                                layoutModel.Headerlogo = item["Headerlogo"].ToString();
                                layoutModel.FooterText = item["FooterText"].ToString();
                                layoutModel.BackgroundImg = item["BackgroundImg"].ToString();
                                layoutModel.ShowMenuBar = false;
                                layoutModel.Client = item["CLIENT"].ToString();
                            }
                        }
                    }
                  
                    HttpContext.Current.Session["LayoutDetails"] = layoutModel;
                }
                else
                {
                    layoutModel = (LayoutModel)HttpContext.Current.Session["LayoutDetails"];
                }

            }
            catch (System.Exception)
            {

              
                    DBManager db = new DBManager("CustomerServicePortal");
                    string CommandText = "Select * from LayoutDetails Where CLIENT='ABC'";
                    DataTable dt = new DataTable();
                    dt = db.GetDataTable(CommandText, CommandType.Text);

                    foreach (DataRow item in dt.Rows)
                    {
                        layoutModel.HeaderTitle = item["HeaderTitle"].ToString();
                        layoutModel.Headerlogo = item["Headerlogo"].ToString();
                        layoutModel.FooterText = item["FooterText"].ToString();
                        layoutModel.BackgroundImg = item["BackgroundImg"].ToString();
                       layoutModel.ShowMenuBar = false;
                      layoutModel.Client = item["CLIENT"].ToString();
                }
                    HttpContext.Current.Session["LayoutDetails"] = layoutModel;
   
            }
        

            return layoutModel;
        }
    }
}