using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.IO;

using System.Diagnostics;



namespace nodemon
{

    class Program
    {

        static DateTime lastRead = DateTime.MinValue;

        static string monitoring = @"";

        static string startup = @"server.js";

        static int processId;



        static void Main(string[] args)
        {

            var watcher = new FileSystemWatcher();
            watcher.IncludeSubdirectories = true;
            if (args.Length > 0)
                if (args[0].StartsWith("--path"))
                    monitoring = args[0].Replace("--path=","");
            if (args[1].StartsWith("--startup"))
                startup = args[1].Replace("--startup=", "");
            
            watcher.Path = monitoring;

            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

            watcher.Filter = "*.*";
            
            watcher.Changed += Changed;

            watcher.Created += Changed;

            watcher.Deleted += Changed;

            watcher.Renamed += Renamed;



            // Begin watching

            watcher.EnableRaisingEvents = true;



            Start();



            Console.WriteLine("Press Enter to exit");
            Console.WriteLine("Press R Enter to restart");
            string input;
            while((input = Console.ReadLine()).Length > 0){
                if (input == "r")
                    Start();
                else
                    break;
            }



            Kill();

        }



        static void Kill()
        {

            var matches = Process.GetProcessesByName("node");



            matches.ToList().ForEach(match =>
            {

                Console.WriteLine("attempting to close node.js [" + match.Id + "]");

                match.Kill();

                match.WaitForExit(3000);

                Console.WriteLine("successfully closed");

            });

        }



        static void Start()
        {

            Kill();
            Console.WriteLine("starting node.js " + DateTime.Now.ToString());

            var start = new ProcessStartInfo();

            start.FileName = @"C:\Program Files\nodejs\node.exe";

            start.UseShellExecute = false;

            start.CreateNoWindow = true;

            start.RedirectStandardOutput = true;
            start.RedirectStandardError = true;
            start.RedirectStandardInput = true;

            start.Arguments = Path.Combine( monitoring, startup);



            var node = new Process();

            node.EnableRaisingEvents = true;
            node.ErrorDataReceived += ErrorHandler;
            node.OutputDataReceived += OutputHandler;

            node.StartInfo = start;

            node.Start();

            node.Refresh();

            Console.WriteLine(node.ProcessName);

            Console.WriteLine("[" + node.Id + "] node.exe started");

            processId = node.Id;



            // close the input, we won't use it

            var input = node.StandardInput;

            input.Close();


            node.BeginErrorReadLine();
            node.BeginOutputReadLine();

        }



        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine(outLine.Data);
            }
        }
        static void ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                Console.WriteLine(outLine.Data);
            }
        }


        static void Renamed(object sender, System.IO.RenamedEventArgs e)
        {

            Start();

        }



        static void Changed(object sender, System.IO.FileSystemEventArgs e)
        {

            // sometimes a text editor makes multiple operations in a single save

            // we only handle the first

            var lastWriteTime = File.GetLastWriteTime(e.FullPath);

            if (lastWriteTime != lastRead)
            {

                Start();

                lastRead = lastWriteTime;

            }

        }

    }

}