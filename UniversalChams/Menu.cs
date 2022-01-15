using UnityEngine;

namespace UniversalChams {
    class Menu : MonoBehaviour {
        private readonly Rect watermarkPosition = new Rect(5f, 5f, 400f, 50f);

        private Rect windowRect = new Rect(5f, 5f, 300f, 100f);
        private Rect colorRect = new Rect(316f, 110f, 150f, 75f);

        private const int mainWindowID = 5555;
        private const int colorWindowID = 5556;

        private bool drawMenu;
        private bool drawWatermark = true;

        private int assetIndex = 0;
        private int assetLength;

        /// <summary>
        /// Keybind to apply chams via a raycast.
        /// </summary>
        private readonly KeyCode raycastChams = KeyCode.Backslash;
        /// <summary>
        /// Keybind to open / close the menu.
        /// </summary>
        private readonly KeyCode menuToggle = KeyCode.Insert;

        private Color selectedColor = Color.clear;

        private Material CreateMaterial() {
            // Start by indexing into materials.
            if (assetIndex < Main.ChamMaterials.Length) {
                return Main.ChamMaterials[assetIndex];
            }

            // Fix our index, we want to start from the beginning of the shader array.
            int realIndex = assetIndex - Main.ChamMaterials.Length;

            // Then index into shaders.
            return new Material(Main.ChamShaders[realIndex]) {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        private void Update() {
            if (assetLength == 0) {
                assetLength = Main.ChamMaterials.Length + Main.ChamShaders.Length;
            }

            // Don't check for input if no keys are being pressed.
            if (!Input.anyKey || !Input.anyKeyDown) {
                return;
            }

            if (Input.GetKeyDown(menuToggle)) {
                drawMenu = !drawMenu;

                // Don't draw the watermark after the user has opened the menu once.
                drawWatermark = false;
            }

            // Shoot a ray and apply chams to the first object we hit.
            if (Input.GetKeyDown(raycastChams)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit)) {
                    if (hit.collider != null) {
                        Main.ApplyChamsSingle(hit.collider.gameObject, selectedColor, CreateMaterial());
                    }
                }
            }
        }

        private void OnGUI() {
            if (drawWatermark && !drawMenu) {
                GUI.Label(watermarkPosition, "<color=yellow>[Universal Chams] Press INSERT to open/close menu.</color>");
            }

            if (drawMenu) {
                windowRect = GUILayout.Window(mainWindowID, windowRect, MainWindow, "Universal Chams");
                colorRect = new Rect(windowRect.x + windowRect.width + 5f, windowRect.y, 150f, 75f);
                GUILayout.Window(colorWindowID, colorRect, ColorWindow, "Color Picker");
            }
        }

        private void MainWindow(int id) {
            GUILayout.BeginVertical(GUI.skin.box); {
                GUILayout.BeginHorizontal(); {
                    GUILayout.Label($"Loaded shaders: {Main.ChamShaders.Length}");
                    GUILayout.Label($"Loaded materials: {Main.ChamMaterials.Length}");
                }
                GUILayout.EndHorizontal();

                GUILayout.Label($"<color=yellow>Press '{raycastChams}' to apply chams to an object you're looking at.</color>");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Options", GUI.skin.box); {
                GUILayout.Space(20f);

                GUILayout.Label($"Current asset index: {assetIndex}");

                if (GUILayout.Button("+")) {
                    if (assetIndex < assetLength - 1) {
                        assetIndex++;
                        ChamPreview.PreviewMaterial(CreateMaterial(), selectedColor);
                    }
                }
                if (GUILayout.Button("-")) {
                    if (assetIndex > 0) {
                        assetIndex--;
                        ChamPreview.PreviewMaterial(CreateMaterial(), selectedColor);
                    }
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Preview", GUI.skin.box); {
                GUILayout.Space(20f);
                GUILayout.Box(ChamPreview.cameraTexture);

                if (GUILayout.Button("Apply")) {
                    Main.DoChams(selectedColor, CreateMaterial());
                }
            }
            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        private void ColorWindow(int id) {
            GUILayout.BeginVertical(GUI.skin.box);
            ColoredButton("Black", Color.black);
            ColoredButton("Red", Color.red);
            ColoredButton("Cyan", Color.cyan);
            ColoredButton("Blue", Color.blue);
            ColoredButton("Green", Color.green);
            ColoredButton("Magenta", Color.magenta);
            ColoredButton("Yellow", Color.yellow);
            ColoredButton("White", Color.white);
            ColoredButton("Gray", Color.gray);
            ColoredButton("Reset", Color.clear);
            GUILayout.EndVertical();
        }

        private void ColoredButton(string text, Color color) {
            Color oldColor = GUI.backgroundColor;

            if (color != Color.clear) {
                GUI.backgroundColor = color;
            }
            
            if (GUILayout.Button(text)) {
                selectedColor = color;
                ChamPreview.PreviewMaterial(CreateMaterial(), color);
            }

            GUI.backgroundColor = oldColor;
        }
    }
}
