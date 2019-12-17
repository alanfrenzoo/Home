using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Samples.Boids
{
    public struct SampledAnimationClip : IComponentData
    {
        public float SampleRate;
        public int FrameCount;
        
        // Playback State
        public float CurrentTime;
        public int FrameIndex;
        public float TimeOffset;

        public BlobAssetReference<TransformSamples> TransformSamplesBlob;
    }

    [WriteGroup(typeof(Translation))]
    [WriteGroup(typeof(Rotation))]
    [WriteGroup(typeof(NonUniformScale))]
    public struct TransformSamples
    {
        public BlobArray<float3> TranslationSamples;
        public BlobArray<quaternion> RotationSamples;
        public BlobArray<float3> ScaleSamples;
    }
}
