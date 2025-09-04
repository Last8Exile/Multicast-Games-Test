using Scripts.Unity.UI;

namespace Scripts.Unity.Systems
{
    public class MainMenuSystem : BaseMenuSystem<MainMenu>
    {
        protected override void OnAfterSetVisisble(bool isVisible)
        {
            if (isVisible)
                _menuInstance.Refresh();
        }
    }
}
