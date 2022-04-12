using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;

namespace Maglumi2000N
{
    public static  class FileWatcher
    {
        private static FileSystemWatcher watcher = new FileSystemWatcher();
        private static AppDbContext _db = new AppDbContext();

        public static void Startwatching()
        {
            FileWatcher.watcher.Path = Constants.DumpPath;
            FileWatcher.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.LastAccess;
            FileWatcher.watcher.Filter = "*.txt";
            FileWatcher.watcher.Created += new FileSystemEventHandler(FileWatcher.OnChanged);
            FileWatcher.watcher.Deleted += new FileSystemEventHandler(FileWatcher.OnChanged);
            FileWatcher.watcher.EnableRaisingEvents = true;
        }
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                string PId = "";
                string InstrumentName = "";
                string code = "";
                Thread.Sleep(2000);
                string content = File.ReadAllText(e.FullPath);
                //Console.Write(content);
                if (!string.IsNullOrEmpty(content))
                {
                    Parse(content);
                }
                else
                {
                    Console.WriteLine("There Is NO Data!");
                }
                //string[] lines = content.Split(new[] { Environment.NewLine},StringSplitOptions.None);
                //Console.WriteLine("splitted lenght first: " + lines.Length);
               // Console.WriteLine("splitted lenght second: " + lines[0].Trim().Length);
               // Console.WriteLine("splitted lenght substring: " + lines[0].Substring(0, 151));
                //foreach (var item in lines[0])
                //{
                //    Console.Write(item);
                //}
                //Console.WriteLine();
              // Console.WriteLine("splitted lenght third: " + lines[1].Trim().Length);
                //InstrumentName = lines[0].Split('|')[4];
               // Console.WriteLine(InstrumentName);
               // foreach (var item in lines)
               // {
                   // Console.WriteLine(item);
                    //if (item.Substring(0, 1) == "R")
                    //{
                    //    code = item.Split('|')[2];
                    //    Console.WriteLine(code);
                    //}
                //}



            }
            else
            {
                if (e.ChangeType != WatcherChangeTypes.Deleted)
                    return;
                Console.WriteLine("File Deleted: " + e.FullPath);
            }
        }
        public static void Parse(string data)
        {
            string PId = "";
            string MachineName = "";
            string code = "";
            string Pid = "";
            string value = "";
            string codde = "";
            string reportdate = "";
            string unit = "";
            string range = "";
            string temp_result = "";
            string final_result = "";
            int Esr_value = 0;
            DateTime? Date = new DateTime?();
            try
            {
                //string[] lines = data.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string[] lines = data.Split(' ');
                //foreach(var item in lines)
                //{
                //    Console.WriteLine(item);
                //}
                Pid = lines[0].Remove(0, 4);
                Pid = Pid.TrimStart(new char[] { '0' });
                Console.WriteLine("PatientId : " + Pid);
                temp_result = lines[lines.Length - 1];
                if (temp_result.Length == 12)
                {
                    final_result = temp_result.Substring(7, 3);
                    int.TryParse(final_result, out Esr_value);
                    Console.WriteLine("Esr :" + Esr_value);
                }
                else
                {
                    Console.WriteLine("Try Again for PatientId :" + Pid);
                }

                if (Esr_value > 0)
                {
                    PatientRecord pr = new PatientRecord()
                    {
                        PatientId = Pid,
                        InstrumentName = "ESRRoller20",
                        ReportDate = Date

                    };
                    _db.PatientRecords.Add(pr);
                    _db.SaveChanges();
                    ResultRecord rr = new ResultRecord()
                    {
                        PatientRecordId = pr.PatientRecordId,
                        Code = "ESR",
                        Value = Esr_value.ToString(),
                        Name = "ESR",
                        Unit = unit,
                        Range = range,
                        ReportDate = Date

                    };
                    _db.Resultrecords.Add(rr);
                    _db.SaveChanges();
                   // Task.Factory.StartNew((Action)(() => FileWatcher.Execute_DataBase(Pid)));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Task.Factory.StartNew((Action)(() => FileWatcher.Execute_DataBase(Pid)));
        }

        private static void Execute_DataBase(string Pid)
        {
            Console.WriteLine("Executing Sql for PatientId: " + Pid);
            using (AppDbContext _dbContext = new AppDbContext())
            {
                var param1 = new SqlParameter("@pid1", Pid);
                var param2 = new SqlParameter("@pid2", Pid);
                _dbContext.Database.ExecuteSqlCommand(@"Exec [dbo].[spUpdateReportDefId] @pid1 ", parameters: new[] { param1 });
                Thread.Sleep(100);
                _dbContext.Database.ExecuteSqlCommand(@"Exec [dbo].[spUpdateMAHDIAGTEMPLIS]  @pid2 ", parameters: new[] { param2 });
            }
        }

        public static DateTime? ConvertMaglumi2000DateTime( string datetime)
        {


            //20 08 01 12 20 16
            var year = int.Parse("20" + datetime.Substring(0, 2));
            var month = int.Parse(datetime.Substring(2, 2));
            var day = int.Parse(datetime.Substring(4, 2));
            var hour = int.Parse(datetime.Substring(6, 2));
            var min = int.Parse(datetime.Substring(8, 2));
            var sec = int.Parse(datetime.Substring(10, 2));

            return new DateTime(year, month, day, hour, min, sec);
        }
        //public DateTime rp_date(string date)
        //{
        //    return 
        //}
    }
}
