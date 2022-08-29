using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using Unity.Profiling;
using UnityEngine.TestRunner.NUnitExtensions.Runner;

namespace UnityEngine.TestTools
{
    internal class SetUpTearDownCommand : BeforeAfterTestCommandBase<MethodInfo>
    {
        static readonly Dictionary<Type, List<MethodInfo>> m_BeforeActionsCache = new Dictionary<Type, List<MethodInfo>>();
        static readonly Dictionary<Type, List<MethodInfo>> m_AfterActionsCache = new Dictionary<Type, List<MethodInfo>>();

        public SetUpTearDownCommand(TestCommand innerCommand)
            : base(innerCommand, "SetUp", "TearDown", true)
        {
            using (new ProfilerMarker(nameof(SetUpTearDownCommand)).Auto())
            {
                if (Test.TypeInfo.Type != null)
                {
                    BeforeActions = GetActions(m_BeforeActionsCache, Test.TypeInfo.Type, typeof(SetUpAttribute), typeof(void));
                    AfterActions =  GetActions(m_AfterActionsCache, Test.TypeInfo.Type, typeof(TearDownAttribute), typeof(void)).Reverse().ToArray();
                }
            }
        }
        
        protected override IEnumerator InvokeBefore(MethodInfo action, Test test, UnityTestExecutionContext context)
        {
            using (new ProfilerMarker(test.Name + ".Setup").Auto())
                Reflect.InvokeMethod(action, context.TestObject);
            yield return null;
        }

        protected override IEnumerator InvokeAfter(MethodInfo action, Test test, UnityTestExecutionContext context)
        {
            using (new ProfilerMarker(test.Name + ".TearDown").Auto())
                Reflect.InvokeMethod(action, context.TestObject);
            yield return null;
        }

        protected override BeforeAfterTestCommandState GetState(UnityTestExecutionContext context)
        {
            return null;
        }
    }
}
