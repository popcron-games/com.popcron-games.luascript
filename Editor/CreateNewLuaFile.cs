#nullable enable
using UnityEditor;

namespace Paintime
{
    public static class CreateNewLuaFile
    {
        [MenuItem("Assets/Create/Lua Script", false, 1)]
        private static void DoIt()
        {
            ProjectWindowUtil.CreateAssetWithContent("Script.lua", string.Empty);
        }
    }
}