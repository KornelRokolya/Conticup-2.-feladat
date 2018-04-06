using System;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ContiCup_TCP
{
    public class Program
    {
        public static bool SendMessageToServer(String server, int portNumber, String message, 
            Encoding encoding = null, bool addTerminalCharacter = false)
        {
            bool ok = true;

            try
            {
                //Set up TcpClient
                TcpClient client = new TcpClient(server, portNumber);
                NetworkStream stream = client.GetStream();
                
                if (message != null)
                {
                    //Convert message String to bytes
                    if (encoding == null) encoding = Encoding.ASCII;
                    Byte[] sentData = Encoding.ASCII.GetBytes(message);

                    //add null-character if neccessary
                    if (addTerminalCharacter)
                    {
                        Byte[] nullTerminatedString = new Byte[sentData.Length + 1];
                        Array.Copy(sentData, nullTerminatedString, sentData.Length);
                        nullTerminatedString[nullTerminatedString.Length - 1] = 0;
                        sentData = nullTerminatedString;
                    }

                    //send data
                    stream.Write(sentData, 0, sentData.Length);
                    //Console.WriteLine("Sent: {0}", message);
                }

                //Variables to hold received data in.
                Byte[] data = new Byte[256];
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //Console.WriteLine("Received: {0}", responseData);

                // Close everything
                stream.Close();
                client.Close();
            }
            catch
            {
                ok = false;
            }

            return ok;
        }

        public static void Test(String server, int portNumber, List<Encoding> encodings, List<String> messages)
        {
            //igen, ennyi exception handling lesz és kész, semmi több :D
            //ha elcseszitek akkor ti vagytok a hülyék
            if (encodings == null || messages == null) throw new ArgumentNullException();

            for (int i = 0; i < encodings.Count; i++)
                for (int j = 0; j < messages.Count; j++)
                {
                    Console.Clear();
                    Console.WriteLine("using...");
                    Console.WriteLine("Encoding: {0} / {1}", i + 1, encodings.Count);
                    Console.WriteLine("Message: {0} / {1}", j + 1, messages.Count);

                    bool ok1 = SendMessageToServer(server, portNumber, messages[j], encodings[i], true);
                    bool ok2 = SendMessageToServer(server, portNumber, messages[j], encodings[i], false);

                    if (ok1 || ok2)
                    {
                        Console.WriteLine("Encoding: {0}", encodings[i]);
                        Console.WriteLine("Message: {0}", messages[j]);
                        if(ok1) Console.WriteLine("Null char: " + ok1);
                        Console.WriteLine();
                        return;
                    }
                }

            Console.Clear();
            Console.WriteLine("Nothing received!");
        }

        static void Main(string[] args)
        {
            String server = "ecovpn.dyndns.org";
            int portNumber = 5950;

            List<Encoding> possibleEncodings = new List<Encoding>()
            {
                Encoding.ASCII,
                Encoding.BigEndianUnicode,
                Encoding.Default,
                Encoding.Unicode,
                Encoding.UTF32,
                Encoding.UTF7,
                Encoding.UTF8
            };

            List<String> possibleMessages = new List<string>()
            {
                "MQTT",
                "FormatC",
                "formatC",
                "formatc",
                "FORMATC",
                "Continental",
                "topic Continental",
                "Continental since 1871",
                "CONTINENTAL SINCE 1871"
            };

            Test(server, portNumber, possibleEncodings, possibleMessages);
            Console.Read();
        }
    }
}
