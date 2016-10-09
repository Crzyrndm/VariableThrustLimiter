using System;

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
            
            ModuleEngines EngineModule = part.Modules.GetModules<ModuleEngines>().Find(e => e.throttleLocked);
            
            // We found a SR engine on this part, time to do stuff
            if (EngineModule != null)
            {
                if (EngineModule.useThrustCurve)
                {
                    for (int i = 0; i < EngineModule.thrustCurve.Curve.length; ++i)
                    {
                        float modifier = 1 - EngineModule.thrustCurve.Curve.keys[i].time * (1 - LimiterEnd);
                        EngineModule.thrustCurve.Curve.keys[i].value *= modifier;
                        EngineModule.thrustCurve.Curve.keys[i].inTangent *= modifier;
                        EngineModule.thrustCurve.Curve.keys[i].outTangent *= modifier;

                    }
                }
                else
                {
                    // else, I can use it for my own benefit
                    EngineModule.useThrustCurve = true;
                    EngineModule.thrustCurve.Add(1f, 1f);
                    EngineModule.thrustCurve.Add(0f, LimiterEnd / (Math.Max(EngineModule.thrustPercentage, 0.00001f)));
                }
            }
        }
    }
}
