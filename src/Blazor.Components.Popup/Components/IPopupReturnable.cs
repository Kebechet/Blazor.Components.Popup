using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Popup.Components;

public interface IPopupReturnable<T> : IComponent
{
    EventCallback<T?> OnReturnValue { get; set; }
}
