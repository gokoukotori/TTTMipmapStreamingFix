using System.Runtime.CompilerServices;
using nadena.dev.ndmf;
using UnityEditor;
using UnityEngine;

[assembly: ExportsPlugin(typeof(Gokoukotori.TTTMipmapStreamingFix.MipmapStreamingPlugin))]
[assembly: InternalsVisibleTo("com.gokoukotori.ttt-mipmap-streaming-fix.tests")]

namespace Gokoukotori.TTTMipmapStreamingFix
{
    internal sealed class MipmapStreamingPlugin : Plugin<MipmapStreamingPlugin>
    {
        public override string QualifiedName => "com.gokoukotori.ttt-mipmap-streaming-fix";
        public override string DisplayName => "TTT Mipmap Streaming Fix";

        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .AfterPlugin("net.rs64.tex-trans-tool")
                .Run("Enable streaming on TTT-generated textures", context =>
                {
                    if (!TttGeneratedTextures.TryGet(context, out var textures, out var error))
                    {
                        Debug.LogWarning("[TTT Mipmap Streaming Fix] " + error +
                            " テクスチャは変更していません。対応する TexTransTool のバージョンを確認してください。",
                            context.AvatarRootObject);
                        return;
                    }
                    foreach (var texture in textures) EnableStreaming(context, texture);
                });
        }

        internal static bool EnableStreaming(BuildContext context, Texture2D texture)
        {
            if (!texture || !context.IsTemporaryAsset(texture) ||
                texture.mipmapCount <= 1 || texture.streamingMipmaps) return false;

            using (var serialized = new SerializedObject(texture))
            {
                serialized.FindProperty("m_StreamingMipmaps").boolValue = true;
                serialized.ApplyModifiedPropertiesWithoutUndo();
            }
            return true;
        }
    }
}
