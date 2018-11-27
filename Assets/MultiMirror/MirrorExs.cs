using UnityEngine;
namespace MultiMirrorScriptPro {
    public static class MirrorExs {
        
        public static void SetTransform(this Camera cam, CamTransform camTr ) {
            cam.transform.rotation = camTr.rotation;
            cam.transform.position = camTr.position;
            cam.nearClipPlane = cam.nearClipPlane;
        }

        public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
        }
    }

    public struct CamTransform {
        public Vector3 position;
        public Quaternion rotation;
        public float nearClipPlane;
        public MirrorScript mirrorScript;

        public CamTransform(Vector3 position, Quaternion rotation, float nearClipPlane, MirrorScript mirrorScript) {
            this.position = position;
            this.rotation = rotation;
            this.nearClipPlane = nearClipPlane;
            this.mirrorScript = mirrorScript;
        }
    }
}