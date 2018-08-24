using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.UI;

namespace UIMessageManagement
{


    [RequireComponent(typeof(Text))]
    public class UIMessageArea : MonoBehaviour
    {


        public string areaName = "Alert";

        [Space(10)]
        public MessageEntranceAnimation entranceAnimation = MessageEntranceAnimation.Fade;
        public MessageExitAnimation exitAnimation = MessageExitAnimation.Fade;
        public MessageMode mode = MessageMode.Timer;


        [Space(10)]
        public float entranceTime = 1.0f;
        public float displayTime = 3.0f;
        public float exitTime = 1.0f;



        [HideInInspector]

        //[Space(10)]
        public Text text;



        void Awake()
        {
            text = GetComponent<Text>();


        }


        void Start()
        {


            UIMessageManager.Instance.AddMessgaeArea(this);

        }
        
    }

}