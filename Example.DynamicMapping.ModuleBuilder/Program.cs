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

namespace Example.DynamicMapping.ModuleBuilder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CreateDynamicDatabaseModelModule();
        }
        private static void CreateDynamicDatabaseModelModule()
        {
            // Get tables and their columns
            var databaseTableInfo = GetDatabaseTableInfo();

            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName aName = new AssemblyName("Example.DynamicMapping.DynamicDatabaseMapping");
            AssemblyBuilder ab = domain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Save);
            System.Reflection.Emit.ModuleBuilder mb = ab.DefineDynamicModule(aName.Name, aName.Name + ".dll");

            // Define mapping static class of the database
            TypeBuilder typeDatabaseModel = mb.DefineType("Example.DynamicMapping.DynamicDatabaseMapping.DatabaseModels.DatabaseMapping", TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed);

            // Get base table class
            Type tableBase = typeof(SqlTableModelBase);
            ConstructorInfo tableBaseCtor = tableBase.GetConstructor(Type.EmptyTypes); // no parameters

            // Get base column class
            Type columnBase = typeof(SqlColumnModelBase);
            ConstructorInfo fieldBaseCtor = columnBase.GetConstructor(new[] { typeof(string) });

            // Needed for creating the class constructors in a loop
            var tableFields = new List<ReflectionDefineField>();
            foreach (var table in databaseTableInfo)
            {
                var columnFields = new List<ReflectionDefineField>();
                TypeBuilder typeTable = mb.DefineType($"Example.DynamicMapping.DynamicDatabaseMapping.DatabaseModels.{table.Name}", TypeAttributes.Public, typeof(SqlTableModelBase));

                // Create the column classes for the table
                foreach (var column in table.Columns)
                {
                    TypeBuilder typeColumn = mb.DefineType($"Example.DynamicMapping.DynamicDatabaseMapping.DatabaseModels.{table.Name}.{column.Name}", TypeAttributes.Public, typeof(SqlColumnModelBase));

                    ConstructorBuilder staticColumnConstructorBuilder =
                        typeColumn.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, new[] { typeof(string) });

                    ILGenerator staticColumnConstructorILGenerator = staticColumnConstructorBuilder.GetILGenerator();
                    staticColumnConstructorILGenerator.Emit(OpCodes.Ldarg_0);
                    staticColumnConstructorILGenerator.Emit(OpCodes.Ldarg_1);
                    staticColumnConstructorILGenerator.Emit(OpCodes.Call, fieldBaseCtor);
                    staticColumnConstructorILGenerator.Emit(OpCodes.Nop);
                    staticColumnConstructorILGenerator.Emit(OpCodes.Nop);
                    staticColumnConstructorILGenerator.Emit(OpCodes.Ret);
                    var createdColumn = typeColumn.CreateType();

                    ConstructorInfo columnConstructor = createdColumn.GetConstructor(new[] { typeof(string) });
                    columnFields.Add(new ReflectionDefineField()
                    {
                        ConstructorInfo = columnConstructor,
                        Type = createdColumn
                    });
                }

                ConstructorBuilder staticTableConstructorBuilder =
                    typeTable.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.HasThis, Type.EmptyTypes);
                ILGenerator staticTableConstructorILGenerator = staticTableConstructorBuilder.GetILGenerator();
                staticTableConstructorILGenerator.Emit(OpCodes.Ldarg_0);
                staticTableConstructorILGenerator.Emit(OpCodes.Call, tableBaseCtor);

                // Initialise constructor of column classes and assign to property on table class
                foreach (var field in columnFields)
                {
                    FieldBuilder fieldColumn = typeTable.DefineField(field.ConstructorInfo.DeclaringType.Name, field.Type, FieldAttributes.InitOnly | FieldAttributes.Public);
                    staticTableConstructorILGenerator.Emit(OpCodes.Ldarg_0);
                    staticTableConstructorILGenerator.Emit(OpCodes.Ldstr, table.Name);
                    staticTableConstructorILGenerator.Emit(OpCodes.Newobj, field.ConstructorInfo);
                    staticTableConstructorILGenerator.Emit(OpCodes.Stfld, fieldColumn); // Stfld => non-static field!
                }
                staticTableConstructorILGenerator.Emit(OpCodes.Ret);
                // Create the table class
                var createdTable = typeTable.CreateType();

                ConstructorInfo tableConstructor = createdTable.GetConstructor(Type.EmptyTypes);
                tableFields.Add(new ReflectionDefineField()
                {
                    ConstructorInfo = tableConstructor,
                    Type = createdTable
                });
            }
            ConstructorBuilder staticDatabaseConstructorBuilder =
                typeDatabaseModel.DefineConstructor(MethodAttributes.Private | MethodAttributes.PrivateScope | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Static, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator staticDatabaseConstructorILGenerator = staticDatabaseConstructorBuilder.GetILGenerator();
         
            // Initialise constructor of table classes and assign to property on mapping class
            foreach (var field in tableFields)
            {
                FieldBuilder fieldDatabase = typeDatabaseModel.DefineField(field.ConstructorInfo.DeclaringType.Name, field.Type, FieldAttributes.InitOnly | FieldAttributes.Public | FieldAttributes.Static);
                staticDatabaseConstructorILGenerator.Emit(OpCodes.Nop);
                staticDatabaseConstructorILGenerator.Emit(OpCodes.Newobj, field.ConstructorInfo);
                staticDatabaseConstructorILGenerator.Emit(OpCodes.Stsfld, fieldDatabase); // Stsfld => static field!
            }
            staticDatabaseConstructorILGenerator.Emit(OpCodes.Ret);

            // Create the mapping class
            var createdDatabaseModel = typeDatabaseModel.CreateType();
            ab.Save(aName.Name + ".dll"); // Save .dll module
        }
        private static List<DbTable> GetDatabaseTableInfo()
        {
            var temp = new List<DbTable>();
            var tables = ReadTableNames();
            foreach (var table in tables)
            {
                var columns = ReadColumnNames(table);
                temp.Add(
                    new DbTable() { Name = table, Columns = columns.Select(x => new DbColumn() { Name = x }).ToList() });
            }

            return temp;
        }

        #region Get Table Names from Database
        private static List<string> ReadTableNames()
        {
            var tableNames = new List<string>();
            string queryString =
                "SELECT * FROM sys.Tables";

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ExampleConnection"].ConnectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var tableName = ReadTableName((IDataRecord)reader);
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        tableNames.Add(tableName);
                    }
                }

                reader.Close();
            }

            return tableNames;
        }
        private static string ReadTableName(IDataRecord record)
        {
            return record["name"].ToString();
        } 
        #endregion

        #region Get Column Names for Table
        private static List<string> ReadColumnNames(string tableName)
        {
            var columnNames = new List<string>();
            string queryString =
                $"select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{tableName}'";

            using (SqlConnection connection =
                new SqlConnection(ConfigurationManager.ConnectionStrings["ExampleConnection"].ConnectionString))
            {
                SqlCommand command =
                    new SqlCommand(queryString, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var columnName = ReadColumnName((IDataRecord)reader);
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        columnNames.Add(columnName);
                    }
                }

                reader.Close();
            }

            return columnNames;
        }
        private static string ReadColumnName(IDataRecord record)
        {
            return record["COLUMN_NAME"].ToString();
        } 
        #endregion
    }
}
