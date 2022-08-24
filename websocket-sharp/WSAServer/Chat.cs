using System;
using System.Threading;
using WebSocketSharp;
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
            _prefix = "Client#";
        }

        public string Prefix
        {
            get
            {
                return _prefix;
            }

            set
            {
                _prefix = !value.IsNullOrEmpty() ? value : "Client#";
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
            foreach (var adminSession in WebSocketSessionManager.clientDict)
                if (adminSession.Key.StartsWith("Admin#"))
                    Sessions.SendTo(Convert.ToString(msg), adminSession.Value);
           // Sessions.Broadcast(msg);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var fmt = "{0}: {1}";
            var msg = String.Format(fmt, _name, e.Data);
            Console.WriteLine(msg);
            if (_name.StartsWith("name=Admin#"))
                Sessions.Broadcast(msg);
            else if (_name.StartsWith("name=Client#"))
            {
                Console.WriteLine("Message should only go back to admin");
                int _count = Sessions.Count;
                foreach (var adminSession in WebSocketSessionManager.clientDict)
                    if (adminSession.Key.StartsWith("Admin#"))
                        Sessions.SendTo(Convert.ToString(msg), adminSession.Value);
                // Sessions.SendTo(msg, "Admin#");
            }
        }

        protected override void OnOpen()
        {
            _name = getName();

            var fmt = "{0} has logged in!";
            var msg = String.Format(fmt, _name);

            Console.WriteLine(msg);
            if (_name.StartsWith("name=Admin#"))
                Sessions.Broadcast(msg);
            else if (_name.StartsWith("name=Client#"))
            {
                Console.WriteLine("Message should only go back to admin");
                int _count = Sessions.Count;
                foreach (var adminSession in WebSocketSessionManager.clientDict)
                    if (adminSession.Key.StartsWith("Admin#"))
                        Sessions.SendTo(Convert.ToString(msg), adminSession.Value);
            }
        }
    }
}
