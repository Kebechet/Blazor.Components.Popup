using Blazor.Components.Popup.Enums;
using Blazor.Components.Popup.Extensions;
using Microsoft.AspNetCore.Components;
using System.Globalization;

namespace Blazor.Components.Popup.Components;

public partial class PopupContainer
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool CanBeClosedByOutsideClick { get; set; } = true;
    [Parameter] public string ClassesForPopupCover { get; set; } = string.Empty;
    [Parameter] public string ClassesForPopupContent { get; set; } = string.Empty;
    public EventCallback OnCoverClickHide { get; set; }

    public bool IsVisible { get; set; }

    private readonly bool _isContentCenteredDefault = true;
    private readonly double _opacityDefault = 0.8;
    private readonly double _blurDefault = 4;

    private bool _isContentCentered;
    private double _currentOpacity;
    private double _currentBlur;

    private readonly string _styleToCenterContent =
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
        $"background-color: rgba(0, 0, 0, {_currentOpacity.ToString(CultureInfo.InvariantCulture)});" +
        $"-webkit-backdrop-filter: blur({_currentBlur.ToString(CultureInfo.InvariantCulture)}px);" + //fix for iOS Safari
        $"backdrop-filter: blur({_currentBlur.ToString(CultureInfo.InvariantCulture)}px);" +
        (_isContentCentered
            ? _styleToCenterContent
            : string.Empty
        );

    protected override void OnInitialized()
    {
        _popupService.Initialize(this);
        SetDefaultValues();
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
            (_currentOpacity, _currentBlur) = modalType.Value.ToProperties();
        }
        else
        {
            SetDefaultValues();
        }

        IsVisible = true;
        InvokeAsync(StateHasChanged);
    }

    public void Hide()
    {
        IsVisible = false;
        InvokeAsync(StateHasChanged);
    }

    public async Task WaitForHide()
    {
        while (true)
        {
            if (!IsVisible)
            {
                break;
            }

            await Task.Delay(50);
        }
    }

    private void OnCoverClick()
    {
        if (CanBeClosedByOutsideClick)
        {
            Hide();
            OnCoverClickHide.InvokeAsync();
        }
    }

    private void SetDefaultValues()
    {
        _isContentCentered = _isContentCenteredDefault;
        _currentOpacity = _opacityDefault;
        _currentBlur = _blurDefault;
    }
}
