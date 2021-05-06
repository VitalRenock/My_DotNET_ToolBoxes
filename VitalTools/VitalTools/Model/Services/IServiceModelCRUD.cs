using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VitalTools.Model.Services
{
	public interface IServiceModelCRUD<TEntityModel>
	{
		IEnumerable<TEntityModel> GetAll();
		TEntityModel Get(int id);
		int Add(TEntityModel item);
		bool Edit(int id, TEntityModel item);
		bool Delete(int id);
	}
}