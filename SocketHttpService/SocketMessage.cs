using SocketHttpService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHttpService
{
    public struct SocketMessage
    {
        private SocketMessageData _data;
        private Socket _socket;
        public async ValueTask Complete(CancellationToken cancellationToken)
        {

        }
        internal SocketMessage(Socket socket, SocketMessageData data)
        {
            _data = data;
            _socket = socket;
        }
    }
}
