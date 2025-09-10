using Microsoft.Extensions.Configuration;

namespace Goober.Base.Services.Implementation
{
    public class Settings<T> : ISettings<T> where T : class, new()
    {
        public Settings(IConfiguration configuration)
        {
            Value = configuration.GetSection(typeof(T).Name)?.Get<T>() ?? new T();
        }
        public T Value { get; }
    }
}
