//======= Copyright (c) Beijing Noitom Technology Ltd., All rights reserved. ===============
//
// Purpose: Connect and disconnect the Hi5 devices.
//
//==========================================================================================

using HI5;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive
{
    public class Hi5Bones : MonoBehaviour
    {
        public Hand HandType;
        public Transform[] HandBones = new Transform[(int)Bones.NumOfHI5Bones];
        protected HI5_Source m_BindSource = null;

        protected void OnEnable()
        {
            Connect();
        }

        protected void OnDisable()
        {
        }

        protected void OnApplicationQuit()
        {
            Disconnect();
        }

        private static int m_INDEX_Hand = (int)Bones.Hand;

        protected void Update()
        {
        }

        float getAngle(int handIndex)
        {
            Transform t;
            float fingerAngles = 0;
            t = HandBones[handIndex];
            fingerAngles += 360 - t.localEulerAngles.z;
            t = HandBones[handIndex + 1];
            fingerAngles += 360 - t.localEulerAngles.z;
            t = HandBones[handIndex + 2];
            fingerAngles += 360 - t.localEulerAngles.z;
            return (fingerAngles / 3);
        }

        public bool IsGrab()
        {
            float angles = getAngle((int)Bones.HandIndex1);
            angles += getAngle((int)Bones.HandMiddle1);
            angles += getAngle((int)Bones.HandRing1);
            angles += getAngle((int)Bones.HandPinky1);

            float ava = angles / 4;
            bool isGrip = ava > 50.0f;
            if (ava >= 90)
                isGrip = false;

            return isGrip;
        }

        public void ApplyFingers(Hi5BonesOriginal original)
        {
            for (int i = (m_INDEX_Hand + 1); i < (int)Bones.NumOfHI5Bones && i < HandBones.Length; i++)
            {
                if (HandBones[i] != null)
                {
                    if (i == (int)Bones.HandIndex1 ||
                        i == (int)Bones.HandIndex2 ||
                        i == (int)Bones.HandIndex3 ||
                        i == (int)Bones.HandMiddle1 ||
                        i == (int)Bones.HandMiddle2 ||
                        i == (int)Bones.HandMiddle3 ||
                        i == (int)Bones.HandPinky1 ||
                        i == (int)Bones.HandPinky2 ||
                        i == (int)Bones.HandPinky3 ||
                        i == (int)Bones.HandRing1 ||
                        i == (int)Bones.HandRing2 ||
                        i == (int)Bones.HandRing3 ||
                        //i == (int)Bones.HandThumb1 ||
                        i == (int)Bones.HandThumb2 ||
                        i == (int)Bones.HandThumb3)
                    {
                        Transform t;
                        float fingerAngles = 0;
                        t = original.HandBones[i];
                        fingerAngles += 360 - t.localEulerAngles.z;
                        HandBones[i].localEulerAngles = new Vector3(0, 0, HandType == Hand.RIGHT ? -fingerAngles : fingerAngles);
                    }
                }
            }
        }

        protected void Connect()
        {
            if (!HI5_Manager.IsConnected) { HI5_Manager.Connect(); }
            m_BindSource = HI5_Manager.GetHI5Source();
        }

        protected void Disconnect()
        {
            if (HI5_Manager.IsConnected) { HI5_Manager.DisConnect(); }
            else { return; }
        }

        public Transform GetBoneTransform(Bones bone)
        {
            return HandBones[(int)bone];
        }
    }
}