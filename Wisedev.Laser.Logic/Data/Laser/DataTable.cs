using System.Reflection.Metadata.Ecma335;
using Wisedev.Laser.Logic.Data.Laser.Objects;
using Wisedev.Laser.Titan.CSV;
using Wisedev.Laser.Titan.Debug;

namespace Wisedev.Laser.Logic.Data.Laser;

public class DataTable
{
    private DataType _tableIndex;
    private string _tableName;
    private bool _loaded;

    protected CSVTable _table;
    protected List<Data> _items;

    public DataTable(CSVTable table, DataType idx)
    {
        _tableIndex = idx;
        _table = table;
        _items = new();

        LoadTable();
    }

    public void SetTable(CSVTable table)
    {
        _table = table;
    }

    public void LoadTable()
    {
        for (int i = 0, j = _table.GetRowCount(); i < j; i++)
        {
            AddItem(_table.GetRowAt(i));
        }
    }

    public void AddItem(CSVRow row)
    {
        _items.Add(CreateItem(row));
    }

    public Data CreateItem(CSVRow row)
    {
        Data data = null;

        switch (_tableIndex)
        {
            case DataType.Resource:
                data = new ResourceData(row, this);
                break;
            default:
                Debugger.Error($"Invalid data table id: {_tableIndex}");
                break;
        }

        return data;
    }

    public virtual void CreateReferences()
    {
        if (!_loaded)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].CreateReferences();
            }

            _loaded = true;
        }

    }

    public Data GetDataByName(string name, Data caller)
    {
        if (!string.IsNullOrEmpty(name))
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Data data = _items[i];

                if (data.GetName().Equals(name))
                {
                    return data;
                }
            }

            if (caller != null)
            {
                Console.WriteLine(string.Format("CSV row ({0}) has an invalid reference ({1})", caller.GetName(), name));
            }
        }

        return null;
    }

    public Data GetItemAt(int index)
    {
        return _items[index];
    }

    public Data GetItemById(int globalId)
    {
        int instanceId = GlobalID.GetInstanceID(globalId);

        if (instanceId < 0 || instanceId >= _items.Count)
        {
            Console.WriteLine("LogicDataTable::getItemById() - Instance id out of bounds! " + (instanceId + 1) + "/" + _items.Count);
            return null;
        }

        return _items[instanceId];
    }

    public int GetItemCount()
    {
        return _items.Count;
    }

    public DataType GetTableIndex()
    {
        return _tableIndex;
    }

    public string GetTableName()
    {
        return _tableName;
    }

    public void SetName(string name)
    {
        _tableName = name;
    }
}
