using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace JsonServerTest
{
    public class Employee
    {
        public int id { get; set; }
        public string name { get; set; }
        public string salary { get; set; }
    }

    [TestClass]
    public class UnitTests
    {
        RestClient client;

        [TestInitialize]
        public void Setup()
        {
            //RestSharp is a HTTP client library.We use REST to consume HTTP APIs in DotNet.
            //RestSharp is used for creating a request, adding parameters to the request,
            //execution, and handling of requests etc.
            client = new RestClient("http://localhost:3000");   //connection with server 
        }

        public IRestResponse getEmployeeList()
        {
            //Arrange
            //create an instance of the RestRequest - pass resource name, method
            RestRequest request = new RestRequest("/employee", Method.GET); //creates a new request 

            //Act
            //execute the request
            IRestResponse response = client.Execute(request);   
            return response;
        }

        [TestMethod]
        public void OnCallingGETApi_ReturnEmployeeList()
        {
            //IRestResponse contains the info - headers, content, HTTP status, etc returned from server.
            IRestResponse response = getEmployeeList();

            //Assert
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.OK);
            List<Employee> dataResponse;
            //Deserialize - convert JSON string to object/string.
            dataResponse = JsonConvert.DeserializeObject<List<Employee>>(response.Content);
            Assert.AreEqual(6, dataResponse.Count);

            foreach (Employee emp in dataResponse)
            {
                System.Console.WriteLine("Id: " + emp.id + "\t Name: " + emp.name + "\t Salary: " + emp.salary);
            }
        }

        [TestMethod]
        public void GivenEmployee_OnPOSTApi_ShouldReturnAddedEmployee()
        {
            RestRequest request = new RestRequest("/employee", Method.POST);
            JObject jObject = new JObject();        //JObject=Json Object
            jObject.Add("name", "Om");
            jObject.Add("salary", "400");

            request.AddParameter("application/json", jObject, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

            Employee dataResponce = JsonConvert.DeserializeObject<Employee>(response.Content);

            Assert.AreEqual("Om", dataResponce.name); 
            Assert.AreEqual("400", dataResponce.salary);
            System.Console.WriteLine(response.Content);
        }
    }
}
