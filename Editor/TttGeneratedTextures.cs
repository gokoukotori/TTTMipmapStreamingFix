using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using nadena.dev.ndmf;
using UnityEngine;

namespace Gokoukotori.TTTMipmapStreamingFix
{
    internal static class TttGeneratedTextures
    {
        private const string AssemblyName = "net.rs64.tex-trans-tool.editor";
        private const BindingFlags InstanceMembers = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        internal static bool TryGet(BuildContext context, out List<Texture2D> textures, out string error)
        {
            textures = new List<Texture2D>();
            error = null;
            var sessionType = Type.GetType("net.rs64.TexTransTool.Build.TexTransBuildSession, " + AssemblyName);
            if (sessionType == null)
            {
                error = "TTT のビルドセッション型を取得できません。";
                return false;
            }

            object session;
            try
            {
                session = typeof(TttGeneratedTextures).GetMethod(nameof(GetExistingSession), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(sessionType).Invoke(null, new object[] { context });
            }
            catch (TargetInvocationException ex) when (ex.InnerException is MissingSessionException)
            {
                return true;
            }

            var domainProperty = sessionType.GetProperty("Domain", InstanceMembers);
            var domainType = Type.GetType("net.rs64.TexTransTool.RenderersDomain, " + AssemblyName);
            var managerField = domainType?.GetField("_renderTextureDescriptorManager", InstanceMembers);
            var descriptorsProperty = managerField?.FieldType.GetProperty("DownloadedDescriptors", InstanceMembers);
            if (session == null || domainProperty == null || managerField == null || descriptorsProperty == null)
            {
                error = "TTT の生成テクスチャ一覧の構造が対応バージョンと異なります。";
                return false;
            }
            var domain = domainProperty.GetValue(session);
            if (domain == null || !domainType.IsInstanceOfType(domain))
            {
                error = "TTT のビルドドメインを取得できません。";
                return false;
            }
            var manager = managerField.GetValue(domain);
            var descriptors = manager == null ? null : descriptorsProperty.GetValue(manager) as IDictionary;
            if (descriptors == null)
            {
                error = "TTT の生成テクスチャ一覧を取得できません。";
                return false;
            }
            foreach (var key in descriptors.Keys)
            {
                if (!(key is Texture2D texture))
                {
                    textures.Clear();
                    error = "TTT の生成テクスチャ一覧に未対応の型が含まれています。";
                    return false;
                }
                if (texture) textures.Add(texture);
            }
            return true;
        }

        private static object GetExistingSession<T>(BuildContext context) =>
            context.GetState<T>(_ => throw new MissingSessionException());

        private sealed class MissingSessionException : Exception { }
    }
}
