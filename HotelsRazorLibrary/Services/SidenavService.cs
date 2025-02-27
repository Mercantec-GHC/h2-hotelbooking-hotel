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
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("toggleNav");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task Open()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("openNav");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task Close()
        {
            try
            {
                var module = await moduleTask.Value;
                await module.InvokeVoidAsync("closeNav");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
