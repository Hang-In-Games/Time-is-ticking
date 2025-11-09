using System;
using UnityEngine;

public class TimeEventTarget : MonoBehaviour
{
   [SerializeField] private string key;
   
   public string Key => key;
   
   public void InvokeTimeEvent(TimeEventDefine define)
   {
      if (key.Equals(define.targetName) == false)
         return;
      
      switch (define.timeEventType)
      {
         case TimeEventType.Show:
            gameObject.SetActive(true);
            break;
         case TimeEventType.Hide:
            gameObject.SetActive(false);
            break;
      }
   }

}
