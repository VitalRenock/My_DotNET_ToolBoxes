using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace VitalTools.Api.Services
{
	public interface IServiceApiCRUD<TEntity>
	{
		IEnumerable<TEntity> GetAll();
		TEntity Get(int id);
		int Add([FromBody] TEntity entity);
		bool Edit(int id, [FromBody] TEntity entity);
		bool Delete(int id);
	}
}