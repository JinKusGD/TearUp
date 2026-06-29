using UnityEngine;

public class HealthPosion : MonoBehaviour
{
    [SerializeField] private float _value  = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

         var instanceable = other.GetComponent<IInstanceable>();

        ObjectManager.Instance.RequestHealByInstanceId(instanceable.InstanceId, _value);

        gameObject.SetActive(false);
    }
}
