using UnityEngine;

namespace SaiUtils.Extensions
{
    public static class GameObjectExtensions
    {
        public static void SetAllChildrenActive(this GameObject gameObject, bool active)
        {
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(active);
            }
        }

        /// <summary>
        ///  This method is used to get a component from a GameObject. If the component is not found, it will add the component to the GameObject.
        /// </summary>
        /// <typeparam name="T"> The Type of the component to get or add. </typeparam>
        /// <param name="gameObject">The GameObject to get or add the component to.</param>
        /// <returns></returns>
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();
            return component;
        }

        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

    }
}
