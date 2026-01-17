namespace Loupedeck.HapticMouseHoverPlugin
{
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Automation;

    /// <summary>
    /// Detects mouse hover over clickable UIAutomation elements and invokes a callback.
    /// </summary>
    public sealed class MouseHoverDetector : IDisposable
    {
        private static readonly AutomationPattern[] ClickablePatterns =
        {
            InvokePattern.Pattern,
            TogglePattern.Pattern,
            SelectionItemPattern.Pattern,
            ExpandCollapsePattern.Pattern,
            ValuePattern.Pattern,
        };

        private readonly Action _onHoverAction;
        private readonly TimeSpan _pollInterval;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _pollingTask;
        private AutomationElement _lastHoveredElement;
        private bool _disposed;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// Creates a new hover detector.
        /// </summary>
        /// <param name="onHoverAction">Invoked when the cursor hovers a new clickable control.</param>
        public MouseHoverDetector(Action onHoverAction)
        {
            this._onHoverAction = onHoverAction;
            this._pollInterval = TimeSpan.FromMilliseconds(10);
            this._cancellationTokenSource = new CancellationTokenSource();
            this._pollingTask = this.RunPollingLoopAsync(this._cancellationTokenSource.Token);
        }

        private async Task RunPollingLoopAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(this._pollInterval);

            try
            {
                while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        this.ProcessHover();
                    }
                    catch (Exception)
                    {
                        // Ignore UIAutomation errors (e.g., element disappeared).
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void ProcessHover()
        {
            if (!GetCursorPos(out POINT cursorPos))
            {
                return;
            }

            var element = AutomationElement.FromPoint(new System.Windows.Point(cursorPos.X, cursorPos.Y));

            if (element == null)
            {
                return;
            }

            if (this.IsClickableElement(element))
            {
                if (!IsSameElement(element, this._lastHoveredElement))
                {
                    this._lastHoveredElement = element;
                    this._onHoverAction();
                }
            }
            else
            {
                this._lastHoveredElement = null;
            }
        }

        private bool IsClickableElement(AutomationElement element)
        {
            foreach (var pattern in ClickablePatterns)
            {
                try
                {
                    if (element.TryGetCurrentPattern(pattern, out _))
                    {
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }

            return false;
        }

        private static bool IsSameElement(AutomationElement element1, AutomationElement element2)
        {
            if (element1 == null || element2 == null)
            {
                return false;
            }

            try
            {
                return Automation.Compare(element1, element2);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._cancellationTokenSource.Cancel();

                try
                {
                    this._pollingTask?.Wait(TimeSpan.FromSeconds(1));
                }
                catch (AggregateException)
                {
                }

                this._cancellationTokenSource.Dispose();
            }

            this._disposed = true;
        }
    }
}
