using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace WSAServer
{
    public class Echo : WebSocketBehavior
    {
        protected override void OnMessage (MessageEventArgs e)
        {
            Console.WriteLine(e.Data);
            Send (e.Data);
        }
    }
}
