/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace XenAdmin.Controls.GradientPanel
{
    public partial class GradientPanel : Panel
    {
        public enum Schemes
        {
            Tab,
            Properties,
            Title,
        }

#if xmas
        private readonly Timer refreshTimer;
        private readonly Random random = new Random();
        private const int MaxStars = 16;
        private const float MaxSpeed = 0.012f;
        private const float MaxAngularVelocity = 0.001f;
        private const int MinStarSize = 5;
        private const int MaxStarSize = 13;
        private const int MinStarSeparation = 30;
        private const int MinStarPoints = 5;
        private const int MaxStarPoints = 9;
        private const int MinSpikiness = 3;
        private const int MaxSpikiness = 6;
        private const double NewStarProbability = 0.04;
        private const double EggProbability = 0.001;

        private bool starsEnabled;
        public bool StarsEnabled
        {
            get
            {
                return starsEnabled;
            }
            set
            {
                if (value == starsEnabled)
                    return;

                if (value)
                {
                   refreshTimer.Start();
                }
                else
                {
                    refreshTimer.Stop();
                    Invalidate();
                }

                starsEnabled = value;
            }
        }
#endif

        public GradientPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
#if xmas
            refreshTimer = new Timer();
            refreshTimer.Interval = 60;
            refreshTimer.Tick += new EventHandler(refreshTimer_Tick);

            StarsEnabled = true;
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            // Hack
            if (this.Height != 50)
            {
                StarsEnabled = false;
            }
            Invalidate();
#endif
        }

        #region Properties

        private Schemes scheme;
        public Schemes Scheme
        {
            set
            {
                scheme = value;
                Invalidate();
            }

            get
            {
                return scheme;
            }
        }

        #endregion

        private Brush NonVisualStylesBrush
        {
            get
            {
                switch (scheme)
                {
                    case Schemes.Tab:
                        return SystemBrushes.Control;
                    case Schemes.Properties:
                        return SystemBrushes.Control;
                    case Schemes.Title:
                        return SystemBrushes.Control;
                    default:
                        System.Diagnostics.Trace.Assert(false);
                        return null;
                }
            }
        }

        private Brush GradientBrush(Rectangle bounds)
        {
            switch (scheme)
            {
                case Schemes.Tab:
                    return new LinearGradientBrush(bounds, Program.HeaderGradientStartColor, Program.HeaderGradientEndColor, LinearGradientMode.Horizontal);
                case Schemes.Properties:
                    return new LinearGradientBrush(bounds, Program.HeaderGradientStartColor, Program.HeaderGradientEndColor, LinearGradientMode.Horizontal);
                case Schemes.Title:
                    return new LinearGradientBrush(bounds, Program.TitleBarStartColor, Program.TitleBarEndColor, LinearGradientMode.Vertical);
                default:
                    System.Diagnostics.Trace.Assert(false);
                    return null;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Size.Width == 0 || Size.Height == 0)
                return;

            Rectangle bounds = new Rectangle(Point.Empty, Size);

            if (!Application.RenderWithVisualStyles)
            {
                e.Graphics.FillRectangle(NonVisualStylesBrush, bounds);
            }
            else
            {
                using (Brush b = GradientBrush(bounds))
                {
                    ButtonRenderer.DrawParentBackground(e.Graphics, bounds, this);
                    e.Graphics.FillRectangle(b, bounds);
#if xmas
                    if (StarsEnabled)
                    {
                        // Update position of each star
                        foreach (Star star in stars)
                        {
                            if (star.isEgg)
                            {
                                star.Y += MaxSpeed * 5;
                            }
                            else
                            {
                                star.Y += star.Radius * MaxSpeed;
                                star.Theta += star.AngularVelocity;
                            }
                        }

                        // Remove stars that have fallen off the bottom
                        stars.RemoveAll(delegate(Star star)
                        {
                            return star.Y - star.Radius > this.Bottom;
                        });

                        if (stars.Count < MaxStars && random.NextDouble() < NewStarProbability)
                        {
                            // Add new star
                            Star newStar = new Star();

                            newStar.Radius = random.Next(MinStarSize, MaxStarSize);
                            newStar.X = random.Next(0, bounds.Width);
                            newStar.Y = -newStar.Radius;

                            newStar.AngularVelocity = (float)(random.NextDouble() * MaxAngularVelocity);
                            if (random.NextDouble() > 0.5)
                            {
                                newStar.AngularVelocity = -newStar.AngularVelocity;
                            }

                            if (random.NextDouble() < EggProbability)
                            {
                                newStar.isEgg = true;
                                newStar.Radius = 40;
                                newStar.Y = -newStar.Radius;
                                newStar.Theta = 0.1f * (float)(-0.5 + random.NextDouble());
                            }
                            else
                            {
                                newStar.NumPoints = random.Next(MinStarPoints, MaxStarPoints);
                                newStar.Spikiness = random.Next(MinSpikiness, MaxSpikiness) / 10.0f;
                            }

                            if (!ContainsClose(stars, newStar, MinStarSeparation))
                            {
                                // Reject star if there is another nearby
                                stars.Add(newStar);
                            }
                        }

                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

                        foreach (Star star in stars)
                        {
                            e.Graphics.ResetTransform();
                            e.Graphics.TranslateTransform(star.X, star.Y);
                            e.Graphics.RotateTransform(360 * star.Theta);
                            if (star.isEgg)
                            {
                                e.Graphics.ScaleTransform(0.35f, 0.35f);

                                using (Pen bluePen = new Pen(Color.SteelBlue, 3f), redPen = new Pen(Color.Red, 3f))
                                {
                                    e.Graphics.FillPath(Brushes.MistyRose, EggBeziers);
                                    e.Graphics.DrawPath(redPen, EggStripe);
                                    e.Graphics.FillPath(Brushes.Yellow, EggStripe);
                                    e.Graphics.DrawPath(bluePen, EggBeziers);
                                }
                            }
                            else
                            {
                                PointF[] points = GetStarShape(star.NumPoints, star.Radius, star.Spikiness);
                                e.Graphics.DrawPolygon(Pens.DodgerBlue, points);
                                using (SolidBrush brush = new SolidBrush(Color.FromArgb(200, Color.LightBlue)))
                                {
                                    e.Graphics.FillPolygon(brush, points);
                                }
                            }
                        }
                    }
#endif
                }
            }

            if (scheme == Schemes.Title)
            {
                using (Pen p = new Pen(Program.TitleBarBorderColor))
                {
                    e.Graphics.DrawRectangle(p, new Rectangle(bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1));
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }

#if xmas
        private static readonly List<Star> stars = new List<Star>();

        private class Star
        {
            public float X, Y, Radius, Theta, AngularVelocity;
            public int NumPoints;
            public double Spikiness;
            public bool isEgg;
        }

        /// <summary>
        /// Determines if there is a star in the list within maxRadius of the given star.
        /// </summary>
        /// <param name="centres"></param>
        /// <param name="centre"></param>
        /// <param name="maxRadius"></param>
        /// <returns></returns>
        private static bool ContainsClose(List<Star> stars, Star s, int maxRadius)
        {
            foreach (Star star in stars)
            {
                if (Math.Sqrt(Math.Pow(star.X - s.X, 2) + Math.Pow(star.Y - s.Y, 2)) < maxRadius)
                {
                    return true;
                }
            }
            return false;
        }

        private static PointF[] GetStarShape(int points, double radius, double ratio)
        {
            List<PointF> result = new List<PointF>();

            bool odd = false;
            for (double theta = 0; theta < 2 * Math.PI; theta += ((2 * Math.PI) / (2 * points)))
            {
                result.Add(RadialToCartesian(theta, odd ? ratio * radius : radius));
                odd = !odd;
            }

            return result.ToArray();
        }

        private static PointF RadialToCartesian(double theta, double r)
        {
            return new PointF((float)(r * Math.Cos(theta)), (float)(r * Math.Sin(theta)));
        }

        private static GraphicsPath eggBeziers;
        private static GraphicsPath EggBeziers
        {
            get
            {
                if (eggBeziers == null)
                {
                    eggBeziers = new GraphicsPath();
                    eggBeziers.AddBeziers(new PointF[] {
                        new PointF(0, 0),
                        
                        new PointF(12, 0),
                        new PointF(24, 18),
                        new PointF(24, 40),
                        new PointF(24, 56),
                        new PointF(12, 65),
                        
                        new PointF(0, 65),

                        new PointF(-12, 65),
                        new PointF(-24, 56),
                        new PointF(-24, 40),
                        new PointF(-24, 18),
                        new PointF(-12, 0),

                        new PointF(0, 0)
                    });
                }

                return eggBeziers;
            }
        }

        private static GraphicsPath eggStripe;
        private static GraphicsPath EggStripe
        {
            get
            {
                if (eggStripe == null)
                {
                    eggStripe = new GraphicsPath();
                    eggStripe.AddLines(new PointF[] {
                        new PointF(-23 ,32),
                        new PointF(-16, 36),
                        new PointF(-8, 31),
                        new PointF(0, 36),
                        new PointF(8, 31),
                        new PointF(16, 36),

                        new PointF(23, 32),
                        new PointF(23, 42),

                        new PointF(16, 51),
                        new PointF(8, 46),
                        new PointF(0, 51),
                        new PointF(-8, 46),
                        new PointF(-16, 51),
                        new PointF(-23, 47)
                    });
                }

                return eggStripe;
            }
        }
#endif
    }
}
