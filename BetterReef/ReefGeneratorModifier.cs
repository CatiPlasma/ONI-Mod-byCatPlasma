using Klei.AI;
using KSerialization;
using UnityEngine;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class ReefGeneratorModifier: KMonoBehaviour
    {
        [Serialize] private float normalMultiplier;
        [Serialize] private float tuneupMultiplier;
        [Serialize] private bool isTuneup;
        public float multiplier = -1f;

        private AttributeModifier modifier;
        
        private bool isApplied;
        private SchedulerHandle retryHandler;

        private ReefGenerator.Instance smi;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            smi = gameObject.GetSMI<ReefGenerator.Instance>();
            ApplyMultiplier();
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
            
            normalMultiplier = Mathf.Sqrt(geyser.GetComponent<ReefGeyserRandomizer>().GetExhaleMultiplier());
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
            isTuneup = true;
            if (Mathf.Approximately(multiplier, tuneupMultiplier)) return;
            multiplier = tuneupMultiplier;
            UpdateModifier();
        }

        public void TuneupEnd()
        {
            isTuneup = false;
            if (Mathf.Approximately(multiplier, normalMultiplier)) return;
            multiplier = normalMultiplier;
            UpdateModifier();
        }
    }
}