using UnityEngine;

namespace UniversalChams {
    static class ChamPreview {
        private static GameObject cameraHolder;
        public static Camera ChamPreviewCamera { get; private set; }
        private static GameObject chamPreviewSphere;
        public static RenderTexture cameraTexture;

        public static void Initialise() {
            cameraHolder = new GameObject("ChamPreviewCamHolder");

            cameraTexture = new RenderTexture(256, 256, 1, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);

            ChamPreviewCamera = cameraHolder.GetOrAddComponent<Camera>();
            ChamPreviewCamera.transform.position = new Vector3(0f, -1000f, 0f);
            ChamPreviewCamera.enabled = false;
            ChamPreviewCamera.targetTexture = cameraTexture;

            // Only render objects on layer 30.
            ChamPreviewCamera.cullingMask = 1 << 30; 

            chamPreviewSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            chamPreviewSphere.transform.position = new Vector3(0f, -1000f, 1.2f);
            chamPreviewSphere.layer = 30; // Put our sphere on layer 30 so that our camera can render ONLY this sphere.
        }
        
        public static void Update() {
            // Some games destroy old cameras or disable them when you switch scenes.
            if (!ChamPreviewCamera) {
                Initialise();
            }

            ChamPreviewCamera.enabled = true;
        }

        public static void PreviewMaterial(Material material, Color color) {
            Renderer renderer;

            if (chamPreviewSphere) {
                renderer = chamPreviewSphere.GetComponent<Renderer>();
                renderer.material = material;

                if (color != Color.clear) {
                    renderer.material.SetColor("_Color", color);
                }
            }
        }
    }
}
