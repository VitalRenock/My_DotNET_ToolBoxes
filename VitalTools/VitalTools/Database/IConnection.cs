using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Database
{
	public interface IConnection
	{
		object ExecuteScalar(Command command);
		IEnumerable<TResult> ExecuteReader<TResult>(Command command, Func<IDataRecord, TResult> selector);
		int ExecuteNonQuery(Command command);
		DataTable GetDataTable(Command command);
	}
}
