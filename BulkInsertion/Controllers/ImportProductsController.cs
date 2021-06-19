using BulkInsertion.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace BulkInsertion.Controllers
{
    public class ImportProductsController : Controller
    {
        // GET: ImportProducts
        ContosoPetsEntities db = new ContosoPetsEntities();
        private string conn = ConfigurationManager.AppSettings["connectionString"].ToString();
        public ActionResult Index()
        {
            var productlist = db.Products.OrderByDescending(a=>a.Id).ToList();
            return View(productlist);
        }
        public ActionResult ImportData()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PreviewData(HttpPostedFileBase ExternalFile)
        {
            List<Product> list = new List<Product>();
            try
            {

                if ((ExternalFile != null) && (ExternalFile.ContentLength>0) && (!string.IsNullOrEmpty(ExternalFile.FileName)))
                {
                    string fileName = ExternalFile.FileName;
                    string fileContentType = ExternalFile.ContentType;
                    byte[] fileBytes = new byte[ExternalFile.ContentLength];
                    var data = ExternalFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(ExternalFile.ContentLength));
                    using (var package = new ExcelPackage(ExternalFile.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        
                        for (int rowIterator=2; rowIterator <= noOfRow; rowIterator++)
                        {
                            string prodName = string.Empty;
                            string prodCat = string.Empty;
                            string prodDesc = string.Empty;
                            decimal price =0;

                            
                            if (workSheet.Cells[rowIterator,1].Value !=null)
                            {
                                prodName = workSheet.Cells[rowIterator, 1].Value.ToString();
                            }
                            if (workSheet.Cells[rowIterator, 2].Value != null)
                            {
                                prodCat = workSheet.Cells[rowIterator, 2].Value.ToString();
                            }
                            if (workSheet.Cells[rowIterator, 3].Value != null)
                            {
                                prodDesc = workSheet.Cells[rowIterator, 3].Value.ToString();
                            }
                            if (workSheet.Cells[rowIterator, 4].Value != null)
                            {
                                price = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value);
                            }
                            Product product = new Product();
                            product.Name = prodName;
                            product.ProdCategory = prodCat;
                            product.ProdDescriptions = prodDesc;
                            product.Price = price;

                            list.Add(product);
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(list);
        }
        [HttpPost]
        public string SaveData(string jsondata)
        {
            List<Product> list = new List<Product>();
            ///Deserialize jsondata to product list and pass as parameter to BulkInsertion method
            list = JsonConvert.DeserializeObject<List<Product>>(jsondata);
        var result=    BulkInsertion(list, conn);
            if (result !=null)
            {
                TempData["Success"] = "Data Import  Successfully";
            }

            return "";
        }
        public string BulkInsertion(List<Product> list,string conn)
        {
            // convert product list to xml 
            var XmlList = ListToXml<List<Product>>(list);
            string result = "";
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.AppSettings["connectionString"].ToString());
                SqlCommand cmd = new SqlCommand("ImportProducts", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@CustomerID", 0);    
                cmd.Parameters.AddWithValue("@xml", XmlList);
               
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                con.Close();
                return result;
            }
            catch (Exception ex)
            {
                return result = ex.ToString();
            }
            //finally
            //{
            //    con.Close();
            //}

        }
        public  String ListToXml<T>(T obj)
        {

            if (obj == null)
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                //Encoding = new UnicodeEncoding(false, false), // no BOM in a .NET string
                Indent = false,
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8


            };

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj);
                }
                string xml = textWriter.ToString();
                return xml;
            }
        }
    }
}