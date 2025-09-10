namespace Goober.Base.Services
{
    public interface ISettings<out T> where T : class, new()
    {
        T Value { get; }
    }
}