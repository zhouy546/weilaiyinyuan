﻿using UnityEngine;
using System.Collections;
using System.IO;

namespace nobnak.Gist {
    
    public class ImageLoader : System.IDisposable {
        public enum StateEnum { None = 0, Load }

        public event System.Action<Texture2D> TextureCreate;
        public event System.Action<Texture2D> TextureDestroy;
        public event System.Action<Texture2D> TextureUpdate;

        protected Texture2D image;
        protected System.DateTime lastFileTime;

        protected TextureFormat format;
        protected bool mipmap;
        protected bool linear;

        public ImageLoader(TextureFormat format, bool mipmap, bool linear) {
            this.format = format;
            this.mipmap = mipmap;
            this.linear = linear;

            this.lastFileTime = System.DateTime.MinValue;
        }
        public ImageLoader() : this(TextureFormat.ARGB32, true, false) {}

        #region IDisposable implementation
        public void Dispose () {
            ReleaseTexture();
        }
        #endregion

        #region Static
        public static string FullPath(System.Environment.SpecialFolder folder, string file) {
            return System.IO.Path.Combine (System.Environment.GetFolderPath (folder), file);
        }
        #endregion

        public Texture2D Image { get { return image; } }
        public TextureFormat Format { get { return format; } set { format = value; } }
        public bool Mipmap { get { return mipmap; } set { mipmap = value; } }
        public bool Linear { get { return linear; } set { linear = value; } }

        public virtual void Update(string path) {
            try {
                var next = (File.Exists (path) ? StateEnum.Load : StateEnum.None);

                switch (next) {
                case StateEnum.Load:
                    var writeTime = File.GetLastWriteTime (path);
                    if (writeTime != lastFileTime) {
                        lastFileTime = writeTime;
                        LoadImage (path);
                    }
                    break;
                default:
                    lastFileTime = System.DateTime.MinValue;
                    ReleaseTexture();
                    break;
                }
            } catch (System.Exception e) {
                Debug.LogError (e);
            }
        }

        public virtual void InitTexture () {
            if (image == null) {
                image = new Texture2D (2, 2, format, mipmap, linear);
                NotifyOnTextureCreate();
            }
        }
        public virtual void ReleaseTexture () {
            if (image != null) {
                NotifyOnTextureDestroy ();
                if (Application.isPlaying)
                    Object.Destroy (image);
                else
                    Object.DestroyImmediate (image);
            }
        }

        protected virtual void LoadImage (string imageFilePath) {
            InitTexture ();
            image.LoadImage (File.ReadAllBytes (imageFilePath));
            NotifyOnTextureUpdate ();        
        }

        protected virtual void NotifyOnTextureCreate () {
            if (TextureCreate != null)
                TextureCreate (image);
        }
        protected virtual void NotifyOnTextureUpdate () {
            if (TextureUpdate != null)
                TextureUpdate (image);
        }
        protected virtual void NotifyOnTextureDestroy () {
            if (TextureDestroy != null)
                TextureDestroy (image);
        }



    }

    public class ImageLoaderWorker : System.IDisposable {
        protected float interval;
        protected System.Func<string> getPath;

        protected ImageLoader loader;
        protected MonoBehaviour m;
        protected Coroutine work;

        public ImageLoaderWorker(MonoBehaviour m, ImageLoader loader, float interval, System.Func<string> getPath) {
            this.m = m;
            this.loader = loader;
            this.interval = interval;
            this.getPath = getPath;

            Start ();
        }

        #region IDisposable implementation
        public void Dispose () {
            Stop ();
        }
        #endregion

        public void Start () {
            Stop ();
            this.work = m.StartCoroutine (Worker ());
        }

        public void Stop () {
            if (work != null) {
                m.StopCoroutine (work);
                work = null;
            }
        }

        protected virtual IEnumerator Worker() {
            while (true) {
                yield return (interval > 0 ? new WaitForSeconds (interval) : null);

                loader.Update (getPath ());
            }
        }
    }
}
