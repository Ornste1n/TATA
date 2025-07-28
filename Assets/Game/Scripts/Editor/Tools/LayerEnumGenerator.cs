using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Editor.Tools
{
    public static class LayerEnumGenerator
    {
        private const string EnumName = "WorldLayers";
        private const string FilePath = "Assets/Game/Scripts/WorldSettings/WorldLayers.cs";

        [MenuItem("Tools/Generate Layers %#&i")]
        private static async void Execute()
        {
            string directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("//This file is auto-generated.");
            builder.AppendLine("namespace Game.Scripts.WorldSettings");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic enum {EnumName}");
            builder.AppendLine("\t{");

            for (int i = 0; i < 16; i++)
            {
                string name = LayerMask.LayerToName(i);
                
                if(string.IsNullOrEmpty(name)) continue;

                if (name.Contains(" ")) name = name.Replace(" ", "");
                
                builder.AppendLine($"\t\t{name} = {i},");
            }
            
            builder.AppendLine("\t}");
            builder.AppendLine("}");
            
            using (StreamWriter writer = new (FilePath))
            {
                await writer.WriteAsync(builder.ToString());
            }
            
            builder.Clear();
            AssetDatabase.Refresh();
            Debug.Log($"✅ Enum {EnumName} generated to {FilePath}");
        }
    }
}