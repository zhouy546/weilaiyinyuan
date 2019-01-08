using UnityEngine;

namespace nobnak.Blending.Field {

    [System.Serializable]
    public class Data {
        [SerializeField]
        protected bool invalid;

        [SerializeField]
        protected Int2 screens;

        [SerializeField]
        protected KwOutput outputKeyword;
        [SerializeField]
        protected KwWireframe wireframeKeyword;
        [SerializeField]
        protected KwTextureBlend textureBlendkeyword;
        [SerializeField]
        protected int maskTextureIndex;

        [SerializeField]
        protected Trapezium[] trapeziums;

        [SerializeField]
        protected Vector4[] edges;

        [SerializeField]
        protected Vector4[] viewportOffsets;

        public bool MakeSureValidated() {
            if (invalid) {
                invalid = false;
                Validate();
                return true;
            }
            return false;
        }
        public void Invalidate() {
            invalid = true;
        }
        public void Reset() {
            screens = new Int2(1, 1);
            trapeziums = new Trapezium[1];
            edges = new Vector4[1];
            viewportOffsets = new Vector4[1];
        }

        protected void Validate() {
            screens.x = Mathf.Max(screens.x, 1);
            screens.y = Mathf.Max(screens.y, 1);
            var screenCount = screens.x * screens.y;
            System.Array.Resize(ref trapeziums, screenCount);
            System.Array.Resize(ref edges, screenCount);
            System.Array.Resize(ref viewportOffsets, screenCount);
        }

        #region Interface
        public Int2 Screens {
            get { return screens; }
            set {
                invalid = true;
                screens = value;
            }
        }

        public Trapezium[] Trapeziums {
            get { return trapeziums; }
            set {
                invalid = true;
                trapeziums = value;
            }
        }

        public Vector4[] Edges {
            get { return edges; }
            set {
                invalid = true;
                edges = value;
            }
        }
        public Vector4[] ViewportOffsets {
            get { return viewportOffsets; }
            set {
                invalid = true;
                viewportOffsets = value;
            }
        }

        public KwOutput OutputKeyword {
            get { return outputKeyword; }
            set { outputKeyword = value; }
        }
        public KwWireframe WireframeKeyword {
            get { return wireframeKeyword; }
            set { wireframeKeyword = value; }
        }
        public KwTextureBlend TextureBlendkeyword {
            get { return textureBlendkeyword; }
            set { textureBlendkeyword = value; }
        }
        public int MaskTextureIndex {
            get { return maskTextureIndex; }
            set { maskTextureIndex = value; }
        }
        #endregion
    }
}
