using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Geometry {

    public class EdgeJoint {
        public static readonly int[] EDGE_AXIS = new int[] { 0, 1, 0, 1 };
        public static readonly int[] EDGE_DIRS = new int[] { 1, 1, -1, -1 };

        public static readonly int[] PAIR_EDGES = new int[] { 2, 3, 0, 1 };
        public static readonly int[] PAIR_SCREENS = new int[] { -1, -1, 1, 1 };

        public readonly Int2 screens;
        public readonly Vector4[] edges;

        public readonly int axis;
        public readonly int dir;

        public readonly Int2 selectedScreen;
        public readonly int selectedEdge;

        protected float bandwidth;

        public EdgeJoint(Data data, Int2 selectedScreen, int selectedEdge) {
            this.screens = data.Screens;
            this.edges = data.Edges;

            this.selectedScreen = selectedScreen;
            this.selectedEdge = selectedEdge;

            this.axis = EDGE_AXIS[selectedEdge];
            this.dir = EDGE_DIRS[selectedEdge];

            this.bandwidth = GetBandwidthAtSelectedScreen();
        }

        #region Static
        public static float ClampBandwidth(int edge, float width) {
            var dir = EDGE_DIRS[edge];
            return (dir >= 0 ? Mathf.Clamp(width, 0f, 1f) : Mathf.Clamp(width, -1f, 0f));
        }
        #endregion

        public int ScreenIndex { get { return selectedScreen.x + selectedScreen.y * screens.x; } }
        public float GetBandwidthAtSelectedScreen() {
            return edges[ScreenIndex][selectedEdge];
        }

        public void SetBandwidthAllOnColumn(int x, int edge, float width) {
            if (x < 0 || screens.x <= x)
                return;
            width = ClampBandwidth(edge, width);
            for (var y = 0; y < screens.y; y++)
                edges[x + y * screens.x][edge] = width;
        }

        public void SetBandwidthAllOnRow(int y, int edge, float width) {
            if (y < 0 || screens.y <= y)
                return;
            width = ClampBandwidth(edge, width);
            for (var x = 0; x < screens.x; x++)
                edges[x + y * screens.x][edge] = width;
        }

        public float Bandwidth {
            get { return bandwidth; }
            set {
                bandwidth = value;

                var i0 = selectedScreen[axis];
                var i1 = i0 + PAIR_SCREENS[selectedEdge];
                var pairEdge = PAIR_EDGES[selectedEdge];

                switch (axis) {
                    case 0:
                        SetBandwidthAllOnColumn(i0, selectedEdge, bandwidth);
                        SetBandwidthAllOnColumn(i1, pairEdge, -bandwidth);
                        break;
                    case 1:
                        SetBandwidthAllOnRow(i0, selectedEdge, bandwidth);
                        SetBandwidthAllOnRow(i1, pairEdge, -bandwidth);
                        break;
                }
            }
        }
    }
}
