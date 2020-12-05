using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TurtleGraphics.BlazorCanvas
{
    // This class provides an example of how JavaScript functionality can be wrapped
    // in a .NET class for easy consumption. The associated JavaScript module is
    // loaded on demand when first needed.
    //
    // This class can be registered as scoped DI service and then injected into Blazor
    // components for use.

    public class TurtleJsInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private Turtle? _turtleInstance;

        public TurtleJsInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/TurtleGraphics.BlazorCanvas/turtleJsInterop.js").AsTask());
        }

        public async ValueTask Init(Turtle instance)
        {
            _turtleInstance = instance;
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("init", DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async ValueTask OnResize(int width, int height)
        {
            await _turtleInstance.OnResize(width, height);
        }

        [JSInvokable]
        public async ValueTask Step(float timeStamp)
        {
            await _turtleInstance.Step();
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}
