using EcsRx.Groups;
using EcsRx.Events;
using UniRx;
using System.Collections.Generic;
using System;
using EcsRx.Extensions;
using EcsRx.Collections.Database;
using EcsRx.Unity.Extensions;
using InterVR.IF.VR.Components;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Entities;
using UniRx.Triggers;
using EcsRx.Plugins.Views.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Glove.Modules;
using UnityEngine;
using InterVR.IF.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Components;
using InterVR.IF.VR.Plugin.Steam.InteractionSystem;
using Valve.VR;
using InterVR.IF.Blueprints;
using InterVR.IF.Defines;
using InterVR.IF.Components;
using InterVR.IF.VR.Events;
using HI5;
using InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules;
using InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Installer;

namespace InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Systems
{
    public class IF_VR_Glove_SteamVRHiFive_HandSystem : ISetupSystem, ITeardownSystem
    {
        public IGroup Group => new Group(typeof(IF_VR_Glove_Hand), typeof(ViewComponent));

        private Dictionary<IEntity, List<IDisposable>> subscriptionsPerEntity = new Dictionary<IEntity, List<IDisposable>>();
        private readonly IF_IGameObjectTool gameObjectTool;
        private readonly IF_VR_IInterface vrInterface;
        private readonly IF_VR_Glove_IInterface vrGloveInterface;
        private readonly IEntityDatabase entityDatabase;
        private readonly IEventSystem eventSystem;
        private readonly IHI5Interface hi5Interface;
        private readonly IF_VR_Glove_SteamVRHiFive_Installer.Settings settings;
        private readonly IHI5Interface hI5Interface;

        public IF_VR_Glove_SteamVRHiFive_HandSystem(IF_IGameObjectTool gameObjectTool,
            IF_VR_IInterface vrInterface,
            IF_VR_Glove_IInterface vrGloveInterface,
            IEntityDatabase entityDatabase,
            IEventSystem eventSystem,
            IHI5Interface hi5Interface,
            IF_VR_Glove_SteamVRHiFive_Installer.Settings settings,
            IHI5Interface hI5Interface)
        {
            this.gameObjectTool = gameObjectTool;
            this.vrInterface = vrInterface;
            this.vrGloveInterface = vrGloveInterface;
            this.entityDatabase = entityDatabase;
            this.eventSystem = eventSystem;
            this.hi5Interface = hi5Interface;
            this.settings = settings;
            this.hI5Interface = hI5Interface;
        }

        bool vrGloveIsActivated(IEntity entity)
        {
            var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
            return vrGloveHand.Active.Value;
        }

        IF_VR_Steam_Hand convertSteamVRHand(IF_VR_Hand hand)
        {
            IF_VR_Steam_Hand steamVRHand;
            if (hand.Type == IF_VR_HandType.Left)
                steamVRHand = IF_VR_Steam_Player.instance.leftHand;
            else
                steamVRHand = IF_VR_Steam_Player.instance.rightHand;
            return steamVRHand;
        }

