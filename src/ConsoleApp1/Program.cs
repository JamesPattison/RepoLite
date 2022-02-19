using System;
using System.Collections.Generic;
using System.Linq;
using Biomind.Data.Procedures;
using Biomind.Data.Repositories;

namespace ConsoleApp1
{
    class Program
    {
        //private const string cn = "Data Source=.;Initial Catalog=Repolite;Integrated Security=true;";
        private const string cn = "Server=localhost;Port=3306;Database=democlient;User Id=root;Password=passw0rd;";
        
        static void Main(string[] args)
        {
            // var sproctester = new SprocTester(cn);
            //
            // var draftEventRepository = new event__drafteventRepository(cn);
            // var draftEvent = draftEventRepository.Get(1);
            // var all = draftEventRepository.GetAll();
            //
            // //sproctester.CreateOwner(2, "Test", "Data", DateTime.Now);
            // //var cars = sproctester.FindOwnersCarByName("Petra", "Donnelly");
            // //var companyInfo = sproctester.GetCompanyInfo(1);
            // //sproctester.CreateCars(new[] {new SprocTester.CreateCars_Request_Car("Test", "Todel", "Karishma")});
            // //var createdCars = sproctester.CreateCarsAndReturn(new[] {new SprocTester.CreateCarsAndReturn_Request_Car("Test", "Todel", "Taran")});
            // //var allCars = sproctester.GetAllCars();
            // //var database = sproctester.GetDatabase();

            var entityRepository = new organisation_data__entityRepository(cn);
            var entities = entityRepository.Get(1);
            
            var events = new Procedures(cn).event__get_events_by_ids("11111111-1111-1111-1111-11111111111", new List<Procedures.event__get_events_by_ids___temp_event_ids_Param>
            {
                new (1)
            });
            
            Console.WriteLine("Hello World!");
        }
    }
}