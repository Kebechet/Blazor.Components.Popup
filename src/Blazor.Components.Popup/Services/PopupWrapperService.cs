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

		#region https://github.com/dotnet/aspnetcore/issues/51987
		var taskSource = new TaskCompletionSource<T?>();

		_cancellationTokenSource = new();
		_cancellationTokenSource.Token.Register(() =>
		{
			taskSource.TrySetCanceled();
		});

		componentToRender.OnReturn = EventCallback.Factory.Create<T?>(namespaceComponent, taskSource.SetResult);

		T? returnValue = default;
		try
		{
			returnValue = await taskSource.Task;
		}
		catch (OperationCanceledException)
		{
			returnValue = default;
		}
		#endregion

		_currentPopupWrapper.Hide();

		return returnValue;
	}

	private void OnCoverClickHide()
	{
		_cancellationTokenSource.Cancel();
	}
}
