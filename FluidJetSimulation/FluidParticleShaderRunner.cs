using ComputeSharp;
using ComputeSharp.WinUI;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidJetSimulation;
public class FluidParticleShaderRunner : IShaderRunner {
    public FluidParticleShaderRunner() {
    }

    private float GetVelocity2(SimulationParticle p) => p.velocityY * p.velocityY + p.velocityX * p.velocityX;

    public bool TryExecute(IReadWriteNormalizedTexture2D<float4> texture, TimeSpan timespan, object parameter) {
        var sim = IOCContainer.Instance.GetInstance<IFluidJetSimmulator>(InstanceType.Singleton);
        var arr = sim.SimulationParticles.ToArray();
        if (arr.Length <= 0)
            arr = new SimulationParticle[1] { new(-1000, -1000, 0, 0) };

        var min = arr[0];
        var max = arr[0];

        foreach (var item in arr) {
            if (GetVelocity2(item)<GetVelocity2(min))
                min = item;
            if(GetVelocity2(item)>GetVelocity2(max))
                max = item;
        }

        using ReadOnlyBuffer<SimulationParticle> buffer = GraphicsDevice.GetDefault().AllocateReadOnlyBuffer(arr);
        texture.GraphicsDevice.ForEach(texture, new FluidParticleShader(buffer, sim.Scale, sim.ParticleRenderRadius, GetVelocity2(min), GetVelocity2(max)));
        return true;
    }
}
