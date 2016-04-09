using System;
using System.Linq;
using UnityEngine;

namespace VariableThrustLimiter
{
    public class ModuleSRBThrustLimiterLinear : PartModule
    {
        [KSPField(isPersistant = true), UI_FloatRange(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, minValue = 0, maxValue = 100, scene = UI_Scene.Editor, stepIncrement=0.01f)]
        public float LimiterEnd = 100;

        public override void OnStart(StartState state)
        {
 	        base.OnStart(state);

            // only want to modify anything in the flight scene..., for now...
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            ModuleEngines SRBModule = part.Modules.GetModules<ModuleEngines>().FirstOrDefault(e => e.propellants.FindAll(p => p.name == "SolidFuel").Count > 0);
            
            // We found a SR engine on this part, time to do stuff
            if (SRBModule != null)
            {
                if (SRBModule.useThrustCurve)
                {
                    // if it already has a thrust curve, I want to reshape it to meet my needs
                    // should really adjust tangents as well. Some other time
                    float maxTime = Math.Max(SRBModule.thrustCurve.Curve.keys.Max(k => k.time), 0.00001f);
                    float thrustRatio = (SRBModule.thrustPercentage - LimiterEnd) / (100 * Math.Max(SRBModule.thrustPercentage, 0.00001f));
                    for (int i = 0; i < SRBModule.thrustCurve.Curve.length; ++i)
                        SRBModule.thrustCurve.Curve.keys[i].value *= 1 - ((maxTime - SRBModule.thrustCurve.Curve.keys[i].time) / maxTime) * thrustRatio;
                }
                else
                {
                    // else, I can use it for my own benefit
                    SRBModule.useThrustCurve = true;
                    SRBModule.thrustCurve.Add(1f, 1f);
                    SRBModule.thrustCurve.Add(0f, LimiterEnd / (100f * Math.Max(SRBModule.thrustPercentage, 0.00001f)));
                }
            }
        }
    }
}
