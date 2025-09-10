using System.Collections.Generic;

namespace Goober.Base.Models;

public class SortedPageModel : ISortedPageModel
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public IList<ISortModel> Sorts { get; set; } = new List<ISortModel>();
}
