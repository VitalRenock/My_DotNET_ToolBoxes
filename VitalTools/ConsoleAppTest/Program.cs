using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VitalTools.Database;
using VitalTools.Database.RequestFactory;
using VitalTools.Extensions;

namespace ConsoleAppTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=VitalDB;Integrated Security=True;";
			Connection connection = new Connection(SqlClientFactory.Instance, connectionString);

			//Command command = new Command("SELECT Count([Name]) FROM [Game]");
			//Console.WriteLine(connection.ExecuteScalar(command));

			Command command = new Command("SELECT [Name] FROM [Game]");
			IEnumerable<string> names = connection.ExecuteReader(command, g => g["Name"].ToString());
			foreach (string name in names)
				Console.WriteLine(name);

			//Command command = new Command("INSERT INTO [Movie](Title, Director) OUTPUT INSERTED.Id VALUES (@title, @director);");
			//command.AddParameter("title", "Star Wars");
			//command.AddParameter("director", "G.Lucas");
			//Console.WriteLine(connection.ExecuteScalar(command));

			Console.ReadLine();
		}
	}
}