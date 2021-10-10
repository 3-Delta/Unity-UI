using UnityEngine;
using Unity.Mathematics;

namespace Unity.DebugWatch.Visualizers.InternalDetails
{ 
    public class DebugWatchFloat4x4Gizmo : MonoBehaviour
    {
        public IAccessor<float4x4> Accessor;
        bool initialized = false;
        Vector3 previousScale;
        Quaternion previousRotation;
        Vector3 previousPosition;
        public float Scale = 0.2f;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            var vRight = transform.right * Scale;
            var vUp = transform.up * Scale;
            var vForward = transform.forward * Scale;
            Gizmos.DrawLine(transform.position, transform.position + vRight);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + vUp);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + vForward);
        }
        void OnDrawGizmosSelected()
        {
            OnDrawGizmos();
            Refresh();
        }
        void UpdateTransform(float4x4 value)
        {

            previousRotation = transform.rotation = new quaternion(value);
            previousPosition = transform.position = value.c3.xyz;
            previousScale = transform.localScale = new Vector3(math.length(value.c0.xyz), math.length(value.c1.xyz), math.length(value.c2.xyz));
        }
        private void Init()
        {
            if (initialized) return;
            initialized = true;
            if (Accessor.TryGet(out var value))
            {
                UpdateTransform(value);
            }
            else
            {
                previousScale = transform.localScale;
                previousPosition = transform.position;
                previousRotation = transform.rotation;
            }
        }
        
        public void Refresh()
        {
            Init();
            if (previousPosition != transform.position
                || previousRotation != transform.rotation
                || previousScale != transform.localScale)
            {
                if (Accessor != null)
                {
                    float4x4 v = new float4x4(transform.rotation, transform.position);
                    v.c0.xyz *= transform.localScale.x;
                    v.c1.xyz *= transform.localScale.y;
                    v.c2.xyz *= transform.localScale.z;
                    if (Accessor.TrySet(v))
                    {
                        previousScale = transform.localScale;
                        previousPosition = transform.position;
                        previousRotation = transform.rotation;
                    }
                }
            }
            else if (Accessor != null)
            {
                if (Accessor.TryGet(out var value))
                {
                    UpdateTransform(value);
                }
            }
        }


        void Update()
        {
            Refresh();
        }
    }
}
