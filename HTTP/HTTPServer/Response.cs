using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;

            // TODO: Create the request string

                this.responseString =
                    GetStatusLine(code) +
                    "Date : " + DateTime.Now + "\r\n" +
                    "Server : FCIS_SERVER\r\n" +
                    "Contetnt-Type : " + contentType + "\r\n" +
                    "Content-Length : " + content.Length + "\r\n";
            if (code == StatusCode.Redirect)
                this.responseString = this.responseString +
                    "Location : " + redirectoinPath + "\r\n";    
        
            this.responseString = this.responseString + "\r\n" +
                    content;
         
        }
        public Response(StatusCode code, string contentType, int content_Length, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.code = code;
            // TODO: Create the request string
           this.responseString =
           GetStatusLine(code) +
           "Date : " + DateTime.Now + "\r\n" +
           "Server : FCIS_SERVER\r\n" +
           "Contetnt-Type : " + contentType + "\r\n" +
           "Content-Length : " + content_Length + "\r\n";

            if (code == StatusCode.Redirect)
                this.responseString = this.responseString +
                    "Location : " + redirectoinPath + "\r\n";

            
                    
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            if (code == StatusCode.OK)
                statusLine = "HTTP/1.1 200 OK\r\n";
            else if (code == StatusCode.BadRequest)
                statusLine = "HTTP/1.1 400 Bad Request\r\n";
            else if (code == StatusCode.InternalServerError)
                statusLine = "HTTP/1.1 500 Internal Server Error\r\n";
            else if (code == StatusCode.Redirect)
                statusLine = "HTTP/1.1 301 Moved Permanently\r\n";
            else if (code == StatusCode.NotFound)
                statusLine = "HTTP/1.1 404 Not Found\r\n";
          


            return statusLine;
        }
    }
}
