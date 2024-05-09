/// <summary>
///  Lavet af Jozsef
/// </summary>
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace ElevPortalen.Pages
{
    public class CustomErrorBoundary : ErrorBoundary
    {
        [Inject]
        private IWebHostEnvironment? Envirounment { get; set; }

        protected override Task OnErrorAsync(Exception ex)
        {
            if (Envirounment.IsDevelopment())
                return base.OnErrorAsync(ex);
            else
                return Task.CompletedTask;
        }
    }
}
