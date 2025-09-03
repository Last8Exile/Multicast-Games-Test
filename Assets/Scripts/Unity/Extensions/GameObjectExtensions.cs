using UnityEngine;

namespace Scripts.Unity
{
    public static class GameObjectExtensions
    {
        public static T RemoveCloneFromName<T>(this T component) where T : Component
        {
            if (component)
                component.gameObject.RemoveCloneFromName();
            return component;
        }

        public static GameObject RemoveCloneFromName(this GameObject gameObject)
        {
            if (gameObject)
                gameObject.name = gameObject.name[..^7];
            return gameObject;
        }
    }
}
