using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using EasyBuildSystem.Runtimes.Internal.Managers;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("Easy Build System/Features/Builders Behaviour/Android Builder Behaviour")]
public class AndroidBuilderBehaviour : BuilderBehaviour
{
    public override void Start()
    {
        base.Start();

        if (BuildManager.Instance == null)
            return;

        if (BuildManager.Instance.PartsCollection != null)
            SelectPrefab(BuildManager.Instance.PartsCollection.Parts[0]);
    }

    public override void Update()
    {
        // pointerId default: -1
        // 0 is the first finger, 1 is the second finger
        if (EventSystem.current && (EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject(1) || EventSystem.current.IsPointerOverGameObject()))
            return;

        base.Update();
    }
}