using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;


using UnityEngine.SceneManagement;

using UnityEngine.UI;

namespace UIMessageManagement
{

    public class UIMessageManager : MonoBehaviour
    {



        public static UIMessageManager Instance { get; private set; }



        /// <summary>
        /// areaNameとareaのmap
        /// </summary>
        Dictionary<string, UIMessageArea> messageAreaMap;


        Dictionary<Message, bool> messageMap;

        List<Message> messagesToRemove;

        Dictionary<Text, bool> lastUpdatedTextMap;


        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// MessageAreaを登録します.
        /// </summary>
        /// <param name="areaToAdd"></param>
        public void AddMessgaeArea(UIMessageArea areaToAdd)
        {
            if (messageAreaMap.ContainsKey(areaToAdd.areaName))
            {
                Debug.LogWarning("Such area name is already registed. areaName: " + areaToAdd.areaName);
                return;
            }

            messageAreaMap.Add(areaToAdd.areaName, areaToAdd);
        }


        void OnScenelWasLoaded(Scene scenename, LoadSceneMode SceneMode)
        {
           
            foreach (var messageKeyValue in messageMap)
            {
                var message = messageKeyValue.Key;

                if (message.content.lifespan == MessageLifespan.UntilSceneLoaded)
                {
                    messagesToRemove.Add(message);
                }
            }

            foreach(var messageToRemove in messagesToRemove)
            {
                messageMap.Remove(messageToRemove);
            }


            messagesToRemove.Clear();


        }




        void Awake()
        {


            messageAreaMap = new Dictionary<string, UIMessageArea>();
            messageMap = new Dictionary<Message, bool>();

            messagesToRemove = new List<Message>();

            lastUpdatedTextMap = new Dictionary<Text, bool>();


            SceneManager.sceneLoaded += OnScenelWasLoaded;

            Instance = this;
        }


        void Start()
        {

        }

        void Update()
        {
            UpdateMessages();

            WriteMessages();

            //Debug.Log(messageMap.Count);
        }



        void UpdateMessages()
        {
            foreach (var messageKeyValue in messageMap)
            {
                Message message = messageKeyValue.Key;

                switch (message.controlBlock.state)
                {
                    case MessageState.Ready:


                        break;



                    case MessageState.Start:

                        switch (message.content.entranceAnimation)
                        {
                            case MessageEntranceAnimation.Appear:

                                message.controlBlock.transparency = 255.0f;

                                message.controlBlock.state = MessageState.Showing;

                                break;



                            case MessageEntranceAnimation.Fade:
                                if (message.content.entranceTime > 0)
                                {
                                    message.controlBlock.transparency += (255.0f / message.content.entranceTime) * Time.unscaledDeltaTime;
                                }
                                else
                                {
                                    message.controlBlock.transparency = 255.0f;
                                }

                                if (message.controlBlock.transparency >= 255.0f)
                                {
                                    message.controlBlock.transparency = 255.0f;
                                    message.controlBlock.state = MessageState.Showing;
                                }

                                break;
                        }

                        break; // End MessageState.Start


                    case MessageState.ToEnd:

                        switch (message.content.exitAnimation)
                        {
                            case MessageExitAnimation.Disappear:
                                message.controlBlock.transparency = 0.0f;
                                message.controlBlock.state = MessageState.End;
                                message.controlBlock.isPlaying = false;
                                break;

                            case MessageExitAnimation.Fade:
                                if (message.content.exitTime > 0)
                                {
                                    message.controlBlock.transparency -= (255.0f / message.content.exitTime) * Time.unscaledDeltaTime;
                                }
                                else
                                {
                                    message.controlBlock.transparency = 0.0f;
                                }

                                if (message.controlBlock.transparency <= 0.0f)
                                {
                                    message.controlBlock.transparency = 0.0f;
                                    message.controlBlock.state = MessageState.End;
                                    message.controlBlock.isPlaying = false;
                                }
                                break;
                        }

                        break; // End MessageState.ToEnd



                    case MessageState.Showing:

                        switch (message.content.mode)
                        {
                            case MessageMode.Normal:


                                break;



                            case MessageMode.Timer:

                                if (Time.unscaledTime - message.controlBlock.messageStartTime > message.content.displayTime)
                                {
                                    ExitMessage(message);
                                }

                                break;
                        }

                        break;

                    case MessageState.End:
                        if(message.content.lifespan == MessageLifespan.UntilShowEnd)
                        {
                            messagesToRemove.Add(message);
                        }

                        message.controlBlock.state = MessageState.Ready;
                        break;
                } // End MessageState
            } // End 各Messageにおいて


            // 削除リストにあるMessageを削除
            foreach(var messageToRemove in messagesToRemove)
            {
                messageMap.Remove(messageToRemove);
            }

            messagesToRemove.Clear();

        }


