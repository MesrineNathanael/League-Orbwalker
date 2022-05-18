using System;

namespace Gumayusi_Orbwalker.Core.League.Commons.Enums
{
    [Flags]
    public enum KeyEventFlags : uint
    {
        KEYEVENTF_EXTENDEDKEY = 0x0001u,
        KEYEVENTF_KEYUP = 0x0002u,
        KEYEVENTF_SCANCODE = 0x0008u,
        KEYEVENTF_UNICODE = 0x0004u
    }
}
