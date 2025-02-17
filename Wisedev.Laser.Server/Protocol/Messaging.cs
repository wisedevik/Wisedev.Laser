using Microsoft.Extensions.Logging;
using Wisedev.Laser.Server.Network.Connection;
using Wisedev.Laser.Titan.DataStream;
using Wisedev.Laser.Titan.Message;

namespace Wisedev.Laser.Server.Protocol;

internal class Messaging : IConnectionListener
{
    private const int HeaderSize = 7;

    private readonly ILogger _logger;
    private readonly MessageFactory _factory;

    private IConnectionListener.SendCallback? _sendCallback;
    private IConnectionListener.ReceiveCallback? _receiveCallback;

    public Messaging(MessageFactory factory, ILogger<Messaging> logger)
    {
        _logger = logger;
        _factory = factory;
    }

    public IConnectionListener.SendCallback OnSend
    {
        set
        {
            _sendCallback = value;
        }
    }

    public IConnectionListener.ReceiveCallback RecvCallback
    {
        set
        {
            _receiveCallback = value;
        }
    }

    public async ValueTask<int> OnReceive(Memory<byte> buffer, int size)
    {
        int consumedBytes = 0;

        while (size >= HeaderSize)
        {
            ReadHeader(buffer.Span, out int messageType, out int length, out int messageVersion);

            if (size < HeaderSize + length) break;

            size -= length + HeaderSize;
            consumedBytes += length + HeaderSize;

            byte[] encodingBytes = buffer.Slice(HeaderSize, length).ToArray();
            buffer = buffer[consumedBytes..];

            int encodingLength = length;

            PiranhaMessage? message = _factory.CreateMessageByType(messageType);
            if (message == null)
            {
                _logger.LogWarning("Ignoring message of unknown type {messageType}", messageType);
                continue;
            }

            message.SetMessageVersion(messageVersion);
            message.GetByteStream().SetByteArray(encodingBytes, encodingLength);
            message.Decode();

            await _receiveCallback!(message);
        }

        return consumedBytes;
    }

    public async Task Send(PiranhaMessage message)
    {
        message.Encode();

        byte[] encodingBytes = message.GetByteStream().GetByteArray()!.Take(message.GetEncodingLength()).ToArray();

        byte[] fullPayload = new byte[encodingBytes.Length + HeaderSize];

        WriteHeader(fullPayload, message, encodingBytes.Length);
        encodingBytes.CopyTo(fullPayload, HeaderSize);

        await _sendCallback!(fullPayload);

        _logger.LogInformation("Message with type {type} sent", message.GetMessageType());
    }

    private static void ReadHeader(ReadOnlySpan<byte> buffer, out int messageType, out int encodingLength, out int messageVersion)
    {
        messageType = buffer[0] << 8 | buffer[1];
        encodingLength = buffer[2] << 16 | buffer[3] << 8 | buffer[4];
        messageVersion = buffer[5] << 8 | buffer[6];
    }

    private static void WriteHeader(Span<byte> buffer, PiranhaMessage message, int length)
    {
        int messageType = message.GetMessageType();
        int messageVersion = message.GetMessageVersion();

        buffer[0] = (byte)(messageType >> 8);
        buffer[1] = (byte)messageType;
        buffer[2] = (byte)(length >> 16);
        buffer[3] = (byte)(length >> 8);
        buffer[4] = (byte)length;
        buffer[5] = (byte)(messageVersion >> 8);
        buffer[6] = (byte)messageVersion;
    }
}
