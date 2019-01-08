using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Blending.Field {

    [System.Serializable]
    public struct Trapezium {

        public Vector2 p00;
        public Vector2 p10;
        public Vector2 p01;
        public Vector2 p11;

        public Trapezium(Vector2 p00, Vector2 p10, Vector2 p01, Vector2 p11) {
            this.p00 = p00;
            this.p10 = p10;
            this.p01 = p01;
            this.p11 = p11;
        }

        public Vector2 this[int index] {
            get {
                switch (index) {
                    default:
                        return p00;
                    case 1:
                        return p10;
                    case 2:
                        return p01;
                    case 3:
                        return p11;
                }
            }
            set {
                switch (index) {
                    default:
                        p00 = value;
                        break;
                    case 1:
                        p10 = value;
                        break;
                    case 2:
                        p01 = value;
                        break;
                    case 3:
                        p11 = value;
                        break;
                }
            }
        }
        public void Clamp01() {
            p00.x = Mathf.Clamp(p00.x, 0f, 1f);
            p00.y = Mathf.Clamp(p00.y, 0f, 1f);

            p10.x = Mathf.Clamp(p10.x, -1f, 0f);
            p10.y = Mathf.Clamp(p10.y, 0f, 1f);

            p01.x = Mathf.Clamp(p01.x, 0f, 1f);
            p01.y = Mathf.Clamp(p01.y, -1f, 0f);

            p11.x = Mathf.Clamp(p11.x, -1f, 0f);
            p11.y = Mathf.Clamp(p11.y, -1f, 0f);

        }
    }
}
