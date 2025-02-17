using Wisedev.Laser.Titan.Debug;
using Wisedev.Laser.Titan.Util;

namespace Wisedev.Laser.Titan.CSV;

public class CSVColumn
{
    private int _type;

    private ArrayList<string> _stringValues; // type: 0
    private ArrayList<int> _integerValues; // type: 1
    private ArrayList<byte> _booleanValues; // type: 2

    public CSVColumn(int type, int size)
    {
        _type = type;

        _stringValues = new ArrayList<string>();
        _integerValues = new ArrayList<int>();
        _booleanValues = new ArrayList<byte>();

        switch (type)
        {
            case 0:
                _stringValues.EnsureCapacity(size);
                break;
            case 1:
                _integerValues.EnsureCapacity(size);
                break;
            case 2:
                _booleanValues.EnsureCapacity(size);
                break;
            default:
                Debugger.Error($"Invalid CSVColumn type ({type})");
                break;
        }
    }

    public void AddStringValue(string value)
    {
        _stringValues.Add(value);
    }

    public void AddIntegerValue(int value)
    {
        _integerValues.Add(value);
    }

    public void AddBooleanValue(bool value)
    {
        _booleanValues.Add((byte) (value ? 1 : 0));
    }

    public void SetIntegerValue(int value, int idx)
    {
        _integerValues[idx] = value;
    }

    public void SetBooleanValue(byte value, int idx)
    {
        _booleanValues[idx] = value;
    }

    public void SetStringValue(string value, int idx)
    {
        _stringValues[idx] = value;
    }

    public string GetStringValue(int idx)
    {
        return _stringValues[idx];
    }

    public int GetIntegerValue(int idx)
    {
        return _integerValues[idx];
    }

    public bool GetBooleanValue(int index)
    {
        return _booleanValues[index] == 1;
    }

    public void AddEmptyValue()
    {
        switch (_type)
        {
            case 0:
                _stringValues.Add(string.Empty);
                break;
            case 1:
                _integerValues.Add(0x7FFFFFFF);
                break;
            case 2:
                _booleanValues.Add(0x2);
                break;
        }
    }

    public int GetArraySize(int startOffset, int endOffset)
    {
        switch (_type)
        {
            default:
                for (int i = endOffset - 1; i + 1 > startOffset; i--)
                {
                    if (_stringValues[i].Length > 0)
                    {
                        return i - startOffset + 1;
                    }
                }

                break;
            case 1:
                for (int i = endOffset - 1; i + 1 > startOffset; i--)
                {
                    if (_integerValues[i] != 0x7FFFFFFF)
                    {
                        return i - startOffset + 1;
                    }
                }

                break;

            case 2:
                for (int i = endOffset - 1; i + 1 > startOffset; i--)
                {
                    if (_booleanValues[i] != 0x2)
                    {
                        return i - startOffset + 1;
                    }
                }

                break;
        }

        return 0;
    }

    public int GetSize()
    {
        switch (_type)
        {
            case 0:
                return _stringValues.Count;
            case 1:
                return _integerValues.Count;
            case 2:
                return _booleanValues.Count;
            default:
                return 0;
        }
    }

    public int GetType()
    {
        return _type;
    }
}

public enum CSVCOlumnType
{
    STRING = 0,
    INT = 1,
    BOOL = 2
}
