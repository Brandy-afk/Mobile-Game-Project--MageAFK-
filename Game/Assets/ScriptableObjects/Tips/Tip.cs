using System.Collections;
using System.Collections.Generic;
using MageAFK.UI;
using UnityEngine;

namespace MageAFK.Tutorial
{
    public abstract class Tip : ScriptableObject
    {
        [SerializeField] private UIPanel target;
        public UIPanel Target => target;
        public abstract string ReturnTitle();
        public abstract string ReturnDesc();
    }
}
