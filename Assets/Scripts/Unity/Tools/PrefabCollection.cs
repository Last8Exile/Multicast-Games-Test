using System.Collections.Generic;

using UnityEngine;

namespace Scripts.Unity.Tools
{
    [CreateAssetMenu(fileName = nameof(PrefabCollection), menuName = "Data/" + nameof(PrefabCollection))]
    public class PrefabCollection : ScriptableObject
    {
        public IReadOnlyList<GameObject> Prefabs => _prefabs;
        [SerializeField] private List<GameObject> _prefabs;
    }
}
