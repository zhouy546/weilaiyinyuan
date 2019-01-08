﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nobnak.Gist.Layers;

namespace nobnak.Gist.Examples.Layer {

    public class NormalizedPositionReceiver : MonoBehaviour {
        [SerializeField]
        AbstractLayer layer;

        public void Receive(Vector2 normalizedPosition) {
            transform.position = layer.Position (normalizedPosition.x, normalizedPosition.y);
        }
    }
}
