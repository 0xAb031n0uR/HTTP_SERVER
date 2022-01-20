using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        //Server Members
        Socket serverSocket;
        private int port;
        IPAddress iPAddress;
        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            //TODO: initialize this.serverSocket       
            this.LoadRedirectionRules(redirectionMatrixPath);
            try
            {
                this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.port = portNumber;
                //iPAddress = IPAddress.Parse("192.168.43.169");
                iPAddress = IPAddress.Any;
                IPEndPoint iep = new IPEndPoint(iPAddress, port);
                serverSocket.Bind(iep);
                StartServer();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            Console.WriteLine("Started on {0}:{1}", iPAddress, port);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                Socket ClientSocket = serverSocket.Accept();
                // Console.WriteLine("New Clinet {0} ", ClientSocket.RemoteEndPoint);
                //TODO: accept connections and start thread for each accepted connection.
                Thread NewClinetThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                 NewClinetThread.Start(ClientSocket);
            }
        }
        public void HandleConnection(object obj)
        {
            Socket ClientSocket = (Socket)obj;
            // TODO: Create client socket 
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            ClientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int recieved_Len;
            while (true)
            {
                try
                {
                    byte [] REQUEST = new byte[1024]; // Byte Array to store Recieved Request
                    // TODO: Receive request
                    recieved_Len = ClientSocket.Receive(REQUEST);
                    // TODO: break the while loop if receivedLen==0
                    if (recieved_Len == 0) {
                        Console.WriteLine("Client : {0} Ended The connection", ClientSocket.RemoteEndPoint); // Client End The Connection
                        Console.WriteLine("----------------------------------");
                        
                        break;
                    }
                    string x = Encoding.ASCII.GetString(REQUEST);
                    if (string.IsNullOrEmpty(x) )
                    {
                        string ReadFile = LoadDefaultPage(Configuration.InternalErrorDefaultPageName, ClientSocket);
                        Response resp = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", ReadFile, string.Empty);
                        ClientSocket.Send(Encoding.ASCII.GetBytes(resp.ResponseString));
                    }
                    Console.WriteLine("REQUEST :");
                    Console.WriteLine(Encoding.ASCII.GetString(REQUEST));  //Print all Request that server Recieved
                    
                    Console.WriteLine("----------------------------------");
                    // TODO: Create a Request object using received request string
                   


                    Request All_request = new Request(x);

                    Response response = HandleRequest(All_request, ClientSocket);// TODO: Call HandleRequest Method that returns the response

                    Console.WriteLine("RESPONSE :\n");
                    Console.WriteLine(response.ResponseString); // Print Response String 
                    Console.WriteLine("----------------------------------");
                    byte[] response_byte_arr = Encoding.ASCII.GetBytes(response.ResponseString); // GET BYTES OF Response String 

                    ClientSocket.Send(response_byte_arr);
                    // TODO: Send Response back to client

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(new Exception(ClientSocket.RemoteEndPoint.ToString() + " | " + ex.Message.ToString()));
                    string ReadFile = LoadDefaultPage(Configuration.InternalErrorDefaultPageName, ClientSocket);
                    Response response = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", ReadFile, string.Empty);
                    ClientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                    break;
                }
            }
            ClientSocket.Close();
            // TODO: close client socket
        }

        Response HandleRequest(Request request , Socket client)
        {
            //TODO: check for bad request
            //TODO: map the relativeURI in request to get the physical path of the resource.
            //TODO: check for redirect//TODO: check file exists 
            //TODO: read the physical file 
            // Create OK response
            try
            {
                //Bad Or Not
                if (request.ParseRequest())
                {
                    //Console.WriteLine(request.relativeURI);
                    //Get Redirection page if exist 
                    if (request.Method == RequestMethod.GET)
                    {
                        string Redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                        if (Redirect != string.Empty)
                        {
                            
                            string Read_file = LoadDefaultPage(Configuration.RedirectionDefaultPageName , client);
                            //Console.WriteLine(Read_file);
                            if(Read_file == String.Empty)
                            {
                                goto H;
                            }
                            // Create Redirect response
                            Response response = new Response(StatusCode.Redirect, "text/html; charset=UTF-8", Read_file, Redirect);
                            return response;

                        }
                        // check if file can't be read so throw execption to be internal server error
                        H: 
                        if (LoadDefaultPage(request.relativeURI, client) == string.Empty) // path Not found so return NoutFound.html
                        {

                            string Read_file = LoadDefaultPage(Configuration.NotFoundDefaultPageName , client);
                            // Create NotFound response
                            if(Read_file == string.Empty)
                            {
                                return new Response(StatusCode.NotFound, "text/html; charset=UTF-8", "<b>NOT FOUND PAGE</b>", string.Empty);
                            }
                            Response response = new Response(StatusCode.NotFound, "text/html; charset=UTF-8", Read_file, string.Empty);
                            return response;
                        }
                        //if program run here so all is fine and return page ; 
                        else
                        {
                            string Read_file = LoadDefaultPage(request.relativeURI , client);
                            // Create OK response
                            Response response = new Response(StatusCode.OK, "text/html; charset=UTF-8", Read_file, string.Empty);
                            return response;
                        }
                    }
                    else if(request.Method == RequestMethod.HEAD)
                    {
                        string Redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                        if (Redirect != string.Empty)
                        {
                            string Read_file = LoadDefaultPage(Configuration.RedirectionDefaultPageName , client);
                            //Console.WriteLine(Read_file);
                            // Create Redirect response
                            Response response = new Response(StatusCode.Redirect, "text/html; charset=UTF-8", Read_file.Length, Redirect);
                            return response;

                        }
                        // check if file can't be read so throw execption to be internal server error
                        
                        if (LoadDefaultPage(request.relativeURI , client) == string.Empty) // path Not found so return NoutFound.html
                        {

                            string Read_file = LoadDefaultPage(Configuration.NotFoundDefaultPageName , client);
                            // Create NotFound response
                            Response response = new Response(StatusCode.NotFound, "text/html; charset=UTF-8", Read_file.Length, string.Empty);
                            return response;
                        }
                        //if program run here so all is fine and return page ; 
                        else
                        {
                            string Read_file = LoadDefaultPage(request.relativeURI , client);
                            // Create OK response
                            Response response = new Response(StatusCode.OK, "text/html; charset=UTF-8", Read_file.Length, string.Empty);
                            return response;
                            
                        }
                    }
                    else //if (request.Method == RequestMethod.POST)
                    {

                        string Redirect = GetRedirectionPagePathIFExist(request.relativeURI);
                        if (Redirect != string.Empty)
                            return new Response(StatusCode.Redirect, "text/html; charset=UTF-8", "<b>Method Not Accepted</b>", Redirect);

                        if (LoadDefaultPage(request.relativeURI, client) == string.Empty)
                        {
                            string Read_file = LoadDefaultPage(Configuration.NotFoundDefaultPageName, client);
                            // Create NotFound response
                            Response response = new Response(StatusCode.NotFound, "text/html; charset=UTF-8", Read_file, string.Empty);
                            return response;
                        }
                        
                        else
                        {
                            
                            foreach (string page in Configuration.POST_PAGES)
                            {
                                if (request.relativeURI == "/" + page)
                                {
                                    string path = "DATA.txt";
                                    if (!File.Exists(path))
                                    {
                                        // Create a file to write to.
                                        using (StreamWriter sw = File.CreateText(path))
                                        {
                                            sw.WriteLine("client :" + client.RemoteEndPoint);
                                           
                                            sw.WriteLine(request.contentLines);
                                            sw.WriteLine("-----------------------");
                                            sw.Close();
                                        }
                                        
                                    }
                                    else
                                    {
                                        using (StreamWriter sw = File.AppendText(path))
                                        {
                                            sw.WriteLine("client :" + client.RemoteEndPoint);
                                            sw.WriteLine(request.contentLines);
                                            sw.WriteLine("-----------------------");
                                            sw.Close();

                                        }
                                    }
                                    return new Response(StatusCode.OK, "text/html; charset=UTF-8", "<b>Data Recieved</b>", string.Empty);
                                    

                                }
                            }
 
                            return new Response(StatusCode.Redirect, "text/html; charset=UTF-8", "<b>Method Not Accepted</b>", request.relativeURI.Remove(0,1));

                        }


                        //Response response = new Response(StatusCode.OK, "text/html; charset=UTF-8", "", string.Empty);
                        //return response;
                    }


                }
                else //BAD REQUEST so return BadRequest.html
                {
                    string Read_file = LoadDefaultPage(Configuration.BadRequestDefaultPageName , client);
                    if(Read_file == string.Empty)
                        return new Response(StatusCode.BadRequest, "text/html; charset=UTF-8", "<b>BAD REQUEST</b>", string.Empty);
                    Response response = new Response(StatusCode.BadRequest, "text/html; charset=UTF-8", Read_file, string.Empty);
                    return response;
                }

             }
            catch (Exception ex)
            {
                Logger.LogException(new Exception(client.RemoteEndPoint.ToString() + " | " + ex.Message.ToString())); // TODO: log exception using Logger class
                string ReadFile = LoadDefaultPage(Configuration.InternalErrorDefaultPageName, client);
                if (request.Method == RequestMethod.GET || request.Method == RequestMethod.POST)
                {
                    Response response = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", ReadFile, string.Empty);
                    return response;
                }
                else
                {
                    Response response = new Response(StatusCode.InternalServerError, "text/html; charset=UTF-8", ReadFile.Length, string.Empty);
                    return response;
                }
                // TODO: in case of exception, return Internal Server Error.      
            }
}
        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach (KeyValuePair<string, string> rule in Configuration.RedirectionRules) // iterate on Redirection Rules
            {
                if (relativePath == ("/"+ rule.Key))
                {
                    string path = rule.Value;
                    return path;
                }
            }
             return string.Empty;
            //if (Configuration.RedirectionRules.Keys.ElementAt(i) == relativePath)
            //{
            //    return Configuration.RedirectionRules.Values.ElementAt(i);
            //}
        }
        // 
        
        private string LoadDefaultPage(string defaultPageName , Socket client)
        {
            string filePath; 
            if (defaultPageName.First() == '/')
                filePath = Configuration.RootPath + defaultPageName;
            else  
                filePath = Path.Combine(Configuration.RootPath, defaultPageName); //C:\\inetpub\\wwwroot\\fcis1\\Redirect.html

            // TODO: check if filepath not exist log exception using Logger class and return empty string
            //Console.WriteLine(filePath);
            if (!File.Exists(filePath))
            {
            
                Logger.LogException(new Exception(client.RemoteEndPoint.ToString() + " | " + "Could not find file '" + filePath + "'."));
                return string.Empty;
            }
            else
            {
                string Read_file = File.ReadAllText(filePath);
                return Read_file;
            }
            // else read file and return its content

        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(filePath);
                foreach(string line in lines)
                {       
                    string[] rules = line.Split(',');
                    // then fill Configuration.RedirectionRules dictionary 
                    Configuration.RedirectionRules.Add(rules[0], rules[1]);
                }
                string[] pages = File.ReadAllLines("POST_PAGES.txt");
                int i = 0;
                foreach (string line in pages)
                {
                    Configuration.POST_PAGES[i] = line;
                    i++;
                }
            }

            catch (Exception ex)
            {
                Logger.LogException(ex); // error when read redirectionRules 
            }
        }
    }
}
