using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using SRPSimulator;

internal class Program
{
    private static void Main(string[] args)
    {
        Receiver receiver = new Receiver();
        SRPData data = new();
        while (true) {
            receiver.Receive(ref data!);

            if (data == null)
                continue;

            Console.WriteLine("Time={0},\tValue={1},\tFreq={2},\tX={3},\tF={4},\tddp={5}", 
                data.Time, data.Power, data.Freq, data.X, data.F, data.DDP);
        }

    }
}

public class Receiver
{
    const string defaultIP = "127.0.0.1";
    const int defaultPort = 5557;

    Socket socket;
    byte[] buffer = new byte[256];

    public Receiver()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(new IPEndPoint(IPAddress.Parse(defaultIP), defaultPort));
    }

    public void Receive(ref SRPData data)
    {
        socket.Receive(buffer);
        SRPData.Deserialize(buffer, ref data);
    }

    ~Receiver()
    {
        socket.Dispose();
    }
}