using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using Newtonsoft.Json;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using SqlKata.Contract.CreateTable;
using SqlKata.DbTypes.DbColumn;
using SqlKata.DbTypes.Enums;

namespace Program
{
    class Program
    {
        private class Loan
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public List<Installment> Installments { get; set; } = new List<Installment>();
        }

        private class Installment
        {
            public string Id { get; set; }
            public string LoanId { get; set; }
            public int DaysCount { get; set; }
        }

        static void Main(string[] args)
        {
            var query = new Query("Users").CreateTable(new List<TableColumnDefenitionDto>()
            {
                new TableColumnDefenitionDto()
                {
                    ColumnName= "id",
                    ColumnDbType = new PostgresqlDBColumn()
                    {
                        PostgresqlDbType = PostgresqlDbType.Integer
                    },
                    IsAutoIncrement=true,
                    IsNullable=false,
                    IsPrimaryKey=true,
                    IsUnique=false,
                },
                new TableColumnDefenitionDto()
                {
                    ColumnName= "FullName",
                    ColumnDbType = new PostgresqlDBColumn()
                    {
                        PostgresqlDbType = PostgresqlDbType.Character_varying,
                        Length = 30
                    },
                    IsAutoIncrement=false,
                    IsNullable=false,
                    IsPrimaryKey=false,
                    IsUnique=true,
                }
            },TableType.Temporary);
            var compiler = new PostgresCompiler();
            Console.WriteLine(compiler.Compile(query));

/*            using (var db = SqlLiteQueryFactory())
            {
                var query = db.Query("accounts")
                    .Where("balance", ">", 0)
                    .GroupBy("balance")
                .Limit(10);

                var accounts = query.Clone().Get();
                Console.WriteLine(JsonConvert.SerializeObject(accounts, Formatting.Indented));

                var exists = query.Clone().Exists();
                Console.WriteLine(exists);
            }*/
        }

        private static void log(Compiler compiler, Query query)
        {
            var compiled = compiler.Compile(query);
            Console.WriteLine(compiled.ToString());
            Console.WriteLine(JsonConvert.SerializeObject(compiled.Bindings));
        }

        private static QueryFactory SqlLiteQueryFactory()
        {
            var compiler = new SqliteCompiler();

            var connection = new SQLiteConnection("Data Source=Demo.db");

            var db = new QueryFactory(connection, compiler);

            db.Logger = result =>
            {
                Console.WriteLine(result.ToString());
            };

            if (!File.Exists("Demo.db"))
            {
                Console.WriteLine("db not exists creating db");

                SQLiteConnection.CreateFile("Demo.db");

                db.Statement("create table accounts(id integer primary key autoincrement, name varchar, currency_id varchar, balance decimal, created_at datetime);");
                for (var i = 0; i < 10; i++)
                {
                    db.Statement("insert into accounts(name, currency_id, balance, created_at) values(@name, @currency, @balance, @date)", new
                    {
                        name = $"Account {i}",
                        currency = "USD",
                        balance = 100 * i * 1.1,
                        date = DateTime.UtcNow,
                    });
                }

            }

            return db;

        }

        private static QueryFactory SqlServerQueryFactory()
        {
            var compiler = new PostgresCompiler();
            var connection = new SqlConnection(
               "Server=tcp:localhost,1433;Initial Catalog=Lite;User ID=sa;Password=P@ssw0rd"
           );

            var db = new QueryFactory(connection, compiler);

            db.Logger = result =>
            {
                Console.WriteLine(result.ToString());
            };

            return db;
        }

    }
}
