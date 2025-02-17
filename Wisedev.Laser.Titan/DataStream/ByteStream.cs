using System.Text;
using Wisedev.Laser.Titan.Math;

namespace Wisedev.Laser.Titan.DataStream;

public class ByteStream
{
    private int m_bitIdx;

    private byte[] m_buffer;
    private int m_length;
    private int m_offset;

    public ByteStream(int capacity)
    {
        this.m_buffer = new byte[capacity];
    }

    public ByteStream(byte[] buffer, int length)
    {
        this.m_length = length;
        this.m_buffer = buffer;
    }

    public int GetLength()
    {
        if (this.m_offset < this.m_length)
        {
            return this.m_length;
        }

        return this.m_offset;
    }

    public int GetOffset()
    {
        return this.m_offset;
    }

    public bool IsAtEnd()
    {
        return this.m_offset >= this.m_length;
    }

    public void Clear(int capacity)
    {
        this.m_buffer = new byte[capacity];
        this.m_offset = 0;
    }

    public byte[] GetByteArray()
    {
        return this.m_buffer;
    }

    public virtual void WriteLong(LogicLong value)
    {
        value.Encode(this);
    }

    public bool ReadBoolean()
    {
        if (this.m_bitIdx == 0)
        {
            ++this.m_offset;
        }

        bool value = (this.m_buffer[this.m_offset - 1] & (1 << this.m_bitIdx)) != 0;
        this.m_bitIdx = (this.m_bitIdx + 1) & 7;
        return value;
    }

    public byte ReadByte()
    {
        this.m_bitIdx = 0;
        return this.m_buffer[this.m_offset++];
    }

    public short ReadShort()
    {
        this.m_bitIdx = 0;

        return (short)((this.m_buffer[this.m_offset++] << 8) |
                        this.m_buffer[this.m_offset++]);
    }

    public int ReadInt()
    {
        this.m_bitIdx = 0;

        return (this.m_buffer[this.m_offset++] << 24) |
               (this.m_buffer[this.m_offset++] << 16) |
               (this.m_buffer[this.m_offset++] << 8) |
               this.m_buffer[this.m_offset++];
    }

    public int ReadVInt()
    {
        m_bitIdx = 0;
        int value = 0;
        byte byteValue = m_buffer![m_offset++];

        if ((byteValue & 0x40) != 0)
        {
            value |= byteValue & 0x3F;

            if ((byteValue & 0x80) != 0)
            {
                value |= ((byteValue = ReadByte()) & 0x7F) << 6;

                if ((byteValue & 0x80) != 0)
                {
                    value |= ((byteValue = ReadByte()) & 0x7F) << 13;

                    if ((byteValue & 0x80) != 0)
                    {
                        value |= ((byteValue = ReadByte()) & 0x7F) << 20;

                        if ((byteValue & 0x80) != 0)
                        {
                            value |= ((byteValue = ReadByte()) & 0x7F) << 27;
                            return (int)(value | 0x80000000);
                        }

                        return (int)(value | 0xF8000000);
                    }

                    return (int)(value | 0xFFF00000);
                }

                return (int)(value | 0xFFFFE000);
            }

            return (int)(value | 0xFFFFFFC0);
        }

        value |= byteValue & 0x3F;

        if ((byteValue & 0x80) != 0)
        {
            value |= ((byteValue = ReadByte()) & 0x7F) << 6;

            if ((byteValue & 0x80) != 0)
            {
                value |= ((byteValue = ReadByte()) & 0x7F) << 13;

                if ((byteValue & 0x80) != 0)
                {
                    value |= ((byteValue = ReadByte()) & 0x7F) << 20;

                    if ((byteValue & 0x80) != 0)
                    {
                        value |= ((byteValue = ReadByte()) & 0x7F) << 27;
                    }
                }
            }
        }

        return value;
    }

    public LogicLong ReadLong()
    {
        LogicLong longValue = new LogicLong();
        longValue.Decode(this);
        return longValue;
    }

