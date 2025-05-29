using Unity.Entities;
using UnityEngine;

class AbilityAuthoring : MonoBehaviour
{
    public float dodgeDuration;
}

class AbilityAuthoringBaker : Baker<AbilityAuthoring>
{
    public override void Bake(AbilityAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new AbilityData
        {
            dodgeDuration = authoring.dodgeDuration,
        });
    }
}
