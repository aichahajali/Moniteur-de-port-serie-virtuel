using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringDll
{
    public class Monitoring
    {
        // To get the static IP of the server 
        public static String getIP()
        {
            var myhost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipaddr in myhost.AddressList)
            {
                if (ipaddr.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipaddr.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address was found");
        }

        //Get the Mac adress of the RS485 converter 
        public String getInfo(int port, String ip)
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse(ip);
                server = new TcpListener(localAddr, port);
                //Start listening for client requests 
                server.Start();
                //Buffer for reading data
                Byte[] bytes = new Byte[256];
                String data = null;
                // Enter the listening loop 
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    data = null;
                    //Get a stream object for reading and writing 
                    NetworkStream stream = client.GetStream();
                    int i;
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        //data bytes to String 
                        data = BitConverter.ToString(bytes, 0, i).Replace("-", "");
                        return (data);
                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return null;
            }
            finally
            {
              //Stop listening for new clients
              server.Stop();
            }
            
        }
        //this function detect alarm trigger
        public String alarmTrigger(String data)
        {
            String state = "";
            DateTime dateTime = DateTime.Now;
            //Checking data existence 
            if (data.Contains("12000008"))
            {
                //First index of string
                int i = data.IndexOf("12000008");
                String str = data.Substring(i, 10);
                String intrusion = "Gate state : Intrusion Alarm";
                String tailing = "Gate state : Tailing Alarm";
                String stayed = "Gate state : Stayed Alarm";
                String external = "Gate state : External Alarm";
                String reverse = "Gate state : Reverse Alarm";
                switch (str)
                {
                    case "1200000860":
                        state = intrusion + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000862":
                        state = tailing + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000861":
                        state = reverse + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "120000085F":
                        state = stayed + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000863":
                        state = external + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                }
            }
            return state;
        }
        //checking exit door state 
        public String exitDoor(String data)
        {
            DateTime dateTime = DateTime.Now;
            String state = "";
            //Checking data existence
            if (data.Contains("12000008"))
            {
                //First index of string 
                int i = data.IndexOf("12000008");
                String str = data.Substring(i, 10);
                String opening = "Gate state :Opening (Exit)";
                String opened = "Gate state :Opened (Exit)";
                String closing = "Gate state :Closing (Exit)";

                switch(str)
                {
                    case "120000080D":
                        state = opening + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000809":
                        state = closing + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000807":
                        state = opened + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                }
            }
            return state;
        }
        //checking entry door state 
        public static String entryDoor(String data)
        {
            DateTime dateTime = DateTime.Now;
            String state = "";
            // Checking Data
            if (data.Contains("12000008"))
            {
                String opening = "Gate state: Opening(Entry)";
                String closing = "Gate state: Closing(Entry)";
                String closed = "Gate state: Closed(Idle)";
                // First index of string
                int i = data.IndexOf("12000008");
                String str = data.Substring(i, 10);

                switch (str)
                {
                    case "120000080C":
                        state = opening + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000808":
                        state = closing + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                    case "1200000800":
                        state = closed + dateTime.ToString(" dd/MM/yyyy HH:mm tt");
                        break;
                }  
            }
            return state;
        }
    }
}

