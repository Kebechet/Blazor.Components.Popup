using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Reflection;

namespace Blazor.Components.Popup.Extensions;

public static class IComponentExtensions
{
	public static RenderFragment CreateRenderFragmentFromInstance(this IComponent instance, IComponent namespaceComponent)
	{
		int attributeNumber = 0;

		return builder =>
		{
			builder.OpenComponent(attributeNumber++, instance.GetType());

			var properties = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				if (property.PropertyType == typeof(EventCallback))
				{
					var originalCallback = (EventCallback)property.GetValue(instance)!;

					var wrappedCallback = EventCallback.Factory.Create(namespaceComponent, async arg =>
					{
						if (originalCallback.HasDelegate)
						{
							await originalCallback.InvokeAsync(arg);
						}
					});

					builder.AddAttribute(attributeNumber++, property.Name, wrappedCallback);
				}
				else
				{
					builder.AddAttribute(attributeNumber++, property.Name, property.GetValue(instance));
				}
			}

			builder.CloseComponent();
		};
	}

	//https://github.com/dotnet/aspnetcore/issues/51987
	public static (EventCallback<T> WrappedEventCallback, TaskCompletionSource<T> TaskCompletionSource) WrapEventCallback<T>(
		this EventCallback<T> originalCallback,
		IComponent namespaceComponent,
		CancellationToken cancellationToken = default)
	{
		var taskSource = new TaskCompletionSource<T>();
		cancellationToken.Register(() => taskSource.TrySetCanceled());

		var wrappedCallback = EventCallback.Factory.Create<T>(namespaceComponent, async arg =>
		{
			if (originalCallback.HasDelegate)
			{
				await originalCallback.InvokeAsync(arg);
			}
			taskSource.SetResult(arg);
		});

		return (wrappedCallback, taskSource);
	}
}
