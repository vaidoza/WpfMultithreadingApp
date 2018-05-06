using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfSQLApp
{
    class MyThread
    {
        private string _threadId;
        private string _threadTime;
        private string _threadData;

        public string ThreadId
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        public string ThreadTime
        {
            get { return _threadTime; }
            set { _threadTime = value; }
        }

        public string ThreadData
        {
            get { return _threadData; }
            set { _threadData = value; }
        }


        public MyThread()
        {

        }

        public MyThread(string threadid,string threadtime,string threaddata)
        {
            _threadId = threadid;
            _threadTime = threadtime;
            _threadData = threaddata;
        }
    }
}
