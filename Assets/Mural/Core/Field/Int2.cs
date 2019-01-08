using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Blending.Field {

    [System.Serializable]
    public struct Int2 {
        public int x;
        public int y;

        public Int2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public int this[int index] {
            get {
                switch(index) {
                    default:
                        return x;
                    case 1:
                        return y;
                }
            }
            set {
                switch (index) {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                }
            }
        }

        public static implicit operator Vector2(Int2 i) {
            return new Vector2(i.x, i.y);
        }
        public static explicit operator Int2(Vector2 v) {
            return new Int2((int)v.x, (int)v.y);
        }
    }
}
