using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using CarDealers.Models;
using System.Configuration;

namespace CarDealers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(new List<CarsalesModel>());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile)
        {
            List<CarsalesModel> Carsales = new List<CarsalesModel>();
            List<SalesQuantityModel> SalesQuantity = new List<SalesQuantityModel>();

            string mostSoldCar = "";
            string cars = ConfigurationManager.AppSettings["cars"];
            string[] carNames = cars.Split(',');
            foreach (string car in carNames)
            {
                SalesQuantity.Add(new SalesQuantityModel { CarName = car, QuantitySold = 0 });
            }

            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);
                csvData = csvData.Replace("\"", "");

                //Execute a loop over the rows.
                foreach (string row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        Carsales.Add(new CarsalesModel
                        {
                            DealNumber = row.Split(',')[0],
                            CustomerName = row.Split(',')[1],
                            DealershipName = row.Split(',')[2],
                            Vehicle = row.Split(',')[3],
                            Price = "CAD$" + row.Split(',')[4],
                            Date = row.Split(',')[5]
                        });

                        string VehicleDescription = (row.Split(',')[3]).ToLower();
                        foreach (SalesQuantityModel sale in SalesQuantity)
                        {
                            if (VehicleDescription.Contains(sale.CarName))
                            {
                                sale.QuantitySold = sale.QuantitySold + 1;
                            }
                        }
                    }
                }

                var mostSoldCars = SalesQuantity.OrderByDescending(x => x.QuantitySold).FirstOrDefault();
                mostSoldCar = mostSoldCars.CarName.ToUpperInvariant();
            }

            ViewBag.Message = "Vehicle that was sold most often is " + mostSoldCar;
            return View(Carsales);
        }

        public ActionResult About()
        {
            ViewBag.Message = "This app is prepared by S M Imrul Kabir";

            return View();
        }
    }
}