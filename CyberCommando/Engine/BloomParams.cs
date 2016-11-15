using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Engine
{
    public enum BloomStates
    {
        SOFT,
        HARDSOFT,
        RESATURATED,
        SATURATED,
        BLURRY,
        SUBTLE
    }


    class BloomParams
    {
        public readonly BloomStates State;

        // how bright a pixel needs to be before it will bloom.
        public readonly float BloomThreshold;

        // how much blurring is applied to the bloom 
        public readonly float BlurAmount;

        // amount of the bloom and base images that mixed in final
        public readonly float BloomIntensity;
        public readonly float BaseIntensity;

        // Independently control saturation of the bloom and and base img
        public readonly float BloomSaturation;
        public readonly float BaseSaturation;

        public BloomParams(BloomStates state, float bloomThreshold, float blurAmount,
                            float bloomIntensity, float baseIntensity,
                            float bloomSaturation, float baseSaturation)
        {
            this.State = state;
            this.BloomThreshold = bloomThreshold;
            this.BlurAmount = blurAmount;
            this.BloomIntensity = bloomIntensity;
            this.BaseIntensity = baseIntensity;
            this.BloomSaturation = bloomSaturation;
            this.BaseSaturation = baseSaturation;
        }

        public static Dictionary<BloomStates, BloomParams> BModes = new Dictionary<BloomStates, BloomParams>() {

            //                                           State                  Thresh    Blur   Bloom   Base   BloomSat  BaseSat
            {BloomStates.SOFT,          new BloomParams(BloomStates.SOFT,        0.04f,  0.001f,  2.5f,   1.5f,   1.25f,   .5f) },
            {BloomStates.HARDSOFT,      new BloomParams(BloomStates.HARDSOFT,    0.25f,  0.5f,    1,      1,      1,       1) },
            {BloomStates.RESATURATED,   new BloomParams(BloomStates.RESATURATED, 0.5f,   8,       2,      1,      0,       1) },
            {BloomStates.SATURATED,     new BloomParams(BloomStates.SATURATED,   0.25f,  4,       2,      1,      2,       0) },
            {BloomStates.BLURRY,        new BloomParams(BloomStates.BLURRY,      0,      2,       1,      0.1f,   1,       1) },
            {BloomStates.SUBTLE,        new BloomParams(BloomStates.SUBTLE,      0.5f,   2,       1,      1,      1,       1) },
        };
    }
}
