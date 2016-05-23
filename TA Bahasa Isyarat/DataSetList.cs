using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA_Bahasa_Isyarat
{
    class DataSetList
    {
        private List<DataSet> _dataSetList;

        public List<DataSet> dataSetList
        {
            get { return _dataSetList; }
            set { _dataSetList = value; }
        }

        public int Count
        {
            get
            {
                return _dataSetList.Count;
            }
        }

        public DataSet this[int index]
        {
            set
            {
                _dataSetList[index] = value;
            }
            get
            {
                return _dataSetList[index];
            }
        }

        public DataSetList()
        {
            _dataSetList = new List<DataSet>();
        }

        public DataSetList(DataSetList dsl)
        {
            _dataSetList = new List<DataSet>();

            for(int i = 0; i < dsl.Count; i++)
            {
                _dataSetList.Add(new DataSet(dsl[i]));
            }
        }

        public void Add(DataSet ds)
        {
            _dataSetList.Add(ds);
        }
    }
}
