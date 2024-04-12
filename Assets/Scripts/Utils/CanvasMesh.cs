

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Utils
{
    /**
    *  ============================================================================
    *  MIT License
    *
    *  Copyright (c) 2017 Eric Phillips
    *
    *  Permission is hereby granted, free of charge, to any person obtaining a
    *  copy of this software and associated documentation files (the "Software"),
    *  to deal in the Software without restriction, including without limitation
    *  the rights to use, copy, modify, merge, publish, distribute, sublicense,
    *  and/or sell copies of the Software, and to permit persons to whom the
    *  Software is furnished to do so, subject to the following conditions:
    *
    *  The above copyright notice and this permission notice shall be included in
    *  all copies or substantial portions of the Software.
    *
    *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    *  DEALINGS IN THE SOFTWARE.
    *  ============================================================================
    *
    *
    *  This script renders a mesh in canvas space using a CanvasRenderer.
    *  This allows all the scaling and canvas layout constraints to be
    *  applied to the mesh.
    *
    *  Created by Eric Phillips on January 1, 2017.
    *  Modified by Alexander Izmailov on April 12, 2024.
    */
    [ExecuteInEditMode]
    public class CanvasMesh : Graphic
    {
        // Inspector properties
        public Mesh mesh;
        public Material[] materials;

        
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            canvasRenderer.materialCount = materials.Length + 1;
            for (int index = 0; index < materials.Length; index++)
                canvasRenderer.SetMaterial(materials[index], index + 1);
            
        }

        /// <summary>
        /// Callback function when a UI element needs to generate vertices.
        /// </summary>
        /// <param name="vh">VertexHelper utility.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (mesh == null) return;
            // Get data from mesh
            Vector3[] verts = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            if (uvs.Length < verts.Length)
                uvs = new Vector2[verts.Length];
            // Get mesh bounds parameters
            Vector2 meshMin = mesh.bounds.min;
            Vector2 meshSize = mesh.bounds.size;
            // Add scaled vertices
            for (int ii = 0; ii < verts.Length; ii++)
            {
                Vector2 v = verts[ii];
                v.x = (v.x - meshMin.x) / meshSize.x;
                v.y = (v.y - meshMin.y) / meshSize.y;
                v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
                vh.AddVert(v, color, uvs[ii]);
            }

            // Add triangles
            int[] tris = mesh.triangles;
            for (int ii = 0; ii < tris.Length; ii += 3)
                vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
        }

        /// <summary>
        /// Converts a vertex in mesh coordinates to a point in world coordinates.
        /// </summary>
        /// <param name="vertex">The input vertex.</param>
        /// <returns>A point in world coordinates.</returns>
        public Vector3 TransformVertex(Vector3 vertex)
        {
            // Convert vertex into local coordinates
            Vector2 v;
            v.x = (vertex.x - mesh.bounds.min.x) / mesh.bounds.size.x;
            v.y = (vertex.y - mesh.bounds.min.y) / mesh.bounds.size.y;
            v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
            // Convert from local into world
            return transform.TransformPoint(v);
        }

        /// <summary>
        /// Converts a vertex in world coordinates into a vertex in mesh coordinates.
        /// </summary>
        /// <param name="vertex">The input vertex.</param>
        /// <returns>A point in mesh coordinates.</returns>
        public Vector3 InverseTransformVertex(Vector3 vertex)
        {
            // Convert from world into local coordinates
            Vector2 v = transform.InverseTransformPoint(vertex);
            // Convert into mesh coordinates
            v.x /= rectTransform.rect.size.x;
            v.y /= rectTransform.rect.size.y;
            v += rectTransform.pivot;
            v = Vector2.Scale(v, mesh.bounds.size);
            v.x += mesh.bounds.min.x;
            v.y += mesh.bounds.min.y;
            return v;
        }
    }
}