using Blazor.Components.Popup.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Components.Popup.Extensions;

public static class IServiceCollectionExtensions
{
	public static IServiceCollection AddPopupWrapperServices(this IServiceCollection services)
	{
		services.AddSingleton<PopupWrapperService>();
		return services;
	}
}
