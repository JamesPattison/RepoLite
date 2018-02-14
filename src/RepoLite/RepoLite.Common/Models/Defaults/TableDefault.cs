using System.Collections.Generic;

namespace RepoLite.Common.Models.Defaults
{
    public sealed class TableDefault
    {
        public string Name;
        public List<ColumnDefault> Columns = new List<ColumnDefault>();
    }
}
