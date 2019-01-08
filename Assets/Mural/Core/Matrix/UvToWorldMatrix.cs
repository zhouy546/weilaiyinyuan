using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Matrix {
    public class UvToWorldMatrix : MatrixBuffer {

        protected Int2 screens;
        protected Trapezium[] trapeziums;

        #region Input
        public Int2 Screens {
            get { return screens; }
            set {
                invalid = true;
                screens = value;
            }
        }
        public Trapezium[] Pivots {
            get { return trapeziums; }
            set {
                invalid = true;
                trapeziums = value;
            }
        }
        #endregion

        protected override void UpdateMatrix() {
            matrices.Clear();

            var dscreen = new Vector2(2f / screens.x, 2f / screens.y);
            
            for (var y = 0; y < screens.y; y++) {
                for (var x = 0; x < screens.x; x++) {
                    var quad = trapeziums[x + y * screens.x];
                    var p00 = quad.p00;
                    var p10 = quad.p10 + new Vector2(1f, 0f);
                    var p01 = quad.p01 + new Vector2(0f, 1f);
                    var p11 = quad.p11 + new Vector2(1f, 1f);

                    var currScreen = new Vector2(x, y);
                    var p0 = Vector2.Scale(dscreen, p00 + currScreen) - Vector2.one;
                    var p1 = Vector2.Scale(dscreen, p10 + currScreen) - Vector2.one;
                    var p2 = Vector2.Scale(dscreen, p01 + currScreen) - Vector2.one;
                    var p3 = Vector2.Scale(dscreen, p11 + currScreen) - Vector2.one;

                    var hmat = Matrix4x4.identity;
                    hmat[0] = p1.x; hmat[4] = p2.x; hmat[8] = -p3.x;
                    hmat[1] = p1.y; hmat[5] = p2.y; hmat[9] = -p3.y;
                    hmat[2] = 1f;   hmat[6] = 1f;   hmat[10] = -1f;
                    var hinv = hmat.inverse;

                    var h0 = new Vector3(p0.x, p0.y, 1f);
                    var z = hinv.MultiplyPoint(h0);
                    var h1 = z.x * new Vector3(p1.x, p1.y, 1f);
                    var h2 = z.y * new Vector3(p2.x, p2.y, 1f);
                    var h3 = z.z * new Vector3(p3.x, p3.y, 1f);

                    var m = Matrix4x4.zero;
                    m[0] = h0.x;    m[4] = h1.x;    m[8] = h2.x;    m[12] = h3.x;
                    m[1] = h0.y;    m[5] = h1.y;    m[9] = h2.y;    m[13] = h3.y;
                    m[2] = h0.z;    m[6] = h1.z;    m[10] = h2.z;   m[14] = h3.z;
                    matrices.Add(m);
                }
            }
        }
    }
}
