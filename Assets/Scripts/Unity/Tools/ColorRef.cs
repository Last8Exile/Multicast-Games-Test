using UnityEngine;

namespace Scripts.Unity
{
    [CreateAssetMenu(fileName = nameof(ColorRef), menuName = "Data/" + nameof(ColorRef))]
    public class ColorRef : ScriptableObject
    {
        public Color Color => _color;
        [SerializeField] private Color _color = Color.white;
    }
}
