using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        //static StreamWriter sw = new StreamWriter("log.txt");
        public static void LogException(Exception ex )
        {
            string path = "log.txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                File.CreateText(path);
                File.AppendAllText(path, "Date time: " + DateTime.Now +
                  "\n" + "Message: " + ex.Message + "\n" + "------------------------------------\n");
            }
            else
            {
                File.AppendAllText(path, "Date time: " + DateTime.Now +
                   "\n" + "Message: " + ex.Message + "\n" + "------------------------------------\n");

            }
        }
    }
}

