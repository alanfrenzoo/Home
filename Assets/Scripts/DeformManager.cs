using Deform;
using EasyBuildSystem.Runtimes.Extensions;
using EasyBuildSystem.Runtimes.Internal.Builder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformManager : MonoBehaviour
{
    public static DeformManager instance;

    public SquashAndStretchDeformer squashDeformer;
    private Animator animator;

    void Start()
    {
        instance = this;
        animator = transform.GetComponentInChildren<Animator>();
        squashDeformer = transform.GetComponentInChildren<SquashAndStretchDeformer>();
    }

    void Update()
    {
        if (BuilderBehaviour.Instance.CurrentPreview)
            animator.transform.localPosition = BuilderBehaviour.Instance.CurrentPreview.transform.position + new Vector3(0f, 0.3f, 0f);
    }

    public void OneShotAnimation()
    {
        BuilderBehaviour.Instance.CurrentPreview.gameObject.ChangeAllMaterialsInChildren(BuilderBehaviour.Instance.CurrentPreview.Renderers.ToArray(), BuilderBehaviour.Instance.CurrentPreview.InitialsRenders);
        StartCoroutine(WaitForAnimation());
    }
    
    private IEnumerator WaitForAnimation()
    {
        animator.Play("squash");

        yield return new WaitForSeconds(animator.runtimeAnimatorController.animationClips[0].averageDuration);

        ItemManager.instance.PlaceItem();
    }
}
