using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;



namespace JsonSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerThread.startThread();
        }
    }

    class ServerThread
    {
        public Socket serverSocket;

        public static String serverIp = "192.168.1.105";

        public static int serverPort = 1234;

        public static int testCount = 0;

        public ServerThread()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(10);
        }

        public static void ExeThread()
        {
            ServerThread serverThread = new ServerThread();
            while(true)
            {
                serverThread.HandleConnection(serverThread.serverSocket.Accept());
            }
        }


        public static void startThread()
        {
            new Thread(new ThreadStart(ExeThread)).Start();
        }

        public void HandleConnection(Socket incomingSocket)
        {
            Thread worker = new Thread(this.RecieveAndSend);
            worker.Start(incomingSocket);
            worker.Join();
        }

        public void RecieveAndSend(object incoming)
        {
            Socket socket = (Socket)incoming;
            byte[] bytes = new byte[1000];

            int bytesRecieved = socket.Receive(bytes);
            string strRecieved = Encoding.UTF8.GetString(bytes, 0, bytesRecieved);
            testCount++;
            string json = @"{
                                'title' : 'Jelly',
                                'content' : 'delusion" + testCount+@"'
                            }
                            ";

            json += "\n<End of json>";
            socket.Send(Encoding.UTF8.GetBytes(json));

            socket.Close();
        }


    }

}
