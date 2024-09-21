using System.Net.Sockets;

namespace SRPSimulator
{
    class DataSender
    {
        Socket socket;

        const string defaultIP = "127.0.0.1";
        const int defaultPort = 5557;
        byte[] buffer = new byte[512];

        public DataSender(string IP = defaultIP, int port = defaultPort)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SetTarget(IP, defaultPort);
        }

        public void Send(SRPData data)
        {
            data.Serialize(buffer);
            socket.Send(buffer, SRPData.Size, 0);
        }

        public void SetTarget(string targetIP, int port) => socket.Connect(targetIP, port);

        ~DataSender() => socket.Dispose();
    }
}
