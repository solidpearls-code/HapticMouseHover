namespace Loupedeck.HapticMouseHoverPlugin
{
    /// <summary>
    /// Toggles hover detection to raise a haptic event over clickable controls.
    /// </summary>
    public class HoverHapticCommand : PluginDynamicCommand
    {
        private const string PluginEventName = "HapticMouseHover";
        private const string HapticEventName = "buttonPress";

        private MouseHoverDetector _hoverDetector;
        private bool _isEnabled = true;

        /// <summary>
        /// Creates a new command instance.
        /// </summary>
        public HoverHapticCommand() : base(
            displayName: "Haptic Mouse Hover",
            description: "Provides haptic feedback when the mouse hovers over a clickable control.",
            groupName: "Commands")
        {
        }

        /// <summary>
        /// Registers the plugin event and starts hover detection if enabled.
        /// </summary>
        protected override Boolean OnLoad()
        {
            this.Plugin.PluginEvents.AddEvent(
                PluginEventName,
                "Haptic Mouse Hover",
                "Provides haptic feedback when the mouse hovers over a clickable control."
            );

            if (this._isEnabled)
            {
                this.StartHoverDetector();
            }

            return true;
        }

        /// <summary>
        /// Toggles hover detection on/off when the command is invoked.
        /// </summary>
        protected override void RunCommand(String actionParameter)
        {
            this._isEnabled = !this._isEnabled;

            if (this._isEnabled)
            {
                this.StartHoverDetector();
            }
            else
            {
                this.StopHoverDetector();
            }

            this.ActionImageChanged();
        }

        /// <summary>
        /// Displays the ON/OFF state on the button.
        /// </summary>
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Haptic Hover{Environment.NewLine}{(this._isEnabled ? "ON" : "OFF")}";

        /// <summary>
        /// Stops hover detection and cleans up resources.
        /// </summary>
        protected override Boolean OnUnload()
        {
            this.StopHoverDetector();
            return base.OnUnload();
        }

        private void StartHoverDetector()
        {
            if (this._hoverDetector != null)
            {
                return;
            }

            this._hoverDetector = new MouseHoverDetector(this.OnHoverDetected);
        }

        private void StopHoverDetector()
        {
            this._hoverDetector?.Dispose();
            this._hoverDetector = null;
        }

        private void OnHoverDetected()
        {
            this.Plugin.PluginEvents.RaiseEvent(HapticEventName);
        }
    }
}
