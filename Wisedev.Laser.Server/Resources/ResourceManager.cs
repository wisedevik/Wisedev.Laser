using Wisedev.Laser.Logic.Data.Laser;
using Wisedev.Laser.Titan.CSV;

namespace Wisedev.Laser.Server.Resources;

public static class ResourceManager
{
    public static void Init()
    {
        LoadResources();
    }

    private static void LoadResources()
    {
        DataTables.Init();
        List<DataTableResource> resources = Logic.Data.Laser.Resources.CreateDataTableResourcesArray();

        for (int i = 0; i < resources.Count; i++)
        {
            string fileName = resources[i].GetFileName();

            Logic.Data.Laser.Resources.Load(resources, i, new CSVNode(File.ReadAllLines(fileName), fileName));
        }
    }
}