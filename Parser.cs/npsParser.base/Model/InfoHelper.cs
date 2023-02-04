using System;
using System.Collections.Generic;

namespace nf.protoscript
{

    /// <summary>
    /// Helper functions for Info.
    /// </summary>
    public static class InfoHelper
    {

        public static char NameSplitter { get; } = '/';

        /// <summary>
        /// Get fullname of an info.
        /// </summary>
        /// <remarks>
        /// 'Access-by-name' functions cannot be used with Infos whose name may have conflicts with others of the same parent.
        /// </remarks>
        /// <param name="InInfo"></param>
        /// <returns></returns>
        public static string GetFullnameOfInfo(Info InInfo)
        {
            List<string> names = new List<string>();

            // Iterate the Info-tree and gather all info's names.
            Info curInfo = InInfo;
            while (curInfo != null
                && curInfo != Info.Root
                )
            {
                names.Add(curInfo.Name);
                curInfo = curInfo.ParentInfo;
            }
            System.Diagnostics.Debug.Assert(curInfo == Info.Root);

            if (names.Count == 0)
            {
                throw new ArgumentException("Invalid info");
            }

            // Merge names to a single name.
            // N0/N1/N2/N3/N4
            names.Reverse();
            string result = names[0];
            for (int nameIdx = 1; nameIdx < names.Count; nameIdx++)
            {
                string subName = names[nameIdx];
                result += $"{NameSplitter}{subName}";
            }
            return result;
        }

        /// <summary>
        /// Find info by its fullname.
        /// </summary>
        /// <param name="InInfoFullname"></param>
        /// <returns></returns>
        public static Info FindInfoByFullname(string InInfoFullname)
        {
            string[] names = InInfoFullname.Split(NameSplitter);

            // Find Project or package from the first level.
            Info curInfo = Info.Root;

            for (int nameIdx = 0; nameIdx < names.Length; nameIdx++)
            {
                string subName = names[nameIdx];
                curInfo = curInfo.FindTheFirstSubInfoWithName<Info>(subName);
                if (curInfo == null)
                { break; }
            }
            return curInfo;
        }

        /// <summary>
        /// Find property along scope tree.
        /// </summary>
        /// <param name="InContextInfo">The first scope to find the target property.</param>
        /// <param name="InIDName"></param>
        /// <returns></returns>
        public static Info FindPropertyAlongScopeTree(Info InContextInfo, string InIDName)
        {
            Info contextInfo = InContextInfo;
            while (contextInfo != null)
            {
                // find properties of the context
                Info foundInfo = contextInfo.FindTheFirstSubInfoWithName<MemberInfo>(InIDName);
                if (foundInfo != null)
                { return foundInfo; }

                // if the context is a member, find properties in its archetype.
                MemberInfo member = contextInfo as MemberInfo;
                if (member != null)
                {
                    foundInfo = member.Archetype.FindTheFirstSubInfoWithName<MemberInfo>(InIDName);
                    if (foundInfo != null)
                    { return foundInfo; }
                }

                // if the context is a method, find properties in its signature.
                MethodInfo method = contextInfo as MethodInfo;
                if (method != null)
                {
                    foundInfo = method.MethodSignature.FindTheFirstSubInfoWithName<MemberInfo>(InIDName);
                    if (foundInfo != null)
                    { return foundInfo; }
                }

                contextInfo = contextInfo.ParentInfo;
            }
            return null;
        }
    }

}