    public long ReadVLong()
    {
        int high = ReadVInt();
        int low = ReadVInt();

        return ((long)high << 32) | (uint)low;
    }

    public int ReadBytesLength()
    {
        this.m_bitIdx = 0;
        return (this.m_buffer[this.m_offset++] << 24) |
               (this.m_buffer[this.m_offset++] << 16) |
               (this.m_buffer[this.m_offset++] << 8) |
               this.m_buffer[this.m_offset++];
    }

    public byte[]? ReadBytes(int length, int maxCapacity)
    {
        this.m_bitIdx = 0;

        if (length <= -1)
        {
            return null;
        }

        if (length <= maxCapacity)
        {
            byte[] array = new byte[length];
            System.Buffer.BlockCopy(this.m_buffer, this.m_offset, array, 0, length);
            this.m_offset += length;
            return array;
        }

        return null;
    }

    public string? ReadString(int maxCapacity = 9000000)
    {
        int length = this.ReadBytesLength();

        if (length <= -1)
        {
            return null;
        }
        else
        {
            if (length <= maxCapacity)
            {
                string value = Encoding.UTF8.GetString(this.m_buffer, this.m_offset, length);
                this.m_offset += length;
                return value;
            }

            return null;
        }
    }

    public string ReadStringReference(int maxCapacity)
    {
        int length = this.ReadBytesLength();

        if (length <= -1)
        {
            return string.Empty;
        }
        else
        {
            if (length <= maxCapacity)
            {
                string value = Encoding.UTF8.GetString(this.m_buffer, this.m_offset, length);
                this.m_offset += length;
                return value;
            }
        }

        return string.Empty;
    }

    public bool WriteBoolean(bool value)
    {
        if (this.m_bitIdx == 0)
        {
            this.EnsureCapacity(1);
            this.m_buffer[this.m_offset++] = 0;
        }

        if (value)
        {
            this.m_buffer[this.m_offset - 1] |= (byte)(1 << this.m_bitIdx);
        }

        this.m_bitIdx = (this.m_bitIdx + 1) & 7;
        return value;
    }

    public void WriteByte(byte value)
    {
        this.EnsureCapacity(1);

        this.m_bitIdx = 0;

        this.m_buffer[this.m_offset++] = value;
    }

    public void WriteShort(short value)
    {
        this.EnsureCapacity(2);

        this.m_bitIdx = 0;

        this.m_buffer[this.m_offset++] = (byte)(value >> 8);
        this.m_buffer[this.m_offset++] = (byte)value;
    }

    public void WriteInt(int value)
    {
        this.EnsureCapacity(4);

        this.m_bitIdx = 0;

        this.m_buffer[this.m_offset++] = (byte)(value >> 24);
        this.m_buffer[this.m_offset++] = (byte)(value >> 16);
        this.m_buffer[this.m_offset++] = (byte)(value >> 8);
        this.m_buffer[this.m_offset++] = (byte)value;
    }

    public void WriteVInt(int value)
    {
        EnsureCapacity(5);

        m_bitIdx = 0;

        switch (value)
        {
            case >= 0 and >= 64:
                {
                    if (value >= 0x2000)
                    {
                        if (value >= 0x100000)
                        {
                            if (value >= 0x8000000)
                            {
                                m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
                                m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                                m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
                                m_buffer[m_offset++] = (byte)(((value >> 20) & 0x7F) | 0x80);
                                m_buffer[m_offset++] = (byte)((value >> 27) & 0xF);
                            }
                            else
                            {
                                m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
                                m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                                m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
                                m_buffer[m_offset++] = (byte)((value >> 20) & 0x7F);
                            }
                        }
                        else
                        {
                            m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
                            m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)((value >> 13) & 0x7F);
                        }
                    }
                    else
                    {
                        m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x80);
                        m_buffer[m_offset++] = (byte)((value >> 6) & 0x7F);
                    }

                    break;
                }
            case >= 0:
                m_buffer[m_offset++] = (byte)(value & 0x3F);
                break;
            case <= -0x40 and <= -0x2000:
                {
                    if (value <= -0x100000)
                    {
                        if (value <= -0x8000000)
                        {
                            m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
                            m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)(((value >> 20) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)((value >> 27) & 0xF);
                        }
                        else
                        {
                            m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
                            m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)(((value >> 13) & 0x7F) | 0x80);
                            m_buffer[m_offset++] = (byte)((value >> 20) & 0x7F);
                        }
                    }
                    else
                    {
                        m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
                        m_buffer[m_offset++] = (byte)(((value >> 6) & 0x7F) | 0x80);
                        m_buffer[m_offset++] = (byte)((value >> 13) & 0x7F);
                    }

