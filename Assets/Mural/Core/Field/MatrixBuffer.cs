using nobnak.Gist.GPUBuffer;
using UnityEngine;

namespace nobnak.Blending.Field {

    public abstract class MatrixBuffer : System.IDisposable {

        protected bool invalid;

        protected GPUList<Matrix4x4> matrices;

        public MatrixBuffer() {
            invalid = true;
            matrices = new GPUList<Matrix4x4>();
        }

        #region IDisposable
        public void Dispose() {
            if (matrices != null) {
                matrices.Dispose();
                matrices = null;
            }
        }
        #endregion

        #region Output
        public GPUList<Matrix4x4> Matrices {
            get {
                if (invalid) {
                    invalid = false;
                    Validate();
                    UpdateMatrix();
                    matrices.Upload();
                }
                return matrices;
            }
        }
        public ComputeBuffer Buffer {
            get {
                return Matrices.Buffer;
            }
        }
        #endregion

        protected virtual void Validate() { }

        protected abstract void UpdateMatrix();

    }
}
