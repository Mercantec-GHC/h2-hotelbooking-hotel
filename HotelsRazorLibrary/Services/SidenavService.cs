using Microsoft.JSInterop;

namespace Microsoft.Extensions.DependencyInjection
{
    public class SidenavService
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public SidenavService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/HotelsRazorLibrary/sidenav.js").AsTask());
        }

        public async Task Toggle()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("toggleNav");
        }

        public async Task Open()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("openNav");
        }

        public async Task Close()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("closeNav");
        }
    }
}
