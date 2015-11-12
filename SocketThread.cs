using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net;
using System.Net.Sockets;
using System.Net.Json;

namespace socketTest
{
    class NameListThraed
    {
        public Socket nameListSocket;
        
        public static String serverIp = "127.0.0.1"
        
        public static int serverPort = 1234
        
        public NameListThraed()
        {
            IPEndPoint objEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
            nameListSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            nameListSocket.Bind(objEndpoint);
            nameListSocket.Listen(10);
        }

        public static void ExeThread()
        {
            SocketThread socketThread = new SocketThread();
            while (true)
            {
                nameList.HandleConnection(nameList.nameListSocket.Accept());
            }
        }

        public static void startThread()
        {
            new Thread(new ThreadStart(ExeThread)).Start();
        }

        public void HandleConnection(Socket iIncomingSocket)
        {
            Thread worker = new Thread(this.RecieveAndSend);
            worker.Start(iIncomingSocket);
            worker.Join();
        }

        public void RecieveAndSend(object iIncoming)
        {
            Socket objSocket = (Socket)iIncoming;
            byte[] bytes = new byte[10240000];

            int bytesRecieved = objSocket.Receive(bytes);
            string strReceived = System.Text.Encoding.UTF8.GetString(bytes, 0, bytesRecieved);


            string strSend = null;
            JsonArrayCollection nameList = new JsonArrayCollection();
            int count = 1;
            foreach (ePresentation.Database.DataSet.PeopleRow row in GlobalVar.m_tbPeople)
            {
                if (row == null)
                    continue;
                JsonObjectCollection name = new JsonObjectCollection();


                try
                {
                    name.Add(new JsonNumericValue("orderNumber", count));
                    count++;
                    name.Add(new JsonStringValue("name", row.ppComName));
                    name.Add(new JsonStringValue("corporation", row.ppCorporation));
                    name.Add(new JsonStringValue("position", row.ppPosition));

                    nameList.Add(name);



                } catch (Exception ex2)
                {
                    GlobalVar.m_log.write("request_ClientInfoList", ex2);
                }
            }
            strSend = nameList.ToString() + "\n<NameListEnd>";
            objSocket.Send(System.Text.Encoding.UTF8.GetBytes(strSend));

            objSocket.Close();
        }

    }
}
