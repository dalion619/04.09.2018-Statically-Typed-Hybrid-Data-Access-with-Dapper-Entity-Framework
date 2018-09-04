using Example.DynamicMapping.EntityModels;
using Example.DynamicMapping.Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Example.DynamicMapping.DynamicDatabaseMapping.DatabaseModels;

namespace Example.DynamicMapping
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dapperSW = new Stopwatch();
            dapperSW.Start();
            var dapper = await PersonLogic.GetPeopleDapper();
            dapperSW.Stop();

            var entityFrameworkSW = new Stopwatch();
            entityFrameworkSW.Start();
            var entityFramework = await PersonLogic.GetPeopleEf();
            entityFrameworkSW.Stop();
           
            Console.ReadLine();
        }
       
    }

   
}
