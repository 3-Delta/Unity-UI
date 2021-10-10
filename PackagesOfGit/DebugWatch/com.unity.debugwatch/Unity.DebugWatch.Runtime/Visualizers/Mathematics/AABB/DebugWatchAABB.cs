using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.DebugWatch.Visualizers.InternalDetails
{
    [RequireComponent(typeof(MeshFilter))]
    public class DebugWatchAABB : MonoBehaviour
    {
        public IAccessor<AABB> Accessor;
        public float Scale = 0.2f;
        MeshBuilder mb = new MeshBuilder();
        MeshFilter meshFilter;
        List<Mesh> meshes;
        bool initialized = false;
        public List<AABB> Values = new List<AABB>();
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
            if (meshFilter == null) Debug.LogError("DebugWatchAABB component require a MeshFilter component");
        }
        public void Refresh()
        {
            Init();
            if (Values.Count == 0) return;


            transform.position = Values[0].Center;
            float3 offset = -Values[0].Center;
            var color = new Color(1, 1, 1, 1);

            mb.Clear();
            foreach (var v in Values)
            {

                var vMin = new Vector3(v.Min.x + offset.x, v.Min.y + offset.y, v.Min.z + offset.z);
                var vMax = new Vector3(v.Max.x + offset.x, v.Max.y + offset.y, v.Max.z + offset.z);
                var v0 = vMin;
                var v1 = new Vector3(vMax.x, vMin.y, vMin.z);
                var v2 = new Vector3(vMax.x, vMax.y, vMin.z);
                var v3 = new Vector3(vMin.x, vMax.y, vMin.z);
                var v4 = new Vector3(vMin.x, vMin.y, vMax.z);
                var v5 = new Vector3(vMax.x, vMin.y, vMax.z);
                var v6 = new Vector3(vMax.x, vMax.y, vMax.z);
                var v7 = new Vector3(vMin.x, vMax.y, vMax.z);
                mb.Add(new MeshBuilder.Line(v0, v1, color));
                mb.Add(new MeshBuilder.Line(v1, v2, color));
                mb.Add(new MeshBuilder.Line(v2, v3, color));
                mb.Add(new MeshBuilder.Line(v3, v0, color));
                mb.Add(new MeshBuilder.Line(v4, v5, color));
                mb.Add(new MeshBuilder.Line(v5, v6, color));
                mb.Add(new MeshBuilder.Line(v6, v7, color));
                mb.Add(new MeshBuilder.Line(v7, v4, color));
                mb.Add(new MeshBuilder.Line(v0, v4, color));
                mb.Add(new MeshBuilder.Line(v1, v5, color));
                mb.Add(new MeshBuilder.Line(v2, v6, color));
                mb.Add(new MeshBuilder.Line(v3, v7, color));
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
            ClearValues();
            AddCurrentValue();
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