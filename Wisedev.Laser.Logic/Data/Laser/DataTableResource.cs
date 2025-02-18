namespace Wisedev.Laser.Logic.Data.Laser;

public class DataTableResource
{
    private string _fileName;

    private DataType _tableIndex;
    private int _type;

    public DataTableResource(string fileName, DataType tableIndex, int type = 0)
    {
        _fileName = fileName;
        _tableIndex = tableIndex;
        _type = type;
    }

    public void Destruct()
    {
        _fileName = null;
        _tableIndex = 0;
        _type = 0;
    }

    public string GetFileName()
    {
        return _fileName;
    }

    public DataType GetTableIndex()
    {
        return _tableIndex;
    }

    public int GetTableType()
    {
        return _type;
    }
}