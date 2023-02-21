using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using IceEngine.Internal;
using IceHomeServerWPF;

namespace IceEngine
{
    internal class Debug
    {
        static SettingGlobal Setting => SettingGlobal.Setting;
        static TextBlock DebugLabel => MainWindow.Instance?.DebugLabel;

        static DateTimeOffset? tStart = null;
        static DateTimeOffset? tLast = null;
        public static int lineCount = 0;

        static readonly Queue<string> msgQueue = new Queue<string>();
        static void DoLog(string message)
        {
            if (DebugLabel is null)
            {
                msgQueue.Enqueue(message);
                return;
            }

            static void Line(string message)
            {
                DebugLabel.Text += $"{message}\n";
                lineCount++;
            }

            var now = DateTimeOffset.Now;
            if (tLast is null || (now - tLast.Value).TotalMinutes > Setting.timeMarkMinutes)
            {
                tLast = now;
                Line(now.ToString("【---- yyyy/MM/dd HH:mm:ss ----】"));
            }

            while (msgQueue.Count > 0) Line(msgQueue.Dequeue());
            Line(message);

            if (tStart is null) tStart = now;
            else
            {
                if (lineCount > Setting.logMaxLineCount || (now - tStart.Value).TotalHours > Setting.logMaxHours)
                {
                    SaveLog();
                }
            }
        }
        public static void SaveLog()
        {
            if (tStart is null || lineCount == 0) return;

            var now = DateTimeOffset.Now;
            string title = tStart.Value.ToString("yyyyMMdd-HHmmss") + " to " + now.ToString("yyyyMMdd-HHmmss");
            string path = $"{Ice.Save.DataPath}\\Log\\{title}.log";
            path.TryCreateFolderOfPath();
            File.WriteAllText(path, (string)DebugLabel.Text, Encoding.UTF8);

            DebugLabel.Text = "";
            tStart = now;
            lineCount = 0;
        }
        public static void Log(object message, object context = null)
        {
            DoLog(message.ToString());
        }
        public static void LogWarning(object message, object context = null)
        {
            message = "[Warning] " + message;
            DoLog(message.ToString());
        }
        public static void LogError(object message, object context = null)
        {
            message = "【ERROR】 " + message;
            DoLog(message.ToString());
        }
    }
}
