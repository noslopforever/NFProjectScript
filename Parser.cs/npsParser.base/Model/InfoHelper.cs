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
        /// Try find variables in global.
        /// </summary>
        /// <param name="InProjectInfo"></param>
        /// <param name="InIDName"></param>
        /// <returns></returns>
        public static ElementInfo FindPropertyInGlobal(ProjectInfo InProjectInfo, string InIDName)
        {
            ElementInfo foundInfo = InProjectInfo.FindTheFirstSubInfoWithName<ElementInfo>(InIDName);

            return foundInfo;

            // TODO global object or singleton object
            throw new NotImplementedException();
        }


        /// <summary>
        /// Try find variables in element's archetype
        /// </summary>
        /// <param name="InElementInfo"></param>
        /// <param name="InIDName"></param>
        /// <returns></returns>
        public static ElementInfo FindPropertyInArchetype(ElementInfo InElementInfo, string InIDName)
        {
            ElementInfo foundInfo = null;

            // If the context is a member/method, try find properties in its archetype first.
            // We can find parameters (of method) or subs (of members) here.
            foundInfo = FindPropertyOfType(InElementInfo.ElementType, InIDName);
            if (foundInfo != null)
            {
                return foundInfo;
            }

            // find in context's sub-elements.
            //
            // These sub-elements will be taken as members of the 'inline' archetype of the InElement.
            foundInfo = InElementInfo.FindTheFirstSubInfoWithName<ElementInfo>(InIDName);
            if (foundInfo != null)
            {
                return foundInfo;

                // TODO The conception of Inline-Archetype should be more clear.
                throw new NotImplementedException();
            }

            return null;
        }


        /// <summary>
        /// Find property along scope tree.
        /// </summary>
        /// <param name="InContextInfo">The first scope to find the target property.</param>
        /// <param name="InIDName"></param>
        /// <returns></returns>
        /// TODO Deprecate, Replaced by FindInXXXX series.
        public static ElementInfo FindPropertyAlongScopeTree(Info InContextInfo, string InIDName)
        {
            Info contextInfo = InContextInfo;
            while (contextInfo != null)
            {
                ElementInfo foundInfo = null;

                // if current context is a Type, try find its properties.
                if (contextInfo is TypeInfo)
                {
                    foundInfo = FindPropertyOfType(contextInfo as TypeInfo, InIDName);
                    if (foundInfo != null)
                    {
                        return foundInfo;
                    }
                }
                else
                {
                    // find in context's sub-elements.
                    //
                    // Most of times, the context should be a method.
                    // We can find local-parameters here.
                    foundInfo = contextInfo.FindTheFirstSubInfoWithName<ElementInfo>(InIDName);
                    if (foundInfo != null)
                    {
                        return foundInfo;
                    }
                }

                // If the context is a member/method, try find properties in its archetype.
                // We can find parameters (of method) or subs (of members) here.
                ElementInfo member = contextInfo as ElementInfo;
                if (member != null)
                {
                    foundInfo = FindPropertyOfType(member.ElementType, InIDName);
                    if (foundInfo != null)
                    {
                        return foundInfo;
                    }
                }

                // Cannot found in this level, try the parent level.
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