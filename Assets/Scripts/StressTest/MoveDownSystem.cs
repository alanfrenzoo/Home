using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveDownSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // If a MoveUp component is present, then the system updates the Translation component to move the entity upwards.
        // Once the entity reaches a predetermined height, the function removes the MoveUp component.
        Entities.WithAllReadOnly<MoveDownTag, MovingTag>().ForEach(
            (Entity id, ref Translation translation) =>
            {
                var deltaTime = Time.deltaTime;
                translation = new Translation()
                {
                    Value = new float3(translation.Value.x, translation.Value.y, translation.Value.z - deltaTime)
                };

                if (translation.Value.z < -50.0f)
                    EntityManager.RemoveComponent<MovingTag>(id);
            }
        );

        // If an entity does not have a MoveUp component (but does have a Translation component),
        // then the system moves the entity down to its starting point and adds a MoveUp component.
        Entities.WithAllReadOnly<MoveDownTag>().WithNone<MovingTag>().ForEach(
            (Entity id, ref Translation translation) =>
            {
                translation = new Translation()
                {
                    Value = new float3(translation.Value.x, translation.Value.y, 50f)
                };

                EntityManager.AddComponentData(id, new MovingTag());
            }
        );
    }
}
