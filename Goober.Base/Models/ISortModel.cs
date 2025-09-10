using Goober.Base.Enums;

namespace Goober.Base.Models;

public interface ISortModel
{
    public string FieldName { get; set; }
    public SortOrder SortOrder { get; set; }
}
