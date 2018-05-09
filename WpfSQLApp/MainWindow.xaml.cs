using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSQLApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {

        private string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=BWThreadDb.mdb;User Id=admin;Password=;";
        private bool _cancel;

        public MainWindow()
        {
            InitializeComponent();

        }

        BackgroundWorker bgWorker;

        private int StrToIntParser(int defaultval,string tstr)
        {
            return string.IsNullOrEmpty(tstr) ? defaultval : int.Parse(tstr);
        }

        private void StartBacgroundWorkerThread()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += BgWorker_DoWork;
            bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
            bgWorker.ProgressChanged += BgWorker_ProgressChanged;
            bgWorker.RunWorkerAsync(StrToIntParser(10000, tbxPeriod.Text));
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
            string thrcntstr = tbxThrdCnt.Text;
            int tcnt = StrToIntParser(1, tbxThrdCnt.Text);
            lblStatusInfo.Content = "Multi-threading process started with " + tcnt + " BackgroundWorker Thread" + (tcnt == 1 ? "" : "s")  +".";

            for (int i = 0; i < tcnt; i++)
            {
                StartBacgroundWorkerThread();
            }

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

        }

        private void BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            prgrBarOperationProgress.Value = e.ProgressPercentage;
            if (e.UserState != null)
                lblStatusInfo.Content = e.UserState;
            string queryString;
            queryString = "SELECT TOP 20 ThreadId,Time AS ThrTime,Data AS ThrData FROM ThreadData ORDER BY ID DESC";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbCommand command = new OleDbCommand(queryString, connection);
                OleDbDataReader reader = command.ExecuteReader();

                ThreadsList.Items.Clear();
                // Open the connection in a try/catch block. 
                // Create and execute the DataReader, writing the result
                // set to the listview
                while (reader.Read())
                {
                    var dbDateTime = reader["ThrTime"];
                    DateTime? dt = (dbDateTime == System.DBNull.Value)          //check and parse date
                        ? (DateTime?)null
                        : Convert.ToDateTime(dbDateTime);
                    DateTime dt2 = dt ?? DateTime.MinValue;
                    ThreadsList.Items.Add(new MyThread() {
                        ThreadId = reader["ThreadId"].ToString(),
                        ThreadTime = dt2.ToString(@"yyyy\/MM\/dd HH:mm:ss"),
                        ThreadData = reader["ThrData"].ToString()
                    });
                }
                reader.Close();
            }

 
        }

        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                lblStatusInfo.Content = "Cancelled..";          //fnc. could be exetended in nearly future
            } else
                lblStatusInfo.Content = "Work completed.";
            //MessageBox.Show("Numbers between 0 and 10000 divisible by 7: " + e.Result);
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            //int max = (int)e.Argument;
            //int result = 0;
            //for (int i = 0; i < max; i++)
            //{
            //    int progressPercentage = Convert.ToInt32(((double)i / max) * 100);
            //    if (i % 42 == 0)
            //    {
            //        result++;
            //        BackgroundWorker bw = sender as BackgroundWorker;
            //        if (bw == null)
            //            Console.WriteLine("{0} is not a BackgroundWorker", sender.GetType().Name);
            //        else
            //            bw.ReportProgress(progressPercentage, i);
            //        //(sender as BackgroundWorker).ReportProgress(progressPercentage, i);
            //    }
            //    else
            //    {
            //        BackgroundWorker bg = sender as BackgroundWorker;
            //        if (bg == null)
            //            Console.WriteLine("{0} ins not a BackgroundWorker",sender.GetType().Name);
            //        else
            //        {
            //            bg.ReportProgress(progressPercentage);
            //        }
            //    }
            //    if (bgWorker.CancellationPending)
            //    {
            //        // this is important as it set the cancelled property of RunWorkerCompletedEventArgs to true
            //        e.Cancel = true;
            //        break;
            //    }
            //    System.Threading.Thread.Sleep(1);

            //}
            //e.Result = result;

            string queryString = "INSERT INTO ThreadData (ThreadID,[Time],[Data]) VALUES (@threadId,@threadtime,@threaddata);";
            int max = (int)e.Argument;
            int cnt=0;
            int ProgressPercentage = 0;
            _cancel = false;    //pakeista vieta is btn1

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Create the Command and Parameter objects.

                Random rnd = new Random(); //Random Object test
                while ((!_cancel) && (cnt < max)) 
                {
                    
                    OleDbCommand command = new OleDbCommand(queryString, connection);
                    command.Parameters.AddWithValue("@threadId", System.Threading.Thread.CurrentThread.ManagedThreadId);
                    command.Parameters.AddWithValue("@threadtime", DateTime.Now);
                    command.Parameters.AddWithValue("@threaddata", StringGenerator.RandomString(rnd.Next(5, 10)));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        BackgroundWorker bgwork = sender as BackgroundWorker;
                        ProgressPercentage = Convert.ToInt32((double) cnt / max * 100);
                        bgwork.ReportProgress(ProgressPercentage);
                        System.Threading.Thread.Sleep(rnd.Next(1000, 2500));
                        //if (workerThread.CancellationPending)
                        //{
                        //    // this is important as it set the cancelled property of RunWorkerCompletedEventArgs to true
                        //    e.Cancel = true;
                        //    break;
                        //}

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    cnt++;
                }
            }
        
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            _cancel = true;
            bgWorker.CancelAsync();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbxThrdCnt.Text = "1";
            tbxPeriod.Text = "60";
            btnStop.IsEnabled = false;
        }
    }
}
