using Goober.Base.Enums;

namespace Goober.Base.Models;

public class SortModel : ISortModel
{
    public string FieldName { get; set; }
    public SortOrder SortOrder { get; set; }
}
