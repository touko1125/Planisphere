using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryClass
{
    [System.Serializable]
    public class Story
    {
        public List<StorySection> story_Chart=new List<StorySection>();

        public Enum.StoryType story_Type;

        public int unlock_StoryStageNum;

        public Story(int[] storyNumbers,Enum.StoryType storyType,int unlockStageNum)
        {
            foreach(int num in storyNumbers)
            {
                story_Chart.Add(new StorySection(num));
            }

            story_Type = storyType;

            unlock_StoryStageNum = unlockStageNum;
        }
    }

    public class StorySection
    {
        public int message_Num; //セリフの番号

        public string message_String;   //セリフ本文

        public string message_Person;    //言っている人(表情込み)

        public string message_BackGround;   //会話の背景

        public Sprite message_BackGroudSprite;  //背景の画像

        public Sprite message_PersonSprite;  //言っている人の顔

        public Enum.LR message_PersonPos;    //言っている人の場所

        public StorySection(int messageNum=-1)
        {
            message_Num = messageNum;

            message_String = CSVReader.Instance.getStoryText(messageNum);

            message_Person = CSVReader.Instance.getStoryPerson(message_Num);

            message_BackGround = CSVReader.Instance.getStoryBackGround(messageNum);

            message_PersonPos = (Enum.LR)System.Enum.Parse(typeof(Enum.LR),CSVReader.Instance.getStoryPersonPos(messageNum));

            message_BackGroudSprite = Resources.Load<Sprite>("Sprite/"+message_BackGround);

            message_PersonSprite = Resources.Load<Sprite>("Sprite/"+message_Person);

            //Debug.Log(message_Num);

            //Debug.Log(message_String);

            //Debug.Log(message_Person);

            //Debug.Log(message_BackGround);

            //Debug.Log(message_PersonPos);

            //Debug.Log(message_BackGroudSprite);

            //Debug.Log(message_PersonSprite);
        }
    }
}
