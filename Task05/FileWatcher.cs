using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace WindowsFormsApp1
{
    public partial class FileWatcher : Form
    {
        FileSystemWatcher fileSystemWatcher;
        LogCreator logCreator = new LogCreator();
        private int fireCount = 0;
        private string logDir = "C:\\Backuper\\log.txt";
        private string backupDir = "C:\\Backuper\\";
        private string recoverdate;
        DateTime recoveryTime;

        public FileWatcher()
        {
            InitializeComponent();
            fileSystemWatcher = new FileSystemWatcher();

            logCreator.CreateLog(logDir, backupDir);
            
            fileSystemWatcher.Filter = "*.txt";

            if (radioButton1.Checked == true)
            {
                fileSystemWatcher.IncludeSubdirectories = true;
            }
            else
            {
                fileSystemWatcher.IncludeSubdirectories = false;
            }
            fileSystemWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size 
                | NotifyFilters.Attributes | NotifyFilters.DirectoryName | NotifyFilters.LastAccess | NotifyFilters.CreationTime;
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);         
            fileSystemWatcher.Created += new FileSystemEventHandler(OnCreated);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);            
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            object obj = new object();
            lock (obj)
            {
                fireCount++;
                string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss");
                string textlog = $"{Environment.NewLine}{currentTime}{Environment.NewLine}{e.FullPath}{Environment.NewLine}{e.ChangeType}";
                BeginInvoke(new Action(() =>
                {
                    richTextBox1.AppendText(textlog);
                    logCreator.WriteLog(textlog, logDir, backupDir);
                    BackupFile(e.FullPath, $"{backupDir}{currentTime}\\", e.Name);
                }
              ));
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            object obj = new object();
            lock (obj)
            {
                string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss");
                string textlog = $"{Environment.NewLine}{currentTime}{Environment.NewLine}{e.FullPath}{Environment.NewLine}{e.ChangeType}";
                fireCount++;
                if (fireCount == 1)
                {
                    BeginInvoke(new Action(() =>
                    {
                        richTextBox1.AppendText(textlog);
                        logCreator.WriteLog(textlog, logDir, backupDir);
                        BackupFile(e.FullPath, $"{backupDir}{currentTime}\\", e.Name);
                    }
                ));
                }
                else
                {
                    fireCount = 0;
                }
            }
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            object obj = new object();
            lock (obj)
            {
                string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss");
                string textlog = $"{Environment.NewLine}{currentTime}{Environment.NewLine}{e.FullPath}{Environment.NewLine}{e.ChangeType}";
                fireCount++;
                BeginInvoke(new Action(() =>
                richTextBox1.AppendText(textlog + "\n")
                ));
                logCreator.WriteLog(textlog, logDir, backupDir);
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            object obj = new object();
            lock (obj)
            {
                string currentTime = DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss");
                string textlog = $"{Environment.NewLine}{currentTime}{Environment.NewLine}{e.FullPath}{Environment.NewLine}{e.ChangeType}{Environment.NewLine}{e.OldFullPath}";
                BeginInvoke(new Action(() =>
                {
                    richTextBox1.AppendText(textlog);
                    logCreator.WriteLog(textlog, logDir, backupDir);
                    BackupFile(e.FullPath, $"{backupDir}{currentTime}\\", e.Name);
                }));
            }
        }

        private void BackupFile (string sourcefn, string destinfn, string backupingfile)
        {
            object obj = new object();
            lock (obj)
            {
                var fileName = backupingfile.Substring(backupingfile.LastIndexOf('\\') + 1);
                var additionOfFileName = backupingfile.Replace(fileName, string.Empty);
                string pathtofile = $"{destinfn}{additionOfFileName}";
                if (!Directory.Exists(pathtofile))
                {
                    Directory.CreateDirectory(pathtofile);
                }
                File.Copy(sourcefn, $"{pathtofile + fileName}", true);
                richTextBox1.AppendText($"\n{pathtofile}{fileName}");
                logCreator.WriteLog($"{pathtofile}{fileName}\n", logDir, backupDir);
                richTextBox1.AppendText($"\n");
            }
        }

        private void RecoverFiles()
        {
            object obj = new object();
            lock (obj)
            {
                DirectoryInfo directory = new DirectoryInfo(fileSystemWatcher.Path);
                foreach (var file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo subDirectory in directory.GetDirectories())
                {
                    subDirectory.Delete(true);
                }

                List<ObjectInfo> objectInfos = new List<ObjectInfo>();
                List<string> listlog = new List<string>();
                StringBuilder strLog = new StringBuilder();
                DateTime remainnigdatetime = DateTime.Now;
                using (StreamReader readlog = new StreamReader(logDir))
                {
                    while (!readlog.EndOfStream)
                    {
                        var logLine = readlog.ReadLine();

                        if (logLine != "")
                        {
                            listlog.Add(logLine);
                        }
                    }
                }


                for (int skip = 0; skip < listlog.Count; skip += 5)
                {
                    var logData = listlog.Skip(skip).Take(5).ToList();
                    if (logData.Any(x => x.Equals("Deleted")))
                    {
                        var deletedElement = logData.Take(3).ToList();
                        objectInfos.Add(new ObjectInfo(deletedElement[0], deletedElement[1], deletedElement[2]));
                        skip -= 2;
                    }
                    if (logData.Any(x => x.Equals("Renamed")))
                    {
                        objectInfos.Add(new ObjectInfo(logData[0], logData[1], logData[2], logData[4], logData[3]));
                    }
                    if(logData.Any(x => x.Equals("Changed") || x.Equals("Created")))
                    {
                        var changedElement = logData.Take(4).ToList();
                        objectInfos.Add(new ObjectInfo(changedElement[0], changedElement[1], changedElement[2], changedElement[3]));
                        skip--;
                    }
                }


                for (int i = 0; i < objectInfos.Count; i++)
                {
                    SwitchAction(objectInfos[i]);
                }
            }
        }


        private void SwitchAction(ObjectInfo objectInfo)
        {
            Dictionary<string, Action> d = new Dictionary<string, Action>();
            d.Add("Created", () => {
                var filePath = objectInfo.PathBeforeChange.Substring(0, objectInfo.PathBeforeChange.LastIndexOf("\\"));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                File.Copy(objectInfo.BackUpPath, objectInfo.PathBeforeChange, true); });
            d.Add("Changed", () => { File.Copy(objectInfo.BackUpPath, objectInfo.PathBeforeChange, true); });
            d.Add("Deleted", () => { File.Delete(objectInfo.PathBeforeChange); });
            d.Add("Renamed", () => { File.Copy(objectInfo.BackUpPath, objectInfo.PathBeforeChange); });
            d[objectInfo.Action].Invoke();
        }




        private void startToWatch_Click(object sender, EventArgs e)
        {
            try
            {
                string path = textBox1.Text;
                fileSystemWatcher.Path = path;
                fileSystemWatcher.EnableRaisingEvents = true;
                richTextBox1.AppendText($"Path set {path}\n");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Wrong path", "Error!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        private void buttonRetrive_Click(object sender, EventArgs e)
        {
           string folderIdToRetrive =  dateTimePicker1.Text;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            recoveryTime = dateTimePicker1.Value;
            RecoverFiles();
        }

        

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                RecoverButton.Visible = false;
                dateTimePicker1.Visible = false;
                BackupButton.Visible = true;
                fileSystemWatcher.IncludeSubdirectories = true;
                richTextBox1.AppendText("Pasring continued\n");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                RecoverButton.Visible = true;
                dateTimePicker1.Visible = true;
                BackupButton.Visible = false;
                fileSystemWatcher.IncludeSubdirectories = false;
                richTextBox1.AppendText("Pasring paused\n");
            }                
        }

        
        private void ButtonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
