using Blazor.Components.Popup.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Components.Popup.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPopupServices(this IServiceCollection services)
    {
        services.AddSingleton<PopupService>();
        return services;
    }
}
