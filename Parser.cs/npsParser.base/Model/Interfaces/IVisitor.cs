using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace nf.protoscript.syntaxtree
{

    ///// <summary>
    ///// A visitable object which can accept a visitor.
    ///// </summary>
    //public interface IVisitable
    //{

    //    void AcceptVisitor(IVisitor InVisitor);


    //}


    ///// <summary>
    ///// Visitor to visit a visitable object.
    ///// </summary>
    //public interface IVisitor
    //{

    //    void Visit(IVisitable InVisitable);

    //}


    public static class VisitByReflectionHelper
    {
        /// <summary>
        /// Find the best Visit method which can match this visitable's type.
        /// </summary>
        public static bool FindAndCallVisit(object InThis, object InVisitor)
        {
            Type thisType = InThis.GetType();

            // Find all 'Visit' methods.
            Type visitorType = InVisitor.GetType();
            var visitorMethods = visitorType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var visits = from m in visitorMethods where m.Name == "Visit" select m;

            // Find the best method which matches the target IVisitable.
            MethodInfo bestVisit = _FindTheBestVisit(visits, thisType);
            if (bestVisit != null)
            {
                // Invoke the Visit function.
                bestVisit.Invoke(InVisitor, new object[] { InThis });
                return true;
            }

            // TODO Find the method with the parameter which implements IVisitable interface.

            return false;
        }

        /// <summary>
        /// Find the best 'Visit' to match the InThisType.
        /// </summary>
        /// <example>
        /// TODO: fix comments
        /// </example>
        /// <param name="InVisitMethods"></param>
        /// <param name="InThisType"></param>
        /// <returns></returns>
        private static MethodInfo _FindTheBestVisit(IEnumerable<MethodInfo> InVisitMethods, Type InThisType)
        {
            List<(int, MethodInfo)> matchingMethods = new List<(int, MethodInfo)>();
            foreach (var visitMtd in InVisitMethods)
            {
                var visitParams = visitMtd.GetParameters();

                // Ignore Visits with the wrong numbers of parameters.
                if (visitParams.Length != 1)
                { continue; }

                // Check if the parameter type matches the target type.
                int inheritDepth = _CheckInheritDepth(visitParams[0].ParameterType, InThisType);
                if (inheritDepth < 0)
                { continue; }

                matchingMethods.Add((inheritDepth, visitMtd));
            }

            if (matchingMethods.Count == 0)
            { return null; }

            // Sort matching methods
            matchingMethods.Sort((lhs, rhs) => lhs.Item1 - rhs.Item1);
            return matchingMethods[0].Item2;
        }


        /// <summary>
        /// Checking the inherit-depth between InBaseType and InTargetType.
        /// 
        /// If the InTargetType is not inherit from InBaseType, return -1.
        /// 
        /// </summary>
        /// <example>
        ///     class C0 {};
        ///     class C1:C0{};
        ///     class C2:C1 {};
        ///     
        ///     BaseType | TargetType | Result
        ///     C0       | C0         | 0
        ///     C0       | C1         | 1
        ///     C0       | C2         | 2
        ///     C1       | C2         | 1
        ///     C2       | C0         | -1
        ///     C0       | Object     | -1
        /// </example>
        /// <param name="InBaseType"></param>
        /// <param name="InTargetType"></param>
        /// <returns></returns>
        private static int _CheckInheritDepth(Type InBaseType, Type InTargetType)
        {
            Type checkingType = InTargetType;
            for (int i = 0; ; i++)
            {
                if (checkingType == null)
                { break; }

                if (checkingType == InBaseType)
                { return i; }
                checkingType = checkingType.BaseType;
            }
            return -1;
        }


    }


}
