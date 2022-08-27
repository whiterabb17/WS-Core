using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
using WSServer;
using WSServer.Properties;

namespace WSAServer
{
    internal class HttpServerClass
    {
        private static void RTC()
        {
            CFManager.ReadAllSettings();
            string _drp = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
            string _drpj = System.IO.Path.Combine(_drp, "Js");
            string _drpi = System.IO.Path.Combine(_drp, "index.html");
            string _drpjs = System.IO.Path.Combine(_drpj, "echotest.js");
            string _crp = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "certs");
            if (!System.IO.Directory.Exists(_drp))
            { 
                System.IO.Directory.CreateDirectory(_drp);
                if (!System.IO.File.Exists(_drpi))
                    System.IO.File.WriteAllText(_drpi, Resources.index);
                if (!System.IO.Directory.Exists(_drpj))
                    System.IO.Directory.CreateDirectory(_drpj);
                if (!System.IO.File.Exists(_drpjs))
                    System.IO.File.WriteAllText(_drpjs, Resources.echo);
            }
            if (!System.IO.Directory.Exists(_crp))
                System.IO.Directory.CreateDirectory(_crp);
            CFManager.AddUpdateAppSettings("DocumentRootPath", _drp);
            CFManager.AddUpdateAppSettings("CertFilePath", _crp);
        }
        public static void HtRuntime()
        {
            var httpsv = new HttpServer(4649);
            //var httpsv = new HttpServer (5963, true);
            //var httpsv = new HttpServer (System.Net.IPAddress.Any, 4649);
            //var httpsv = new HttpServer (System.Net.IPAddress.Any, 5963, true);
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Any, 4649);
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Any, 5963, true);
            //var httpsv = new HttpServer ("http://0.0.0.0:4649");
            //var httpsv = new HttpServer ("https://0.0.0.0:5963");
            //var httpsv = new HttpServer ("http://[::0]:4649");
            //var httpsv = new HttpServer ("https://[::0]:5963");
            //var httpsv = new HttpServer (System.Net.IPAddress.Loopback, 4649);
            //var httpsv = new HttpServer (System.Net.IPAddress.Loopback, 5963, true);
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Loopback, 4649);
            //var httpsv = new HttpServer (System.Net.IPAddress.IPv6Loopback, 5963, true);
            //var httpsv = new HttpServer ("http://localhost:4649");
            //var httpsv = new HttpServer ("https://localhost:5963");
            //var httpsv = new HttpServer ("http://127.0.0.1:4649");
            //var httpsv = new HttpServer ("https://127.0.0.1:5963");
            //var httpsv = new HttpServer ("http://[::1]:4649");
            //var httpsv = new HttpServer ("https://[::1]:5963");
#if DEBUG
            // To change the logging level.
            httpsv.Log.Level = LogLevel.Trace;
            // To change the wait time for the response to the WebSocket Ping or Close.
            httpsv.WaitTime = TimeSpan.FromSeconds(2);
            // Not to remove the inactive WebSocket sessions periodically.
            httpsv.KeepClean = false;
