using nobnak.Blending.Matrix;
using nobnak.Blending.Field;
using nobnak.Gist.Scoped;
using UnityEngine;

namespace nobnak.Blending {

    [ExecuteInEditMode]
    public class Blending : MonoBehaviour {
        public const string PROP_MAIN_TEX = "_MainTex";
        public const string PROP_MASK_TEX = "_MaskTex";
        public const string PROP_GRID_DENSITY = "_GridDensity";

        public const string PROP_WORLD_TO_SCREEN_MATRIX = "_WorldToScreenMatrix";
        public const string PROP_UV_TO_WORLD_MATRICES = "_UVToWorldMatrices";
        public const string PROP_EDGE_TO_LOCAL_UV_MATRICES = "_EdgeToLocalUVMatrices";
        public const string PROP_LOCAL_TO_WORLD_UV_MATRICES = "_LocalToWorldUVMatrices";

        public const string SHADER_BLENDING = "Hidden/Blending";

        [SerializeField]
        protected Data data = new Data();

        [SerializeField]
        protected Shader shader;

        [Range(1, 20)]
        [SerializeField]
        protected int gridDentisy = 5;
        [SerializeField]
        protected Texture[] maskTextureSelection;

        protected ScopedObject<Material> mat;
        protected UvToWorldMatrix worldMatrix;
        protected EdgeToLocalUvMatrix edgeMatrices;
        protected LocalToWorldUvMatrix uvMatrix;

        #region Unity
        void OnEnable() {
            if (shader == null)
                shader = Shader.Find(SHADER_BLENDING);
            mat = new Material(shader);
            worldMatrix = new UvToWorldMatrix();
            edgeMatrices = new EdgeToLocalUvMatrix();
            uvMatrix = new LocalToWorldUvMatrix();

            UpdateInputData();
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            using (new ScopedRenderTextureActivator (dst)) {
                if (data.MakeSureValidated()) {
                    UpdateInputData();
                }
                GL.Clear(true, true, Color.clear);

                mat.Data.shaderKeywords = null;
                if (data.OutputKeyword != KwOutput.None)
                    mat.Data.EnableKeyword(
                        System.Enum.GetName(typeof(KwOutput), 
                        data.OutputKeyword));
                if (data.WireframeKeyword != KwWireframe.None)
                    mat.Data.EnableKeyword(
                        System.Enum.GetName(typeof(KwWireframe),
                        data.WireframeKeyword));
                if (data.TextureBlendkeyword != KwTextureBlend.None)
                    mat.Data.EnableKeyword(
                        System.Enum.GetName(typeof(KwTextureBlend),
                        data.TextureBlendkeyword));

                var screens = data.Screens;
                var gridDensityAcrossScreens = gridDentisy * new Vector2(screens.x, screens.y);
                mat.Data.SetTexture(PROP_MAIN_TEX, src);
                mat.Data.SetTexture(PROP_MASK_TEX, 
                    (maskTextureSelection != null 
                    && data.MaskTextureIndex >= 0 
                    && data.MaskTextureIndex < maskTextureSelection.Length)
                        ? maskTextureSelection[data.MaskTextureIndex] : null);
                mat.Data.SetVector(PROP_GRID_DENSITY, gridDensityAcrossScreens);

                mat.Data.SetBuffer(PROP_UV_TO_WORLD_MATRICES, worldMatrix.Buffer);
                mat.Data.SetBuffer(PROP_EDGE_TO_LOCAL_UV_MATRICES, edgeMatrices.Buffer);
                mat.Data.SetBuffer(PROP_LOCAL_TO_WORLD_UV_MATRICES, uvMatrix.Buffer);
                mat.Data.SetMatrix(PROP_WORLD_TO_SCREEN_MATRIX, CreateWorldToScreenMatrix());

                var screen = data.Screens;
                var screenCount = screen.x * screen.y;
                mat.Data.SetPass(0);
                Graphics.DrawProcedural(MeshTopology.Triangles, 54, screenCount);
            }
        }

        private void UpdateInputData() {
            worldMatrix.Screens = data.Screens;
            worldMatrix.Pivots = data.Trapeziums;
            edgeMatrices.Edges = data.Edges;
            uvMatrix.Screens = data.Screens;
            uvMatrix.ViewportOffsets = data.ViewportOffsets;
        }

        void OnDisable() {
            if (mat != null)
                mat.Dispose();
            if (worldMatrix != null)
                worldMatrix.Dispose();
            if (edgeMatrices != null)
                edgeMatrices.Dispose();
            if (uvMatrix != null)
                uvMatrix.Dispose();
        }
        #endregion

        #region Prop
        public Data BlendingData { get { return data; } }
        #endregion

        private Matrix4x4 CreateWorldToScreenMatrix() {
            var screens = data.Screens;
            var m = Matrix4x4.zero;
            m[0] = 2f / screens.x; m[12] = -1f;
            m[5] = 2f / screens.y; m[13] = -1f;
            m[15] = 1f;
            return m;
        }
    }
}
