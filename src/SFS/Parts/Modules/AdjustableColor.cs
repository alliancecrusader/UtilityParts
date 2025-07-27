using SFS.Variables;
using System.Runtime.InteropServices;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace SFS.Parts.Modules
{
    public class AdjustableColor : ColorModule
    {
       public static Color HexToColor(string hex)
        {
            Debug.Log(hex);
            hex = hex.TrimStart('#');
            int value;

            try
            {
                value = Convert.ToInt32(hex, 16);
            }
            catch (FormatException)
            {
                Debug.LogError($"Invalid hex format: {hex}");
                return Color.white; // Return a default color in case of error
            }
            catch (OverflowException)
            {
                Debug.LogError($"Hex value too large: {hex}");
                return Color.white; // Return a default color in case of error
            }
            
            if (hex.Length == 6)
            {
                return new Color(
                    ((value >> 16) & 0xFF) / 255.0f,
                    ((value >> 8) & 0xFF) / 255.0f,
                    (value & 0xFF) / 255.0f
                );
            }
            else
            {
                return new Color(
                    ((value >> 16) & 0xFF) / 255.0f,
                    ((value >> 8) & 0xFF) / 255.0f,
                    (value & 0xFF) / 255.0f,
                    ((value >> 24) & 0xFF) / 255.0f
                );
            }
        }
        [BoxGroup("Tex", false)] public Composed_String Hex;
        public override Color GetColor()
        {
            return HexToColor(Hex.Value);
        }
    }
}