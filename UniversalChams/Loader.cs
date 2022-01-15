using UnityEngine;

namespace UniversalChams {
    public class Loader {
        static GameObject gameObject;

        public static void Load() {
            gameObject = new GameObject();
            gameObject.AddComponent<Main>();
            gameObject.AddComponent<Menu>();

            Object.DontDestroyOnLoad(gameObject);
        }

        public static void Unload() {
            Object.Destroy(gameObject);
        }
    }
}
