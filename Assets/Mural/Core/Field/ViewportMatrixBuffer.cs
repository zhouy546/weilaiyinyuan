using UnityEngine;

namespace nobnak.Blending.Field {
    public abstract class ViewportMatrixBuffer : MatrixBuffer {

        protected Matrix4x4 CreateMatrix(float x, float y, float w, float h) {
            var m = Matrix4x4.zero;
            m[0] = w; m[12] = x;
            m[5] = h; m[13] = y;
            m[15] = 1f;
            return m;
        }
        protected Matrix4x4 CreateMatrix(float x, float y, float w, float h, 
            float offsetx, float offsety) {

            var m = Matrix4x4.zero;
            m[0] = w;   m[8] = offsetx;     m[12] = x;
            m[5] = h;   m[9] = offsety;     m[13] = y;
            m[15] = 1f;
            return m;
        }
    }
}