        void WriteMessages()
        {

            // 前フレームで描画したテキスト欄を消去する
            foreach(var textKeyValue in lastUpdatedTextMap)
            {
                textKeyValue.Key.text = "";
            }
            lastUpdatedTextMap.Clear();


            foreach(var messageKeyValue in messageMap)
            {
                Message message = messageKeyValue.Key;

                if (message.controlBlock.isPlaying)
                {

                    int transparencyInt = (int)message.controlBlock.transparency;

                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.Append("<color=#");
                    stringBuilder.Append(((int)(message.content.color.r * 255)).ToString("X2"));
                    stringBuilder.Append(((int)(message.content.color.g * 255)).ToString("X2"));
                    stringBuilder.Append(((int)(message.content.color.b * 255)).ToString("X2"));
                    stringBuilder.Append("{a}>");
                    stringBuilder.Append(message.content.text);
                    stringBuilder.Append("</color>\n");

                    
                    message.controlBlock.messageArea.text.text += stringBuilder.ToString().Replace("{a}", transparencyInt.ToString("X2"));


                    // 更新したTextはMapに登録する.
                    if (!lastUpdatedTextMap.ContainsKey(message.controlBlock.messageArea.text))
                    {
                        lastUpdatedTextMap.Add(message.controlBlock.messageArea.text, true);
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageToExit"></param>
        public void ExitMessage(Message messageToExit)
        {
            if (!messageMap.ContainsKey(messageToExit))
            {
                return;
            }



            if (messageToExit.controlBlock.isPlaying)
            {
                messageToExit.controlBlock.state = MessageState.ToEnd;
            }
        }


        public void SetMessage(Message message)
        {
            if (messageMap.ContainsKey(message))
            {
                return;
            }


            if (!messageAreaMap.ContainsKey(message.content.areaName))
            {
                Debug.LogWarning("Such area name doesn't exist. areaName: " + message.content.areaName);
                return;
            }
            var messageArea = messageAreaMap[message.content.areaName];
            message.controlBlock.messageArea = messageArea;

            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.DisplayTime))
            {

                message.content.displayTime = messageArea.displayTime;
            }


            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.EntranceAnimation))
            {

                message.content.entranceAnimation = messageArea.entranceAnimation;
            }


            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.EntranceTime))
            {

                message.content.entranceTime = messageArea.entranceTime;
            }


            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.ExitAnimation))
            {

                message.content.exitAnimation = messageArea.exitAnimation;
            }


            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.ExitTime))
            {

                message.content.exitTime = messageArea.exitTime;
            }


            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.Mode))
            {

                message.content.mode = messageArea.mode;
            }

            if (message.content.inheritStyleMaskFromArea.ContainsParameter(MessageStyleParameter.Color))
            {

                message.content.color = messageArea.text.color;
            }



            message.controlBlock.isPlaying = false;
            message.controlBlock.state = MessageState.Ready;
            message.controlBlock.messageStartTime = 0.0f;
            message.controlBlock.transparency = 0.0f;

            messageMap.Add(message, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(Message message)
        {
            if (!messageMap.ContainsKey(message))
            {
                SetMessage(message);
            }


            message.controlBlock.isPlaying = true;
            message.controlBlock.messageStartTime = Time.unscaledTime;
            message.controlBlock.state = MessageState.Start;
            message.controlBlock.transparency = 0.0f;
        }

        public void ShowMessageDontOverride(Message message)
        {
            if (!messageMap.ContainsKey(message))
            {
                SetMessage(message);
            }

            if (!message.controlBlock.isPlaying)
            {
                ShowMessage(message);
            }
        }
    }
}