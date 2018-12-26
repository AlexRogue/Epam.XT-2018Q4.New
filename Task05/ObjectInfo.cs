using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class ObjectInfo
    {
        public DateTime ChangeTime { get; private set; }
        public string PathBeforeChange { get; private set;}
        public string Action { get; private set;}
        public string BackUpPath { get; private set; }
        public string OldPath { get; private set; }


        public ObjectInfo(string changeTime, string pathBeforeChange, string action, string backUpPath = null, string oldPath = null)
        {
            ChangeTime = DateConvert(changeTime);
            PathBeforeChange = pathBeforeChange;
            Action = action;
            BackUpPath = backUpPath;
            OldPath = oldPath;
        }

        public DateTime DateConvert(string changeTime)
        {
            return DateTime.ParseExact(changeTime, "yyyy.MM.dd HH-mm-ss", CultureInfo.InvariantCulture); 
        }
            
    }
}
