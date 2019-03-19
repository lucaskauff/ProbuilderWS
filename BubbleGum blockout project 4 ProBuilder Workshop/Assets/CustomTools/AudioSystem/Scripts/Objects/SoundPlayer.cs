using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] protected Transform target;
        [SerializeField] protected Sound sound = null;

        protected virtual void Awake() {
            if(target == null){
                sound?.Init(GetComponent<Transform>());
            }
            else{
                sound?.Init(target);
            }
            
        }
    }
}


