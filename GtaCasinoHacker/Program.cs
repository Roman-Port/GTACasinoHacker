using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GtaCasinoHacker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Load fingers
            registeredFingers = LoadRegisteredFingers();

            //Get process
            Process process = FindGtaProcess();

            //Start
            if(process == null)
                Application.Run(new ProcessFinder());
            else
                Application.Run(new Form1(process));
        }

        public static List<GtaFingerPackFile> registeredFingers;

        public static List<GtaFingerPackFile> LoadRegisteredFingers()
        {
            //Find files here
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());
            var registeredFingers = new List<GtaFingerPackFile>();
            foreach (var f in files)
            {
                //Check if it has the right extension
                if (!f.EndsWith(".gtafpak"))
                    continue;

                //Open and read
                GtaFingerPackFile file = new GtaFingerPackFile();
                using (FileStream fs = new FileStream(f, FileMode.Open))
                    file.LoadFile(fs);

                //Add
                registeredFingers.Add(file);
            }
            return registeredFingers;
        }

        public static Process FindGtaProcess()
        {
            var processes = Process.GetProcessesByName("GTA5");
            if (processes.Length == 0)
                return null;
            return processes[0];
        }
    }
}
