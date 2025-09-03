using Scripts.Unity.Tools;
using Scripts.Unity.UI;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public class MainMenuSystem : BaseMenuSystem<MainMenuSystem>
    {
        [SerializeField] private PrefabContainer<MainMenu> _menuContainer;
        protected override MonoBehaviour GetMenuInstance() => _menuContainer.Instance;
        protected override MonoBehaviour CreateMenuInstance() => _menuContainer.GetOrCreateInstance();
    }
}
