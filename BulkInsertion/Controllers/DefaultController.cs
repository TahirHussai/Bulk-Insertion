using BulkInsertion.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BulkInsertion.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        string connection = System.Configuration.ConfigurationManager.AppSettings["connectionString"];

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// In this method we are convertin model list to XML the passing xml as parameter from stored procedure
        /// </summary>
        /// <returns></returns>
        public string BulkSaveInsertion()
        {
            List<Product> list = new List<Product>();
            list.Add(new Product { Name = "Product1", Price = 90 });
            list.Add(new Product { Name = "Product2", Price = 91 });
            list.Add(new Product { Name = "Product3", Price = 92 });
            list.Add(new Product { Name = "Product4", Price = 93 });
            list.Add(new Product { Name = "Product5", Price = 94 });
            list.Add(new Product { Name = "Product6", Price = 95 });
            list.Add(new Product { Name = "Product7", Price = 96 });
            list.Add(new Product { Name = "Product8", Price = 97 });
            list.Add(new Product { Name = "Product9", Price = 98 });
            list.Add(new Product { Name = "Product10", Price = 99 });
            list.Add(new Product { Name = "Product11", Price = 100 });
            list.Add(new Product { Name = "Product12", Price = 101 });
            list.Add(new Product { Name = "Product13", Price = 102 });

           var xml= ConvetlistToXml.ObjectToXMLGeneric(list);

            var parameters = new SqlParameter[] {
            new SqlParameter("@xml", xml),
           };
            DataTable table = Datatble(connection, "Import_Products", parameters);
            if (table.Rows.Count > 0)
            {

            }
            return "";
        }

        public static DataTable Datatble(string con, string sql, SqlParameter[] parameters = null)
        {
            DataTable tab = new DataTable();
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = con;
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            if (parameters != null)
            {
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parameters)
                {
                    cmd.Parameters.Add(p);
                }
            }
            else
            {
                cmd.CommandType = CommandType.Text;
            }
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(tab);
            conn.Close();
            return tab;
        }
    }
}