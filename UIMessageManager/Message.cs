using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UIMessageManagement
{


    public enum MessageEntranceAnimation
    {
        Appear,
        Fade
    }

    public enum MessageExitAnimation
    {
        Disappear,
        Fade
    }

    public enum MessageMode
    {
        Normal,
        Timer
    }


    public enum MessageLifespan
    {
        UntilShowEnd,
        UntilSceneLoaded,
        UntilGameEnd
    }



    public enum MessageState
    {
        Ready,
        Start,
        Showing,
        ToEnd,
        End
    }

    public enum MessageStyleParameter
    {
        EntranceAnimation = 1,
        ExitAnimation = 2,
        
        EntranceTime = 4,
        DisplayTime = 8,
        ExitTime = 16,

        Mode = 32,

        Color = 64

    }


    public struct MessageStyleParameterMask
    {
        public uint value;

        public void SetParameter(MessageStyleParameter parameter, bool flag)
        {
            if (flag)
            {
                value |= (uint)parameter;
                //Debug.Log(value);
            }
            else
            {
                value &= ~(uint)parameter;
            }
        }


        public bool ContainsParameter(MessageStyleParameter parameter)
        {

            return ((value & (uint)parameter) != 0x00);

        }

        public static MessageStyleParameterMask GetMask(params MessageStyleParameter[] parameters)
        {
            MessageStyleParameterMask mask;

            mask.value = 0x00;


            foreach (var parameter in parameters)
            {
                mask.value |= (uint)parameter;
            }


            return mask;
        }
        



    }


    public class Message
    {
        public Message(string text, string areaName)
        {
            content = new MessageContent();
            controlBlock = new MessageControlBlock();

            content.text = text;
            content.areaName = areaName;

            content.inheritStyleMaskFromArea = MessageStyleParameterMask.GetMask(MessageStyleParameter.DisplayTime,
                MessageStyleParameter.EntranceAnimation,
                MessageStyleParameter.EntranceTime,
                MessageStyleParameter.ExitAnimation,
                MessageStyleParameter.ExitTime,
                MessageStyleParameter.Mode, 
                MessageStyleParameter.Color);
        }

        public Message(string text, string areaName, MessageLifespan lifespan)
        {
            content = new MessageContent();
            controlBlock = new MessageControlBlock();

            content.text = text;
            content.areaName = areaName;

            content.lifespan = lifespan;

            content.inheritStyleMaskFromArea = MessageStyleParameterMask.GetMask(
                MessageStyleParameter.DisplayTime,
                MessageStyleParameter.EntranceAnimation,
                MessageStyleParameter.EntranceTime,
                MessageStyleParameter.ExitAnimation,
                MessageStyleParameter.ExitTime,
                MessageStyleParameter.Mode,
                MessageStyleParameter.Color);
        }

        public Message(string text, string areaName, float displayTime)
        {

            content = new MessageContent();
            controlBlock = new MessageControlBlock();

            content.text = text;
            content.areaName = areaName;
            
            content.displayTime = displayTime;


            content.inheritStyleMaskFromArea = MessageStyleParameterMask.GetMask(
                MessageStyleParameter.EntranceAnimation,
                MessageStyleParameter.EntranceTime,
                MessageStyleParameter.ExitAnimation,
                MessageStyleParameter.ExitTime,
                MessageStyleParameter.Mode,
                MessageStyleParameter.Color);

        }

        public Message(string text, string areaName, MessageMode mode)
        {

            content = new MessageContent();
            controlBlock = new MessageControlBlock();

            content.text = text;
            content.areaName = areaName;
            content.mode = mode;
            

            content.inheritStyleMaskFromArea = MessageStyleParameterMask.GetMask(
                MessageStyleParameter.DisplayTime,
                MessageStyleParameter.EntranceAnimation,
                MessageStyleParameter.EntranceTime,
                MessageStyleParameter.ExitAnimation,
                MessageStyleParameter.ExitTime,
                MessageStyleParameter.Color);

        }

        public Message(string text, string areaName, Color color, MessageEntranceAnimation entranceAnimation, MessageExitAnimation exitAnimation,
            MessageMode mode, float entranceTime, float displayTime, float exitTime, MessageLifespan lifespan)
        {
            content = new MessageContent();
            controlBlock = new MessageControlBlock();

            content.text = text;
            content.areaName = areaName;


            content.entranceAnimation = entranceAnimation;
            content.exitAnimation = exitAnimation;
            content.entranceTime = entranceTime;
            content.displayTime = displayTime;
            content.exitTime = exitTime;
            content.mode = mode;
            content.color = color;

            content.lifespan = lifespan;

            content.inheritStyleMaskFromArea.value = 0x00;
        }


        public MessageControlBlock controlBlock;
        public MessageContent content;
    }


    public class MessageControlBlock
    {
        public bool isPlaying = false;

        public MessageState state = MessageState.Ready;


        public float transparency = 255.0f;
        public float messageStartTime;

        public UIMessageArea messageArea;
    }

    /// <summary>
    /// Mesageの内容. 内容とは, 以下の通り.
    /// 
    ///  What: Messageの文章
    ///  Where: Messageの場所
    ///  How: どのように表示するか(Style)
    ///  
    /// </summary>
    public class MessageContent
    {
        public MessageEntranceAnimation entranceAnimation = MessageEntranceAnimation.Appear;
        public MessageExitAnimation exitAnimation = MessageExitAnimation.Disappear;

        public MessageMode mode = MessageMode.Normal;

        public string text;

        public float displayTime = 2.0f;
        public float exitTime = 1.0f;
        public float entranceTime = 1.0f;
        public Color color = Color.black;

        public MessageLifespan lifespan = MessageLifespan.UntilShowEnd;

        public string areaName;

        public MessageStyleParameterMask inheritStyleMaskFromArea;
    }
}