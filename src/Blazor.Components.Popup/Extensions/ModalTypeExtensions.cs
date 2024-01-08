using Blazor.Components.Popup.Enums;

namespace Blazor.Components.Popup.Extensions;

public static class ModalTypeExtensions
{
    public static (double opacity, double blur) ToProperties(this ModalType modalType)
    {
        return modalType switch
        {
            ModalType.None => (0, 0),
            ModalType.Opacity10 => (0.1, 0),
            ModalType.Opacity20 => (0.2, 0),
            ModalType.Opacity50 => (0.5, 0),
            ModalType.Opacity80 => (0.8, 0),
            ModalType.Opacity100 => (1.0, 0),
            ModalType.Opacity10WithBlur => (0.1, 4),
            ModalType.Opacity20WithBlur => (0.2, 4),
            ModalType.Opacity50WithBlur => (0.5, 4),
            ModalType.Opacity80WithBlur => (0.8, 4),
            ModalType.Opacity100WithBlur => (1.0, 4),
            _ => throw new NotImplementedException()
        };
    }
}
