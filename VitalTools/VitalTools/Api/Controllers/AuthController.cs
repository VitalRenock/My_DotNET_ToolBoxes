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
	public abstract class AuthController<TOrigin, TTarget> : ApiController, IServiceApiAUTH<TTarget>
	{
		// Service Client
		protected IServiceModelAUTH<TOrigin> Service;

		// Mapper
		protected IMapper<TOrigin, TTarget> Mapper;

		// AUTH Methods
		public virtual TTarget Login(string pseudo, string password)
		{
			return Mapper.ToTarget(Service.Login(pseudo, password));
		}
		public virtual bool Register([FromBody] TTarget user)
		{
			return Service.Register(Mapper.ToOrigin(user));
		}
	}
}