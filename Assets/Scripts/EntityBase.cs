using UnityEngine;

public class EntityBase : MonoBehaviour, IInstanceable
{
    public int InstanceId { get; private set; }

    public void SetInstanceId(int instanceId)
    {
        if (instanceId == 0 || InstanceId != 0)
        {
            return;
        }

        InstanceId = instanceId;
    }
}
