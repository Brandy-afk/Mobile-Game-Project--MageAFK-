using System.Collections.Generic;
using MageAFK.Core;
using MageAFK.Items;
using MageAFK.Management;
using UnityEngine;

namespace MageAFK.Pooling
{
    public class ItemPooler : AbstractPooler
    {

        [SerializeField] private Transform parent;

        private Dictionary<ItemIdentification, (GameObject, List<GameObject>)> pool;

        protected override void RegisterSelf() => ServiceLocator.RegisterService(this);
        protected override void OnWaveStateChanged(WaveState state)
        {
            if (state == WaveState.Wave)
                Create();
            else if (state == WaveState.Intermission)
                Clear();
        }

        protected override void Pool()
        {
            if (pool == null) pool = new Dictionary<ItemIdentification, (GameObject, List<GameObject>)>();

            foreach (var values in ServiceLocator.Get<GearHandler>().ReturnPoolableGear())
            {
                pool[values.iD] = (values.prefab.ReturnPrefab(), new List<GameObject>());
                CreateObject(values.iD);
            }
        }

        public GameObject Get(ItemIdentification iD)
        {
            if (pool != null && pool.ContainsKey(iD))
            {
                var list = pool[iD].Item2;
                for (int i = 0; i < list.Count; i++)
                {
                    if (!list[i].activeInHierarchy)
                    {
                        return list[i];
                    }
                }

                //If object not found
                return CreateObject(iD);
            }
            return null;
        }

        private GameObject CreateObject(ItemIdentification iD)
        {
            var newObj = Instantiate(pool[iD].Item1, parent);
            newObj.SetActive(false);
            pool[iD].Item2.Add(newObj);
            return newObj;
        }

        protected override void Clear()
        {
            if (pool == null || pool.Count <= 0) return;
            foreach (var item in pool)
            {
                foreach (var obj in item.Value.Item2)
                {
                    Destroy(obj);
                }
            }
            pool.Clear();
        }
    }
}
