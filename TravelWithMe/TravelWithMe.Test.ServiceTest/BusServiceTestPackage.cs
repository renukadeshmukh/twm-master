using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.Test.ServiceTest
{
    [TestClass]
    public class BusServiceTestPackage
    {
        public const string BaseUrl = "http://localhost:8971/";

        [TestMethod]
        public void GetAllBusesTest()
        {
            //string authUrl = 
            string url = BaseUrl + "get/BusService.svc/GetAllBuses/"+Guid.NewGuid();
            HttpClient hc = new HttpClient(url, ContentType.Xml);
            GetAllBusesResponse resp =  hc.Get<GetAllBusesResponse>();

            Assert.IsNotNull(resp.Buses);
            //HttpClient hc=new HttpClient(
        }
    }
}
