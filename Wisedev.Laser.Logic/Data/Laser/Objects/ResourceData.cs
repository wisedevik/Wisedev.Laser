using Wisedev.Laser.Titan.CSV;
using Wisedev.Laser.Logic.Data.Laser;
namespace Wisedev.Laser.Logic.Data.Laser.Objects;

public class ResourceData : Data
{
    public ResourceData(CSVRow row, DataTable table) : base(row, table)
    {
    }

    public override void CreateReferences()
    {
        base.CreateReferences();
    }
}
