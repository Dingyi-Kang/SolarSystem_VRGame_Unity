using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ModifyConstraintValues : MonoBehaviour
{
    public Animator animator;
    public string parameter;

    public ScaleConstraint constraint;

    public void Grabbed()
    {
        // Obtain, reconstruct, and set the list of constraint sources with opposite weights.
        List<ConstraintSource> sources = new List<ConstraintSource>();
        constraint.GetSources(sources);
        List<ConstraintSource> newSources = new List<ConstraintSource>()
        {
            new ConstraintSource(){ sourceTransform = sources[0].sourceTransform, weight = 0f },
            new ConstraintSource(){ sourceTransform = sources[1].sourceTransform, weight = 1f }
        };
        constraint.SetSources(newSources);

        // Make the planet stop spinning
        animator.SetBool(parameter, false);
    }

    public void Released()
    {
        // Obtain, reconstruct, and set the list of constraint sources with opposite weights.
        List<ConstraintSource> sources = new List<ConstraintSource>();
        constraint.GetSources(sources);
        List<ConstraintSource> newSources = new List<ConstraintSource>()
        {
            new ConstraintSource(){ sourceTransform = sources[0].sourceTransform, weight = 1f },
            new ConstraintSource(){ sourceTransform = sources[1].sourceTransform, weight = 0f }
        };
        constraint.SetSources(newSources);

        // Make the planet resume spinning
        animator.SetBool(parameter, true);
    }
}
