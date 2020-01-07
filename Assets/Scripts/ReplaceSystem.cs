using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class ReplaceSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<ReplaceTag>().ForEach(
            (Entity replace_id, ref Translation replace_tranlation, ref ReplaceTag replace_index) =>
            {
                var t = replace_tranlation;
                var i = replace_index;

                Entities.WithAllReadOnly<FloorTag>().ForEach(
                    (Entity floor_id, ref Translation floor_tranlation, ref Rotation rot) =>
                    {
                        if (t.Equals(floor_tranlation))
                        {
                            var mesh = BuildManager.Instance.PartsCollection.Parts[i.Value].gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
                            var mat = BuildManager.Instance.PartsCollection.Parts[i.Value].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                            EntityManager.SetSharedComponentData(floor_id, new RenderMesh 
                            {
                                mesh = mesh,
                                material = mat,
                                castShadows = UnityEngine.Rendering.ShadowCastingMode.On,
                                receiveShadows = true
                            }); 

                            EntityManager.DestroyEntity(replace_id);

                        }

                    }
                );

            }
        );

    }

}
