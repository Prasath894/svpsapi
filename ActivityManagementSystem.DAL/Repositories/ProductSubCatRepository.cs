using Dapper;
using ActivityManagementSystem.DAL.Constants;
using ActivityManagementSystem.DAL.Infrastructure;
using ActivityManagementSystem.DAL.Interfaces;
using ActivityManagementSystem.Domain.AppSettings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiShop.Product.DAL.Interfaces;

namespace MultiShop.Product.DAL.Repositories
{
	public class ProductSubCatRepository: IProductSubCatRepository
	{
		private readonly AppSettings _appSettings;
		private readonly IDataBaseConnection _db;

		public  ProductSubCatRepository(AppSettings appSettings, IDataBaseConnection db)
		{
			_appSettings = appSettings;
			_db = db;
		}


		//public Task<bool> CreateProductSubcategory(ProductSubCategory productSubCategory, int currentUserId)
		//{
		//	throw new NotImplementedException();
		//}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_db.Connection?.Dispose();
			}
		}
		//public Task<List<ProductSubCategory>> GetAllProductSubCategories()
		//{
		//	var spName = ConstantSPnames.GetProductCategory;
		//	return Task.Factory.StartNew(() => _db.Connection.Query<ProductSubCategory>(spName, new { NPRODUCTSUBCATID = "", NPRODUCTSUBCATENAME="", IsActive ="" }, commandType: CommandType.StoredProcedure).ToList());
		//}

  //      public Task<bool> CreateProductSubcategory(ProductSubCategory productSubCategory, int currentUserId)
  //      {
  //          throw new NotImplementedException();
  //      }
    }
}
