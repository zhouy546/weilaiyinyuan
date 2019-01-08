﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist {

    [System.Serializable]
    public class Reactive<T> where T : System.IComparable {

        public event System.Action<Reactive<T>> Changed;

        protected T data;

        public Reactive(T initialData) {
            this.data = initialData;
        }
        public Reactive() : this(default(T)) { }

        public T Value {
            get { return data; }
            set {
                if (data.CompareTo(value) != 0) {
                    data = value;
                    ForceNotifyChanged();
                }
            }
        }
        public static implicit operator T(Reactive<T> reactive) {
            return reactive.data;
        }
        public static implicit operator Reactive<T>(T data) {
            return new Reactive<T>(data);
        }

        public void ForceNotifyChanged() {
            if (Changed != null)
                Changed(this);
        }
    }
}
