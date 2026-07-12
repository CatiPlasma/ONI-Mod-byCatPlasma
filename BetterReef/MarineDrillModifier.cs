using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace BetterReef
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class MarineDrillModifier: KMonoBehaviour
    {
        [Serialize] private float durationMultiplier = -1;
        [Serialize] private float diamondConsumptionMultiplier = -1;

        protected override void OnSpawn()
        {
            base.OnSpawn();
        }

        private void SetupMultiplier(UnderwaterVentDrill.Instance smi)
        {
            int cell = Grid.PosToCell(smi.transform.GetPosition());
            GameObject geyser = Grid.Objects[cell, (int)ObjectLayer.Building];
            UnderwaterVent.Instance ventSMI = geyser.GetComponent<UnderwaterVent.Instance>();
            Element buildupElement = ElementLoader.FindElementByHash(ventSMI.def.data.SolidElement);
            float buildupElementTemp = ventSMI.def.data.SolidTemp;
            Element standardElement = ElementLoader.FindElementByHash(SimHashes.Sulfur);
            float harnessFactor = Mathf.Exp(buildupElement.hardness - standardElement.hardness);
            float massFactor = ventSMI.def.data.SolidMass / 1000;
            gameObject.GetComponent<PrimaryElement>();
        }

        private void ApplyModifiers()
        {
            UnderwaterVentDrill.Def oldDef = smi.def;
            UnderwaterVentDrill.Def newDef = new UnderwaterVentDrill.Def
            {
                DiamondConsumptionRate = oldDef.DiamondConsumptionRate,
                DiamondTag = oldDef.DiamondTag,
                WorkDuration = oldDef.WorkDuration,
                ProgressBarOffset = oldDef.ProgressBarOffset
            };
        }

        private class TurnTemp
        {
            Dictionary<SimHashes, float> dic = new Dictionary<SimHashes, float>()
            {
                {SimHashes.Gold, 973.15f}
            };
        }
    }
}