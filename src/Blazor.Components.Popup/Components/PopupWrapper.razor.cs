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

	protected override void OnInitialized()
	{
		_popupWrapperService.Initialize(this);
	}

	public void RenderPopupContent(RenderFragment renderFragment)
	{
		ChildContent = renderFragment;
		InvokeAsync(StateHasChanged);
	}

	public void Show()
	{
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
