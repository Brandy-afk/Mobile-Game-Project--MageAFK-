using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MageAFK.Management
{
    public abstract class UIData<Data, Content> : ScriptableObject, IData<Data>
    {
        protected event Action<Content> DataAltered;
        protected virtual void InvokeDataAltered(Content data) => DataAltered?.Invoke(data);
        public virtual void SubToDataEvent(Action<Content> handler) => DataAltered += handler;
        public abstract void InitializeData(Data data);
        public abstract Data SaveData();
    }
}
