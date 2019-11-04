using System;
using System.Net.Sockets;

namespace ProjetoChatCliente
{
    class ClientTCP
    {
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static bool isConnected = false;

        public static void ConnectToServer(string ip, int port)
        {
            try
            {
                clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectionCallback), clientSocket);
                isConnected = true;
            }
            catch (Exception ex)
            {
                isConnected = false;
                throw ex;
            }
        }

        private static void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);
                while (isConnected)
                {
                    byte[] dataReceived = new byte[1024 * 100];
                    clientSocket.Receive(dataReceived);
                    Form1.MesageRecievedFromServer(dataReceived);
                    isConnected = true;
                }
            }
            catch (Exception)
            {
                isConnected = false;
                ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem("Erro o conectar ao servidor", null, "Client", DateTime.Now, null);
                Form1.MesageRecievedFromServer(Form1.SerializeMensagem(msg));
            }
        }

        public static void SendData(byte[] data)
        {
            if (!isConnected)
                return;
            try{ clientSocket.Send(data);}
            catch (Exception ex){ throw ex;}
        }

    }
}
