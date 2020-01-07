﻿using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class DestroySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAllReadOnly<DestroyTag>().ForEach(
            (Entity destroy_id, ref Translation destroy_tranlation, ref DestroyTag destroy_index) =>
            {
                var t = destroy_tranlation;
                var i = destroy_index;

                Entities.WithAllReadOnly<FurniTag>().ForEach(
                    (Entity furni_id, ref Translation furni_tranlation, ref Rotation rot, ref FurniTag index) =>
                    {
                        if (t.Equals(furni_tranlation) && i.Value == index.Value)
                        {
                            BuilderBehaviour.Instance.SelectPrefab(BuildManager.Instance.PartsCollection.Parts[index.Value]);
                            BuilderBehaviour.Instance.ChangeMode(BuildMode.Placement);

                            BuilderBehaviour.Instance.CreatePreview(BuilderBehaviour.Instance.SelectedPrefab.gameObject);
                            BuilderBehaviour.Instance.CurrentPreview.transform.position = ItemManager.instance.TargetCollider.transform.position;
                            var y = ItemManager.instance.TargetCollider.transform.rotation.eulerAngles.y - BuilderBehaviour.Instance.CurrentPreview.RotationAxis.y - BuilderBehaviour.Instance.CurrentRotationOffset.y;
                            var rotateAxis = new Vector3(0f, y, 0f);
                            BuilderBehaviour.Instance.RotatePreview(rotateAxis);

                            ItemManager.instance.TargetEditingItem = BuilderBehaviour.Instance.CurrentPreview.gameObject;
                            ItemManager.instance.TargetItemIndex = index.Value;
                            EntityManager.DestroyEntity(furni_id); //destroy-wise with all its children
                            EntityManager.DestroyEntity(destroy_id);

                        }

                    }
                );

            }
        );

    }

}
