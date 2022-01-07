﻿using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;
using System;
using PlayerEvents = Exiled.Events.Handlers.Player;
using ServerEvents = Exiled.Events.Handlers.Server;
namespace FacilityDoorManager
{

    public class FacilityDoorManager : Plugin<Config>
    {



        public static Config early_config;

        /// <summary>
        /// Medium priority, lower prioritys mean faster loadin
        /// </summary>
        public override PluginPriority Priority { get; } = PluginPriority.Higher;

        private Harmony harmony;
        private string harmony_id = "com.Undid-Iridium.FacilityDoorManager";

        public override Version RequiredExiledVersion { get; } = new Version(4, 2, 0);



        private Handlers.RoundStartingBehaviors round_start_behavior;
        private Handlers.PlayerRespawningBehaviors player_respawn_behavior;
        private Handlers.PlayerDoorInteraction player_door_behavior;


        /// <summary>
        /// Entrance function called through Exile
        /// </summary>
        public override void OnEnabled()
        {
            RegisterEvents();
            RegisterHarmony();
            base.OnEnabled();
        }

        /// <summary>
        /// Destruction function called through Exile
        /// </summary>
        public override void OnDisabled()
        {
            UnRegisterEvents();
            UnRegisterHarmony();
            base.OnDisabled();
        }

        private void RegisterHarmony()
        {
            harmony = new Harmony(harmony_id);
            //https://harmony.pardeike.net/articles/basics.html cute patching method
            if (Config.behavior_rules.patch_all)
            {
                harmony.PatchAll();
                return;
            }

            //var chaos_original = typeof(ChaosInsurgencySpawnHandler).GetMethod(nameof(NineTailedFoxSpawnHandler.GenerateQueue));
            //var chaos_prefix = typeof(ChaosSpawnPatcher).GetMethod(nameof(ChaosSpawnPatcher.ChaosSpawnPatch));
            //harmony.Patch(chaos_original, prefix: new HarmonyMethod(chaos_prefix));

            //var mtf_original = typeof(NineTailedFoxSpawnHandler).GetMethod(nameof(NineTailedFoxSpawnHandler.GenerateQueue));
            //var mtf_prefix = typeof(MtfSpawnPatcher).GetMethod(nameof(MtfSpawnPatcher.MtfSpawnPatch));
            //harmony.Patch(chaos_original, prefix: new HarmonyMethod(chaos_prefix));

        }


        private void UnRegisterHarmony()
        {
            harmony.UnpatchAll(harmony.Id);
            harmony = null;
        }


        /// <summary>
        /// Registers events for EXILE to hook unto with cororotines (I think?)
        /// </summary>
        public void RegisterEvents()
        {
            // Register the event handler class. And add the event,
            // to the EXILED_Events event listener so we get the event.
            if (this.Config.behavior_rules.random_doors)
            {
                round_start_behavior = new Handlers.RoundStartingBehaviors(this);
                player_respawn_behavior = new Handlers.PlayerRespawningBehaviors(this);

                early_config = Config;
                ServerEvents.RoundStarted += round_start_behavior.OnRoundStarted;
                PlayerEvents.Spawning += player_respawn_behavior.OnRespawning;
            }

            if (this.Config.behavior_rules.safe_facility)
            {
                player_door_behavior = new Handlers.PlayerDoorInteraction(this);
                PlayerEvents.InteractingDoor += player_door_behavior.OnDoorInteraction;
            }

            Log.Info("FacilityDoorManager has been loaded");

        }
        /// <summary>
        /// Unregisters the events defined in RegisterEvents, recommended that everything created be destroyed if not reused in some way.
        /// </summary>
        public void UnRegisterEvents()
        {
            if (this.Config.behavior_rules.random_doors)
            {
                ServerEvents.RoundStarted -= round_start_behavior.OnRoundStarted;
                PlayerEvents.Spawning -= player_respawn_behavior.OnRespawning;
            }

            Log.Info("FacilityDoorManager has been unloaded");
        }
    }

}