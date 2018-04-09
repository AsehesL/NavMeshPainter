using UnityEngine;
using System.Collections;
using System;

namespace ASL.NavMeshPainter.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomNavMeshToolEditorAttribute : Attribute
    {
        public Type navMeshToolType;

        public CustomNavMeshToolEditorAttribute(Type navMeshToolType)
        {
            this.navMeshToolType = navMeshToolType;
        }
    }
}