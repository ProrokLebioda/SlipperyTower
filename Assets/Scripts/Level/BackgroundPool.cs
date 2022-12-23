using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPool : MonoBehaviour
{
    public static BackgroundPool Instance;

    private List<GameObject> pooledObjects = new List<GameObject>();
    [SerializeField]
    private int amountToPool = 4;

    [SerializeField]
    private GameObject backgroundPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(backgroundPrefab);
            obj.SetActive(false); 
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledGameObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
