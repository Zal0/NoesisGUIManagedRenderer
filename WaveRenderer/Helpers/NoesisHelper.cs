// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.NoesisGUI.Helpers
{
    /// <summary>
    /// Noesis Helper Class.
    /// </summary>
    public static class NoesisHelper
    {
        /// <summary>
        /// Converts WaveEngine MouseButtons enumerator to Noesis MouseButton enumerator.
        /// </summary>
        /// <param name="waveButton">Button from wave Button enumerator.</param>
        /// <returns>Converted input to Noesis button enumerator.</returns>
        public static Noesis.MouseButton ToNoesis(this Common.Input.Mouse.MouseButtons waveButton)
        {
            switch (waveButton)
            {
                default:
                case Common.Input.Mouse.MouseButtons.Left:
                    return Noesis.MouseButton.Left;
                case Common.Input.Mouse.MouseButtons.Right:
                    return Noesis.MouseButton.Right;
                case Common.Input.Mouse.MouseButtons.Middle:
                    return Noesis.MouseButton.Middle;
                case Common.Input.Mouse.MouseButtons.XButton1:
                    return Noesis.MouseButton.XButton1;
                case Common.Input.Mouse.MouseButtons.XButton2:
                    return Noesis.MouseButton.XButton2;
            }
        }
    }
}
