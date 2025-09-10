using System;
using Microsoft.Extensions.DependencyInjection;

namespace Goober.Standard.Tests.Tools
{
    public class TestScope: IDisposable
    {
        private readonly IServiceScope _serviceScope;


        public TestScope(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            (ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