        public void Setup(IEntity entity)
        {
            var subscriptions = new List<IDisposable>();
            subscriptionsPerEntity.Add(entity, subscriptions);

            init(entity);

            eventSystem.Receive<IF_VR_Event_OnAttachedToHand>().Subscribe(evt =>
            {
                var hand = evt.HandEntity.GetComponent<IF_VR_Hand>();
                var steamVRHand = convertSteamVRHand(hand);
                var gloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
                if (hand.Type == gloveHand.Type)
                {
                    steamVRHand.Show();
                    if (gloveHand.RenderModel)
                        gloveHand.RenderModel.SetActive(false);
                }
            }).AddTo(subscriptions);

            eventSystem.Receive<IF_VR_Event_OnDetachedToHand>().Subscribe(evt =>
            {
                var hand = evt.HandEntity.GetComponent<IF_VR_Hand>();
                var steamVRHand = convertSteamVRHand(hand);
                var gloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
                if (hand.Type == gloveHand.Type)
                {
                    steamVRHand.Hide();
                    if (gloveHand.RenderModel)
                        gloveHand.RenderModel.SetActive(true);
                }
            }).AddTo(subscriptions);

            Observable.EveryUpdate()
                .Where(x => vrGloveIsActivated(entity))
                .Subscribe(x =>
                {
                    var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
                    var vrGloveHandView = entity.GetGameObject();
                    var wristEntity = vrGloveHand.WristEntity;
                    var wristView = wristEntity.GetGameObject();
                    var hi5Bones = vrGloveHand.RenderModel.GetComponent<Hi5Bones>();

                    var vrHandEntity = vrInterface.GetHandEntity(vrGloveHand.Type);
                    var vrHand = vrHandEntity.GetComponent<IF_VR_Hand>();
                    var followEntityEntities = entityDatabase.GetEntitiesFor(new Group(typeof(IF_FollowEntity)), 0);
                    foreach (var followEntityEntity in followEntityEntities)
                    {
                        var followEntity = followEntityEntity.GetComponent<IF_FollowEntity>();
                        if (followEntity.FollowSourceEntity.HasComponent<IF_VR_Hand>())
                        {
                            var vrHandSource = followEntity.FollowSourceEntity.GetComponent<IF_VR_Hand>();
                            if (vrHandSource.Type == vrGloveHand.Type)
                            {
                                followEntity.FollowTargetEntity = entity;
                                if (vrHand.Type == IF_VR_HandType.Left)
                                {
                                    followEntity.OffsetPosition = new Vector3(-0.15f, 0.2f, -0.12f);
                                    followEntity.OffsetRotation = new Vector3(-150.15f, 69.06f, 0.0f);
                                }
                                else
                                {
                                    followEntity.OffsetPosition = new Vector3(0.09f, -0.22f, -0.06f);
                                    followEntity.OffsetRotation = new Vector3(90f, -60f, 60f);
                                }
                                break;
                            }
                        }
                    }

                    ApplyHandMotion_Position(hi5Bones.HandBones, vrGloveHand.Type, vrGloveHandView.transform.position, vrGloveHandView.transform.rotation);
                    ApplyHandMotion_Rotation(hi5Bones.HandBones, vrGloveHand.Type, HI5_Manager.GetHI5Source());

                }).AddTo(subscriptions);
        }

        private int m_INDEX_Hand = (int)Bones.Hand;

        private Quaternion ToSteamVRHandSkeletonRight(Quaternion rot)
        {
            var playerRot = IF_VR_Steam_Player.instance.transform.eulerAngles;
            Quaternion q1 = Quaternion.identity;
            if (playerRot.y == 0)
            {
                q1.Set(
                    newX: -rot.z,
                    newY: rot.y,
                    newZ: rot.x,
                    newW: rot.w
                    );
            }
            else if (playerRot.y == 90)
            {
                q1 = rot;
            }
            else if (playerRot.y == 180)
            {
                q1.Set(
                    newX: rot.z,
                    newY: rot.y,
                    newZ: -rot.x,
                    newW: rot.w
                    );
            }
            else if (playerRot.y == 270)
            {
                q1.Set(
                    newX: rot.x,
                    newY: rot.y,
                    newZ: rot.z,
                    newW: -rot.w
                    );
            }

            Quaternion q2 = Quaternion.Euler(new Vector3(0, playerRot.y, 90));
            return q1 * q2;
        }

        private Quaternion ToSteamVRHandSkeletonLeft(Quaternion rot)
        {
            var playerRot = IF_VR_Steam_Player.instance.transform.eulerAngles;
            Quaternion q1 = Quaternion.identity;
            if (playerRot.y == 0)
            {
                q1.Set(
                    newX: rot.z,
                    newY: rot.y,
                    newZ: -rot.x,
                    newW: rot.w
                    );
            }
            else if (playerRot.y == 90)
            {
                q1.Set(
                    newX: rot.x,
                    newY: -rot.y,
                    newZ: rot.z,
                    newW: -rot.w
                    );
            }
            else if (playerRot.y == 180)
            {
                q1.Set(
                    newX: -rot.z,
                    newY: rot.y,
                    newZ: rot.x,
                    newW: rot.w
                    );
            }
            else if (playerRot.y == 270)
            {
                q1.Set(
                    newX: rot.x,
                    newY: -rot.y,
                    newZ: rot.z,
                    newW: rot.w
                    );
            }

            Quaternion q2 = Quaternion.Euler(new Vector3(0, playerRot.y, -90));
            return q1 * q2;
        }

