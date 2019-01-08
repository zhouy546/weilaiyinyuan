using nobnak.Blending.Field;
using UnityEngine;

namespace nobnak.Blending.Geometry {

    public static class UvMapper {

        public static void UpdateUv(Int2 screens, Vector4[] edges, Vector4[] viewportOffsets) {

            var duv = new Vector2(1f / screens.x, 1f / screens.y);

            var offsetY = 0f;
            int i;
            Vector4 edge;
            for (var y = 0; y < screens.y; y++) {
                var offsetX = 0f;

                for (var x = 0; x < screens.x; x++) {
                    i = x + y * screens.x;

                    viewportOffsets[i] = new Vector4(
                        offsetX * duv.x, offsetY * duv.y, 0, 0);

                    edge = edges[i];
                    offsetX += edge[2];
                }

                i = y * screens.x;
                edge = edges[i];
                offsetY += edge[3];
            }
        }
    }
}
