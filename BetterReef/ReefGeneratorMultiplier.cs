using System;
using System.Reflection;
using CatUtilLib;
using HarmonyLib;
using Klei.AI;
using KSerialization;
using UnityEngine;
using Attribute = Klei.AI.Attribute;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReefGeneratorMultiplier: KMonoBehaviour
    {
        [Serialize] private float normalMultiplier;
        [Serialize] private float tuneupMultiplier;
        [Serialize] private bool isTuneup = false;
        public float multiplier = -1f;

        private AttributeModifier modifier;
        
        private static readonly FieldInfo GeyserTargetFieldInfo = AccessTools.Field(typeof(ReefGenerator), "GeyserTarget");
        
        private bool isApplied = false;
        private SchedulerHandle retryHandler = new SchedulerHandle();

        protected override void OnSpawn()
        {
            ApplyMultiplier();
            base.OnSpawn();
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();
            modifier = new AttributeModifier(Db.Get().Attributes.GeneratorOutput.Id, 0, is_readonly: false);
            gameObject.GetAttributes().Add(modifier);
        }

        protected override void OnCleanUp()
        {
            retryHandler.ClearScheduler();
            base.OnCleanUp();
        }

        private void ApplyMultiplier()
        {
            if (isApplied) return;

            retryHandler.ClearScheduler();
            
            ReefGenerator.Instance smi = GetComponent<StateMachineController>()?.GetSMI<ReefGenerator.Instance>();
            if (smi == null)
            {
                ScheduledRetry();
                return;
            }
            
            int cell = Grid.PosToCell(smi.transform.GetPosition());
            if (!Grid.IsValidCell(cell))
            {
                ScheduledRetry();
                return;
            }
            
            GameObject geyser = Grid.Objects[cell, (int)ObjectLayer.Building];
            Generator generator = smi.GetComponent<Generator>();
            if  (geyser == null || generator == null)
            {
                ScheduledRetry();
                return;
            }
            
            normalMultiplier = (float)Math.Sqrt(geyser.GetComponent<ReefGeyserRandomizer>().GetExhaleMultiplier());
            tuneupMultiplier = normalMultiplier * 1.5f - 0.5f;
            
            multiplier = isTuneup ? tuneupMultiplier : normalMultiplier;
            UpdateModifier();
            
            isApplied = true;
        }
        
        private void UpdateModifier() => modifier.SetValue((multiplier - 1)*100);

        private void ScheduledRetry()
        {
            if (isApplied || retryHandler.IsValid || GameScheduler.Instance == null) return;
            retryHandler = GameScheduler.Instance.ScheduleNextFrame(
                nameof(ApplyMultiplier),
                _ =>
                {
                    retryHandler = default;
                    if (this != null) ApplyMultiplier();
                }
            );
        }

        public void TuneupStart()
        {
            if (multiplier == tuneupMultiplier) return;
            multiplier = tuneupMultiplier;
            UpdateModifier();
            isTuneup = true;
        }

        public void TuneupEnd()
        {
            if (multiplier == normalMultiplier) return;
            multiplier = normalMultiplier;
            UpdateModifier();
            isTuneup = false;
        }
    }
}