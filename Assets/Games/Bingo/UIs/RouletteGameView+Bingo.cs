using Luna;
using Luna.UI.Navigation;
using UnityEngine;
using USEN.Games.Common;

namespace USEN.Games.Roulette
{
    public partial class RouletteGameView
    {
        private void PopupConfirmView()
        {
            Navigator.ShowModal<PopupOptionsView2>(
                builder: (popup) =>
                {
                    popup.onOption1 = () => Navigator.Pop(RouletteData);
                    // popup.onOption2 = () => Navigator.PopUntil<BingoGameOverView, RouletteData>(RouletteData);
#if UNITY_ANDROID
                    // popup.onOption3 = () => Android.Back();
                    popup.onOption3 = () => Application.Quit();
#else
                    popup.onOption3 = () => Application.Quit();
#endif
                });
        }
    }
}