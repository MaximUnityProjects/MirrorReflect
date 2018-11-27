using System.Collections.Generic;
using UnityEngine;

namespace MultiMirrorScriptPro {
    public class MirrorScript : MonoBehaviour {
        Camera cam, mirrorCam;
        RenderTexture renderTexture, finalTexture;
        new Renderer renderer;
        static List<MirrorScript> allMirrors = new List<MirrorScript>();
        [SerializeField] uint depth;

        void Awake() {
            cam = Camera.main; // Player Camera
            renderer = GetComponent<MeshRenderer>();
            mirrorCam = new GameObject().AddComponent<Camera>();
            TextureInit();
            mirrorCam.enabled = false;
        }
        
        private void OnEnable() {
            allMirrors.Add(this);
        }
        
        private void OnDisable() {
            allMirrors.Remove(this);
        }
        
        private void TextureInit() {
            mirrorCam.CopyFrom(cam);
            renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            finalTexture = new RenderTexture(renderTexture);
            renderer.material.SetTexture("_MainTex", renderTexture);
            mirrorCam.targetTexture = renderTexture;
        }

        private void Update() {
            if (renderTexture.width != Screen.width || renderTexture.height != Screen.height) TextureInit();
            if (!renderer.isVisible || !renderer.IsVisibleFrom(cam)) return;
            RenderMirror(this, depth);
            Graphics.CopyTexture(renderTexture, finalTexture);
        }
        
        private void LateUpdate() {
            Graphics.CopyTexture(finalTexture, renderTexture);
        }

        public static void RenderMirror(MirrorScript thisMirror, uint depth) {
            var levels = GetMirrorLevels(thisMirror, depth, thisMirror.cam);
            levels.Reverse();
            foreach (var level in levels) {
                level.mirrorScript.mirrorCam.SetTransform(level);
                level.mirrorScript.mirrorCam.Render();
            }
        }

        static List<CamTransform> GetMirrorLevels(MirrorScript mirror, uint depth, Camera prevCam, List<CamTransform> levels = null) {
            levels = levels ?? new List<CamTransform>();
            if (depth > 0) {
                for (int i = 0; i < allMirrors.Count; i++) {
                    if (allMirrors[i] == mirror) continue;
                    levels.Add(mirror.ReflectMirrorCam(prevCam.transform));
                    if (allMirrors[i].renderer.IsVisibleFrom(mirror.mirrorCam)) {
                        levels.AddRange(GetMirrorLevels(allMirrors[i], depth - 1, mirror.mirrorCam));
                        break;
                    }
                }
            }
            return levels;
        }

        private CamTransform ReflectMirrorCam(Transform observer) {
            var newCam = new CamTransform();
            mirrorCam.transform.position = newCam.position = Vector3.Reflect(observer.position - transform.position, transform.forward) + transform.position;
            mirrorCam.transform.forward = Vector3.Reflect(observer.forward, transform.forward);
            newCam.rotation = mirrorCam.transform.rotation;
            //mirrorCam.nearClipPlane = newCam.nearClipPlane = Mathf.Max(0.3f, (transform.position - mirrorCamTransform.position).magnitude);
            mirrorCam.nearClipPlane = newCam.nearClipPlane = 0.3f;
            newCam.mirrorScript = this;
            return newCam;
        }
    }
}