using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Popup.Components;

public interface IPopupable<T> : IComponent
{
	EventCallback<T?> OnReturn { get; set; }
}