                    break;
                }
            case <= -0x40:
                m_buffer[m_offset++] = (byte)((value & 0x3F) | 0xC0);
                m_buffer[m_offset++] = (byte)((value >> 6) & 0x7F);
                break;
            default:
                m_buffer[m_offset++] = (byte)((value & 0x3F) | 0x40);
                break;
        }
    }

    public void WriteIntToByteArray(int value)
    {
        this.EnsureCapacity(4);
        this.m_bitIdx = 0;

        this.m_buffer[this.m_offset++] = (byte)(value >> 24);
        this.m_buffer[this.m_offset++] = (byte)(value >> 16);
        this.m_buffer[this.m_offset++] = (byte)(value >> 8);
        this.m_buffer[this.m_offset++] = (byte)value;
    }

    public void WriteBytes(byte[] value, int length)
    {
        if (value == null)
        {
            this.WriteIntToByteArray(-1);
        }
        else
        {
            this.EnsureCapacity(length + 4);
            this.WriteIntToByteArray(length);

            System.Buffer.BlockCopy(value, 0, this.m_buffer, this.m_offset, length);

            this.m_offset += length;
        }
    }

    public void WriteBytesWithoutLength(byte[] value, int length)
    {
        if (value != null)
        {
            this.EnsureCapacity(length);
            System.Buffer.BlockCopy(value, 0, this.m_buffer, this.m_offset, length);
            this.m_offset += length;
        }
    }

    public void WriteString(string value)
    {
        if (value == null)
        {
            this.WriteIntToByteArray(-1);
        }
        else
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            int length = bytes.Length;

            if (length <= 900000)
            {
                this.EnsureCapacity(length + 4);
                this.WriteIntToByteArray(length);

                System.Buffer.BlockCopy(bytes, 0, this.m_buffer, this.m_offset, length);

                this.m_offset += length;
            }
            else
            {
                this.WriteIntToByteArray(-1);
            }
        }
    }

    public void WriteStringReference(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        int length = bytes.Length;

        if (length <= 900000)
        {
            this.EnsureCapacity(length + 4);
            this.WriteIntToByteArray(length);

            System.Buffer.BlockCopy(bytes, 0, this.m_buffer, this.m_offset, length);

            this.m_offset += length;
        }
        else
        {
            this.WriteIntToByteArray(-1);
        }
    }

    public void SetByteArray(byte[] buffer, int length)
    {
        this.m_offset = 0;
        this.m_bitIdx = 0;
        this.m_buffer = buffer;
        this.m_length = length;
    }

    public void ResetOffset()
    {
        this.m_offset = 0;
        this.m_bitIdx = 0;
    }

    public void SetOffset(int offset)
    {
        this.m_offset = offset;
        this.m_bitIdx = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        int bufferLength = this.m_buffer.Length;

        if (this.m_offset + capacity > bufferLength)
        {
            byte[] tmpBuffer = new byte[this.m_buffer.Length + capacity + 100];
            System.Buffer.BlockCopy(this.m_buffer, 0, tmpBuffer, 0, bufferLength);
            this.m_buffer = tmpBuffer;
        }
    }
}
