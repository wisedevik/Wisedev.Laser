using Wisedev.Laser.Logic.Data.Laser.Objects;
using Wisedev.Laser.Titan.CSV;

namespace Wisedev.Laser.Logic.Data.Laser;

public class DataTables
{
    private static DataTable[] _tables;

    public const int TABLE_COUNT = 61; // NEED: enter your value

    public static void Init()
    {
        _tables = new DataTable[TABLE_COUNT];
    }

    public static void InitDataTable(CSVNode node, DataType index)
    {
        if (_tables[(int)index] != null)
        {
            _tables[(int)index].SetTable(node.GetTable());
        }
        else
        {
            _tables[(int)index] = new DataTable(node.GetTable(), index);
        }
    }

    public static void CreateReferences()
    {
        for (int i = 0; i < _tables.Length; i++)
        {
            if (_tables[i] != null)
                _tables[i].CreateReferences();
        }
    }

    public int GetTableCount()
    {
        return TABLE_COUNT;
    }

    public static DataTable GetTable(DataType tableIndex)
    {
        return _tables[(int)tableIndex];
    }

    public static Data GetDataById(int globalId)
    {
        int tableIndex = GlobalID.GetClassID(globalId) - 1;

        if (tableIndex >= 0 && tableIndex < TABLE_COUNT && _tables[tableIndex] != null)
        {
            return _tables[tableIndex].GetItemById(globalId);
        }

        return null;
    }

    public static Data? GetDataById(int globalId, DataType dataType)
    {
        Data data = GetDataById(globalId);

        if (data.GetDataType() != dataType)
            return null;

        return data;
    }

    public static ResourceData GetResourceByName(string name, Data data = null)
    {
        return (ResourceData)_tables[(int)DataType.Resource].GetDataByName(name, data);
    }

}
