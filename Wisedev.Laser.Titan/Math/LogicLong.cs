using Wisedev.Laser.Titan.DataStream;

namespace Wisedev.Laser.Titan.Math;

public class LogicLong
{
    private int _highInteger;
    private int _lowInteger;
    public LogicLong()
    {
        // LogicLong.
    }

    public LogicLong(int highInteger, int lowInteger)
    {
        _highInteger = highInteger;
        _lowInteger = lowInteger;
    }

    public static long ToLong(int highValue, int lowValue)
    {
        return ((long)highValue << 32) | (uint)lowValue;
    }

    public LogicLong Clone()
    {
        return new LogicLong(_highInteger, _lowInteger);
    }

    public bool IsZero()
    {
        return _highInteger == 0 && _lowInteger == 0;
    }

    public int GetHigherInt()
    {
        return _highInteger;
    }

    public int GetLowerInt()
    {
        return _lowInteger;
    }

    public void Decode(ByteStream stream)
    {
        _highInteger = stream.ReadInt();
        _lowInteger = stream.ReadInt();
    }

    public void Encode(ByteStream stream)
    {
        stream.WriteInt(_highInteger);
        stream.WriteInt(_lowInteger);
    }

    public int HashCode()
    {
        return _lowInteger + 31 * _highInteger;
    }

    public override int GetHashCode()
    {
        return HashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj != null && obj is LogicLong logicLong)
            return logicLong._highInteger == _highInteger && logicLong._lowInteger == _lowInteger;
        return false;
    }

    public static bool Equals(LogicLong a1, LogicLong a2)
    {
        if (a1 == null || a2 == null)
            return a1 == null && a2 == null;
        return a1._highInteger == a2._highInteger && a1._lowInteger == a2._lowInteger;
    }

    public override string ToString()
    {
        return string.Format("{0}-{1}", _highInteger, _lowInteger);
    }

    public static implicit operator LogicLong(long Long)
    {
        return new LogicLong((int)(Long >> 32), (int)Long);
    }

    public static implicit operator long(LogicLong Long)
    {
        return ((long)Long._highInteger << 32) | (uint)Long._lowInteger;
    }
}
