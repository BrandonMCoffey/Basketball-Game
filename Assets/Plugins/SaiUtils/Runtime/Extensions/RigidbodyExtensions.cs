using UnityEngine;

namespace SaiUtils.Extensions
{
    public static class RigidbodyExtensions
    {
        public static void SetVelocity(this Rigidbody rigidbody, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            rigidbody.velocity = new Vector3(
                float.IsNaN(x) ? rigidbody.velocity.x : x,
                float.IsNaN(y) ? rigidbody.velocity.y : y,
                float.IsNaN(z) ? rigidbody.velocity.z : z
            );
        }

        public static void ChangeVelocity(this Rigidbody rigidbody, float x = 0, float y = 0, float z = 0)
        {
            rigidbody.velocity = new Vector3(x, y, z).normalized * rigidbody.velocity.magnitude;
        }
    }
}