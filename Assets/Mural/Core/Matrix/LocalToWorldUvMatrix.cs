using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Matrix {
    public class LocalToWorldUvMatrix : ViewportMatrixBuffer {

        protected Int2 screens;
        protected Vector4[] viewportOffsets;

        #region Input
        public Int2 Screens {
            get { return screens; }
            set {
                invalid = true;
                screens = value;
            }
        }
        public Vector4[] ViewportOffsets {
            get { return viewportOffsets; }
            set {
                invalid = true;
                viewportOffsets = value;
            }
        }
        #endregion

        protected Matrix4x4 CreateMatrix(int x, int y, Vector2 dx, Vector4 offset) {
            return CreateMatrix(
                x * dx.x, y * dx.y, 
                dx.x * (1f + offset.z), dx.y * (1f + offset.w),
                offset.x, offset.y
                );
        }
        protected override void UpdateMatrix() {
            matrices.Clear();

            var dx = new Vector2(1f / screens.x, 1f / screens.y);
            for (var y = 0; y < screens.y; y++)
                for (var x = 0; x < screens.x; x++)
                    matrices.Add(CreateMatrix(x, y, dx, viewportOffsets[x + y * screens.x]));
        }

    }
}
