using CustomerServicePortal.Models;
using System.Data;
using System.Web;

namespace CustomerServicePortal.DAL
{
    public class GetLayoutSessionClass
    {
        public static LayoutModel GetLayoutModel()
        {
            LayoutModel layoutModel = new LayoutModel();
            if (HttpContext.Current.Session["LayoutDetails"] == null)
            {
                DBManager db = new DBManager("CustomerServicePortal");
                string CommandText = "Select * from LayoutDetails";
                DataTable dt = new DataTable();
                dt = db.GetDataTable(CommandText, CommandType.Text);

                foreach (DataRow item in dt.Rows)
                {
                    layoutModel.HeaderTitle = item["HeaderTitle"].ToString();
                    layoutModel.Headerlogo = item["Headerlogo"].ToString();
                    layoutModel.FooterText = item["FooterText"].ToString();
                }
                HttpContext.Current.Session["LayoutDetails"] = layoutModel;
            }
            else
            {
                layoutModel = (LayoutModel)HttpContext.Current.Session["LayoutDetails"];
            }

            return layoutModel;
        }
    }
}