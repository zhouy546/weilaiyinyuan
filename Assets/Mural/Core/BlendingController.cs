using nobnak.Blending.Control;
using nobnak.Blending.Field;
using nobnak.Blending.Geometry;
using nobnak.Gist;
using nobnak.Gist.InputDevice;
using nobnak.Gist.StateMachine;
using System.IO;
using UnityEngine;

namespace nobnak.Blending {

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(Blending))]
    public class BlendingController : MonoBehaviour {
        public enum TrackingStateEnum { Untrack = 0, Track }
        public enum TargetModeEnum { None = 0, Corner, Edge, Blend }
        public enum GUIModeEnum { Hidden = 0, Show }

        public const float REGION_GAP = 10f;

        [SerializeField]
        protected Config config;

        protected Blending blending;
        protected Camera targetCamera;

        protected FSM<TargetModeEnum> fsmTrackerTargetMode;
        protected FSM<KwOutput> fsmBlendingOutputMode;

        protected CornerControl cornerControl;
        protected EdgeControl edgeControl;
        protected BlendControl blendControl;

        protected MousePosition mouseCurr;
        protected MouseTracker mouseTracker = new MouseTracker();

        protected Reactive<TargetModeEnum> trackerMode = TargetModeEnum.None;
        protected Reactive<int> trackerIndex = 0;

        protected FSM<GUIModeEnum> fsmGUIMode;
        protected Reactive<int> screenColumnCount = 1;
        protected Reactive<int> screenRowCount = 1;
        protected Reactive<string> screenColumnCountText = "1";
        protected Reactive<string> screenRowCountText = "1";

        protected Reactive<KwOutput> outputMode = KwOutput.None;
        protected Reactive<int> outputIndex = 0;
        protected Reactive<KwWireframe> wireframeMode = KwWireframe.None;
        protected Reactive<int> wireframeIndex = 0;
        protected Reactive<KwTextureBlend> textureBlendMode = KwTextureBlend.None;
        protected Reactive<int> textureBlendIndex = 0;
        protected Reactive<string> maskTextureIndexText = "0";
        protected Reactive<int> maskTextureIndex = 0;

        protected Rect window = new Rect(10, 10, 300, 100);

        #region Unity
        void OnEnable() {
            blending = GetComponent<Blending>();
            targetCamera = GetComponent<Camera>();

            blending.enabled = true;
            mouseTracker.Clear();

            mouseCurr = new MousePosition(targetCamera, blending.BlendingData);
            fsmTrackerTargetMode = new FSM<TargetModeEnum>(this);
            fsmGUIMode = new FSM<GUIModeEnum>(this);

            cornerControl = new CornerControl(this, blending, mouseTracker, mouseCurr);
            edgeControl = new EdgeControl(this, blending, mouseTracker, mouseCurr);
            blendControl = new BlendControl(this, blending, mouseTracker, mouseCurr);

            #region FSM Setup
            fsmTrackerTargetMode.State(TargetModeEnum.None);
            fsmTrackerTargetMode.State(TargetModeEnum.Corner)
                .Enter(fsm => cornerControl.Activity = true)
                .Update(fsm => cornerControl.Update())
                .Exit(fsm => cornerControl.Activity = false);
            fsmTrackerTargetMode.State(TargetModeEnum.Edge)
                .Enter(fsm => edgeControl.Activity = true)
                .Update(fsm => edgeControl.Update())
                .Exit(fsm => edgeControl.Activity = false);
            fsmTrackerTargetMode.State(TargetModeEnum.Blend)
                .Enter(fsm => blendControl.Activity = true)
                .Update(fsm => blendControl.Update())
                .Exit(fsm => blendControl.Activity = false);
            fsmTrackerTargetMode.Init();

            fsmGUIMode.State(GUIModeEnum.Hidden).Update(fsm => {
                if (Input.GetKeyDown(config.guiModeToggleKey))
                    fsm.Goto(GUIModeEnum.Show);
            });
            fsmGUIMode.State(GUIModeEnum.Show).Update(fsm=> {
                if (Input.GetKeyDown(config.guiModeToggleKey))
                    fsm.Goto(GUIModeEnum.Hidden);
            });
            fsmGUIMode.Init();
            #endregion

            #region Reactive setup
            trackerMode.Changed += (r => {
                fsmTrackerTargetMode.Goto(r.Value);
                trackerIndex.Value = EnumOperator<TargetModeEnum>.FindIndex(r.Value);
            });
            trackerIndex.Changed += (r => {
                trackerMode.Value = EnumOperator<TargetModeEnum>.ValueAt(r.Value);
            });

            screenColumnCount.Changed += (r => {
                UpdateScreenSetup(r.Value, screenRowCount.Value);
                screenColumnCountText.Value = r.Value.ToString();
            });
            screenRowCount.Changed += (r => {
                UpdateScreenSetup(screenColumnCount.Value, r.Value);
                screenRowCountText.Value = r.Value.ToString();
            });
            screenColumnCountText.Changed += (r => {
                int nextValue;
                if (int.TryParse(r.Value, out nextValue))
                    screenColumnCount.Value = Mathf.Max(1, nextValue);
            });
            screenRowCountText.Changed += (r => {
                int nextValue;
                if (int.TryParse(r.Value, out nextValue))
                    screenRowCount.Value = Mathf.Max(1, nextValue);
            });

            outputMode.Changed += (r => {
                blending.BlendingData.OutputKeyword = r.Value;
                outputIndex.Value = EnumOperator<KwOutput>.FindIndex(r.Value);
            });
            outputIndex.Changed += (r => {
                outputMode.Value = EnumOperator<KwOutput>.ValueAt(r.Value);
            });

            wireframeMode.Changed += (r => {
                blending.BlendingData.WireframeKeyword = r.Value;
                wireframeIndex.Value = EnumOperator<KwWireframe>.FindIndex(r.Value);
            });
            wireframeIndex.Changed += (r => {
                wireframeMode.Value = EnumOperator<KwWireframe>.ValueAt(r.Value);
            });

            textureBlendMode.Changed += (r => {
                blending.BlendingData.TextureBlendkeyword = r.Value;
                textureBlendIndex.Value = EnumOperator<KwTextureBlend>.FindIndex(r.Value);
            });
            textureBlendIndex.Changed += (r =>
                textureBlendMode.Value = EnumOperator<KwTextureBlend>.ValueAt(r.Value));

            maskTextureIndex.Changed += (r => {
                blending.BlendingData.MaskTextureIndex = r.Value;
                maskTextureIndexText.Value = r.Value.ToString();
            });
            maskTextureIndexText.Changed += (r => {
                int nextValue;
                if(int.TryParse(r.Value, out nextValue))
                    maskTextureIndex.Value = nextValue;
            });
            #endregion

            Load();
            UpdateScreenSetup(screenColumnCount.Value, screenRowCount.Value);
        }
        void Update() {
            mouseTracker.Update();
        }
        void OnGUI() {
            if (fsmGUIMode.Current == GUIModeEnum.Show)
                window = GUILayout.Window(GetInstanceID(), window, Window, name);
        }
        #endregion

        #region UI
        public bool GUIIsVisible { get { return fsmGUIMode.Current == GUIModeEnum.Show; } }
        public bool IsOverGUI {
            get {
                if (!GUIIsVisible)
                    return false;
                var screenPos = Input.mousePosition;
                screenPos.y = Screen.height - screenPos.y;
                return window.Contains(screenPos);
            }
        }

        protected void Window(int id) {
            using (new GUILayout.VerticalScope()) {

                GUILayout.Label("Screen setup");
                using (new GUILayout.HorizontalScope()) {
                    screenColumnCountText.Value = GUILayout.TextField(screenColumnCountText);
                    GUILayout.Space(10f);
                    screenRowCountText.Value = GUILayout.TextField(screenRowCountText);
                }

                GUILayout.Space(REGION_GAP);
                GUILayout.Label("Handle mode");
                trackerIndex.Value = GUILayout.SelectionGrid(trackerIndex, EnumOperator<TargetModeEnum>.NAMES, 2);

                GUILayout.Space(REGION_GAP);
                GUILayout.Label("Output mode");
                outputIndex.Value = GUILayout.SelectionGrid(outputIndex, EnumOperator<KwOutput>.NAMES, 2);

                GUILayout.Space(REGION_GAP);
                GUILayout.Label("Wireframe mode");
                wireframeIndex.Value = GUILayout.SelectionGrid(wireframeIndex, EnumOperator<KwWireframe>.NAMES, 2);

                GUILayout.Space(REGION_GAP);
                GUILayout.Label("Texture blend mode");
                textureBlendIndex.Value = GUILayout.SelectionGrid(textureBlendIndex, EnumOperator<KwTextureBlend>.NAMES, 2);
                using (new GUILayout.HorizontalScope()) {
                    GUILayout.Label("Mask texture index");
                    maskTextureIndexText.Value = GUILayout.TextField(maskTextureIndexText);
                }

                GUILayout.Space(REGION_GAP);
                GUILayout.Label("Operations");
                GUILayout.Label(string.Format("Config:{0}", config.configFile));
                using (new GUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Save"))
                        Save();
                    if (GUILayout.Button("Load"))
                        Load();
                    if (GUILayout.Button("Reset"))
                        Reset();
                }

            }
            UnityEngine.GUI.DragWindow();
        }
        #endregion

        #region Config File
        protected void Load() {
            var data = blending.BlendingData;
            config.Load(data);
            data.Invalidate();
            LoadScreenSetupFromBlendingData();
            LoadOutputSetupFromData(data);
        }
        protected void Save() {
            config.Save(blending.BlendingData);
        }
        #endregion

        protected void UpdateScreenSetup(int x, int y) {
            var data = blending.BlendingData;
            data.Screens = new Int2(x, y);
            data.Invalidate();
        }
        protected void LoadScreenSetupFromBlendingData() {
            var screens = blending.BlendingData.Screens;
            screenColumnCount.Value = screens.x;
            screenRowCount.Value = screens.y;
        }

        protected void LoadOutputSetupFromData(Data data) {
            wireframeMode.Value = data.WireframeKeyword;
            outputMode.Value = data.OutputKeyword;
            textureBlendMode.Value = data.TextureBlendkeyword;
            maskTextureIndex.Value = data.MaskTextureIndex;
        }

        protected void Reset() {
            var data = blending.BlendingData;
            var screens = data.Screens;
            data.Reset();
            data.Screens = screens;
            data.Invalidate();
        }

        public class MousePosition {
            public Vector2 prevMouseUv;
            public Vector2 currMouseUv;
            public Vector2 duv;

            public Int2 selectedScreen;
            public int selectedVertex;
            public int selectedEdge;

            public Camera TargetCamera { get; protected set; }
            public Data BlendingData { get; protected set; }

            public MousePosition(Camera targetCamera, Data data) {
                this.TargetCamera = targetCamera;
                this.BlendingData = data;
            }

            public bool TryInitVertexMode() {
                var mouseUv = TargetCamera.GetMouseUvPosition();
                var result = TryFindNearestVertex(mouseUv);
                if (result)
                    InitMouseUvPosition(mouseUv);
                return result;
            }
            public bool TryInitEdgeMode() {
                var mouseUv = TargetCamera.GetMouseUvPosition();
                var result = TryFindNearestEdge(mouseUv);
                if (result)
                    InitMouseUvPosition(mouseUv);
                return result;
            }
            public void Update() {
                var mouseUv = TargetCamera.GetMouseUvPosition();
                UpdateMouseUvPosition(mouseUv);
            }
            public Vector2 WorldDuv {
                get {
                    var screens = BlendingData.Screens;
                    return new Vector2(screens.x * duv.x, screens.y * duv.y);
                }
            }

            public bool TryFindNearestVertex(Vector2 mouseUv) {
                return ScreenSelector.TryFindNearestVertex(mouseUv, BlendingData.Screens, BlendingData.Trapeziums,
                        out selectedScreen, out selectedVertex);
            }
            public bool TryFindNearestEdge(Vector2 mouseUv) {
                return ScreenSelector.TryFindNearestEdge(mouseUv, BlendingData.Screens, BlendingData.Trapeziums,
                        out selectedScreen, out selectedEdge);
                }

            public void InitMouseUvPosition(Vector2 mouseUv) {
                prevMouseUv = currMouseUv = mouseUv;
                duv = Vector2.zero;
            }
            public void UpdateMouseUvPosition(Vector2 mouseUv) {
                duv = mouseUv - prevMouseUv;
                prevMouseUv = mouseUv;
            }
        }
    }

    [System.Serializable]
    public class Config {
        public string configFile = @"%USERPROFILE%\EdgeBlending_Config.json";
        public KeyCode guiModeToggleKey = KeyCode.B;

        public string ExpandConfigFile {
            get { return System.Environment.ExpandEnvironmentVariables(configFile); }
        }

        public void Save(Data data) {
            try {
                File.WriteAllText(ExpandConfigFile, JsonUtility.ToJson(data));
            } catch(System.Exception e) {
                Debug.LogWarning(e);
            }
        }
        public void Load(Data data) {
            try {
                if (File.Exists(ExpandConfigFile))
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(ExpandConfigFile), data);
            } catch(System.Exception e) {
                Debug.LogWarning(e);
            }
        }
    }

    public static class CameraExtension {
        public static Vector2 ScreenPositionToUVPosition(this Camera targetCamera, Vector3 screenPosition) {
            return (Vector2)targetCamera.ScreenToViewportPoint(screenPosition);
        }
        public static Vector2 GetMouseUvPosition(this Camera targetCamera) {
            return targetCamera.ScreenPositionToUVPosition(Input.mousePosition);
        }
    }
}
