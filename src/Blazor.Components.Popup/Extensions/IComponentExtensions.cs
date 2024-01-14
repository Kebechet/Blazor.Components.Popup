using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Blazor.Components.Popup.Extensions;

public static class IComponentExtensions
{
    public static RenderFragment CreateRenderFragmentFromInstance(this IComponent instance)
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

                    var wrappedCallback = new EventCallback(null, new Func<object, Task>(async (arg) =>
                    {
                        if (originalCallback.HasDelegate)
                        {
                            await originalCallback.InvokeAsync(arg);
                        }
                    }));

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
        CancellationToken cancellationToken = default)
    {
        var taskSource = new TaskCompletionSource<T>();
        cancellationToken.Register(() => taskSource.TrySetCanceled());

        var wrappedCallback = new EventCallback<T>(null, new Func<T, Task>(async (arg) =>
        {
            if (originalCallback.HasDelegate)
            {
                await originalCallback.InvokeAsync(arg);
            }
            taskSource.SetResult(arg);
        }));

        return (wrappedCallback, taskSource);
    }
}
