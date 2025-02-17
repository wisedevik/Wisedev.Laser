﻿using Wisedev.Laser.Titan.DataStream;

namespace Wisedev.Laser.Titan.Message;

public class PiranhaMessage
{
    protected ByteStream _stream;
    protected int _version;

    public PiranhaMessage(short messageVersion = 0)
    {
        _stream = new ByteStream(10);
        _version = messageVersion;
    }

    public virtual void Decode()
    {
    }

    public virtual void Encode()
    {
    }

    public virtual short GetMessageType()
    {
        return 0;
    }
    public virtual int GetServiceNodeType()
    {
        return -1;
    }

    public int GetMessageVersion()
    {
        return _version;
    }

    public void SetMessageVersion(int version)
    {
        _version = version;
    }

    public bool IsServerToClientMessage()
    {
        return GetMessageType() >= 20000;
    }

    public byte[] GetMessageBytes()
    {
        return _stream.GetByteArray();
    }

    public int GetEncodingLength()
    {
        return _stream.GetLength();
    }

    public ByteStream GetByteStream()
    {
        return _stream;
    }
}