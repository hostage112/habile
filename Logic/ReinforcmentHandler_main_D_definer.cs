﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;
using R = Reinforcement;

namespace Logic_Reinf
{
    public partial class ReinforcmentHandler
    {
        public bool define_D(G.Edge mainEdge, G.Edge side1Edge, G.Edge side2Edge)
        {
            bool mainSet = setEdges.Keys.Contains(mainEdge);
            bool side1Set = setEdges.Keys.Contains(side1Edge);
            bool side2Set = setEdges.Keys.Contains(side2Edge);

            double cover1 = _V_.Y_CONCRETE_COVER_2;
            double coverMain = _V_.X_CONCRETE_COVER_1;
            double cover2 = _V_.Y_CONCRETE_COVER_2;
            int parand = 2 * _V_.Y_CONCRETE_COVER_DELTA - 10; // parand magic

            double side1Dist = _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;
            double mainDist = mainEdge.Line.Length();
            double side2Dist = _V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH;

            if (mainSet == true)
            {
                coverMain = coverMain + _V_.Y_CONCRETE_COVER_DELTA;
            }

            G.Corner startCorner = null;
            G.Point IP1 = getCornerPoint(side1Edge, mainEdge, cover1, coverMain, ref startCorner);

            G.Corner endCorner = null;
            G.Point IP2 = getCornerPoint(mainEdge, side2Edge, coverMain, cover2, ref endCorner);

            G.Vector v1 = mainEdge.Line.getOffsetVector();
            G.Vector v2 = side1Edge.Line.getCoolVector(v1);
            G.Vector v3 = side2Edge.Line.getCoolVector(v2);

            G.Point side1Start = IP1.move(side1Dist, v2);
            G.Point side2End = IP2.move(side2Dist, v3);

            if (startCorner != null)
            {
                if (setCorners.Keys.Contains(startCorner))
                {
                    if (!ReferenceEquals(setCorners[startCorner], setEdges[mainEdge]))
                    {
                        coverMain = coverMain + _V_.Y_CONCRETE_COVER_DELTA;
                    }
                    if (!ReferenceEquals(setCorners[startCorner], setEdges[side1Edge]))
                    {
                        cover1 = cover1 + _V_.Y_CONCRETE_COVER_DELTA;
                    }

                    MessageBox.Show("here");

                    IP1 = getCornerPoint(side1Edge, mainEdge, cover1, coverMain, ref startCorner);
                    IP2 = getCornerPoint(mainEdge, side2Edge, coverMain, cover2, ref endCorner);
                    side1Start = IP1.move(side1Dist, v2);
                    side2End = IP2.move(side2Dist, v3);

                    startCorner = null;
                }
            }

            if (endCorner != null)
            {
                if (setCorners.Keys.Contains(endCorner))
                {
                    if (!ReferenceEquals(setCorners[endCorner], setEdges[mainEdge]))
                    {
                        coverMain = coverMain + _V_.Y_CONCRETE_COVER_DELTA;
                    }
                    if (!ReferenceEquals(setCorners[endCorner], setEdges[side2Edge]))
                    {
                        cover2 = cover2 + _V_.Y_CONCRETE_COVER_DELTA;
                    }

                    MessageBox.Show("here2");

                    IP1 = getCornerPoint(side1Edge, mainEdge, cover1, coverMain, ref startCorner);
                    IP2 = getCornerPoint(mainEdge, side2Edge, coverMain, cover2, ref endCorner);
                    side1Start = IP1.move(side1Dist, v2);
                    side2End = IP2.move(side2Dist, v3);

                    endCorner = null;
                }
            }

            G.Line mainLine = new G.Line(IP1, IP2);
            if (mainLine.Length() < _V_.X_REINFORCEMENT_MAIN_RADIUS * 2) return false;

            //A_handler_debug(side1Start, IP1);
            //A_handler_debug(IP1, IP2);
            //A_handler_debug(IP2, side2End);

            bool success = false;

            if (mainSet == true)
            {
                success = D_vs_E_handler(IP1, IP2, side1Start, side2End, null, startCorner, endCorner, parand); // parand magic
            }
            else
            {
                success = D_vs_E_handler(IP1, IP2, side1Start, side2End, mainEdge, startCorner, endCorner, parand); // parand magic
            }


            return success;
        }

    }
}