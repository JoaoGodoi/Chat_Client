using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjetoChatServer
{
    class ServerTCP
    {
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024 * 100];
        public static List<Client> clients = new List<Client>();

        public static void SetupServer(int serverPort)
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
            serverSocket.Listen(100);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback),null);
            Console.WriteLine("Servidor Inicializado!");
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Client clientFound = new Client();
            clientFound.socket = socket;
            clientFound.index = clients.Count;
            clientFound.ip = socket.RemoteEndPoint.ToString();
            clientFound.StartClient();
            clients.Add(clientFound);
            Console.WriteLine("Conexão recebida de " + clientFound.ip);
            ProjetoChat.Mensagem msgToNewCliente = new ProjetoChat.Mensagem("Conexão realizada com sucesso!", null, "Servidor", DateTime.Now, null);
            ProjetoChat.Mensagem msgToAll = new ProjetoChat.Mensagem("Novo membro adicionado ao chat", null, "Servidor", DateTime.Now, null);
            byte[] msgTNCbyte = SerializeMensagem(msgToNewCliente);
            byte[] msgTA = SerializeMensagem(msgToAll);
            SendDataTo(clientFound, msgTNCbyte);
            SendDataToAll(msgTA);
        }

        public static void SendDataTo(Client client, byte[] data)
        {
            if (client.socket == null)
                return;
            client.socket.Send(data);
        }

        public static void SendDataToAll(byte[] data)
        {
            for (int i = 0; i < clients.Count; i++)
                if (clients[i].socket != null)
                    SendDataTo(clients[i], data);
        }

        public static byte[] SerializeMensagem(ProjetoChat.Mensagem msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = new byte[1024 * 100];
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(ms, msg);
                    buffer = ms.ToArray();
                }
                catch (Exception ex){ throw ex;}
                finally{ ms.Close();}
                return buffer;
            }
        }
    }

    public class Client
    {
        public int index;
        public string ip;
        public Socket socket;
        public bool closing = false;
        public byte[] buffer = new byte[1024 * 100];

        public void StartClient()
        {
            socket.BeginReceive(buffer,0,buffer.Length,SocketFlags.None,new AsyncCallback(ReceiveCallback),socket);
            closing = false;
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                int messageSize = socket.EndReceive(ar);
                if (messageSize <= 0)
                    CloseClient(index);
                else
                {
                    while (true)
                    {
                        buffer = new byte[1024 * 100];
                        socket.Receive(buffer);
                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            ProjetoChat.Mensagem msg = new ProjetoChat.Mensagem(null, null, null, DateTime.Now, null);
                            try
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                msg = (ProjetoChat.Mensagem)bf.Deserialize(ms);
                                if (msg.getContent().Equals(string.Empty))
                                {
                                    if (msg.getImage() != null)
                                        Console.WriteLine("(" + msg.getDataEnvio() + ") " + socket.RemoteEndPoint.ToString() + " (" + msg.getRemetente() + "): imagem size " + msg.getImage().Size);
                                    else
                                        Console.WriteLine("(" + msg.getDataEnvio() + ") " + socket.RemoteEndPoint.ToString() + " (" + msg.getRemetente() + "): text file size " + msg.getTxt().Length + " lines");
                                }
                                else
                                    Console.WriteLine("(" + msg.getDataEnvio() + ") " + socket.RemoteEndPoint.ToString() + " (" + msg.getRemetente() + "): " + msg.getContent());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Erro de converçao: " + ex.Message);
                            }
                            finally{ ms.Close();}
                        }
                        ServerTCP.SendDataToAll(buffer);
                    }
                }
            }
            catch{ CloseClient(index);}
        }

        private void CloseClient(int index)
        {
            closing = true;
            socket.Close();
            Console.WriteLine("Conexão com " + ip + " finalizada.");
            ServerTCP.clients.Remove(this);
            ProjetoChat.Mensagem msgToAll = new ProjetoChat.Mensagem("Um usuario foi desconectado", null, "Servidor", DateTime.Now, null);
            byte[] msgTNCbyte = ServerTCP.SerializeMensagem(msgToAll);
            ServerTCP.SendDataToAll(msgTNCbyte);
        }
    }


}
