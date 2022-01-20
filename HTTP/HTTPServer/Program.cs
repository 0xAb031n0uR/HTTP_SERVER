using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static int port;
        public static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            //Start server
            // 1) Make server object on port 1000
            CreateRedirectionRulesFile();
            // 2) Start Server
                port = 1000;
                new Server(port, "redirectionRules.txt");
                Console.WriteLine("please Restart");
           
        }

        static void CreateRedirectionRulesFile()
        {
            // TODO: Create file named redirectionRules.txt
            if (!File.Exists("redirectionRules.txt"))
            {
                using (StreamWriter WriteOnFile = File.CreateText("redirectionRules.txt"))
                {
                    // Add some text to file    
                    // each line in the file specify a redirection rule
                    // example: "aboutus.html,aboutus2.html"
                    // means that when making request to aboustus.html,, it redirects me to aboutus2
                    WriteOnFile.WriteLine("aboutus.html,aboutus2.html");
                   
                }
            }
            
        }
    }
}
