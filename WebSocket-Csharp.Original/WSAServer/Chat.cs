using System;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace WSAServer
{
    public class Chat : WebSocketBehavior
    {
        private string _name;
        private static int _number = 0;
        private string _prefix;

        public Chat()
        {
            _prefix = "Server#";
        }

        public string Prefix
        {
            get
            {
                return _prefix;
            }

            set
            {
                _prefix = !value.IsNullOrEmpty() ? value : "Server#";
            }
        }

        private string getName()
        {
            var name = QueryString["name"];

            return !name.IsNullOrEmpty() ? name : _prefix + getNumber();
        }

        private static int getNumber()
        {
            return Interlocked.Increment(ref _number);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (_name == null)
                return;

            var fmt = "{0} got logged off...";
            var msg = String.Format(fmt, _name);
            Console.WriteLine(msg);
            if (WebSocketSessionManager.adminDict.Count > 0)
                foreach (var adminSession in WebSocketSessionManager.adminDict)
                    Sessions.SendTo(Convert.ToString(msg), adminSession.Value);
            // Sessions.Broadcast(msg);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var fmt = "{0}: {1}";
            var msg = String.Format(fmt, _name, e.Data);
            Console.WriteLine(msg);
            if (_name.Contains("Admin"))
            {
                foreach (var _selectedClient in WebSocketSessionManager.clientDict)
                {
                    if (e.Data.Contains(_selectedClient.Key))
                        Sessions.SendTo(msg.Replace(_selectedClient.Key + ":", ""), _selectedClient.Value);
                    else
                        Sessions.Broadcast(msg);
                }
            }
            else if (_name.Contains("Client"))
            {
                Console.WriteLine("Message should only go back to admin");
                // int _count = Sessions.Count;
                if (WebSocketSessionManager.adminDict.Count > 0)
                {
                    foreach (var _admin in WebSocketSessionManager.adminDict)
                        Sessions.SendTo(Convert.ToString(msg), _admin.Value);
                }
            }
        }

        protected override void OnOpen()
        {
            _name = QueryString["name"];
            //foreach (var _admin in WebSocketSessionManager.adminDict)
            //    _name = _admin.Key;

            var fmt = "{0} has logged in!";
            var msg = String.Format(fmt, _name);
            
            Console.WriteLine(msg);
            if (_name.Contains("Admin"))
                Sessions.Broadcast(msg);
            else if (_name.Contains("Client"))
            {
                Console.WriteLine("Message should only go back to admin");
                int _count = Sessions.Count;
                if (WebSocketSessionManager.adminDict.Count > 0)
                    foreach (var adminSession in WebSocketSessionManager.adminDict)
                        Sessions.SendTo(Convert.ToString(msg), adminSession.Value);
            }
        }
    }
}
