using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace SaiUtils
{
    public static class SaiStaticUtils
    {
        public static bool IsPointerOverUI(bool showUINameInDebug = false)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // List to hold all raycast results
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            bool isHoveringOverUI = false;

            foreach (RaycastResult result in results)
            {
                Graphic graphic = result.gameObject.GetComponent<Graphic>();
                if (graphic != null && graphic.raycastTarget)
                {
                    if (showUINameInDebug) Debug.Log($"Hovering over UI element: {result.gameObject.name}");
                    isHoveringOverUI = true;
                }
            }

            return isHoveringOverUI;
        }

        public static (RaycastHit hit, Ray ray) MouseHit(LayerMask layerMask, float maxDistance = 1000f, bool debug = false) {
            return MouseHit(Camera.main, layerMask, maxDistance, debug);
        }

        public static (RaycastHit hit, Ray ray) MouseHit(Camera cam, LayerMask layerMask, float maxDistance = 1000f, bool debug = false) {
            var mousePos = Input.mousePosition;
            var ray = cam.ScreenPointToRay(mousePos);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, layerMask);

            if (debug) {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            return (hit, ray);
        }

        public static RaycastHit Raycast(Vector3 origin, Vector3 direction, LayerMask searchableLayerMask, float maxDistance = 10f, bool debug = false) {
            var ray = new Ray(origin, direction);
            var hit = new RaycastHit();

            Physics.Raycast(ray.origin, ray.direction, out hit, maxDistance, searchableLayerMask);

            if (debug) {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            return hit;
        }

        public static Vector3 AngleToVector2D(float angleDeg)
        {
            var angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0f);
        }

        public static Vector3 AngleToVector3D(float angleDeg)
        {
            var angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        }

        public static float VectorToAngle2D(Vector3 vector)
        {
            vector.Normalize();

            var angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            if (angle < 0)
                angle += 360;

            return angle;
        }

        public static float VectorToAngle3D(Vector3 vector)
        {
            vector.Normalize();

            var angle = Mathf.Atan2(vector.z, vector.x) * Mathf.Rad2Deg;

            // if (angle < 0)
            //     angle += 360;

            return angle;
        }

        public static Vector3 IgnoreZ(Vector3 value, float z = 0) {
            return new Vector3(value.x, value.y, z);
        }

        public static Vector3 IgnoreY(Vector3 value, float y = 0) {
            return new Vector3(value.x, y, value.z);
        }

        public static Vector3 IgnoreX(Vector3 value, float x = 0) {
            return new Vector3(x, value.y, value.z);
        }

        public static bool CompareLayer(LayerMask layer, LayerMask layerMask)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static Vector3 GetRandomPointWithinBounds(List<Transform> bounds)
        {
            if (bounds == null || bounds.Count == 0)
                return Vector3.zero;

            var randomX = Random.Range(bounds[0].position.x, bounds[1].position.x);
            var randomY = Random.Range(bounds[0].position.y, bounds[1].position.y);
            var randomZ = Random.Range(bounds[0].position.z, bounds[1].position.z);

            return new Vector3(randomX, randomY, randomZ);
        }

        public static void CreateIfNotExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}