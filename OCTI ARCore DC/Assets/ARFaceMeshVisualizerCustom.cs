using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Generates a mesh for an <see cref="ARFace"/>.
    /// </summary>
    /// <remarks>
    /// If this <c>GameObject</c> has a <c>MeshFilter</c> and/or <c>MeshCollider</c>,
    /// this component will generate a mesh from the underlying <c>XRFace</c>.
    /// </remarks>
    [RequireComponent(typeof(ARFace))]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@3.0/api/UnityEngine.XR.ARFoundation.ARFaceMeshVisualizer.html")]
    public sealed class ARFaceMeshVisualizerCustom : MonoBehaviour {
        // [SerializeField] int triIndex = 0;
        [SerializeField] MeshFilter otherMesh = null;
        [FormerlySerializedAs("trianglesData")] [FormerlySerializedAs("collectTris")] [SerializeField] FaceCutoutsData faceCutoutsData;
        /// <summary>
        /// Get the <c>Mesh</c> that this visualizer creates and manages.
        /// </summary>
        public Mesh mesh { get; private set; }

        void SetVisible(bool visible)
        {
            m_MeshRenderer = GetComponent<MeshRenderer>();
            if (m_MeshRenderer == null)
            {
                return;
            }

            //if it is getting visible after being invisible for a while, set its topology
            if (visible && !m_MeshRenderer.enabled)
            {
                SetMeshTopology();
            }

            m_MeshRenderer.enabled = visible;
        }

        static bool TryCopyToList<T>(NativeArray<T> array, List<T> list) where T : struct
        {
            list.Clear();
            if (!array.IsCreated || array.Length == 0)
                return false;

            foreach (var item in array)
                list.Add(item);

            return true;
        }

        void SetMeshTopology()
        {
            var m = mesh;
            if (m == null)
            {
                return;
            }

            m.Clear();

            if (TryCopyToList(m_Face.vertices, s_Vertices) && TryCopyToList(m_Face.indices, s_Indices)) {
                m.SetVertices(s_Vertices);

                // not optimized for performance, just to show the idea
                var cutout = s_Indices.ToArray();
                foreach (var triangle in faceCutoutsData.allTrianglesToCutout) {
                    var i= triangle * 3;
                    cutout[i] = cutout[i+1] = cutout[i+2] = 0;
                }
              
                m.SetTriangles(cutout, 0);
                m.RecalculateBounds();

                if (TryCopyToList(m_Face.normals, s_Vertices)) {
                    m.SetNormals(s_Vertices);
                } else {
                    m.RecalculateNormals();
                }

                /*var other = otherMesh.sharedMesh;
                other.SetVertices(m_Face.vertices);
                other.SetIndices(m_Face.indices, MeshTopology.Triangles, 0);
                other.RecalculateNormals();*/
            }

            if (TryCopyToList(m_Face.uvs, s_UVs))
            {
                m.SetUVs(0, s_UVs);
            }

            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                meshFilter.sharedMesh = m;
            }

            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.sharedMesh = m;
            }

            m_TopologyUpdatedThisFrame = true;
        }

        void UpdateVisibility()
        {
            var visible = enabled &&
                (m_Face.trackingState != TrackingState.None) &&
                (ARSession.state > ARSessionState.Ready);

            SetVisible(visible);
        }

        void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
        {
            UpdateVisibility();
            if (!m_TopologyUpdatedThisFrame)
            {
                SetMeshTopology();
            }
            m_TopologyUpdatedThisFrame = false;
        }

        void OnSessionStateChanged(ARSessionStateChangedEventArgs eventArgs)
        {
            UpdateVisibility();
        }

        void Awake()
        {
            mesh = new Mesh();
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_Face = GetComponent<ARFace>();
            s_Indices = new List<int>();
            s_Vertices = new List<Vector3>();
            s_UVs = new List<Vector2>();
        }

        void OnEnable()
        {
            m_Face.updated += OnUpdated;
            ARSession.stateChanged += OnSessionStateChanged;
            UpdateVisibility();
        }

        void OnDisable()
        {
            m_Face.updated -= OnUpdated;
            ARSession.stateChanged -= OnSessionStateChanged;
        }

        ARFace m_Face;
        MeshRenderer m_MeshRenderer;
        bool m_TopologyUpdatedThisFrame;
        static List<Vector3> s_Vertices;
        static List<Vector2> s_UVs;
        static List<int> s_Indices;
    }
}
