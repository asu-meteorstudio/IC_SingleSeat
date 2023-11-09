// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using FluffyUnderware.DevTools;
using FluffyUnderware.DevTools.Extensions;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Controllers
{
    /// <summary>
    /// SplineController modifying uGUI text
    /// </summary>
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Curvy/Controller/UI Text Spline Controller")]
    [HelpURL(CurvySpline.DOCLINK + "uitextsplinecontroller")]
    public class UITextSplineController : SplineController, IMeshModifier
    {

        protected class GlyphQuad
        {
            public UIVertex[] V = new UIVertex[4];
            public Rect Rect;
            public Vector3 Center { get { return Rect.center; } }

            public void Load(List<UIVertex> verts, int index)
            {
                V[0] = verts[index];
                V[1] = verts[index + 1];
                V[2] = verts[index + 2];
                V[3] = verts[index + 3];

                calcRect();
            }

            public void LoadTris(List<UIVertex> verts, int index)
            {
                V[0] = verts[index];
                V[1] = verts[index + 1];
                V[2] = verts[index + 2];
                V[3] = verts[index + 4];
                calcRect();
            }

            public void calcRect()
            {
                Rect = new Rect(V[0].position.x,
                              V[2].position.y,
                              V[2].position.x - V[0].position.x,
                              V[0].position.y - V[2].position.y);
            }

            public void Save(List<UIVertex> verts, int index)
            {
                verts[index] = V[0];
                verts[index + 1] = V[1];
                verts[index + 2] = V[2];
                verts[index + 3] = V[3];
            }

            public void Save(VertexHelper vh)
            {
                vh.AddUIVertexQuad(V);
            }

            public void Transpose(Vector3 v)
            {
                for (int i = 0; i < 4; i++)
                    V[i].position += v;

            }

            public void Rotate(Quaternion rotation)
            {
                for (int i = 0; i < 4; i++)
                    V[i].position = V[i].position.RotateAround(Center, rotation);
            }

        }

        protected class GlyphPlain
        {
            public Vector3[] V = new Vector3[4];
            public Rect Rect;
            public Vector3 Center { get { return Rect.center; } }

            public void Load(ref Vector3[] verts, int index)
            {
                V[0] = verts[index];
                V[1] = verts[index + 1];
                V[2] = verts[index + 2];
                V[3] = verts[index + 3];

                calcRect();
            }

            public void calcRect()
            {
                Rect = new Rect(V[0].x,
                              V[2].y,
                              V[2].x - V[0].x,
                              V[0].y - V[2].y);
            }

            public void Save(ref Vector3[] verts, int index)
            {
                verts[index] = V[0];
                verts[index + 1] = V[1];
                verts[index + 2] = V[2];
                verts[index + 3] = V[3];
            }

            public void Transpose(Vector3 v)
            {
                for (int i = 0; i < 4; i++)
                    V[i] += v;

            }

            public void Rotate(Quaternion rotation)
            {
                for (int i = 0; i < 4; i++)
                    V[i] = V[i].RotateAround(Center, rotation);
            }

        }

        #region ### Serialized Fields ###

        [Section("Orientation")]
        [Tooltip("If true, the text characters will keep the same orientation regardless of the spline they follow")]
        [SerializeField]
        private bool staticOrientation;

        #endregion

        #region Public properties

        /// <summary>
        /// If true, the text characters will keep the same orientation regardless of the spline they follow
        /// </summary>
        public bool StaticOrientation
        {
            get { return staticOrientation; }
            set
            {
                staticOrientation = value;
            }
        }

        #endregion


        #region Conditional display in the inspector of CurvyController properties

        protected override bool ShowOrientationSection
        {
            get { return false; }
        }
        protected override bool ShowOffsetSection
        {
            get { return false; }
        }

        #endregion

        Graphic m_Graphic;
        RectTransform mRect;
        Text mText;



        protected Text Text
        {
            get
            {
                if (mText == null)
                    mText = GetComponent<Text>();
                return mText;
            }
        }

        protected RectTransform Rect
        {
            get
            {
                if (mRect == null)
                    mRect = GetComponent<RectTransform>();
                return mRect;
            }
        }

        protected Graphic graphic
        {
            get
            {
                if (m_Graphic == null)
                    m_Graphic = GetComponent<Graphic>();

                return m_Graphic;
            }
        }

        protected override void InitializedApplyDeltaTime(float deltaTime)
        {
            base.InitializedApplyDeltaTime(deltaTime);
            graphic.SetVerticesDirty();
        }

        public void ModifyMesh(Mesh verts)
        {
            if (enabled && gameObject.activeInHierarchy && isInitialized)
            {
                Vector3[] vtArray = verts.vertices;
                GlyphPlain glyph = new GlyphPlain();
                for (int c = 0; c < Text.text.Length; c++)
                {
                    glyph.Load(ref vtArray, c * 4);

                    float xDistance = AbsolutePosition + glyph.Rect.center.x;
                    float tf = AbsoluteToRelative(GetClampedPosition(xDistance, CurvyPositionMode.WorldUnits, Clamping, Length));

                    Vector3 pos = this.GetInterpolatedSourcePosition(tf);
                    Vector3 tan = this.GetTangent(tf);
                    Vector3 off = pos - Rect.position - glyph.Center; // position offset to spline

                    glyph.Transpose(new Vector3(0, glyph.Center.y, 0)); // shift to match baseline
                    // Rotate, then offset to real position
                    glyph.Rotate(Quaternion.AngleAxis(Mathf.Atan2(tan.x, -tan.y) * Mathf.Rad2Deg - 90, Vector3.forward));
                    glyph.Transpose(off);

                    glyph.Save(ref vtArray, c * 4);
                }
                verts.vertices = vtArray;
            }
        }


        public void ModifyMesh(VertexHelper vertexHelper)
        {
            if (enabled && gameObject.activeInHierarchy && isInitialized)
            {
                List<UIVertex> verts = new List<UIVertex>();
                GlyphQuad glyph = new GlyphQuad();

                vertexHelper.GetUIVertexStream(verts);
                vertexHelper.Clear();

                for (int c = 0; c < Text.text.Length; c++)
                {

                    glyph.LoadTris(verts, c * 6);

                    float xDistance = AbsolutePosition + glyph.Rect.center.x;
                    float tf = AbsoluteToRelative(GetClampedPosition(xDistance, CurvyPositionMode.WorldUnits, Clamping, Length));

                    Vector3 pos = this.GetInterpolatedSourcePosition(tf);
                    Vector3 tan = this.GetTangent(tf);
                    Vector3 off = pos - Rect.position - glyph.Center; // position offset to spline

                    glyph.Transpose(new Vector3(0, glyph.Center.y, 0)); // shift to match baseline
                    // Rotate, then offset to real position
                    if (StaticOrientation == false)
                        glyph.Rotate(Quaternion.AngleAxis(Mathf.Atan2(tan.x, -tan.y) * Mathf.Rad2Deg - 90, Vector3.forward));
                    glyph.Transpose(off);
                    glyph.Save(vertexHelper);
                }

            }
        }

        #region ### Unity Callbacks ###

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (isInitialized)
            {
                UnbindSplineRelatedEvents();
                BindSplineRelatedEvents();
            }
            base.OnValidate();
            if (this.graphic != null)
            {
                this.graphic.SetVerticesDirty();
            }
        }
#endif

        #endregion

        #region Spline refreshing


        public override CurvySpline Spline
        {
            get { return m_Spline; }
            set
            {
                if (m_Spline != value)
                {
                    if (isInitialized)
                        UnbindSplineRelatedEvents();

                    m_Spline = value;
                    if (isInitialized)
                        BindSplineRelatedEvents();
                }
            }
        }

        protected override void BindEvents()
        {
            base.BindEvents();
            BindSplineRelatedEvents();
        }

        protected override void UnbindEvents()
        {
            base.UnbindEvents();
            UnbindSplineRelatedEvents();
        }

        private void BindSplineRelatedEvents()
        {
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(isInitialized);
#endif
            if (Spline)
            {
                UnbindSplineRelatedEvents();
                Spline.OnRefresh.AddListener(OnSplineRefreshed);
            }
        }

        private void UnbindSplineRelatedEvents()
        {
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(isInitialized);
#endif
            if (Spline)
            {
                Spline.OnRefresh.RemoveListener(OnSplineRefreshed);
            }
        }

        private void OnSplineRefreshed(CurvySplineEventArgs e)
        {
            CurvySpline senderSpline = e.Sender as CurvySpline;
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(senderSpline != null);
#endif
            if (senderSpline != Spline)
                senderSpline.OnRefresh.RemoveListener(OnSplineRefreshed);
            else
            {
                graphic.SetVerticesDirty();
            }
        }

        #endregion


    }
}
