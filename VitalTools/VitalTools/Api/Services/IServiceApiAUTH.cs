using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace VitalTools.Api.Services
{
	public interface IServiceApiAUTH<TUser>
	{
		TUser Login(string pseudo, string password);
		bool Register([FromBody] TUser user);
	}
}