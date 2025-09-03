using Scripts.Unity.Tools;
using Scripts.Unity.UI;

using UnityEngine;

namespace Scripts.Unity.Systems
{
    public class EndLevelMenuSystem : BaseMenuSystem<EndLevelMenuSystem>
    {
        [SerializeField] private PrefabContainer<EndLevelMenu> _menuContainer;
        protected override MonoBehaviour GetMenuInstance() => _menuContainer.Instance;
        protected override MonoBehaviour CreateMenuInstance() => _menuContainer.GetOrCreateInstance();
    }
}
