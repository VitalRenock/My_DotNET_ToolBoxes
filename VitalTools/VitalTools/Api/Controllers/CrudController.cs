using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using VitalTools.Api.Services;
using VitalTools.Mapper;
using VitalTools.Model.Services;

namespace VitalTools.Api.Controllers
{
	public abstract class CrudController<TOrigin, TTarget> : ApiController, IServiceApiCRUD<TTarget>
	{
		// Service Client
		protected IServiceModelCRUD<TOrigin> Service;

		// Mapper
		protected IMapper<TOrigin, TTarget> Mapper;

		// CRUD Methods
		public virtual int Add([FromBody] TTarget entity) 
		{
			return Service.Add(Mapper.ToOrigin(entity));
		}
		
		public virtual bool Delete(int id)
		{
			return Service.Delete(id);
		}

		public virtual bool Edit(int id, [FromBody] TTarget entity)
		{
			return Service.Edit(id, Mapper.ToOrigin(entity));
		}

		public virtual IEnumerable<TTarget> GetAll()
		{
			return Service.GetAll().Select(e => Mapper.ToTarget(e));
		}

		public virtual TTarget Get(int id)
		{
			return Mapper.ToTarget(Service.Get(id));
		}
	}
}