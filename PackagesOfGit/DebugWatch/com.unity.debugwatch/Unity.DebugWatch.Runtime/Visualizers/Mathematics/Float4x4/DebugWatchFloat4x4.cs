using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers.InternalDetails
{
    [RequireComponent(typeof(MeshFilter))]
    public class DebugWatchFloat4x4 : MonoBehaviour
    {
        public IAccessor<float4x4> Accessor;
        public float Scale = 0.2f;
        MeshBuilder mb = new MeshBuilder();
        MeshFilter meshFilter;
        List<Mesh> meshes;
        bool initialized = false;
        public List<float4x4> Values = new List<float4x4>();
        void DisposeMeshes()
        {
            if (meshes == null) return;
            foreach(var m in meshes)
            {
                DestroyImmediate(m);
            }
            meshFilter.mesh = null;
        }
        public void ClearValues()
        {
            Values.Clear();
        }
        public void AddCurrentValue()
        {
            if (Accessor.TryGet(out var value))
            {
                Values.Add(value);
            }
        }
        void Init()
        {
            if (initialized) return;
            initialized = true;
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null) Debug.LogError("DebugWatchFloat4x4 component require a MeshFilter component");
        }
        public void Refresh()
        {
            Init();
            if (Values.Count == 0) return;

            transform.position = Values[0].c3.xyz;
            float3 offset = -Values[0].c3.xyz;
            var colorX = new Color(1, 0, 0, 1);
            var colorY = new Color(0, 1, 0, 1);
            var colorZ = new Color(0, 0, 1, 1);

            mb.Clear();
            foreach (var v in Values)
            {
                float3 axisX = v.c0.xyz * Scale;
                float3 axisY = v.c1.xyz * Scale;
                float3 axisZ = v.c2.xyz * Scale;
                mb.Add(new MeshBuilder.Line(ToVector3(v.c3.xyz - axisX + offset), ToVector3(v.c3.xyz + axisX + offset), colorX));
                mb.Add(new MeshBuilder.Line(ToVector3(v.c3.xyz - axisY + offset), ToVector3(v.c3.xyz + axisY + offset), colorY));
                mb.Add(new MeshBuilder.Line(ToVector3(v.c3.xyz - axisZ + offset), ToVector3(v.c3.xyz + axisZ + offset), colorZ));
            }
            meshes = mb.BuildMesh(meshes);
            mb.Clear();

            if (meshes != null && meshes.Count > 0)
            {
                meshFilter.mesh = meshes[0];
                if (meshes.Count > 1)
                {
                    Debug.LogWarning("Too many debug meshes required, some visualization will not be shown");
                }
            }
            else
            {
                Debug.LogWarning("Failed to create debug visualization mesh");
            }

        }
        // Start is called before the first frame update
        void Start()
        {
            Refresh();
        }

        // Update is called once per frame
        void Update()
        {
            Refresh();
            //if (Accessor != null)
            //{
            //    if (Accessor.TryGet(out var value))
            //    {
            //        //UpdateTransform(value);
            //    }
            //}
        }
        public static Vector3 ToVector3(float3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
    }





}