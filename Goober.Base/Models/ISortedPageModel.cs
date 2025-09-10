using System.Collections.Generic;

namespace Goober.Base.Models;

public interface ISortedPageModel : IPageModel
{
    public IList<ISortModel> Sorts { get; set; }
}
