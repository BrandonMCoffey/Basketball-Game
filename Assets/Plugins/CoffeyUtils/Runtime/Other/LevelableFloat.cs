using UnityEngine;

namespace CoffeyUtils
{
    [System.Serializable]
    public struct LevelableFloat
    {
        [Tooltip("Check to enable multiple levels")]
        [SerializeField] public bool isLevelable;
        [SerializeField] private float[] levels;

        public LevelableFloat(bool levelable = true, float level1value = 0f, float level2value = 0f, float level3value = 0f)
        {
            isLevelable = levelable;
            levels = new float[] { level1value, level2value, level3value };
        }

        //public static implicit operator float(LevelableFloat lf) => lf.GetValue(1);
        public override string ToString() => GetValue(1).ToString();

        // Level starts as 1 and goes up (normally to 3)
        public float GetValue(int level)
        {
            if (levels == null || levels.Length == 0) return 0f;
            if (!isLevelable) return levels[0];

            int index = Mathf.FloorToInt(level) - 1;
            index = Mathf.Clamp(index, 0, levels.Length - 1);

            return levels[index];
        }
    }
}