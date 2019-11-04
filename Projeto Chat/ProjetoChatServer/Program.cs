using System;

namespace ProjetoChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverPort;
            Console.WriteLine("Entre com a porta do servidor:");
            serverPort = Console.ReadLine();
            ServerTCP.SetupServer(Convert.ToInt32(serverPort));

            while(true)
            {
                string msg = Console.ReadLine();
                ServerTCP.SendDataToAll(ServerTCP.SerializeMensagem(new ProjetoChat.Mensagem(msg, null, "Servidor", DateTime.Now, null)));
            }
        }
    }
}
