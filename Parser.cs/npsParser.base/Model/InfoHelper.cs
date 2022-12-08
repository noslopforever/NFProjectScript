﻿using System;

namespace nf.protoscript
{

    /// <summary>
    /// Helper functions for Info.
    /// </summary>
    public static class InfoHelper
    {

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