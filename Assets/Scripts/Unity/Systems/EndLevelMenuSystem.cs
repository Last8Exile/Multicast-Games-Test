using Scripts.Unity.UI;

namespace Scripts.Unity.Systems
{
    public class EndLevelMenuSystem : BaseMenuSystem<EndLevelMenu>
    {
        protected override void OnAfterSetVisisble(bool isVisible)
        {
            if (isVisible)
                _menuInstance.Refresh();
        }
    }
}
