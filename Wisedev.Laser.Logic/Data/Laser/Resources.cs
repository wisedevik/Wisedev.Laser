using Wisedev.Laser.Titan.CSV;

namespace Wisedev.Laser.Logic.Data.Laser;

public class Resources
{
    private static string _globalPath = "Assets/csv_logic/";

    public static List<DataTableResource> CreateDataTableResourcesArray()
    {
        List<DataTableResource> arrayList = new List<DataTableResource>(DataTables.TABLE_COUNT)
        {
            new DataTableResource(_globalPath + "resources.csv", DataType.Resource)
        };

        return arrayList;
    }

    public static void Load(List<DataTableResource> resources, int idx, CSVNode node)
    {
        DataTableResource resource = resources[idx];

        switch (resource.GetTableType())
        {
            case 0:
                DataTables.InitDataTable(node, resource.GetTableIndex());
                break;
        }

        if (resources.Count - 1 == idx)
        {
            DataTables.CreateReferences();
        }
    }
}
