using System;
using System.Net.Sockets;

namespace ContiCup_TCP
{
    class Program
    {
        static void Connect(String server, String message, int port)
        {
            try
            {
                //Set up TcpClient
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();
                
                if (message != null)
                {
                    //Convert message String to bytes and send
                    Byte[] sentData = System.Text.Encoding.ASCII.GetBytes(message);
                    stream.Write(sentData, 0, sentData.Length);
                    Console.WriteLine("Sent: {0}", message);
                }

                //Variables to hold received data in.
                Byte[] data = new Byte[256];
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // Close everything
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch
            {
                Console.WriteLine("Error!");
            }

            Console.Read();
        }

        static void Main(string[] args)
        {
            string message = "Continental";
            int portNumber = 5950;
            Connect("ecovpn.dyndns.org", message, portNumber);
        }
    }
}
