using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace WSAServer
{
    internal class WsServer
    {
        private static string _loginId;
        private static string _password;
        private static string _role;
        public static void WsRuntime(bool secure)
        {

            var wssv = new WebSocketServer(4649);
            #region ConnectionOptions
            //var wssv = new WebSocketServer (5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.Any, 5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Any, 5963, true);

            //var wssv = new WebSocketServer ("ws://0.0.0.0:4649");
            //var wssv = new WebSocketServer ("wss://0.0.0.0:5963");

            //var wssv = new WebSocketServer ("ws://[::0]:4649");
            //var wssv = new WebSocketServer ("wss://[::0]:5963");

            //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.Loopback, 5963, true);

            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 4649);
            //var wssv = new WebSocketServer (System.Net.IPAddress.IPv6Loopback, 5963, true);

            //var wssv = new WebSocketServer ("ws://localhost:4649");
            //var wssv = new WebSocketServer ("wss://localhost:5963");

            //var wssv = new WebSocketServer ("ws://127.0.0.1:4649");
            //var wssv = new WebSocketServer ("wss://127.0.0.1:5963");

            //var wssv = new WebSocketServer ("ws://[::1]:4649");
            //var wssv = new WebSocketServer ("wss://[::1]:5963");
            #endregion ConnectionOptions
#if DEBUG
            // To change the logging level.
            wssv.Log.Level = LogLevel.Debug;

            // To change the wait time for the response to the WebSocket Ping or Close.
            //wssv.WaitTime = TimeSpan.FromSeconds (2);

            // Not to remove the inactive sessions periodically.
            wssv.KeepClean = false;
#endif
            // To provide the secure connection.
            /*
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            wssv.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */

            // To provide the HTTP Authentication (Basic/Digest).
            wssv.AuthenticationSchemes = AuthenticationSchemes.Basic;
            wssv.Realm = "WSServer Ingress";
            NetworkCredential _loginDetails = null;
            try
            {
                wssv.UserCredentialsFinder = id =>
                {
                    string[] CredList = System.IO.File.ReadAllLines("Creds.txt");
                    foreach (string Cred in CredList)
                    {
                        string[] _creds = Cred.Split(':');
                        _loginId = _creds[0];
                        _password = _creds[1];
                        _role = _creds[2];
                        if (id.Name == _loginId)
                        { 
                            _loginDetails = new NetworkCredential(_loginId, _password, _role);
                            break;
                        }
                    }
                    return _loginDetails;
                    #region Old_Login_Schema
                    /*
                    NetworkCredential _netCred = null;
                    if (id.Name == "Client#")
                    {
                        string name = id.Name;
                        // Return user name, password, and roles.
                        _netCred = new NetworkCredential(name, "password", "gunfighter");//  : null; // If the user credentials are not found.
                    }
                    else if (id.Name == "Admin#")
                    {
                        string name = id.Name;
                        // Return user name, password, and roles.
                        _netCred = new NetworkCredential(name, "Password1", "gunfighter");
                        //                            : null; // If the user credentials are not found.
                    }
                    return _netCred;
                    */
                    #endregion Old_Login_Schema
                };
            }
            catch
            {
            }
            // To resolve to wait for socket in TIME_WAIT state.
            wssv.ReuseAddress = true;

            wssv.AddWebSocketService<Chat>(
              "/SAPI",
              s =>
              {
                  // s.Prefix = "Server#";

                  // To send the Sec-WebSocket-Protocol header that has a subprotocol name.
                  s.Protocol = "coms";

                  // To ignore the Sec-WebSocket-Extensions header.
                  s.IgnoreExtensions = true;

                  // To emit a WebSocket.OnMessage event when receives a ping.
                  s.EmitOnPing = true;

                  // To validate the Origin header.
                  s.OriginValidator = val =>
                  {
                      // Check the value of the Origin header, and return true if valid.
                      Uri origin;

                      return !val.IsNullOrEmpty()
                       && Uri.TryCreate(val, UriKind.Absolute, out origin)
                       && origin.Host == "localhost";
                  };

                  // To validate the cookies.
                  s.CookiesValidator = (req, res) =>
                  {
                      // Check the cookies in 'req', and set the cookies to send to
                      // the client with 'res' if necessary.
                      foreach (var cookie in req)
                      {
                          if (cookie.ToString().Contains("Client") || cookie.ToString().Contains("Admin"))
                              s.Prefix = cookie.ToString();
                          cookie.Expired = true;
                          res.Add(cookie);
                      }

                      return true; // If valid.
                  };
              }
            );

            wssv.Start();

            if (wssv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", wssv.Port);

                foreach (var path in wssv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Console.WriteLine("\nPress Enter key to stop the server...");
            string _msg = "";
            do
            {
                _msg = Console.ReadLine();
                if (_msg == "exit")
                    wssv.Stop();
                else if (_msg == "clients")
                    WebSocketSessionManager.ShowClients();
                else if (_msg == "admins")
                    WebSocketSessionManager.ShowAdmin();
                else { }
                System.Threading.Thread.Sleep(10);
            } while (_msg != "exit");
            wssv.Stop();
        }
    }
}
