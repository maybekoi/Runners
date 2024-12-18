using System.IO;
using System.Linq;
using UnityEditor;

public static class RemoveDeletedScenes
{
    [MenuItem("Phlox Tools/RemoveDeletedScenes")]
    public static void CleanUpDeletedScenes()
    {
        var currentScenes = EditorBuildSettings.scenes;
        var filteredScenes = currentScenes.Where(ebss => File.Exists(ebss.path)).ToArray();
        EditorBuildSettings.scenes = filteredScenes;
    }
}