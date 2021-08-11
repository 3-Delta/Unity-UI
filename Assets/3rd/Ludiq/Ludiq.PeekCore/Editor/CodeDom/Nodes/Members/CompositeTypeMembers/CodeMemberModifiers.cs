// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Ludiq.PeekCore.CodeDom
{
    public enum CodeMemberModifiers
    {
        Abstract = 0x0001,
        Virtual = 0x0002,
        Static = 0x0003,
        Override = 0x0004,
        Const = 0x0005,
		Sealed = 0x0006,
        ScopeMask = 0x000F,

		ReadOnly = 0x0010,
        ReadOnlyMask = 0x00F0,

        New = 0x0100,
        NewMask = 0x0F00,

        Internal = 0x1000,
        Private = 0x2000,
        Protected = 0x3000,
        ProtectedInternal = 0x4000,
        Public = 0x5000,
        AccessMask = 0xF000,
    }
}