        private void ApplyHandMotion_Rotation(Transform[] HandBones, IF_VR_HandType handType, HI5_Source source)
        {
            var type = handType == IF_VR_HandType.Left ? Hand.LEFT : Hand.RIGHT;
            if (HandBones[m_INDEX_Hand] != null)
            {
                var t = HandBones[m_INDEX_Hand];
                var euler = HI5_DataTransform.ToUnityEulerAngles(source.GetReceivedRotation(m_INDEX_Hand, type));
                if (type == Hand.RIGHT)
                {
                    euler += new Vector3(0, 90, 0);
                    t.rotation = ToSteamVRHandSkeletonRight(Quaternion.Euler(euler));
                }
                else if (type == Hand.LEFT)
                {
                    euler += new Vector3(0, -90, 0);
                    t.rotation = ToSteamVRHandSkeletonLeft(Quaternion.Euler(euler));
                }
            }
        }

        private void ApplyHandMotion_Position(Transform[] HandBones, IF_VR_HandType handType, Vector3 pos, Quaternion rot)
        {
            Vector3 offset = handType == IF_VR_HandType.Left ? HI5_Manager.LeftOffset : HI5_Manager.RightOffset;
            Vector3 handPos = pos + rot * offset;

            if (HandBones[m_INDEX_Hand] != null)
            {
                if (!float.IsNaN(handPos.x) && !float.IsNaN(handPos.y) && !float.IsNaN(handPos.z))
                    HandBones[m_INDEX_Hand].position = handPos;
            }
        }

