using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaiUtils.Extensions
{
    public static class TransformExtensions
    {
        public static void SetPositionX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetPositionY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetPositionZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void SetLocalPositionX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        public static void SetLocalPositionY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        public static void SetLocalPositionZ(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        public static void SetPosition(this Transform transform, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.position = new Vector3(
                float.IsNaN(x) ? transform.position.x : x,
                float.IsNaN(y) ? transform.position.y : y,
                float.IsNaN(z) ? transform.position.z : z
            );
        }

        public static void SetLocalPosition(this Transform transform, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            transform.localPosition = new Vector3(
                float.IsNaN(x) ? transform.localPosition.x : x,
                float.IsNaN(y) ? transform.localPosition.y : y,
                float.IsNaN(z) ? transform.localPosition.z : z
            );
        }
    
        public static Transform GetClosestEntity(this Transform transform, Transform[] entities)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(Transform potentialTarget in entities)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }

         public static Transform GetClosestEntity(this Transform transform, List<Transform> entities)
        {
            Transform bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            foreach(Transform potentialTarget in entities)
            {
                Vector3 directionToTarget = potentialTarget.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }
            return bestTarget;
        }

        public static void LocalReset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static IEnumerator LerpMoveTo(this Transform transform, Vector3 target, float duration)
        {
            Vector3 diff = target - transform.position;
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.position += diff * (Time.deltaTime / duration);
                yield return null;
            }
        }

        public static IEnumerator LerpRotateTo(this Transform transform, Quaternion target, float duration)
        {
            float counter = 0;
            while (counter < duration)
            {
                counter += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime / duration);
                yield return null;
            }
        }

        public static bool ContainedWithin(this Transform transform, Transform container)
        {
            return container.position.x - container.localScale.x / 2 < transform.position.x &&
                   container.position.x + container.localScale.x / 2 > transform.position.x &&
                   container.position.y - container.localScale.y / 2 < transform.position.y &&
                   container.position.y + container.localScale.y / 2 > transform.position.y &&
                   container.position.z - container.localScale.z / 2 < transform.position.z &&
                   container.position.z + container.localScale.z / 2 > transform.position.z;
        }

        public static bool ContainedWithin(this Transform transform, Collider container)
        {
            return container.bounds.Contains(transform.position);
        }

        public static bool ContainsPoint(this Transform container, Vector3 point, float range = 2, bool ignoreY = false, bool ignoreX = false, bool ignoreZ = false)
        {
            if (ignoreY)
            {
                return container.position.x - range < point.x &&
                       container.position.x + range > point.x &&
                       container.position.z - range < point.z &&
                       container.position.z + range > point.z;
            }
            if (ignoreX)
            {
                return container.position.y - range < point.y &&
                       container.position.y + range > point.y &&
                       container.position.z - range < point.z &&
                       container.position.z + range > point.z;
            }
            if (ignoreZ)
            {
                return container.position.x - range < point.x &&
                       container.position.x + range > point.x &&
                       container.position.y - range < point.y &&
                       container.position.y + range > point.y;
            }
            return container.position.x - range < point.x &&
                   container.position.x + range > point.x &&
                   container.position.y - range < point.y &&
                   container.position.y + range > point.y &&
                   container.position.z - range < point.z &&
                   container.position.z + range > point.z;
        }
    }
}
