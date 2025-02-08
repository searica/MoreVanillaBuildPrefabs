using System;
using System.Diagnostics;
using Logging;

namespace Configs;
internal static class SafeInvokeEvent
{
    /// <summary>
    ///     try/catch the delegate chain so that it doesn't break on the first failing Delegate.
    /// </summary>
    /// <param name="events"></param>
    internal static void SafeInvoke(this Action events)
    {
        if (events == null)
        {
            return;
        }

        foreach (Action @event in events.GetInvocationList())
        {
            try
            {
                @event();
            }
            catch (Exception e)
            {
                Log.LogWarning(
                    $"Exception thrown at event {new StackFrame(1).GetMethod().Name}"
                    + $" in {@event.Method.DeclaringType.Name}.{@event.Method.Name}:\n{e}"
                );
            }
        }
    }
}
