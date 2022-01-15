using System;
using System.IO;
using System.Windows.Forms;
using UnityEngine;

namespace UniversalChams {
    class Main : MonoBehaviour {
        private readonly string bundlePath;
        public static Shader[] ChamShaders { get; private set; } = Array.Empty<Shader>();
        public static Material[] ChamMaterials { get; private set; } = Array.Empty<Material>();

        public Main() {
            bundlePath = UnityEngine.Application.streamingAssetsPath + "\\UniversalChams";

            if (!File.Exists(bundlePath)) {
                MessageBox.Show($"Couldn't find asset bundle at: {bundlePath}.", "Universal Chams");
                Loader.Unload();
                return;
            }
        }

        private void Start() {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            ChamShaders = bundle.LoadAllAssets<Shader>();
            ChamMaterials = bundle.LoadAllAssets<Material>();

            if (ChamShaders.Length == 0 && ChamMaterials.Length == 0) {
                MessageBox.Show("No cham shaders OR materials found.", "Universal Chams");
                Loader.Unload();
                return;
            }

            ChamPreview.Initialise();
            ChamPreview.PreviewMaterial(ChamMaterials[0], Color.clear);
        }

        private void Update() {
            ChamPreview.Update();
        }

        public static void DoChams(Color color, Material chamsMaterial) {
            // 'Entity' is specific to my game. You'll need to find an entity class in your own game that you want to apply chams to.
            Entity[] entities = FindObjectsOfType<Entity>();

            ChamPreview.PreviewMaterial(chamsMaterial, color);
            ApplyChams(entities, color, chamsMaterial);
        }

        public static void ApplyChamsSingle(GameObject gameObject, Color color, Material chamsMaterial) {
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>()) {
                renderer.material = chamsMaterial;

                if (color != Color.clear) {
                    // Not every shader supports this property, but it won't throw an exception if it doesn't.
                    renderer.material.SetColor("_Color", color);
                }
            }
        }

        private static void ApplyChams(Entity[] entities/* Change 'Entity[]' to YourEntityClass[] */, Color color, Material chamsMaterial) {
            foreach (Entity entity in entities) {
                foreach (Renderer renderer in entity.GetComponentsInChildren<Renderer>()) {
                    renderer.material = chamsMaterial;

                    if (color != Color.clear) {
                        renderer.material.SetColor("_Color", color);
                    }
                }
            }
        }

        #region Cham Examples
        [Obsolete]
        private Material EXAMPLE_XrayMaterial() {
            // List of built in shaders here: https://github.com/TwoTailsGames/Unity-Built-in-Shaders
            // This is a basic shader that can be made visible through walls.
            Material xray = new Material(Shader.Find("Hidden/Internal-Colored")) {
                hideFlags = HideFlags.HideAndDontSave
            };

            xray.SetInt("_ZTest", 8); // https://docs.unity3d.com/Manual/SL-ZTest.html
            xray.SetColor("_Color", Color.green);

            return xray;
        }

        [Obsolete]
        private Material EXAMPLE_TransparentMagenta() {
            Material smooth = new Material(Shader.Find("Hidden/InternalErrorShader")) {
                hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy
            };

            // This shader has 0 properties.

            return smooth;
        }
        #endregion
    }
}
