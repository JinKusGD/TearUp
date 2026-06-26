using UnityEngine;

public class HealthPosion : MonoBehaviour
{
    [SerializeField] private float value  = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

         var instanceable = other.GetComponent<IInstanceable>();

        ObjectManager.Instance.RequestHealByInstanceId(instanceable.InstanceId, value);

        gameObject.SetActive(false);
    }
}
