using System;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace VariableThrustLimiter
{
    public class ModuleSRBThrustLimiterLinear : PartModule
    {
        [KSPField(isPersistant = true), UI_FloatRange(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, minValue = 0, maxValue = 100, scene = UI_Scene.Editor, stepIncrement=0.01f)]
        public float LimiterEnd = 100;
        float ThrustLimitStart;
        ModuleEngines SRBModule;
        PartResource SolidFuelRef;
        bool bInit = false;
        public override void OnStart(StartState state)
        {
 	        base.OnStart(state);
            SRBModule = part.Modules.GetModules<ModuleEngines>().FirstOrDefault(e => e.propellants.FindAll(p => p.name == "SolidFuel").Count > 0);
            if (SRBModule != null)
            {
                ThrustLimitStart = SRBModule.thrustPercentage;
                SolidFuelRef = part.Resources.list.FirstOrDefault(r => r.resourceName == "SolidFuel");
                if (SolidFuelRef != null)
                    bInit = true;
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (HighLogic.LoadedSceneIsFlight && !vessel.HoldPhysics && bInit)
                SRBModule.thrustPercentage = LimiterEnd + (ThrustLimitStart - LimiterEnd) * (float)(SolidFuelRef.amount / SolidFuelRef.maxAmount);
        }
    }
}
