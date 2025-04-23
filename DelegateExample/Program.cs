using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DelegateExample
{

    class Program
    {
        delegate void LogDel(string text);
        static void Main(string[] args)
        {

            Log log = new Log();

            LogDel LogTextToScreenDel, LogTextToFileDel;

            LogTextToScreenDel = new LogDel(log.LogTextToScreen);
            LogTextToFileDel = new LogDel(log.LogTextToFile);

            LogDel multiLogDel = LogTextToScreenDel + LogTextToFileDel;
            
            System.Console.WriteLine("Please enter your name");

            var name = Console.ReadLine();

            LogText(multiLogDel, name);

            
        }

        static void LogText(LogDel logDel, string text)
        {
            logDel(text);
        }

        
    }

    public class Log
    {

        public void LogTextToScreen(string text)
        {
            System.Console.WriteLine($"{DateTime.Now}: {text}");
        }
        public void LogTextToFile(string text)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "C#", "test", "DelegateExample", "Log.txt"), true))
            {
                sw.WriteLine($"{DateTime.Now}: {text}");
            }
        }
    }


}