        void init(IEntity entity)
        {
            var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
            var gloveHandType = vrGloveHand.Type;
            var vrGloveView = entity.GetGameObject();

            // create wrist and setup
            var pool = entityDatabase.GetCollection();
            var wristEntity = pool.CreateEntity();
            vrGloveHand.Wrist.gameObject.LinkEntity(wristEntity, pool);
            var wristView = wristEntity.GetGameObject();
            var wrist = wristEntity.AddComponent<IF_VR_Glove_Wrist>();
            wrist.Type = gloveHandType;
            vrGloveHand.WristEntity = wristEntity;

            // instaniate render model prefab
            var parent = wristView.transform;
            var prefab = gloveHandType == IF_VR_HandType.Left ? IF_VR_Steam_Player.instance.leftHand.renderModelPrefab : IF_VR_Steam_Player.instance.rightHand.renderModelPrefab;
            var instance = gameObjectTool.InstantiateWithInit(prefab, parent);
            var skeleton = instance.GetComponentInChildren<SteamVR_Behaviour_Skeleton>();

            var hi5Bones = instance.AddComponent<Hi5Bones>();
            hi5Bones.HandType = gloveHandType == IF_VR_HandType.Left ? Hand.LEFT : Hand.RIGHT;
            hi5Bones.HandBones[(int)Bones.ForeArm] = skeleton.root;
            hi5Bones.HandBones[(int)Bones.Hand] = skeleton.wrist;
            hi5Bones.HandBones[(int)Bones.HandThumb1] = skeleton.thumbProximal;
            hi5Bones.HandBones[(int)Bones.HandThumb2] = skeleton.thumbMiddle;
            hi5Bones.HandBones[(int)Bones.HandThumb3] = skeleton.thumbDistal;
            hi5Bones.HandBones[(int)Bones.InHandIndex] = skeleton.indexMetacarpal;
            hi5Bones.HandBones[(int)Bones.HandIndex1] = skeleton.indexProximal;
            hi5Bones.HandBones[(int)Bones.HandIndex2] = skeleton.indexMiddle;
            hi5Bones.HandBones[(int)Bones.HandIndex3] = skeleton.indexDistal;
            hi5Bones.HandBones[(int)Bones.InHandMiddle] = skeleton.middleMetacarpal;
            hi5Bones.HandBones[(int)Bones.HandMiddle1] = skeleton.middleProximal;
            hi5Bones.HandBones[(int)Bones.HandMiddle2] = skeleton.middleMiddle;
            hi5Bones.HandBones[(int)Bones.HandMiddle3] = skeleton.middleDistal;
            hi5Bones.HandBones[(int)Bones.InHandRing] = skeleton.ringMetacarpal;
            hi5Bones.HandBones[(int)Bones.HandRing1] = skeleton.ringProximal;
            hi5Bones.HandBones[(int)Bones.HandRing2] = skeleton.ringMiddle;
            hi5Bones.HandBones[(int)Bones.HandRing3] = skeleton.ringDistal;
            hi5Bones.HandBones[(int)Bones.InHandPinky] = skeleton.pinkyMetacarpal;
            hi5Bones.HandBones[(int)Bones.HandPinky1] = skeleton.pinkyProximal;
            hi5Bones.HandBones[(int)Bones.HandPinky2] = skeleton.pinkyMiddle;
            hi5Bones.HandBones[(int)Bones.HandPinky3] = skeleton.pinkyDistal;

            hi5Interface.SetBones(gloveHandType, hi5Bones);

            var originalPrefab = gloveHandType == IF_VR_HandType.Left ? settings.Hi5LeftHandPrefab : settings.Hi5RightHandPrefab;
            var originalInstance = gameObjectTool.InstantiateWithInit(originalPrefab, parent);
            var originalBones = originalInstance.GetComponent<Hi5BonesOriginal>();
            originalBones.TargetBonesComponent = hi5Bones;

            //var skeletonRoot = skeleton.root;
            //if (skeleton.mirroring == SteamVR_Behaviour_Skeleton.MirrorType.RightToLeft)
            //{
            //    skeletonRoot.localRotation = Quaternion.Euler(0, 90, -90);
            //}
            //else
            //{
            //    skeletonRoot.localRotation = Quaternion.Euler(0, 90, 90);
            //}

            GameObject.Destroy(skeleton);
            GameObject.Destroy(instance.GetComponentInChildren<Animator>());

            // follow tracker
            var vrHandTrackerEntity = vrInterface.GetHandTrackerEntity(gloveHandType);
            pool.CreateEntity(new IF_FollowEntityBlueprint(IF_UpdateMomentType.Update,
                vrHandTrackerEntity,
                entity,
                true,
                true,
                //new Vector3(0.0f, 0.07f, -0.04f),
                Vector3.zero,
                Vector3.zero));

            if (gloveHandType == IF_VR_HandType.Left)
            {
                IF_VR_Steam_Player.instance.leftHand.Hide();
            }
            else if (gloveHandType == IF_VR_HandType.Right)
            {
                IF_VR_Steam_Player.instance.rightHand.Hide();
            }

            vrGloveHand.RenderModel = instance;
            vrGloveHand.Active.Value = true;
        }

        bool checkIsValid(IF_VR_HandType handType)
        {
            var type = handType == IF_VR_HandType.Left ? Hand.LEFT : Hand.RIGHT;
            if (!hi5Interface.IsGloveAvailable(type))
                return false;

            if (!HI5_BindInfoManager.IsGloveBinded(type))
                return false;

            if (!HI5_Manager.GetGloveStatus().isGloveBPosSuccess())
                return false;

            return true;
        }

        void checkDeviceBinded(IF_VR_HandType handType, int index)
        {
            HI5_BindInfoManager.LoadItems();

            if (handType == IF_VR_HandType.Left)
                HI5_BindInfoManager.LeftID = index;
            else
                HI5_BindInfoManager.RightID = index;
        }

        public void Teardown(IEntity entity)
        {
            if (subscriptionsPerEntity.TryGetValue(entity, out List<IDisposable> subscriptions))
            {
                subscriptions.DisposeAll();
                subscriptions.Clear();
                subscriptionsPerEntity.Remove(entity);
            }
        }
    }
}