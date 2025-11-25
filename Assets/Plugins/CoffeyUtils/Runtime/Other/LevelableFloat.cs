using UnityEngine;

namespace CoffeyUtils
{
    [System.Serializable]
    public struct LevelableFloat
    {
        [Tooltip("Check to enable multiple levels")]
        [SerializeField] private bool isLevelable;
        [SerializeField] private float[] levels;

        public LevelableFloat(float defaultVal = 0f)
        {
            isLevelable = false;
            levels = new float[] { defaultVal, defaultVal, defaultVal };
        }

        public LevelableFloat(float level1value = 10f, float level2value = 15f, float level3value = 20f)
        {
            isLevelable = true;
            levels = new float[] { level1value, level2value, level3value };
        }

        public static implicit operator float(LevelableFloat lf) => lf.GetBaseValue();
        public override string ToString() => GetBaseValue().ToString();

        public float GetBaseValue() => (levels != null && levels.Length > 0) ? levels[0] : 0f;

        public float GetValue(float level)
        {
            if (levels == null || levels.Length == 0) return 0f;
            if (!isLevelable) return levels[0];

            int index = Mathf.FloorToInt(level) - 1;
            index = Mathf.Clamp(index, 0, levels.Length - 1);

            return levels[index];
        }
    }
}