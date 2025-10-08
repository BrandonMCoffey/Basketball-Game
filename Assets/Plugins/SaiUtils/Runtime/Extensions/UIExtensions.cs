using UnityEngine.UI;

namespace SaiUtils.Extensions
{
    public static class UIExtensions
    {
        public static void SetAlpha(this Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}