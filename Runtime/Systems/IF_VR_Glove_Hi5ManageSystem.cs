using EcsRx.Groups;
using System.Collections.Generic;
using System;
using EcsRx.Extensions;
using EcsRx.Collections.Database;
using EcsRx.Unity.Extensions;
using InterVR.IF.VR.Components;
using EcsRx.Plugins.ReactiveSystems.Systems;
using EcsRx.Entities;
using EcsRx.Plugins.Views.Components;
using InterVR.IF.VR.Modules;
using InterVR.IF.VR.Glove.Modules;
using UnityEngine;
using InterVR.IF.Modules;
using InterVR.IF.VR.Defines;
using InterVR.IF.VR.Glove.Components;
using EcsRx.Systems;
using EcsRx.Groups.Observable;
using HI5;
using UniRx;
using InterVR.IF.VR.Glove.Plugin.SteamVRHiFive.Modules;

namespace InterVR.IF.VR.Glove.Systems
{
    public class IF_VR_Glove_Hi5ManageSystem : IManualSystem
    {
        public IGroup Group => new Group(typeof(IF_VR_Hand), typeof(ViewComponent));

        private List<IDisposable> subscriptions = new List<IDisposable>();
        private readonly IEntityDatabase entityDatabase;
        private readonly IF_VR_IInterface vrInterface;
        private readonly IF_VR_Glove_IInterface vrGloveInterface;
        private readonly IF_IGameObjectTool gameObjectTool;
        private readonly IHI5Interface hi5Interface;

        public IF_VR_Glove_Hi5ManageSystem(IEntityDatabase entityDatabase,
            IF_VR_IInterface vrInterface,
            IF_VR_Glove_IInterface vrGloveInterface,
            IF_IGameObjectTool gameObjectTool,
            IHI5Interface hi5Interface)
        {
            this.entityDatabase = entityDatabase;
            this.vrInterface = vrInterface;
            this.vrGloveInterface = vrGloveInterface;
            this.gameObjectTool = gameObjectTool;
            this.hi5Interface = hi5Interface;
        }

        public void StartSystem(IObservableGroup observableGroup)
        {
            Observable.EveryUpdate().Subscribe(x =>
            {
                if (hi5Interface.Status != null)
                {
                    if (HI5_Manager.modifyThreadSave)
                    {
                        hi5Interface.Status.MainThreadUpdate();
                        HI5_Manager.Update();
                    }

                }
            }).AddTo(subscriptions);
        }

        public void StopSystem(IObservableGroup observableGroup)
        {
        }
    }
}