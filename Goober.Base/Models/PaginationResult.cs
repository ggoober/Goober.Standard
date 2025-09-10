using System.Collections.Generic;

namespace Goober.Base.Models;

public class PaginationResult<TModel>
{
    public List<TModel> PageItems { get; set; } = new ();
    public int TotalItemsCount { get; set; }
}
