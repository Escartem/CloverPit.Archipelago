using BepInEx;
using HarmonyLib;
using Panik;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

[HarmonyPatch]
public static class SteamPatch
{
    static MethodBase TargetMethod()
    {
        var init = AccessTools.Method(typeof(PlatformAPI), nameof(PlatformAPI.Initialize));
        var sm = init.GetCustomAttribute<AsyncStateMachineAttribute>().StateMachineType;
        return AccessTools.Method(sm, "MoveNext");
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var steamCtor = AccessTools.Constructor(typeof(PlatformAPI_Steam));
        var nooneCtor = AccessTools.Constructor(typeof(PlatformAPI_Noone));

        foreach (var ins in instructions)
        {
            if (ins.opcode == OpCodes.Newobj && (ConstructorInfo)ins.operand == steamCtor)
                ins.operand = nooneCtor;
            yield return ins;
        }
    }
}
