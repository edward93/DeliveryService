using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using DeliveryService.Helpers.DataTableHelper.Models;

namespace DeliveryService.Helpers.DataTableHelper
{

    public class DataTable<T>
    {
        private readonly int _totalRows;
        private readonly List<T> _data;
        private readonly DataTableData<T> _tableData;
        private readonly DataParam _params;

        public DataTable(List<T> data, DataParam param)
        {
            _totalRows = data.Count;
            _data = data;
            _tableData = new DataTableData<T>();
            _params = param;
        }

        public DataTableData<T> AjaxGetJsonData()
        {
            var search = _params.Search;
            var sortColumn = -1;
            var sortDirection = "asc";

            if (_params.SortColumn != null)
            {
                sortColumn = (int)_params.SortColumn;
            }
            if (_params.SortDirection != null)
            {
                sortDirection = _params.SortDirection;
            }

            _tableData.draw = _params.Draw;
            _tableData.recordsTotal = _data.Count;
            var recordsFiltered = 0;
            _tableData.data = FilterData(ref recordsFiltered, _params.Start, _params.Length, search, sortColumn, sortDirection);
            _tableData.recordsFiltered = recordsFiltered;
            return _tableData;
        }

        private int SortString(string s1, string s2, string sortDirection)
        {
            return sortDirection == "asc" ? string.Compare(s1, s2, StringComparison.Ordinal) : string.Compare(s2, s1, StringComparison.Ordinal);
        }

        private int SortInteger(string s1, string s2, string sortDirection)
        {
            var i1 = int.Parse(s1);
            var i2 = int.Parse(s2);
            return sortDirection == "asc" ? i1.CompareTo(i2) : i2.CompareTo(i1);
        }

        private int SortDateTime(string s1, string s2, string sortDirection)
        {
            var d1 = DateTime.Parse(s1);
            var d2 = DateTime.Parse(s2);
            return sortDirection == "asc" ? d1.CompareTo(d2) : d2.CompareTo(d1);
        }

        public List<T> FilterData(ref int recordFiltered, int start, int length, string search, int sortColumn, string sortDirection)
        {
            var list = _data;
            if (search == null)
            {
                list = _data;
            }
            else
            {
                // simulate search
                /* foreach (T dataItem in Data)
                 {
                     if (dataItem.Name.ToUpper().Contains(search.ToUpper()) ||
                         dataItem.Age.ToString().Contains(search.ToUpper()) ||
                         dataItem.DoB.ToString().Contains(search.ToUpper()))
                     {
                         list.Add(dataItem);
                     }
                 }*/
            }

            // simulate sort
            /*  if (sortColumn == 0)
              {// sort Name
                  list.Sort((x, y) => SortString(x.Name, y.Name, sortDirection));
              }
              else if (sortColumn == 1)
              {// sort Age
                  list.Sort((x, y) => SortInteger(x.Age, y.Age, sortDirection));
              }
              else if (sortColumn == 2)
              {   // sort DoB
                  list.Sort((x, y) => SortDateTime(x.DoB, y.DoB, sortDirection));
              }*/

            recordFiltered = list.Count;

            // get just one page of data
            list = list.GetRange(start, Math.Min(length, list.Count - start));

            return list;
        }
    }
}