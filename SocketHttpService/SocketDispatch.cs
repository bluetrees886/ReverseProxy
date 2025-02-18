using Microsoft.VisualBasic;
using SocketHttpService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketHttpService
{
    using Buffer = Memory<byte>;
    public class SocketDispatch
    {
        private Socket _socket;
        private int _indexHeaderEnd(Buffer buffer, int offset, int len)
        {
            var span = buffer.Span;
            int state = 0;
            for (int i = offset; i < offset + len; i++)
            {
                switch (state)
                {
                    case 0:
                        if (span[i] == (byte)'\r')
                            state = 1;
                        break;
                    case 1:
                        if (span[i] == (byte)'\n')
                            state = 2;
                        else
                            state = 0;
                        break;
                    case 2:
                        if (span[i] == (byte)'\r')
                            state = 3;
                        else
                            state = 0;
                        break;
                    case 3:
                        if (span[i] == (byte)'\n')
                            return i;
                        else
                            state = 0;
                        break;
                }
            }
            return -1;
        }
        protected virtual async ValueTask doDispatchMessage(SocketMessageData messageData, CancellationToken cancellationToken)
        {
            var message = new SocketMessage(_socket, messageData);
        }
        protected virtual async IAsyncEnumerable<SocketMessageData> doReceiveMessage(CancellationToken cancellationToken)
        {
            var data = new SocketMessageData();
            while (!cancellationToken.IsCancellationRequested)
            {
                var len = await _socket.ReceiveAsync(data.Current, SocketFlags.Partial);
                if (len > 0)
                {
                    var headerEnd = _indexHeaderEnd(data.Current, data.CurrentOffset, len);

                    data.TryNext(len);
                    if (headerEnd >= 0)
                    {
                        yield return data;
                        data.Dispose();
                        data = new SocketMessageData();
                    }
                }
                else if (len < 0)
                {
                    yield return data;
                    data.Dispose();
                    break;
                }
            }
        }
        protected virtual async ValueTask doDispatch(CancellationToken cancellationToken)
        {
            await foreach(var messageData in doReceiveMessage(cancellationToken))
            {
                using (messageData)
                {
                    await doDispatchMessage(messageData, cancellationToken);
                }
            }
        }
        public async ValueTask Dispatch(CancellationToken cancellationToken)
        {
            await doDispatch(cancellationToken);
        }
        public SocketDispatch(Socket socket)
        {
            _socket = socket;
        }
    }
}
