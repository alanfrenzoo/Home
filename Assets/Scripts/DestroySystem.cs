using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class DestroySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<DestroyTag>().ForEach(
            (Entity destroy_id, ref Translation destroy_tranlation) =>
            {
                var t = destroy_tranlation;
                Entities.WithAllReadOnly<FurniTag>().ForEach(
                    (Entity furni_id, ref Translation furni_tranlation, ref FurniTag index) =>
                    {
                        if(t.Equals(furni_tranlation))
                        {

                            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[index.Value]);
                            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);

                            // Move to Centre
                            BuilderBehaviour.Instance.CreatePreview(BuilderBehaviour.Instance.SelectedPrefab.gameObject);
                            BuilderBehaviour.Instance.CurrentPreview.transform.position = furni_tranlation.Value;

                            Test_Control.currentFurniIndex = index.Value;
                            EntityManager.DestroyEntity(furni_id); //destroy-wise with all its children
                            EntityManager.DestroyEntity(destroy_id);
                            
                        }

                    }
                );
                
            }
        );

    }
}
