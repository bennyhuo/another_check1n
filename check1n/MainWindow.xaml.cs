using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace check1n
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<LogItem> collection = null;
        List<LogItem> logList = new List<LogItem>();

        private String startDate = null;
        private String endDate = null;

        private int currentMonth = 1;
        private int currentYear = 2014;

        private String logPath = null;

        public String Path
        {
            get
            {
                return logPath;
            }
            set
            {
                logPath = value;
            }
        }

        public int CurrentMonth
        {
            get
            {
                return currentMonth;
            }
            set
            {
                if (value > 12)
                {
                    currentMonth = 1;
                    currentYear += 1;
                }
                else if (value < 1)
                {
                    currentMonth = 12;
                    currentYear -= 1;
                }
                else
                {
                    currentMonth = value;
                }

                startDate = currentYear + "/" + currentMonth + "/" + 1;
                endDate = currentYear + "/" + currentMonth + "/" + DateTime.DaysInMonth(currentYear, currentMonth);

                startTimePicker.Text = startDate;
                endTimePicker.Text = endDate;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }


        private void onClick(object sender, RoutedEventArgs e)
        {
            if (startDate == null || endDate == null)
            {

                MessageBox.Show("请先设置起止时间。");
            }
            else
            {
                collection = new ObservableCollection<LogItem>();
                Thread t = new Thread(this.run);
                t.Start();
                //    MessageBox.Show("已经开始读取日志，请稍候……");

            }

        }

        private void run()
        {
            DateTime startTime = DateTime.Parse(startDate);
            DateTime endTime = DateTime.Parse(endDate);

            SimpleDate curDate = new SimpleDate();
            curDate.Year = startTime.Year;
            curDate.Day = startTime.Day;
            curDate.Month = startTime.Month;

            SimpleDate deadLine = new SimpleDate();
            deadLine.Year = endTime.Year;
            deadLine.Day = endTime.Day;
            deadLine.Month = endTime.Month;


            string[] logs = new string[] { "System" };

            StringBuilder result = new StringBuilder();

            foreach (string log in logs)
            {

                EventLog myLog = new EventLog();
                myLog.Log = log;


                bool isFirstLogComing = true;
                bool isLastLogComing = true;
                EventLogEntry curEntry = myLog.Entries[0];

                int i = 0;
                LogItem item = new LogItem();
                item.Index = ++i;

                foreach (EventLogEntry entry in myLog.Entries)
                {
                    //EventLogEntryType枚举包括：
                    //Error 错误事件。
                    //FailureAudit 失败审核事件。
                    //Information 信息事件。
                    //SuccessAudit 成功审核事件。
                    //Warning 警告事件。

                    SimpleDate tempDate = new SimpleDate();
                    tempDate.Year = entry.TimeGenerated.Year;
                    tempDate.Day = entry.TimeGenerated.Day;
                    tempDate.Month = entry.TimeGenerated.Month;

                    if (isLastLogComing && tempDate.isBefore(deadLine))
                    {
                        item.EndTime = curEntry.TimeGenerated.ToLocalTime().ToString();
                        collection.Add(item);

                        //sw.WriteLine(end + curEntry.TimeGenerated.ToLocalTime());
                        isLastLogComing = false;
                        break;
                    }

                    if (isFirstLogComing && tempDate.equals(curDate))
                    {
                        item.StartTime = entry.TimeGenerated.ToLocalTime().ToString();
                        //sw.WriteLine(begin + entry.TimeGenerated.ToLocalTime());
                        isFirstLogComing = false;
                    }

                    switch (curDate.compare(tempDate))
                    {
                        case -1:
                            break;
                        case 0:
                            break;
                        case 1:
                            //开始记录新一天的日志，同时将上一条记录的时间写入前一天当中。
                            curDate = tempDate;

                            item.EndTime = curEntry.TimeGenerated.ToLocalTime().ToString();
                            collection.Add(item);

                            item = new LogItem();
                            item.Index = ++i;
                            item.StartTime = entry.TimeGenerated.ToLocalTime().ToString();

                            //sw.WriteLine(end + curEntry.TimeGenerated.ToLocalTime());
                            //sw.WriteLine(begin + entry.TimeGenerated.ToLocalTime());
                            break;
                    }

                    curEntry = entry;
                }

                if (isLastLogComing && !isFirstLogComing)
                {
                    item.EndTime = curEntry.TimeGenerated.ToLocalTime().ToString();
                    collection.Add(item);

                    // sw.WriteLine(end + curEntry.TimeGenerated.ToLocalTime());

                    isLastLogComing = false;
                }


            }

            this.Dispatcher.BeginInvoke(new showMessagebox(showMessage), new object[] { "日志写入完毕。" });

        }

        public static void writeBeginTime(DateTime time)
        {
            Console.WriteLine(time);

        }

        public static void writeEndTime(DateTime time)
        {
            Console.WriteLine(time);
        }

        public delegate void showMessagebox(String text);

        public void showMessage(string text)
        {
            //   MessageBox.Show(text);
            list.ItemsSource = collection;

        }

        private void onStartTimeSet(object sender, RoutedEventArgs e)
        {
            startDate = ((DatePicker)sender).Text;
        }

        private void onEndTimeSet(object sender, RoutedEventArgs e)
        {
            endDate = ((DatePicker)sender).Text;
        }

        private void onPreMonth(object sender, RoutedEventArgs e)
        {
            CurrentMonth -= 1;
        }

        private void onCurrentMonth(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            currentYear = dt.Year;
            CurrentMonth = dt.Month;
        }

        private void onNextMonth(object sender, RoutedEventArgs e)
        {
            CurrentMonth += 1;
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            CurrentMonth = dt.Month;
        }

        private void onBrowsePath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "txt文件|*.txt|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";

            if (openFileDialog.ShowDialog() == true)
            {
                // 把 if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)中的语句放在这里就好了
                Path = openFileDialog.FileName;
            }


        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

         private void onSaveClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;

                FileStream fs = null;
                if (!File.Exists(filename))
                {
                    fs = File.Create(filename);
                }
                else
                {
                    try
                    {
                        fs = new FileStream(filename, FileMode.Truncate);
                    }
                    catch (Exception ex)
                    {
                        fs.Close();
                        MessageBox.Show("程序出现异常！\n" + ex.Message);
                    }

                }

                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.WriteLine(LogItem.getHeadString());
                if (collection != null && collection.Count > 0)
                {
                    foreach (LogItem item in collection)
                    {
                        sw.WriteLine(item);
                    }
                }

                sw.Close();
                fs.Close();
            }
        }
    }
}
