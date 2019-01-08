using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Geometry {

    public static class ScreenSelector {
        public const float LESS_THAN_ONE = 0.999f;
        public static readonly int[] PAIR_EDGES = new int[] { 0,2, 0,1, 1,3, 2,3 };
        
        public static float Cross(Vector2 p0, Vector2 p1, Vector2 p2) {
            return (p1.x - p0.x) * (p2.y - p0.y) - (p1.y - p0.y) * (p2.x - p0.x);
        }
        public static bool PointOnTriangle(Vector2 p, Vector2 v0, Vector2 v1, Vector2 v2) {
            var b0 = Cross(p, v0, v1) >= 0f;
            var b1 = Cross(p, v1, v2) >= 0f;
            var b2 = Cross(p, v2, v0) >= 0f;
            return b0 == b1 && b1 == b2;
        }

        public static Vector2 LocalToWorld(int x, int y, Vector2 p, Vector2 dx) {
            return new Vector2((x + p.x) * dx.x, (y + p.y) * dx.y);
        }
        public static Trapezium LocalToWorld(int x, int y, Trapezium trap, Vector2 dx) {
            var p00 = LocalToWorld(x, y, trap.p00, dx);
            var p10 = LocalToWorld(x + 1, y, trap.p10, dx);
            var p01 = LocalToWorld(x, y + 1, trap.p01, dx);
            var p11 = LocalToWorld(x + 1, y + 1, trap.p11, dx);
            return new Trapezium(p00, p10, p01, p11);
        }
        public static Vector2 ScreenSize(Int2 screens) {
            return new Vector2(1f / screens.x, 1f / screens.y);
        }
        public static bool TryFindScreen(
            Vector2 mouseUv, Int2 screens, Trapezium[] trapeziums,
            out Int2 selectedScreen) {

            var dx = ScreenSize(screens);
            for (var y = 0; y < screens.y; y++) {
                for (var x = 0; x < screens.x; x++) {
                    var i = x + y * screens.x;
                    var worldTrap = LocalToWorld(x, y, trapeziums[i], dx);

                    if (PointOnTriangle(mouseUv, worldTrap.p00, worldTrap.p11, worldTrap.p10) 
                        || PointOnTriangle(mouseUv, worldTrap.p00, worldTrap.p01, worldTrap.p11)) {
                        selectedScreen = new Int2(x, y);
                        return true;
                    }
                }
            }

            selectedScreen = default(Int2);
            return false;
        }

        public static bool TryFindNearestVertex(
            Vector2 mouseUv, Int2 screens, Trapezium[] trapeziums,
            out Int2 selectedScreen, out int vertexIndex) {

            vertexIndex = -1;

            if (!TryFindScreen(mouseUv, screens, trapeziums, out selectedScreen))
                return false;

            var dx = new Vector2(1f / screens.x, 1f / screens.y);
            var j = selectedScreen.x + selectedScreen.y * screens.x;
            var worldTrape = LocalToWorld(
                selectedScreen.x, selectedScreen.y, trapeziums[j], dx);

            var nearestSqDistance = float.MaxValue;
            for (var i = 0; i < 4; i++) {
                var vertexUv = worldTrape[i];
                var sqDist = (vertexUv - mouseUv).sqrMagnitude;
                if (sqDist < nearestSqDistance) {
                    nearestSqDistance = sqDist;
                    vertexIndex = i;
                }
            }
            return vertexIndex >= 0;
        }

        public static bool TryFindNearestEdge(
            Vector2 mouseUv, Int2 screens, Trapezium[] trapeziums,
            out Int2 selectedScreen, out int edgeIndex) {

            edgeIndex = -1;

            if (!TryFindScreen(mouseUv, screens, trapeziums, out selectedScreen))
                return false;

            var dx = new Vector2(1f / screens.x, 1f / screens.y);
            var j = selectedScreen.x + selectedScreen.y * screens.x;
            var worldTrape = LocalToWorld(
                selectedScreen.x, selectedScreen.y, trapeziums[j], dx);

            var nearestSqDistance = float.MaxValue;
            for (var i = 0; i < 4; i++) {
                var p0 = worldTrape[PAIR_EDGES[2 * i]];
                var p1 = worldTrape[PAIR_EDGES[2 * i + 1]];
                var edge = p1 - p0;
                var tangent = edge.normalized;
                var normal = new Vector2(-tangent.y, tangent.x);
                var dist = Vector2.Dot(normal, mouseUv - p0);
                var sqDist = dist * dist;
                if (sqDist < nearestSqDistance) {
                    nearestSqDistance = sqDist;
                    edgeIndex = i;
                }
            }

            return edgeIndex >= 0;
        }
    }
}
