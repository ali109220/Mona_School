using Core.SharedDomain.IndexEntity;
using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ConstantService
    {
        public ConstantService()
        {

        }
        public List<IndexEntity> GetResultData(int pageIndex, int pageSize, List<GridSort> sorts, List<GridFilter> filters, List<IndexEntity> data)
        {
            var _data = data;
            if (filters != null && filters.Any())
            {
                if (filters.Any(x=> x.Field == "name"))
                {
                    _data = _data.Where(x => x.Name.Contains(filters.FirstOrDefault(y => y.Field == "name").Value)).ToList();
                }
                else if (filters.Any(x => x.Field == "arabicName"))
                {
                    data = data.Where(x => x.ArabicName.Contains(filters.FirstOrDefault(y => y.Field == "arabicName").Value)).ToList();
                }
                data = _data.Intersect(data).ToList();
            }
            if (sorts != null && sorts.Any())
            {
                if(sorts.Count == 1)
                {
                    if (sorts.Any(x => x.Field == "name"))
                    {
                        if (sorts.FirstOrDefault(y => y.Field == "name").Dir == "ascend")
                            data = data.OrderBy(x => x.Name).ToList();
                        else
                            data = data.OrderByDescending(x => x.Name).ToList();
                    }

                    else if (sorts.Any(x => x.Field == "arabicName"))
                    {
                        if (sorts.FirstOrDefault(y => y.Field == "arabicName").Dir == "ascend")
                            data = data.OrderBy(x => x.ArabicName).ToList();
                        else
                            data = data.OrderByDescending(x => x.ArabicName).ToList();
                    }
                }
                else if(sorts.FirstOrDefault().Field == "name")
                {
                    if (sorts.FirstOrDefault().Dir == "ascend" && sorts.LastOrDefault().Dir == "ascend")
                        data = data.OrderBy(x => x.Name).ThenBy(x=> x.ArabicName).ToList();
                    else if (sorts.FirstOrDefault().Dir == "ascend" && sorts.LastOrDefault().Dir == "decend")
                        data = data.OrderBy(x => x.Name).ThenByDescending(x => x.ArabicName).ToList();
                    else if (sorts.FirstOrDefault().Dir == "decend" && sorts.LastOrDefault().Dir == "ascend")
                        data = data.OrderByDescending(x => x.Name).ThenBy(x => x.ArabicName).ToList();
                    else if (sorts.FirstOrDefault().Dir == "decend" && sorts.LastOrDefault().Dir == "decend")
                        data = data.OrderByDescending(x => x.Name).ThenByDescending(x => x.ArabicName).ToList();
                }
                else
                {
                    if (sorts.LastOrDefault().Dir == "ascend" && sorts.FirstOrDefault().Dir == "ascend")
                        data = data.OrderBy(x => x.ArabicName).ThenBy(x => x.Name).ToList();
                    else if (sorts.LastOrDefault().Dir == "ascend" && sorts.FirstOrDefault().Dir == "decend")
                        data = data.OrderBy(x => x.ArabicName).ThenByDescending(x => x.Name).ToList();
                    else if (sorts.LastOrDefault().Dir == "decend" && sorts.FirstOrDefault().Dir == "ascend")
                        data = data.OrderByDescending(x => x.ArabicName).ThenBy(x => x.Name).ToList();
                    else if (sorts.LastOrDefault().Dir == "decend" && sorts.FirstOrDefault().Dir == "decend")
                        data = data.OrderByDescending(x => x.ArabicName).ThenByDescending(x => x.Name).ToList();
                }
            } 
            data = data.Skip((pageIndex - 1) * pageSize).ToList();
            return data;
        }
    }
}
