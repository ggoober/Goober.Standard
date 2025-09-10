namespace Goober.Base.Models;

public interface IPageModel
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
