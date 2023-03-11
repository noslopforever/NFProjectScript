using System;
using System.Collections.Generic;
using System.Reflection;

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
        public static ElementInfo FindPropertyAlongScopeTree(Info InContextInfo, string InIDName)
        {
            if (InContextInfo is TypeInfo)
            { return FindPropertyOfType(InContextInfo as TypeInfo, InIDName); }

            Info contextInfo = InContextInfo;
            while (contextInfo != null)
            {
                // find in context's properties.
                // Most of times, the context should be a method.
                // We can find local-parameters here.
                Info foundInfo = contextInfo.FindTheFirstSubInfoWithName<ElementInfo>(InIDName);
                if (foundInfo != null)
                { return foundInfo as ElementInfo; }

                // If the context is a member/method, try find properties in its archetype.
                // We can find parameters (of method) or subs (of members) here.
                ElementInfo member = contextInfo as ElementInfo;
                if (member != null)
                {
                    foundInfo = FindPropertyOfType(member.ElementType, InIDName);
                    if (foundInfo != null)
                    { return foundInfo as ElementInfo; }
                }

                contextInfo = contextInfo.ParentInfo;
            }
            return null;
        }

        /// <summary>
        /// Find property of Type.
        /// </summary>
        /// <param name="InContextInfo">The first scope to find the target property.</param>
        /// <param name="InIDName"></param>
        /// <returns></returns>
        public static ElementInfo FindPropertyOfType(TypeInfo InTypeInfo, string InIDName)
        {
            // find in context's properties.
            Info foundInfo = InTypeInfo.FindTheFirstSubInfoWithName<ElementInfo>(InIDName);
            if (foundInfo != null)
            { return foundInfo as ElementInfo; }

            if (InTypeInfo.BaseType != null)
            {
                foundInfo = FindPropertyOfType(InTypeInfo.BaseType, InIDName);
                if (foundInfo != null)
                { return foundInfo as ElementInfo; }
            }

            return null;
        }

        /// <summary>
        /// Find Type from ProjectInfo.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InTypeName"></param>
        /// <returns></returns>
        public static TypeInfo FindType(ProjectInfo InProjectInfo, string InTypeName)
        {
            return InProjectInfo.FindTheFirstSubInfoWithName<TypeInfo>(InTypeName);
        }

    }

}