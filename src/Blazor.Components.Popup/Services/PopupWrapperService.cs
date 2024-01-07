using Blazor.Components.Popup.Components;
using Blazor.Components.Popup.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Popup.Services;

public class PopupWrapperService
{
	private PopupWrapper? _currentPopupWrapper = null;
	private CancellationTokenSource _cancellationTokenSource = new();

	public void Initialize(PopupWrapper popupWrapper)
	{
		_currentPopupWrapper = popupWrapper;
		_currentPopupWrapper.OnCoverClickHide = EventCallback.Factory.Create(
			popupWrapper,
			OnCoverClickHide
		);
	}

	public async Task<T?> Show<T>(IPopupable<T?> componentToRender, IComponent namespaceComponent)
	{
		if (_currentPopupWrapper is null)
		{
			throw new InvalidOperationException("PopupWrapper is not initialized");
		}

		_currentPopupWrapper.Show();

		var renderFragment = componentToRender.CreateRenderFragmentFromInstance(namespaceComponent);
		_currentPopupWrapper.RenderPopupContent(renderFragment);

		_cancellationTokenSource = new();
        var (wrappedEventCallback, taskCompletionSource) = componentToRender.OnReturn.WrapEventCallback(namespaceComponent, _cancellationTokenSource.Token);
        componentToRender.OnReturn = wrappedEventCallback;

        T? returnValue;
        try
        {
            returnValue = await taskCompletionSource.Task;
        }
        catch (OperationCanceledException)
        {
            returnValue = default;
        }

		_currentPopupWrapper!.Hide();

		return returnValue;
	}

	private void OnCoverClickHide()
	{
		_cancellationTokenSource.Cancel();
	}
}
