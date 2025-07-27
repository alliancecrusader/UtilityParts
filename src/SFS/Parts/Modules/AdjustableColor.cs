using SFS.Variables;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class AdjustableColor : ColorModule
    {
        public Composed_String hex;
        public Composed_Float alpha;
        public override Color GetColor()
        {
            if (ColorUtility.TryParseHtmlString(this.hex.Value, out Color newColor)) {
                newColor.a = alpha.Value;
                return newColor;
            } else
            {
                return Color.white;
            }
        }
    }
}
