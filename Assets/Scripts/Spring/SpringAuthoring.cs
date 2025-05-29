using Unity.Entities;
using UnityEngine;

class SpringAuthoring : MonoBehaviour
{
    public float TargetPos;
    public float AngularFrequency;
    public float DampingRatio;
}

class SpringAuthoringBaker : Baker<SpringAuthoring>
{
    public override void Bake(SpringAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new SpringData
        {
            TargetPos =  authoring.TargetPos,
            Frequency = authoring.AngularFrequency,
            DampingRatio = authoring.DampingRatio
        });
    }
}
