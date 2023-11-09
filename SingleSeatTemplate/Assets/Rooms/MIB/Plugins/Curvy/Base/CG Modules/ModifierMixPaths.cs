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
    [ModuleInfo("Modifier/Mix Paths", ModuleName = "Mix Paths", Description = "Lerps between two paths")]
    [HelpURL(CurvySpline.DOCLINK + "cgmixpaths")]
    public class ModifierMixPaths : CGModule, IOnRequestPath
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), Name = "Path A")]
        public CGModuleInputSlot InPathA = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), Name = "Path B")]
        public CGModuleInputSlot InPathB = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGPath))]
        public CGModuleOutputSlot OutPath = new CGModuleOutputSlot();

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
                return (IsConfigured) ? Mathf.Max((InPathA.SourceSlot().OnRequestPathModule).PathLength,
                                                  (InPathB.SourceSlot().OnRequestPathModule).PathLength) : 0;
            }
        }

        public bool PathIsClosed
        {
            get
            {
                return (IsConfigured) ? InPathA.SourceSlot().OnRequestPathModule.PathIsClosed &&
                                        InPathB.SourceSlot().OnRequestPathModule.PathIsClosed : false;
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

            var DataA = InPathA.GetData<CGPath>(requests);
            var DataB = InPathB.GetData<CGPath>(requests);

            return new CGData[1] { MixPath(DataA, DataB, Mix, UIMessages) };
        }
        #endregion

        #region ### Public Static Methods ###

        /// <summary>
        /// Returns the mixed path
        /// </summary>
        /// <param name="pathA"></param>
        /// <param name="pathB"></param>
        /// <param name="mix"> A value between -1 and 1. -1 will select the path with the most points. 1 will select the other </param>
        /// <param name="warningsContainer">Is filled with warnings raised by the mixing logic</param>
        /// <returns>The mixed path</returns>
        public static CGPath MixPath(CGPath pathA, CGPath pathB, float mix, [NotNull] List<string> warningsContainer)
        {
            CGPath mainPath = pathA.Count > pathB.Count
                ? pathA
                : pathB;

            CGPath secondaryPath = pathA.Count > pathB.Count
                ? pathB
                : pathA;

            CGPath data = new CGPath();
            data.Direction = new Vector3[mainPath.Count];//Direction is updated in the overriden call of Recalculate, which is called in InterpolateShape
            ModifierMixShapes.InterpolateShape(data, mainPath, secondaryPath, mix, warningsContainer);

            //TODO BUG: Directions should be recomputed based on positions, and not interpolated. This is already done in the Recalculate() method called inside InterpolateShape() (line above), but Recalculate has a bug that makes it not compute Direction[0], so I kept the code bellow to recompute directions.
            float interpolationTime = (mix + 1) * 0.5f;
            Assert.IsTrue(interpolationTime >= 0);
            Assert.IsTrue(interpolationTime <= 1);
            Vector3[] directions = new Vector3[mainPath.Count];
            for (int i = 0; i < mainPath.Count; i++)
            {
                Vector3 secondaryDirection;
                {
                    float frag;
                    int idx = secondaryPath.GetFIndex(mainPath.F[i], out frag);
                    secondaryDirection = Vector3.SlerpUnclamped(secondaryPath.Direction[idx], secondaryPath.Direction[idx + 1], frag);
                }

                directions[i] = Vector3.SlerpUnclamped(mainPath.Direction[i], secondaryDirection, interpolationTime);
            }

            data.Direction = directions;
            return data;
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */


        /*! \endcond */
        #endregion






    }
}
