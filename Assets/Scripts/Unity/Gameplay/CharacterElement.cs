using System.Buffers;

using TMPro;

using UnityEngine;

namespace Scripts.Unity.Gameplay
{
    public class CharacterElement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetCharacter(char character)
        {
            var arr = ArrayPool<char>.Shared.Rent(1);
            arr[0] = character;
            _text.SetCharArray(arr, 0, 1);
            ArrayPool<char>.Shared.Return(arr);
        }
    }
}
