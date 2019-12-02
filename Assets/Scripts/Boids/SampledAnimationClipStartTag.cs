using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Samples.Boids
{
    public struct SampledAnimationClipStartTag : IComponentData
    {
        public int Value;
    }

}
