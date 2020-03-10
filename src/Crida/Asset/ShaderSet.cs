using System;
using System.Linq;
using System.Text.RegularExpressions;
using Crida.Shared.RawAsset;
using Crida.States;
using Veldrid;
using Veldrid.SPIRV;

namespace Crida.Asset
{
    public class ShaderSet : IManagedAsset
    {
        public ShaderSet(AssetFactoryState assets, RawShaderSet raw)
        {
            var graphics = assets.Graphics;

            Shader? Compile(ShaderStages stage, string? glsl)
            {
                return glsl == null
                    ? null
                    : graphics.Device.ResourceFactory.CreateShader(new ShaderDescription(stage,
                        SpirvCompilation.CompileGlslToSpirv(glsl, stage.ToString(), stage,
                            GlslCompileOptions.Default).SpirvBytes, "main"));
            }

            Vertex = Compile(ShaderStages.Vertex, raw.Vertex);
            Geometry = Compile(ShaderStages.Geometry, raw.Geometry);
            TessellationControl = Compile(ShaderStages.TessellationControl, raw.TessellationControl);
            TessellationEvaluation = Compile(ShaderStages.TessellationEvaluation, raw.TessellationEvaluation);
            Fragment = Compile(ShaderStages.Fragment, raw.Fragment);
            Compute = Compile(ShaderStages.Compute, raw.Compute);
            var vertexElements = Vertex == null
                ? new VertexElementDescription[0]
                : VertexInputPattern.Matches(raw.Vertex!).Select(x =>
                    new VertexElementDescription(x.Groups["name"].Value, VertexElementSemantic.Position,
                        x.Groups["type"].Value switch
                        {
                            "ivec4" => VertexElementFormat.Int4,
                            _ => throw new NotImplementedException()
                        }, uint.Parse(x.Groups["offset"].Value)
                    )).ToArray();
            Description = new ShaderSetDescription(
                vertexElements.Any()
                    ? new[] {new VertexLayoutDescription(vertexElements)}
                    : new VertexLayoutDescription[0],
                new[] {Vertex, Geometry, TessellationControl, TessellationEvaluation, Fragment, Compute}
                    .Where(x => x != null).ToArray());
        }

        private static Regex VertexInputPattern { get; } = new Regex(
            @"layout *\(location = (?<offset>[0-9]+)\) *in +(?<type>[a-zA-Z0-9]+) +(?<name>[^ ]+) *;",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private Shader? Vertex { get; }
        private Shader? Geometry { get; }
        private Shader? TessellationControl { get; }
        private Shader? TessellationEvaluation { get; }
        private Shader? Fragment { get; }
        private Shader? Compute { get; }
        public ShaderSetDescription Description { get; }

        public void Dispose()
        {
            Vertex?.Dispose();
            Geometry?.Dispose();
            TessellationControl?.Dispose();
            TessellationEvaluation?.Dispose();
            Fragment?.Dispose();
            Compute?.Dispose();
        }
    }
}
