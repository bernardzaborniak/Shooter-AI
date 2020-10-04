using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour {

    #region Singleton

    public static PoolingManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [System.Serializable]
    public class AmmoPool
    {
        public AmmoType ammoType;
        public GameObject prefab;
        public int size;
    }

    /*[System.Serializable]
    public class MagPool
    {
        public GunModel gunModel;
        public GameObject prefab;
        public int size;
    }*/

    public Dictionary<AmmoType, Queue<GameObject>> ammoPoolsDictionary;
    public List<AmmoPool> ammoPools;

    //public Dictionary<GunModel, Queue<GameObject>> magPoolsDictionary;
   // public List<MagPool> magPools;


    void Start ()
    {
        ammoPoolsDictionary = new Dictionary<AmmoType, Queue<GameObject>>();

        foreach(AmmoPool pool in ammoPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            ammoPoolsDictionary.Add(pool.ammoType, objectPool);
        }


       // magPoolsDictionary = new Dictionary<GunModel, Queue<GameObject>>();

       /* foreach (MagPool pool in magPools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            magPoolsDictionary.Add(pool.gunModel, objectPool);
        }*/
    }

    public GameObject SpawnAmmoFromPool (AmmoType tag, Vector3 position, Quaternion rotation)
    {
        if (!ammoPoolsDictionary.ContainsKey(tag))
        {
            Debug.Log("wrong Tag");
            return null;
        }

        GameObject objectToSpawn = ammoPoolsDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
     

        ammoPoolsDictionary[tag].Enqueue(objectToSpawn); //now its at the end of the queue

        return objectToSpawn;
    }

    /*public GameObject SpawnMagFromPool(GunModel gunModel, Vector3 position, Quaternion rotation)
    {
        if (!magPoolsDictionary.ContainsKey(gunModel))
        {
            Debug.Log("wrong Tag");
            return null;
        }

        GameObject objectToSpawn = magPoolsDictionary[gunModel].Dequeue();
        
        //only return mags in world
        while (objectToSpawn.GetComponent<Magazine>().IsInGunOrHand())
        {
            magPoolsDictionary[gunModel].Enqueue(objectToSpawn);
            objectToSpawn = magPoolsDictionary[gunModel].Dequeue();
        }

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        objectToSpawn.GetComponent<Magazine>().Refill();


        magPoolsDictionary[gunModel].Enqueue(objectToSpawn); //now its at the end of the queue

        return objectToSpawn;
    }*/


}
