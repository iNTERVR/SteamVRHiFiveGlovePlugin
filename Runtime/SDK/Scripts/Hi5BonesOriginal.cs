//======= Copyright (c) Beijing Noitom Technology Ltd., All rights reserved. ===============
//
// Purpose: Connect and disconnect the Hi5 devices.
//
//==========================================================================================

using HI5;
using UnityEngine;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive
{
    public class Hi5BonesOriginal : MonoBehaviour
    {
        public Hand HandType;
        public Transform[] HandBones = new Transform[(int)Bones.NumOfHI5Bones];
        public Renderer HandRenderer;
        public Hi5Bones TargetBonesComponent;
        protected HI5_Source m_BindSource = null;

        protected void OnEnable()
        {
            HandRenderer.enabled = false;
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
            if (m_BindSource != null)
            {
                ApplyFingerMotion(m_BindSource);

                if (TargetBonesComponent != null)
                {
                    TargetBonesComponent.ApplyFingers(this);
                }
            }
        }

        private void ApplyFingerMotion(HI5_Source source)
        {
            for (int i = (m_INDEX_Hand + 1); i < (int)Bones.NumOfHI5Bones && i < HandBones.Length; i++)
            {
                if (HandBones[i] != null)
                {
                    SetRotation(HandBones, i, source.GetReceivedRotation(i, HandType));
                }
            }
        }

        private void SetRotation(Transform[] bones, int bone, Vector3 rotation)
        {
            Transform t = bones[(int)bone];
            if (t != null)
            {
                Quaternion rot = Quaternion.Euler(rotation);
                if (!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z) && !float.IsNaN(rot.w))
                {
                    t.localRotation = rot;
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

        // Returns the saved transform references in the HandBones list.
        public Transform GetBoneTransform(Bones bone)
        {
            return HandBones[(int)bone];
        }
    }
}