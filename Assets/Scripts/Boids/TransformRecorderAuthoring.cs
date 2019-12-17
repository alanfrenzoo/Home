using System;
using Samples.Boids;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

// Demonstrate taking some data available in editor about GameObjects and save in
// a runtime format suitable for Component system updates.
// - Playback first attached animation clip (only expect one)
// - Record positions and rotations at specified rate
// - Store samples into DynamicBuffer

[AddComponentMenu("DOTS Samples/Boids/TransformRecorder")]
[ConverterVersion("macton", 2)]
public class TransformRecorderAuthoring : MonoBehaviour,  IConvertGameObjectToEntity
{
    [Range(2,120)]
    public int SamplesPerSecond = 60;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var animationClip = gameObject.GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
        var lengthSeconds = animationClip.length;
        var sampleRate = 1.0f / SamplesPerSecond;
        var frameCount = (int)(lengthSeconds / sampleRate);
        if (frameCount < 2) // Minimum two frames of animation to capture.
        {
            return;
        }

        var s = 0.0f;

        var blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var transformSamplesBlob = ref blobBuilder.ConstructRoot<TransformSamples>();
        var translationSamples = blobBuilder.Allocate(ref transformSamplesBlob.TranslationSamples, frameCount);
        var rotationSamples = blobBuilder.Allocate(ref transformSamplesBlob.RotationSamples, frameCount);
        var scaleSamples = blobBuilder.Allocate(ref transformSamplesBlob.ScaleSamples, frameCount);

        for (int i = 0; i < frameCount; i++)
        {
            animationClip.SampleAnimation(gameObject, s);

            translationSamples[i] = gameObject.transform.position;
            rotationSamples[i] = gameObject.transform.rotation;
            scaleSamples[i] = gameObject.transform.localScale;

            s += sampleRate;
        }

        dstManager.AddComponentData(entity, new SampledAnimationClip
        {
            FrameCount = frameCount,
            SampleRate = sampleRate,
            CurrentTime = 0.0f,
            FrameIndex = 0,
            TimeOffset = 0,
            TransformSamplesBlob = blobBuilder.CreateBlobAssetReference<TransformSamples>(Allocator.Persistent)
        });

        dstManager.AddComponentData(entity, new SampledAnimationClipStartTag { Value = 0 });
        
        blobBuilder.Dispose();
    }
}