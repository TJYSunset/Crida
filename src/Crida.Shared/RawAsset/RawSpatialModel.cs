using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices;
using Crida.Shared.GpuReadyStructs;
using Crida.Shared.Math;
using JetBrains.Annotations;
using MessagePack;

namespace Crida.Shared.RawAsset
{
    [MessagePackObject]
    public class RawSpatialModel : IRawAsset
    {
        public RawSpatialModel(ImmutableArray<Bone> bones, ImmutableDictionary<string, Mesh> meshes)
        {
            Bones = bones;
            Meshes = meshes;
        }

        [Key(0)] public ImmutableArray<Bone> Bones { get; }

        [Key(1)] public ImmutableDictionary<string, Mesh> Meshes { get; }
    }

    [MessagePackObject]
    public class Bone
    {
        public Bone(string name, int parentIndex, Vector3D head, Vector3D tail, double roll)
        {
            Name = name;
            ParentIndex = parentIndex;
            Head = head;
            Tail = tail;
            Roll = roll;
        }

        [Key(0)] public string Name { get; }
        [Key(1)] public int ParentIndex { get; }
        [Key(2)] public Vector3D Head { get; }
        [Key(3)] public Vector3D Tail { get; }
        [Key(4)] public double Roll { get; }
    }

    [MessagePackObject]
    public class Mesh
    {
        public Mesh(string name, ImmutableArray<Vertex> vertices, ImmutableArray<Vector2> textureVertices,
            ImmutableArray<double> boneWeights, ImmutableArray<Edge> edges,
            ImmutableArray<Face> faces)
        {
            Name = name;
            Vertices = vertices;
            TextureVertices = textureVertices;
            BoneWeights = boneWeights;
            Edges = edges;
            Faces = faces;
        }

        [Key(0)] public string Name { get; }

        [Key(1)] public ImmutableArray<Vertex> Vertices { get; }

        [Key(2)] public ImmutableArray<Vector2> TextureVertices { get; }

        /// <summary>
        ///     Flattened two dimensional array.
        /// </summary>
        [Key(3)]
        public ImmutableArray<double> BoneWeights { get; }

        [Key(4)] public ImmutableArray<Edge> Edges { get; }

        [Key(5)] public ImmutableArray<Face> Faces { get; }
    }

    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Vertex
    {
        public Vertex(Vector3D coordinates, Vector3D normal)
        {
            Coordinates = coordinates;
            Normal = normal;
        }

        [Key(0)] public readonly Vector3D Coordinates;
        [Key(1)] public readonly Vector3D Normal;
    }

    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Edge
    {
        public Edge(int vertexAIndex, int vertexBIndex, GpuBool isAlwaysDisplayed)
        {
            VertexAIndex = vertexAIndex;
            VertexBIndex = vertexBIndex;
            IsAlwaysDisplayed = isAlwaysDisplayed;
            Pad = 0;
        }

        [Key(0)] public readonly int VertexAIndex;
        [Key(1)] public readonly int VertexBIndex;
        [Key(2)] public readonly GpuBool IsAlwaysDisplayed;
        [IgnoreMember, UsedImplicitly] private readonly int Pad;
    }

    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Face
    {
        public Face(int vertexAIndex, int textureVertexAIndex, int vertexBIndex, int textureVertexBIndex,
            int vertexCIndex, int textureVertexCIndex, GpuBool isFilled)
        {
            VertexAIndex = vertexAIndex;
            VertexBIndex = vertexBIndex;
            VertexCIndex = vertexCIndex;
            TextureVertexAIndex = textureVertexAIndex;
            TextureVertexBIndex = textureVertexBIndex;
            TextureVertexCIndex = textureVertexCIndex;
            IsFilledA = IsFilledB = IsFilledC = isFilled;
            PadA = PadB = PadC = 0;
        }


        [Key(0)] public readonly int VertexAIndex;
        [Key(1)] public readonly int TextureVertexAIndex;
        [IgnoreMember, UsedImplicitly] private readonly GpuBool IsFilledA;
        [IgnoreMember, UsedImplicitly] private readonly int PadA;

        [Key(2)] public readonly int VertexBIndex;
        [Key(3)] public readonly int TextureVertexBIndex;
        [IgnoreMember, UsedImplicitly] private readonly GpuBool IsFilledB;
        [IgnoreMember, UsedImplicitly] private readonly int PadB;

        [Key(4)] public readonly int VertexCIndex;
        [Key(5)] public readonly int TextureVertexCIndex;
        [IgnoreMember, UsedImplicitly] private readonly GpuBool IsFilledC;
        [IgnoreMember, UsedImplicitly] private readonly int PadC;

        // ReSharper disable once ConvertToAutoProperty
        [Key(6), UsedImplicitly] public GpuBool IsFilled => IsFilledA;
    }
}
