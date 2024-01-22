using Blazor.Components.Popup.Components;
using Blazor.Components.Popup.Enums;
using Blazor.Components.Popup.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Popup.Services;

public class PopupService
{
    private PopupContainer? _popupContainer = null;
    private CancellationTokenSource _cancellationTokenSource = new();

    public void Initialize(PopupContainer popupContainer)
    {
        _popupContainer = popupContainer;
        _popupContainer.OnCoverClickHide = EventCallback.Factory.Create(
            popupContainer,
            OnCoverClickHide
        );
    }

    public void Show(IComponent componentToRender)
    {
        Show(componentToRender, true, null);
    }
    public void Show(IComponent componentToRender, bool isContentCentered, ModalType? modalType)
    {
        if (_popupContainer is null)
        {
            throw new InvalidOperationException("PopupContainer is not initialized");
        }

        _popupContainer.Show(isContentCentered, modalType);

        var renderFragment = componentToRender.CreateRenderFragmentFromInstance();
        _popupContainer.RenderPopupContent(renderFragment);
    }

    public async Task<T?> Show<T>(IPopupReturnable<T?> componentToRender)
    {
        return await Show(componentToRender, true, null);
    }
    public async Task<T?> Show<T>(IPopupReturnable<T?> componentToRender, bool isContentCentered, ModalType? modalType)
    {
        if (_popupContainer is null)
        {
            throw new InvalidOperationException("PopupContainer is not initialized");
        }

        _popupContainer.Show(isContentCentered, modalType);

        var renderFragment = componentToRender.CreateRenderFragmentFromInstance();
        _popupContainer.RenderPopupContent(renderFragment);

        _cancellationTokenSource = new();
        var (wrappedEventCallback, taskCompletionSource) = componentToRender.OnReturnValue.WrapEventCallback(_cancellationTokenSource.Token);
        componentToRender.OnReturnValue = wrappedEventCallback;

        T? returnValue;
        try
        {
            returnValue = await taskCompletionSource.Task;
        }
        catch (OperationCanceledException)
        {
            returnValue = default;
        }

        _popupContainer!.Hide();

        return returnValue;
    }

    public void Hide()
    {
        if (_popupContainer is null)
        {
            throw new InvalidOperationException("PopupContainer is not initialized");
        }

        _popupContainer.Hide();
    }

    private void OnCoverClickHide()
    {
        _cancellationTokenSource.Cancel();
    }
}
