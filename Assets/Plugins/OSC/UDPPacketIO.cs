using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UniOSC
{
    /// <summary>
    /// UdpPacket provides packetIO over UDP
    /// </summary>
    public class UDPPacketIO
    {
        private UdpClient Sender;
        private UdpClient Receiver;
        private bool socketsOpen;
        private IPEndPoint remote;
        private IPEndPoint local;
        private IPAddress multicast = null;


        public UDPPacketIO(string remoteIP, int remotePort, int localPort, string multicastIP = null)
        {
            IPAddress remoteAddr;
            if (!IPAddress.TryParse(remoteIP, out remoteAddr))
            {
                Debug.LogErrorFormat("Can not parse Host IP: {0} ", remoteIP);
                return;
            }
            remote = new IPEndPoint(remoteAddr, remotePort);
            local = new IPEndPoint(IPAddress.Any, localPort);

            if (!string.IsNullOrEmpty(multicastIP))
            {
                if (!IPAddress.TryParse(multicastIP, out multicast))
                {
                    Debug.LogError("Can not parse Multicast IP");
                    multicast = null;
                }
            }
            socketsOpen = false;
        }


        ~UDPPacketIO()
        {
            // latest time for this socket to be closed
            if (IsOpen())
            {
                Debug.Log("closing udpclient listener on port " + local.Port);
                Close();
            }

        }

        /// <summary>
        /// Open a UDP socket and create a UDP sender.
        /// 
        /// </summary>
        /// <returns>True on success, false on failure.</returns>
        public bool Open()
        {
            try
            {
                Sender = new UdpClient();
                Debug.Log("Opening OSC listener on port " + local.Port);

                Receiver = new UdpClient(local);
                if (multicast != null)
                {
                    Debug.LogFormat("Joinnin multicast: {0}", multicast);
                    Receiver.JoinMulticastGroup(multicast);
                }
                socketsOpen = true;

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("cannot open udp client interface at port " + local.Port);
                Debug.LogWarning(e);
            }

            return false;
        }

        /// <summary>
        /// Close the socket currently listening, and destroy the UDP sender device.
        /// </summary>
        public void Close()
        {
            if (Sender != null)
                Sender.Close();

            if (Receiver != null)
            {
                Receiver.Close();
                // Debug.Log("UDP receiver closed");
            }
            Receiver = null;
            socketsOpen = false;

        }

        public void OnDisable()
        {
            Close();
        }

        /// <summary>
        /// Query the open state of the UDP socket.
        /// </summary>
        /// <returns>True if open, false if closed.</returns>
        public bool IsOpen()
        {
            return socketsOpen;
        }

        /// <summary>
        /// Send a packet of bytes out via UDP.
        /// </summary>
        /// <param name="packet">The packet of bytes to be sent.</param>
        /// <param name="length">The length of the packet of bytes to be sent.</param>
        public void SendPacket(byte[] packet, int length)
        {
            if (!IsOpen())
                Open();
            if (!IsOpen())
                return;

            Sender.Send(packet, length, remote);
            //Debug.Log("osc message sent to "+remoteHostName+" port "+remotePort+" len="+length);
        }

        /// <summary>
        /// Receive a packet of bytes over UDP.
        /// </summary>
        /// <param name="buffer">The buffer to be read into.</param>
        /// <returns>The number of bytes read, or 0 on failure.</returns>
        public int ReceivePacket(byte[] buffer)
        {
            if (!IsOpen())
                Open();
            if (!IsOpen())
                return 0;

            byte[] incoming = Receiver.Receive(ref local);
            int count = Math.Min(buffer.Length, incoming.Length);
            System.Array.Copy(incoming, buffer, count);
            return count;
        }



        /// <summary>
        /// Get the address is multicast of not
        /// </summary>
        /// <param name="addr">the address</param>
        /// <returns>multicast address or not</returns>
        static bool IsMulticastAddress(IPAddress addr)
        {
            // 224.0.0.0 - 239.255.255.255
            byte[] bytes = addr.GetAddressBytes();
            return (bytes[0] & 0xF0) == 0xE0;
        }
    }
}