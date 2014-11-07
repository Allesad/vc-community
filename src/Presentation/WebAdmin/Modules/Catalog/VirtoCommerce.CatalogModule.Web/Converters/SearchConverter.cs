﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omu.ValueInjecter;
using moduleModel = VirtoCommerce.CatalogModule.Model;
using webModel = VirtoCommerce.CatalogModule.Web.Model;

namespace VirtoCommerce.CatalogModule.Web.Converters
{
	public static class SearchConverter
	{
		public static webModel.SearchCriteria ToWebModel(this moduleModel.SearchCriteria criteria)
		{
			var retVal = new webModel.SearchCriteria();
			retVal.InjectFrom(criteria);
			retVal.ResponseGroup = (webModel.ResponseGroup)(int)criteria.ResponseGroup;
			return retVal;
		}

		public static moduleModel.SearchCriteria ToModuleModel(this webModel.SearchCriteria criteria)
		{
			var retVal = new moduleModel.SearchCriteria();
			retVal.InjectFrom(criteria);
			retVal.ResponseGroup = (moduleModel.ResponseGroup)(int)criteria.ResponseGroup;
			
			return retVal;
		}


	}
}
