using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }
        public RequestMethod Method
        {
            get { return method; }
        }
        
        HTTPVersion httpVersion;
        string requestString;
        public string contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //Validation and Spliting ;

            // check that there is atleast 3 lines: Request line,
            // Host Header,
            // Blank line (usually 4 lines with the last empty line for empty content)
            // Check if the second line is Host:

            // Validate blank line exists

            if (!ValidateBlankLine())
                return false;
            int flag = 0;
            foreach(string line in requestLines)
            {
                if(line.ToUpper().Contains("HOST:"))
                {
                    flag = 1;
                    break;
                }

            }
            if (flag == 0)
                return false; // This meaning HOST header doesn't included 
            //reurn true if The Request is GOOD
            if (ParseRequestLine() && LoadHeaderLines())
            {
                return true;
                //Good Request
            }
            else
            {
                return false;
                //Bad Request
            }

            // Parse Request line


            // Load header lines into HeaderLines dictionary
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            try
            {
                //requestlines[0] refer to requestline for example "GET / HTTP/1.1"
                // request_line contains method and path and version in one line 
                string[] request_line = requestLines[0].Split(' ');
                string Method = request_line[0];
                string path = request_line[1];
                string Version = request_line[2];
                //CHECK IF METHOD IS GET OR POST OR HEAD
                if ((Method == "GET") || (Method == "POST") || (Method == "HEAD"))
                {
                    switch (Method)
                    {
                        case "GET":
                            method = RequestMethod.GET;
                            break;
                        case "POST":
                            method = RequestMethod.POST;
                            break;
                        case "HEAD":
                            method = RequestMethod.HEAD;
                            break;
                    }
                }
                else { return false; }
                /////////////////////////////////////
                //CHECK IS URI IS VALID : START WITH "/"
                if (!ValidateIsURI(path))
                { return false; }
                /////////////////////////////////////
                if (Version == "HTTP/1.0" || Version == "HTTP/1.1" || Version == "")
                {
                    switch (Version)
                    {
                        case "HTTP/1.0":
                            httpVersion = HTTPVersion.HTTP10;
                            break;
                        case "HTTP/1.1":
                            httpVersion = HTTPVersion.HTTP11;
                            break;
                        case "":
                            httpVersion = HTTPVersion.HTTP09;
                            break;
                    }
                }
                else { return false; }

                return true; // return true if all is valid 
            }
            catch // if any exeception thrown , so it will be bad rquest too 
            {
                return false;
            }

        }

        private bool ValidateIsURI(string uri)
        {
            //Check the first of uri
            if (uri.First() == '/')
            {
                relativeURI = uri;
                return true;
            }
            return false;
            //  return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            try
            {
                for (int i = 1; i < requestLines.Length; i++)
                {
                    if (requestLines[i] == "")
                        continue;
                    string[] Key_Value = requestLines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                    headerLines.Add(Key_Value[0], Key_Value[1]);
                }
                //foreach(KeyValuePair<string , string> var in headerLines)
                //{
                //    Console.WriteLine("key : {0} , value : {1}", var.Key, var.Value);
                //}
                return true;
            }
            catch
            {
                return false;
            }

        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            try
            {
                //Check for Blank Line
                string[] Request_and_Content = requestString.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
                //TODO: parse the receivedRequest using the \r\n delimeter   
                //Request_and_Content[0] -->
                requestLines = Request_and_Content[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
                contentLines = Request_and_Content[1];
                //for(int i = 0; i < contentLines.Length; i++)
                //{
                //    Console.WriteLine(contentLines[i]);
                //}
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
