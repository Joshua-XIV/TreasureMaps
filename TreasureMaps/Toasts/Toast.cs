using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Text.SeStringHandling;
namespace TreasureMaps.Toasts;

public class Toast : IDisposable
{
    private string? _lastToast;
    internal Toast()
    {
        P.toastGui.Toast += this.OnToast;
        P.toastGui.QuestToast += this.OnQuestToast;
    }

    public void Dispose()
    {
        P.toastGui.Toast -= this.OnToast;
        P.toastGui.QuestToast -= this.OnQuestToast;
    }

    private bool AnyMatches(string text)
    {
        return C.patterns.Any(regex => regex.IsMatch(text));
    }

    private void OnToast(ref SeString message, ref ToastOptions options, ref bool isHandled)
    {
        this.DoFilter(message, ref isHandled);
    }

    private void OnQuestToast(ref SeString message, ref QuestToastOptions options, ref bool isHandled)
    {
        this.DoFilter(message, ref isHandled);
    }

    private void DoFilter(SeString message, ref bool isHandled)
    {
        _lastToast = message.TextValue;

        if (isHandled)
        {
            return;
        }

        if (this.AnyMatches(message.TextValue))
        {
            isHandled = true;
        }
    }

    public string? GetLastToast()
    {
        return _lastToast;
    }
}
