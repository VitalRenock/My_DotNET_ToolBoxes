using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Database
{
	public class Command
	{
		#region Properties
		
        internal string Query { get; private set; }
		internal bool IsStoredProcedure { get; private set; }
		internal Dictionary<string, object> Parameters { get; private set; }

		#endregion

		#region Constructors
		
		public Command(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				throw new ArgumentException("'query' isn't valid!!");

			Query = query;
			Parameters = new Dictionary<string, object>();
		}

		public Command(string query, bool isStoredProcedure) : this(query)
		{
			IsStoredProcedure = isStoredProcedure;
		}

		#endregion

		#region Public Methods
		
		public void AddParameter(string parameterName, object value)
		{
			Parameters.Add(parameterName, value ?? DBNull.Value);
		}

		#endregion
	}
}
