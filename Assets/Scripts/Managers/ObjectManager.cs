using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }

    [SerializeField] private GameObject[] _gameObject;

    private int _nextInstanceId = 1;

    private Dictionary<int, GameObject> gameObjectList = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{gameObject.name}] 이미 ObjectManager 인스턴스가 존재하여 생성된 오브젝트를 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        foreach (GameObject instance in _gameObject)
        {
            if(!instance.TryGetComponent(out IInstanceable instanceable))
            {
                continue;
            }

            instanceable.SetInstanceId(_nextInstanceId);
            gameObjectList[_nextInstanceId] = instance;
            _nextInstanceId++;
        }
    }

    public GameObject GetObjectByInstanceId(int instanceId)
    {
        GameObject targetObject = gameObjectList[instanceId];

        return targetObject;
    }

    public void RequestHitDamageByInstanceId(int instanceId, float damage)
    {
        GameObject targetObject = gameObjectList[instanceId];

        if (targetObject== null )
        {
            return;
        }

        var component =  targetObject.GetComponent<ITakeDamageable>();

        component.TakeDamage(damage);
    }

    public void RequestHealByInstanceId(int instanceId, float value)
    {
        GameObject targetObject = gameObjectList[instanceId];

        if (targetObject== null )
        {
            return;
        }

        var component =  targetObject.GetComponent<ITakeDamageable>();

        component.Heal(value);
    }
}