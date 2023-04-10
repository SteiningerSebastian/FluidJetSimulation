using ComputeSharp;
using ComputeSharp.WinUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluidJetSimulation;
public class FluidParticleShaderRunner : IShaderRunner {
    private CancellationToken ct;
    public FluidParticleShaderRunner(CancellationToken ct = default) {
        this.ct = ct;
    }

    public FluidParticleShaderRunner() {
        this.ct = default;
    }

    private float GetVelocity2(float4 p) => p.Z * p.Z + p.W * p.W;

    private float GetWindVelocity2(float4 p) => p.X * p.X + p.Y * p.Y;


    private (float, float) GetMinMaxParticleVelocity(List<float4> list) {
        float min = float.MaxValue;
        float max = float.MinValue;

        Parallel.For(0, list.Count, (i) => {
            try {
                if (i < list.Count) {
                    var p = list[i];
                    var v = GetVelocity2(p);
                    if (min > v)
                        Interlocked.Exchange(ref min, v);
                    if (max < v)
                        Interlocked.Exchange(ref max, v);
                }
            }
            catch (Exception e) {

            }

        });

        return (min, max);
    }

    private (float, float) GetMinMaxWindVelocity(ReadWriteTexture2D<float4> texture) {
        float min = float.MaxValue;
        float max = float.MinValue;

        var textureArr = texture.ToArray();

        Parallel.For(0, texture.Width * texture.Height, (i) => {
            try {
                var p = textureArr[i % texture.Width, i / texture.Width];
                var v = GetWindVelocity2(p);
                if (min > v)
                    Interlocked.Exchange(ref min, v);
                if (max < v)
                    Interlocked.Exchange(ref max, v);
            }
            catch (Exception e) {

            }

        });

        return (min, max);
    }

    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object parameter) {
        if(ct.IsCancellationRequested) 
            return false;

        var sim = IOCContainer.Instance.GetInstance<IFluidJetSimmulator>(InstanceType.Singleton);
        Span<float4> span = CollectionsMarshal.AsSpan(sim.SimulationParticles);
        if (span.Length <= 0)
            span = new float4[] { new(-1000, -1000, 0, 0) };

        var minmax = GetMinMaxParticleVelocity(sim.SimulationParticles);
        var minmaxWind = GetMinMaxWindVelocity(sim.WindTexture);

        using (ReadOnlyBuffer<float4> buffer = GraphicsDevice.GetDefault().AllocateReadOnlyBuffer<float4>(span)) {
            texture.GraphicsDevice.ForEach(texture, new FluidParticleShader(buffer, sim.WindTexture, sim.Scale, sim.ParticleRenderRadius, minmax.Item1, minmax.Item2, minmaxWind.Item1, minmaxWind.Item2)); ;
            return true;
        }
    }
}
