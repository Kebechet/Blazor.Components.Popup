using Blazor.Components.Popup.Enums;
using Blazor.Components.Popup.Extensions;
using Microsoft.AspNetCore.Components;

namespace Blazor.Components.Popup.Components;

public partial class PopupWrapper
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool CanBeClosedByOutsideClick { get; set; } = true;
    [Parameter] public string ClassesForPopupCover { get; set; } = string.Empty;
    [Parameter] public string ClassesForPopupContent { get; set; } = string.Empty;
    public EventCallback OnCoverClickHide { get; set; }

    public bool IsVisible { get; set; }

    private bool _isContentCentered = true;
    private double _opacity = 0.8;
    private double _blur = 4;
    private string _styleToCenterContent =
        "justify-content: center;" +
        "align-items: center;";
    private string _style =>
        "z-index: 2000;" +
        "display: flex;" +
        "position: fixed;" +
        "top: 0;" +
        "left: 0;" +
        "height: 100%;" +
        "width: 100%;" +
        "padding: 10px;" +
        "pointer-events: all;" +
        $"background-color: rgba(0, 0, 0, {_opacity});" +
        $"backdrop-filter: blur({_blur}px);" +
        (_isContentCentered
            ? _styleToCenterContent
            : string.Empty
        );

    protected override void OnInitialized()
    {
        _popupWrapperService.Initialize(this);
    }

    public void RenderPopupContent(RenderFragment renderFragment)
    {
        ChildContent = renderFragment;
    }

    public void Show(bool isContentCentered, ModalType? modalType = null)
    {
        _isContentCentered = isContentCentered;

        if (modalType is not null)
        {
            (_opacity, _blur) = modalType.Value.ToProperties();
        }

        IsVisible = true;
        InvokeAsync(StateHasChanged);
    }

    public void Hide()
    {
        IsVisible = false;
        InvokeAsync(StateHasChanged);
    }

    private void OnCoverClick()
    {
        if (CanBeClosedByOutsideClick)
        {
            Hide();
            OnCoverClickHide.InvokeAsync();
        }
    }
}
