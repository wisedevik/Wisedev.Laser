using Wisedev.Laser.Titan.CSV;

namespace Wisedev.Laser.Logic.Data.Laser;

public class Data
{
    private readonly int _globalId;

    protected CSVRow _row;
    protected readonly DataTable _table;

    public Data(CSVRow row, DataTable table)
    {
        _row = row;
        _table = table;

        _globalId = GlobalID.CreateGlobalID((int)table.GetTableIndex(), table.GetItemCount());
    }

    public virtual void CreateReferences()
    {

    }

    public void SetCSVRow(CSVRow row)
    {
        _row = row;
    }


    public DataType GetDataType()
    {
        return _table.GetTableIndex();
    }

    public int GetGlobalID()
    {
        return _globalId;
    }

    public int GetInstanceID()
    {
        return GlobalID.GetInstanceID(_globalId);
    }

    public int GetColumnIndex(string name)
    {
        int columnIndex = _row.GetColumnIndexByName(name);

        if (columnIndex == -1)
        {
            Console.WriteLine(string.Format("Unable to find column {0} from {1} ({2})", name, _row.GetName(), _table.GetTableName()));
        }

        return _row.GetColumnIndexByName(name);
    }

    public string GetDebuggerName()
    {
        return _row.GetName() + " (" + _table.GetTableName() + ")";
    }

    public bool GetBooleanValue(string columnName, int index)
    {
        return _row.GetBooleanValue(columnName, index);
    }

    public bool GetClampedBooleanValue(string columnName, int index)
    {
        return _row.GetClampedBooleanValue(columnName, index);
    }

    public int GetIntegerValue(string columnName, int index)
    {
        return _row.GetIntegerValue(columnName, index);
    }

    public int GetClampedIntegerValue(string columnName, int index)
    {
        return _row.GetClampedIntegerValue(columnName, index);
    }

    public int GetArraySize(string column)
    {
        return _row.GetArraySize(column);
    }

    public string GetValue(string columnName, int index)
    {
        return _row.GetValue(columnName, index);
    }

    public string GetClampedValue(string columnName, int index)
    {
        return _row.GetClampedValue(columnName, index);
    }

    public string GetName()
    {
        return _row.GetName();
    }
}
