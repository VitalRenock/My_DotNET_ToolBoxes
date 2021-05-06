using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Model.Services
{
	public interface IServiceModelAUTH<TUser>
	{
		bool Register(TUser user);
		TUser Login(string email, string password);
	}
}