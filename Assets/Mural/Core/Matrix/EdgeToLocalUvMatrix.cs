using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Matrix {
    public class EdgeToLocalUvMatrix : ViewportMatrixBuffer {

        protected Vector4[] edges = new Vector4[0];

        #region Input
        public Vector4[] Edges {
            get { return edges; }
            set {
                invalid = true;
                edges = value;
            }
        }
        #endregion
        
        protected Matrix4x4 CreateMatrix(Vector4 edge) {
            return CreateMatrix(edge.x, edge.y, 1f + edge.z - edge.x, 1f + edge.w - edge.y);
        }
        protected override void UpdateMatrix() {
            matrices.Clear();
            foreach (var e in edges)
                matrices.Add(CreateMatrix(e));
        }

    }
}
