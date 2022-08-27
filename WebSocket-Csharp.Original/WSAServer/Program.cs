using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace WSAServer
{
    public class Program
    {
        public static void Main (string[] args)
        {
            if (args.Length == 0)
                WsServer.WsRuntime(false);
            else if (args.Length == 1 && args[0] == "secure" || args[0] == "1")
                WsServer.WsRuntime(true);
            else if (args.Length == 1 && args[0] == "http" || args[0] == "2")
                HttpServerClass.HtRuntime();
        }
    }
}
