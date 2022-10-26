using System;
using System.Collections;
using System.Collections.Generic;
using SystemHalf;
using UnityEngine;
using UnityEngine.Rendering;

namespace Realit.Models.Meshes
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct RealitModelVertex
    {
        public float posX, posY, posZ;
        public float normalX, normalY, normalZ;
        public Half TexCoord1X, TexCoord1Y;
        public Half TexCoord2X, TexCoord2Y;
        public Half TexCoord3X, TexCoord3Y;
        public Half TexCoord4X, TexCoord4Y;

        public RealitModelVertex(Vector3[] positions, Vector3[] normals, Vector2[]uv1, int index) 
            : this(positions, normals, uv1, null, null, null, index) { }
        public RealitModelVertex(Vector3[] positions, Vector3[] normals, Vector2[] uv1, Vector2[] uv2, int index) 
            : this(positions, normals, uv1, uv2, null, null, index) { }
        public RealitModelVertex(Vector3[] positions, Vector3[] normals, Vector2[] uv1, Vector2[] uv2, Vector2[] uv3, int index) 
            : this(positions, normals, uv1, uv2, uv3, null, index) { }


        public RealitModelVertex(Vector3[] positions, Vector3[] normals, Vector2[] uv1, Vector2[] uv2, Vector2[] uv3, Vector2[] uv4, int index)
        {
            Vector3 pos = positions[index];
            posX = pos.x;
            posY = pos.y;
            posZ = pos.z;

            Vector3 normal = normals[index];
            normalX = normal.x;
            normalY = normal.y;
            normalZ = normal.z;

            if (uv1 != null)
            {
                TexCoord1X = (Half)uv1[index].x;
                TexCoord1Y = (Half)uv1[index].y;
            }
            else
                TexCoord1X = TexCoord1Y = 0;

            if (uv2 != null)
            {
                TexCoord2X = (Half)uv2[index].x;
                TexCoord2Y = (Half)uv2[index].y;
            }
            else
                TexCoord2X = TexCoord2Y = 0;

            if (uv3 != null)
            {
                TexCoord3X = (Half)uv3[index].x;
                TexCoord3Y = (Half)uv3[index].y;
            }
            else
                TexCoord3X = TexCoord3Y = 0;

            if (uv4 != null)
            {
                TexCoord4X = (Half)uv4[index].x;
                TexCoord4Y = (Half)uv4[index].y;
            }
            else
                TexCoord4X = TexCoord4Y = 0;
        }
        public static VertexAttributeDescriptor[] GetDescriptors()
        {
            return new VertexAttributeDescriptor[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.Float16, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.Float16, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord3, VertexAttributeFormat.Float16, 2)
            };
        }
}
}