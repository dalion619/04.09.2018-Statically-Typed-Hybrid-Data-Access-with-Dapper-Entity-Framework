using Example.DynamicMapping.DataAccess.EF;
using Example.DynamicMapping.EntityModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Example.DynamicMapping.DynamicDatabaseMapping.DatabaseModels;

namespace Example.DynamicMapping.Logic
{
    public static class PersonLogic
    {


        public static async Task<List<PersonEntity>> GetPeopleEf()
        {
            try
            {
                using (var db = new ExampleDbContext())
                {

                    return await db.People.ToListAsync();
                }
            }
            catch (Exception e)
            {
                return new List<PersonEntity>();
            }
        }

        public static async Task<List<PersonEntity>> GetPeopleDapper()
        {
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ExampleConnection"].ConnectionString))
                {
                    await con.OpenAsync();
                    return (await con.QueryAsync<PersonEntity>($"select * from {DatabaseMapping.People}")).ToList();
                   
                }
            }
            catch (Exception e)
            {
                return new List<PersonEntity>();
            }
        }
    }
}
