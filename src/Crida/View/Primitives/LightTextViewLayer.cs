using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Crida.Asset;
using Crida.Shared.Utilities;
using Crida.States;
using Crida.Utilities.Veldrid;
using HardFuzz.HarfBuzz;
using HardFuzz.HarfBuzz.Blob;
using HardFuzz.HarfBuzz.Face;
using HardFuzz.HarfBuzz.Font;
using HardFuzz.HarfBuzz.Shape;
using JetBrains.Annotations;
using NLog;
using Veldrid;
using Buffer = HardFuzz.HarfBuzz.Buffer.Buffer;

namespace Crida.View.Primitives
{
    // todo cache
    public class LightTextViewLayer : IViewLayer
    {
        private const int HarfBuzzOversampling = 64;
        private string _content;
        private volatile bool _isContentDirty;

        public LightTextViewLayer(GraphicsState graphics, AssetFactoryState assets, int x, int y, int width, int height,
            LightFont font, Feature[] features, double size, double lineHeight, RgbaFloat color, string content)
        {
            Graphics = graphics;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Font = font;
            Features = features;
            Size = size;
            LineHeight = lineHeight;
            Color = color;

            var rf = graphics.Device.ResourceFactory;
            GlobalBuffer = new StructBuffer<GlobalParameters>(Graphics, BufferUsage.UniformBuffer,
                ShaderStages.Vertex | ShaderStages.Fragment);
            InstanceBuffer =
                new ArrayBuffer<InstanceParameters>(Graphics, BufferUsage.UniformBuffer, 4096, ShaderStages.Vertex);
            Pipeline = new GraphicsPipeline(graphics, Blend.Alpha, DepthTest.Off, false,
                assets.Get<ShaderSet>("Shaders.LightTextViewLayer"), graphics.IntermediateOutputDescription,
                GlobalBuffer, InstanceBuffer, graphics.RandomnessTexture, font.GlyphInfoBuffer, font.TextureArray);
            Commands = new Commands(Graphics);

            _content = content;
            _isContentDirty = true;

            HarfBuzzBlob = new Blob(Font.RawFont);
            HarfBuzzFace = new Face(HarfBuzzBlob);
            HarfBuzzFont = new Font(HarfBuzzFace);
            var quantizedSize = (int) Math.Round(Size * HarfBuzzOversampling);
            HarfBuzzFont.Scale = (quantizedSize, quantizedSize);
        }

        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        private GraphicsState Graphics { get; }

        // todo caching render
        private int X { get; }
        private int Y { get; }
        private int Width { get; }
        private int Height { get; }
        private LightFont Font { get; }
        private Feature[] Features { get; }
        private double Size { get; }
        private double LineHeight { get; }
        private RgbaFloat Color { get; }

        public string Content
        {
            get => _content;
            set
            {
                if (_content == value) return;
                _content = value;
                _isContentDirty = true;
            }
        }

        private StructBuffer<GlobalParameters> GlobalBuffer { get; }
        private ArrayBuffer<InstanceParameters> InstanceBuffer { get; }
        private int InstanceCount { get; set; }
        private GraphicsPipeline Pipeline { get; }
        private Commands Commands { get; }

        public async Task Draw(Framebuffer target)
        {
            if (_isContentDirty) Layout();

            GlobalBuffer.Update(new GlobalParameters((float) (Size / Font.PrerenderSize), Graphics.RandomnessSeed,
                Width, Height, 2048, 2048, Color));
            Commands.Submit(x =>
            {
                x.SetFramebuffer(target);
                x.SetViewport(0, new Viewport(X, Y, Width, Height, 0, 1));
                x.SetPipeline(Pipeline);
                x.SetIndexBuffer(Graphics.SingleRectIndexBuffer);
                x.DrawIndexed(6, (uint) InstanceCount, 0, 0, 0);
            });
        }

        public void Dispose()
        {
            HarfBuzzFont.Dispose();
            HarfBuzzFace.Dispose();
            HarfBuzzBuffer.Dispose();
            HarfBuzzBlob.Dispose();
            Commands.Dispose();
            Pipeline.Dispose();
            InstanceBuffer.Dispose();
        }

        private Buffer HarfBuzzBuffer { get; } = new Buffer();
        private Blob HarfBuzzBlob { get; }
        private Face HarfBuzzFace { get; }
        private Font HarfBuzzFont { get; }

        private void Layout()
        {
            // harfbuzz is NOT THREAD SAFE so the resources are created over and over
            // or else you get random AccessViolationException after ~10 min
            // todo submit issue to harfbuzz
            var instances = new List<InstanceParameters>();
            var y = 0d;
            foreach (var paragraph in Content.Normalize().Split('\n'))
            {
                HarfBuzzBuffer.Reset();
                HarfBuzzBuffer.AddUtf(paragraph);
                HarfBuzzBuffer.GuessSegmentProperties();
                HarfBuzzBuffer.Shape(HarfBuzzFont, Features);
                var lines = HarfBuzzBuffer.GlyphInfos.Zip(HarfBuzzBuffer.GlyphPositions)!
                    .OverflowSplit(tuple => tuple.Second.XAdvance, Width * HarfBuzzOversampling).ToArray();
                instances.AddRange(lines.SelectMany((line, lineIndex) => line.Select(glyph => new InstanceParameters(
                    (int) glyph.value.First.Codepoint,
                    (glyph.offset + glyph.value.Second.XOffset) / (float) HarfBuzzOversampling,
                    (float) y + (float) ((lineIndex + 1) * LineHeight) +
                    glyph.value.Second.YOffset / (float) HarfBuzzOversampling
                ))));
                y += lines.Length * LineHeight;
            }

            InstanceCount = instances.Count;
            InstanceBuffer.Update(instances.ToImmutableArray());
            _isContentDirty = false;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private readonly struct GlobalParameters
        {
            public readonly float Scaling;
            public readonly float RandomSeed;
            [UsedImplicitly] private readonly int Pad;
            [UsedImplicitly] private readonly int Pad2;
            public readonly int ViewportWidth;
            public readonly int ViewportHeight;
            public readonly int TextureWidth;
            public readonly int TextureHeight;
            public readonly RgbaFloat Color;

            public GlobalParameters(float scaling, float randomSeed, int viewportWidth, int viewportHeight,
                int textureWidth, int textureHeight, RgbaFloat color)
            {
                Scaling = scaling;
                RandomSeed = randomSeed;
                Pad = Pad2 = 0;
                ViewportWidth = viewportWidth;
                ViewportHeight = viewportHeight;
                TextureWidth = textureWidth;
                TextureHeight = textureHeight;
                Color = color;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private readonly struct InstanceParameters
        {
            public InstanceParameters(int index, float positionX, float positionY)
            {
                Index = index;
                Pad = 0;
                PositionX = positionX;
                PositionY = positionY;
            }

            public readonly int Index;
            [UsedImplicitly] private readonly int Pad;
            public readonly float PositionX;
            public readonly float PositionY;
        }
    }
}
