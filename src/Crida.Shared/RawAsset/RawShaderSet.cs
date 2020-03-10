using MessagePack;

namespace Crida.Shared.RawAsset
{
    [MessagePackObject]
    public class RawShaderSet : IRawAsset
    {
        public RawShaderSet(string? vertex, string? tessellationControl, string? tessellationEvaluation,
            string? geometry, string? fragment, string? compute)
        {
            Vertex = vertex;
            TessellationControl = tessellationControl;
            TessellationEvaluation = tessellationEvaluation;
            Geometry = geometry;
            Fragment = fragment;
            Compute = compute;
        }

        [Key(0)] public string? Vertex { get; }
        [Key(2)] public string? TessellationControl { get; }
        [Key(3)] public string? TessellationEvaluation { get; }
        [Key(1)] public string? Geometry { get; }
        [Key(4)] public string? Fragment { get; }
        [Key(5)] public string? Compute { get; }
    }
}
