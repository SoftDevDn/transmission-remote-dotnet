using System;
using System.Reflection;
using System.Windows.Forms;
using TransmissionRemoteDotnet.Localization;

namespace TransmissionRemoteDotnet
{
    public static class Fixes
    {
        public static void SetNotifyIconText(NotifyIcon ni, string text)
        {
            if (text.Length >= 128) throw new ArgumentOutOfRangeException(OtherStrings.NotifyIconLimitWarning);
            Type t = typeof(NotifyIcon);
            BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
            t.GetField("text", hidden)?.SetValue(ni, text);
            var fieldValue = t.GetField("added", hidden)?.GetValue(ni);
            if (fieldValue != null && (bool)fieldValue)
                t.GetMethod("UpdateIcon", hidden)?.Invoke(ni, new object[] { true });
        }
    }
}
