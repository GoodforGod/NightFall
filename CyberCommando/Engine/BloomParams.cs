using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberCommando.Engine
{
    class BloomParams
    {
        // Name of a preset bloom setting, for display to the user.
        public readonly string Name;

        // Controls how bright a pixel needs to be before it will bloom.
        // Zero makes everything bloom equally, while higher values select
        // only brighter colors. Somewhere between 0.25 and 0.5 is good.
        public readonly float BloomThreshold;

        // Controls how much blurring is applied to the bloom image.
        // The typical range is from 1 up to 10 or so.
        public readonly float BlurAmount;

        // Controls the amount of the bloom and base images that
        // will be mixed into the final scene. Range 0 to 1.
        public readonly float BloomIntensity;
        public readonly float BaseIntensity;

        // Independently control the color saturation of the bloom and
        // base images. Zero is totally desaturated, 1.0 leaves saturation
        // unchanged, while higher values increase the saturation level.
        public readonly float BloomSaturation;
        public readonly float BaseSaturation;

        public BloomParams(string name, float bloomThreshold, float blurAmount,
                            float bloomIntensity, float baseIntensity,
                            float bloomSaturation, float baseSaturation)
        {
            this.Name = name;
            this.BloomThreshold = bloomThreshold;
            this.BlurAmount = blurAmount;
            this.BloomIntensity = bloomIntensity;
            this.BaseIntensity = baseIntensity;
            this.BloomSaturation = bloomSaturation;
            this.BaseSaturation = baseSaturation;
        }

        public static BloomParams[] PresetSettings =
        {
              //                Name           Thresh  Blur   Bloom   Base   BloomSat  BaseSat
              new BloomParams("Default",     0.04f,  0.001f,  2.5f,  1.5f,      1.25f,       1.5f),
              new BloomParams("Soft",        0.25f,  0.5f,     1,      1,      1,       1),
              new BloomParams("Desaturated", 0.5f,   8,     2,      1,      0,       1),
              new BloomParams("Saturated",   0.25f,  4,     2,      1,      2,       0),
              new BloomParams("Blurry",      0,      2,     1,      0.1f,   1,       1),
              new BloomParams("Subtle",      0.5f,   2,     1,      1,      1,       1),
        };
    }
}
