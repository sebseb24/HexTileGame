using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayTargetAnimation(string targetAnim)
    {
        anim.CrossFade(targetAnim, 0.2f);
    }
}