#endif
            // To provide the secure connection.
            /*
            var cert = ConfigurationManager.AppSettings["ServerCertFile"];
            var passwd = ConfigurationManager.AppSettings["CertFilePassword"];
            httpsv.SslConfiguration.ServerCertificate = new X509Certificate2 (cert, passwd);
             */
            // To provide the HTTP Authentication (Basic/Digest).
            httpsv.AuthenticationSchemes = AuthenticationSchemes.Basic;
            httpsv.Realm = "WebSocket Test";
            try
            {
                httpsv.UserCredentialsFinder = id =>
                {
                    NetworkCredential _netCred = null;
                    if (id.Name == "Client#")
                    {
                        string name = id.Name;
                        // Return user name, password, and roles.
                        _netCred = new NetworkCredential(name, "password", "gunfighter"); //  : null; // If the user credentials are not found.
                    }
                    else if (id.Name == "Admin#")
                    {
                        string name = id.Name;
                        // Return user name, password, and roles.
                        _netCred = new NetworkCredential(name, "Password1", "gunfighter");
                    //                            : null; // If the user credentials are not found.
                    }

                    return _netCred;
                };
            }
            catch
            {
            }

            // To resolve to wait for socket in TIME_WAIT state.
            httpsv.ReuseAddress = true;
            // Set the document root path.
            RTC();
            var appSettings = ConfigurationManager.AppSettings;
            httpsv.DocumentRootPath = appSettings["DocumentRoot"];
            // Set the HTTP GET request event.
            httpsv.OnGet += (sender, e) =>
            {
                var req = e.Request;
                var res = e.Response;
                var path = req.RawUrl;
                if (path == "/")
                    path += "index.html";
                byte[] contents;
                if (!e.TryReadFile(path, out contents))
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

                if (path.EndsWith(".html"))
                {
                    res.ContentType = "text/html";
                    res.ContentEncoding = Encoding.UTF8;
                }
                else if (path.EndsWith(".js"))
                {
                    res.ContentType = "application/javascript";
                    res.ContentEncoding = Encoding.UTF8;
                }

                res.ContentLength64 = contents.LongLength;
                res.Close(contents, true);
            };
            // Add the WebSocket services.
            httpsv.AddWebSocketService<Echo>("/Echo");
            httpsv.AddWebSocketService<Chat>("/Chat", s =>
            {
                // To send the Sec-WebSocket-Protocol header that has a subprotocol name.
                s.Protocol = "chat";
                // To ignore the Sec-WebSocket-Extensions header.
                s.IgnoreExtensions = true;
                // To emit a WebSocket.OnMessage event when receives a ping.
                s.EmitOnPing = true;
                // To validate the Origin header.
                s.OriginValidator = val =>
                {
                    // Check the value of the Origin header, and return true if valid.
                    Uri origin;
                    return !val.IsNullOrEmpty() && Uri.TryCreate(val, UriKind.Absolute, out origin) && origin.Host == "localhost";
                };
                // To validate the cookies.
                s.CookiesValidator = (req, res) =>
                {
                    // Check the cookies in 'req', and set the cookies to send to
                    // the client with 'res' if necessary.
                    foreach (var cookie in req)
                    {
                        if (cookie.ToString().EndsWith("#"))
                            s.Prefix = cookie.ToString();
                        cookie.Expired = true;
                        res.Add(cookie);
                    }

                    return true; // If valid.
                };
            });
            // Add the WebSocket service with initializing.
            httpsv.AddWebSocketService<Chat>("/SAPI", s =>
            {
                //s.Prefix = "Server#";
                // To send the Sec-WebSocket-Protocol header that has a subprotocol name.
                s.Protocol = "chat";
                // To ignore the Sec-WebSocket-Extensions header.
                s.IgnoreExtensions = true;
                // To emit a WebSocket.OnMessage event when receives a ping.
                s.EmitOnPing = true;
                // To validate the Origin header.
                s.OriginValidator = val =>
                {
                    // Check the value of the Origin header, and return true if valid.
                    Uri origin;
                    return !val.IsNullOrEmpty() && Uri.TryCreate(val, UriKind.Absolute, out origin) && origin.Host == "localhost";
                };
                // To validate the cookies.
                s.CookiesValidator = (req, res) =>
                {
                    // Check the cookies in 'req', and set the cookies to send to
                    // the client with 'res' if necessary.
                    foreach (var cookie in req)
                    {
                        if (cookie.ToString().Contains("Admin") || cookie.ToString().Contains("Client"))
                            s.Prefix = cookie.ToString();
                        cookie.Expired = true;
                        res.Add(cookie);
                    }

                    return true; // If valid.
                };
            });
            httpsv.Start();
            if (httpsv.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", httpsv.Port);
                foreach (var path in httpsv.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            Console.WriteLine("\nPress Enter key to stop the server...");
            Console.ReadLine();
            httpsv.Stop();
        }
    }
}