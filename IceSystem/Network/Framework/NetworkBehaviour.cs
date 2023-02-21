using System;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace IceEngine.Networking.Framework
{
    /// <summary>
    /// General network object(without id)
    /// </summary>
    public abstract class NetworkBehaviour : IDisposable
    {
        protected static bool IsHost => Ice.Network.IsHost;
        protected static bool IsConnected => Ice.Network.IsConnected;
        public virtual NetIDMark ID => null;

        // Thread
        protected static void CallMainThread(Action action) => Ice.Network.CallMainThread(action);

        // Packet Send
        protected static void ClientSend(Pkt pkt) => Ice.Network.ClientSend(pkt);
        protected static void ClientUDPSend(Pkt pkt, IPEndPoint endPoint) => Ice.Network.ClientUDPSend(pkt, endPoint);
        protected static void ServerSend(Pkt pkt, ServerBase.Connection connection) => Ice.Network.ServerSend(pkt, connection);
        protected static void ServerBroadcast(Pkt pkt) => Ice.Network.ServerBroadcast(pkt);
        protected static void ServerUDPSend(Pkt pkt, IPEndPoint endPoint) => Ice.Network.ServerUDPSend(pkt, endPoint);
        protected static void ServerUDPBroadcast(Pkt pkt) => Ice.Network.ServerUDPBroadcast(pkt);

        // Inner Code
        event Action _onDestroy;
        public NetworkBehaviour() => IceNetworkUtility.HandleNetworkAttributes(this, ref _onDestroy, ID);

        bool disposedValue;
        protected virtual void OnDestroy()
        {
            _onDestroy?.Invoke();
            ID?.ClearID();
        }
        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                OnDestroy();
                disposedValue = true;
            }
        }

        ~NetworkBehaviour()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
