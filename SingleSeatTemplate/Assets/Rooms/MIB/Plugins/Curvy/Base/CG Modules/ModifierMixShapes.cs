// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.DevTools;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Modifier/Mix Shapes", ModuleName = "Mix Shapes", Description = "Lerps between two shapes")]
    [HelpURL(CurvySpline.DOCLINK + "cgmixshapes")]
    public class ModifierMixShapes : CGModule, IOnRequestPath
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGShape), Name = "Shape A")]
        public CGModuleInputSlot InShapeA = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGShape), Name = "Shape B")]
        public CGModuleInputSlot InShapeB = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGShape))]
        public CGModuleOutputSlot OutShape = new CGModuleOutputSlot();

        #region ### Serialized Fields ###

        [SerializeField, RangeEx(-1, 1, Tooltip = "Mix between the paths")]
        float m_Mix;

        #endregion

        #region ### Public Properties ###

        public float Mix
        {
            get { return m_Mix; }
            set
            {
                if (m_Mix != value)
                    m_Mix = value;
                Dirty = true;
            }
        }


        public float PathLength
        {
            get
            {
                return (IsConfigured) ? Mathf.Max((InShapeA.SourceSlot().OnRequestPathModule).PathLength,
                                                  (InShapeB.SourceSlot().OnRequestPathModule).PathLength) : 0;
            }
        }

        public bool PathIsClosed
        {
            get
            {
                return (IsConfigured) ? InShapeA.SourceSlot().OnRequestPathModule.PathIsClosed &&
                                        InShapeB.SourceSlot().OnRequestPathModule.PathIsClosed : false;
            }
        }

        #endregion

        #region ### Private Fields & Properties ###
        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.MinWidth = 200;
            Properties.LabelWidth = 50;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Mix = m_Mix;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            Mix = 0;
        }

        /*! \endcond */
        #endregion

        #region ### IOnRequestProcessing ###
        public CGData[] OnSlotDataRequest(CGModuleInputSlot requestedBy, CGModuleOutputSlot requestedSlot, params CGDataRequestParameter[] requests)
        {
            var raster = GetRequestParameter<CGDataRequestRasterization>(ref requests);
            if (!raster)
                return null;

            var DataA = InShapeA.GetData<CGShape>(requests);
            var DataB = InShapeB.GetData<CGShape>(requests);
            CGShape data = MixShapes(DataA, DataB, Mix, UIMessages);
            return new CGData[1] { data };
        }

        #endregion

        #region ### Public Methods ###

        /// <summary>
        /// Returns the mixed shape
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        /// <param name="mix"> A value between -1 and 1. -1 will select the shape with the most points. 1 will select the other </param>
        /// <param name="warningsContainer">Is filled with warnings raised by the mixing logic</param>
        /// <returns> The mixed shape</returns>
        public static CGShape MixShapes(CGShape shapeA, CGShape shapeB, float mix, [NotNull] List<string> warningsContainer)
        {
            CGShape mainShape = shapeA.Count > shapeB.Count
                ? shapeA
                : shapeB;

            CGShape secondaryShape = shapeA.Count > shapeB.Count
                ? shapeB
                : shapeA;

            CGShape data = new CGShape();
            InterpolateShape(data, mainShape, secondaryShape, mix, warningsContainer);
            return data;
        }

        /// <summary>
        /// Returns the mixed shape
        /// </summary>
        /// <param name="resultShape">A shape which will be filled with the data of the mixed shape</param>
        /// <param name="mainShape">This shape will be used for both mixable properties and unmixable ones</param>
        /// <param name="secondaryShape">This shape will be used only for mixable properties</param>
        /// <param name="mix"> A value between -1 and 1. -1 will select mainShape. 1 will select the other </param>
        /// <param name="warningsContainer">Is filled with warnings raised by the mixing logic</param>
        /// <returns> The mixed shape</returns>
        static public void InterpolateShape([NotNull]CGShape resultShape, CGShape mainShape, CGShape secondaryShape, float mix, [NotNull] List<string> warningsContainer)
        {
            float interpolationTime = (mix + 1) * 0.5f;
            Assert.IsTrue(interpolationTime >= 0);
            Assert.IsTrue(interpolationTime <= 1);

            Vector3[] positions = new Vector3[mainShape.Count];
            Vector3[] normals = new Vector3[mainShape.Count];

            Bounds newBounds = new Bounds();
            for (int i = 0; i < mainShape.Count; i++)
            {
                float frag;
                int idx = secondaryShape.GetFIndex(mainShape.F[i], out frag);

                Vector3 secondaryPosition = Vector3.LerpUnclamped(secondaryShape.Position[idx], secondaryShape.Position[idx + 1], frag);
                positions[i] = Vector3.LerpUnclamped(mainShape.Position[i], secondaryPosition, interpolationTime);

                Vector3 secondaryNormal = Vector3.LerpUnclamped(secondaryShape.Normal[idx], secondaryShape.Normal[idx + 1], frag);
                normals[i] = Vector3.LerpUnclamped(mainShape.Normal[i], secondaryNormal, interpolationTime);

                newBounds.Encapsulate(positions[i]);
            }
            resultShape.Position = positions;
            //BUG Normal need to be recomputed based on actual new positions, instead of interpolated from both sources normals. resultShape.RecalculateNormals() doesn't seem to work correctly
            resultShape.Normal = normals;
            resultShape.Map = (float[])mainShape.Map.Clone();

            resultShape.F = new float[mainShape.Count];
            // sets Length and F
            resultShape.Recalculate();

            //BUG SourceF need to be recomputed, because it is based on distance and not TF
            resultShape.SourceF = (float[])mainShape.SourceF.Clone();
#pragma warning disable 618
            resultShape.Bounds = newBounds;
#pragma warning restore 618
            resultShape.MaterialGroups = new List<SamplePointsMaterialGroup>(mainShape.MaterialGroups);

            if (mainShape.Closed != secondaryShape.Closed)
                warningsContainer.Add("Mixing inputs with different Closed values is not supported");
            if (mainShape.Seamless != secondaryShape.Seamless)
                warningsContainer.Add("Mixing inputs with different Seamless values is not supported");
            if (mainShape.SourceIsManaged != secondaryShape.SourceIsManaged)
                warningsContainer.Add("Mixing inputs with different SourceIsManaged values is not supported");
            resultShape.Closed = mainShape.Closed;
            resultShape.Seamless = mainShape.Seamless;
            resultShape.SourceIsManaged = mainShape.SourceIsManaged;


        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */


        /*! \endcond */
        #endregion



    }
}
