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

namespace InterVR.IF.VR.Glove.Plugin.SteamVRManus.Systems
{
    public class IF_VR_Glove_SteamVRManus_HandSystem : ISetupSystem, ITeardownSystem
    {
        public IGroup Group => new Group(typeof(IF_VR_Glove_Hand), typeof(ViewComponent));

        private Dictionary<IEntity, List<IDisposable>> subscriptionsPerEntity = new Dictionary<IEntity, List<IDisposable>>();
        private readonly IF_IGameObjectTool gameObjectTool;
        private readonly IF_VR_IInterface vrInterface;
        private readonly IF_VR_Glove_IInterface vrGloveInterface;
        private readonly IEntityDatabase entityDatabase;
        private readonly IEventSystem eventSystem;

        public IF_VR_Glove_SteamVRManus_HandSystem(IF_IGameObjectTool gameObjectTool,
            IF_VR_IInterface vrInterface,
            IF_VR_Glove_IInterface vrGloveInterface,
            IEntityDatabase entityDatabase,
            IEventSystem eventSystem)
        {
            this.gameObjectTool = gameObjectTool;
            this.vrInterface = vrInterface;
            this.vrGloveInterface = vrGloveInterface;
            this.entityDatabase = entityDatabase;
            this.eventSystem = eventSystem;
        }

        bool manusIsConnected(IEntity entity)
        {
            var vrGloveHand = entity.GetComponent<IF_VR_Glove_Hand>();
            return vrGloveHand.Connected;
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