using System;
using System.Net.Sockets;
using System.Text;

namespace SDRSharp.AntennaSwitch
{
    public sealed class OrangePiClient
    {
        public string Host { get; set; } = "192.168.1.50";
        public int Port { get; set; } = 5000;
        public bool IsConnected { get; private set; }

        public bool Connect(int timeoutMs = 1500)
        {
            try
            {
                using var client = new TcpClient();
                var task = client.ConnectAsync(Host, Port);
                if (!task.Wait(timeoutMs)) return IsConnected = false;
                IsConnected = client.Connected;
                return IsConnected;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        public void Disconnect() => IsConnected = false;

        public bool SendCommand(string command, int timeoutMs = 1500)
        {
            try
            {
                using var client = new TcpClient();
                var task = client.ConnectAsync(Host, Port);
                if (!task.Wait(timeoutMs) || !client.Connected) return false;
                using NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(command + "\n");
                stream.WriteTimeout = timeoutMs;
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch { return false; }
        }
    }
}
