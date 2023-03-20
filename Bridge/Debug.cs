using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IceEngine.Internal;

using static Ice.Island;

namespace IceEngine
{
    internal class Debug : System.ComponentModel.INotifyPropertyChanged
    {
        #region Static Interface
        public static void SaveLog() => Instance._SaveLog();
        public static void Log(object message, object context = null) => Instance._Log(message, context);
        public static void LogWarning(object message, object context = null) => Instance._LogWarning(message, context);
        public static void LogError(object message, object context = null) => Instance._LogError(message, context);
        #endregion
        public static Debug Instance => _instance ??= new Debug(); static Debug _instance;

        public event PropertyChangedEventHandler PropertyChanged;
        public string DebugStr
        {
            get => _debugStr; set
            {
                _debugStr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DebugStr"));
            }
        }
        string _debugStr;


        DateTimeOffset? tStart = null;
        DateTimeOffset? tLast = null;
        int lineCount = 0;

        void DoLog(string message)
        {
            void Line(string message)
            {
                Console.WriteLine(message);
                DebugStr += $"{message}\n";
                lineCount++;
            }

            var now = DateTimeOffset.Now;
            if (tLast is null || (now - tLast.Value).TotalMinutes > Setting.timeMarkMinutes)
            {
                tLast = now;
                Line(now.ToString("【---- yyyy/MM/dd HH:mm:ss ----】"));
            }

            Line(message);

            if (tStart is null) tStart = now;
            else
            {
                if (lineCount > Setting.logMaxLineCount || (now - tStart.Value).TotalHours > Setting.logMaxHours)
                {
                    _SaveLog();
                }
            }
        }
        void _SaveLog()
        {
            if (tStart is null || lineCount == 0) return;

            var now = DateTimeOffset.Now;
            string title = tStart.Value.ToString("yyyyMMdd-HHmmss") + " to " + now.ToString("yyyyMMdd-HHmmss");
            string path = $"{Ice.Save.DataPath}\\Log\\{title}.log";
            path.TryCreateFolderOfPath();
            File.WriteAllText(path, DebugStr, Encoding.UTF8);

            DebugStr = "";
            tStart = now;
            lineCount = 0;
        }
        void _Log(object message, object context = null)
        {
            DoLog(message.ToString());
        }
        void _LogWarning(object message, object context = null)
        {
            message = "[Warning] " + message;
            DoLog(message.ToString());
        }
        void _LogError(object message, object context = null)
        {
            message = "【ERROR】 " + message;
            DoLog(message.ToString());
        }
    }
}
