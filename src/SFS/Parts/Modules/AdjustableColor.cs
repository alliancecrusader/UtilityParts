using SFS.Variables;
using UnityEngine;

namespace SFS.Parts.Modules
{
    public class AdjustableColor : ColorModule
    {
        public Composed_Float alpha;
        public Composed_Float red;
        public Composed_Float green;
        public Composed_Float blue;
        public override Color GetColor()
        {
            return new Color(red.Value, green.Value, blue.Value, alpha.Value);
        }
    }
}
