using System.Collections;
using UnityEngine;

namespace VariableThrustLimiter
{
    public class DynamicGimbal : ModuleGimbal
    {
        ModuleEngines[] engines;
        ModuleEngines activeEngine;
        /// <summary>
        /// remember the default gimbal range
        /// </summary>
        float defaultGimbalRange;

        /// <summary>
        /// Curve describing the relationship between % thrust and % of max gimbal
        /// </summary>
        [KSPField]
        public FloatCurve gimbalPercentage;
        
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            engines = part.Modules.GetModules<ModuleEngines>().ToArray();
            if (part.Modules.GetModules<MultiModeEngine>().Count > 0)
                StartCoroutine(checkEngineSwitch());
            if (engines.Length == 0)
                engines = null;
            else
            {
                activeEngine = engines[0];
                defaultGimbalRange = gimbalRange;
            }
        }

        public IEnumerator checkEngineSwitch()
        {
            while (HighLogic.LoadedSceneIsFlight)
            {
                yield return new WaitForSeconds(1.0f); // check active engine once per second
                if (!activeEngine.isOperational) // last active engine is not thrusting
                {
                    for (int i = 0; i < engines.Length; ++i)
                    {
                        if (engines[i].isOperational)
                            activeEngine = engines[i];
                    }
                }
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (engines == null || !activeEngine.isOperational)
                return;
            if (!activeEngine.isOperational)
                gimbalRange = defaultGimbalRange;
            else
                gimbalRange = defaultGimbalRange * gimbalPercentage.Evaluate(activeEngine.maxThrust / activeEngine.resultingThrust);
        }
    }
}