﻿using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;

//using _Ap = Autodesk.AutoCAD.ApplicationServices;
////using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
//using _Cm = Autodesk.AutoCAD.Colors;
//using _Db = Autodesk.AutoCAD.DatabaseServices;
//using _Ed = Autodesk.AutoCAD.EditorInput;
//using _Ge = Autodesk.AutoCAD.Geometry;
//using _Gi = Autodesk.AutoCAD.GraphicsInterface;
//using _Gs = Autodesk.AutoCAD.GraphicsSystem;
//using _Pl = Autodesk.AutoCAD.PlottingServices;
//using _Brx = Autodesk.AutoCAD.Runtime;
//using _Trx = Autodesk.AutoCAD.Runtime;
//using _Wnd = Autodesk.AutoCAD.Windows;

using _Ap = Bricscad.ApplicationServices;
//using _Br = Teigha.BoundaryRepresentation;
using _Cm = Teigha.Colors;
using _Db = Teigha.DatabaseServices;
using _Ed = Bricscad.EditorInput;
using _Ge = Teigha.Geometry;
using _Gi = Teigha.GraphicsInterface;
using _Gs = Teigha.GraphicsSystem;
using _Gsk = Bricscad.GraphicsSystem;
using _Pl = Bricscad.PlottingServices;
using _Brx = Bricscad.Runtime;
using _Trx = Teigha.Runtime;
using _Wnd = Bricscad.Windows;
//using _Int = Bricscad.Internal;

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    class Tabler
    {
        string boxName;

        string tableHeadName;
        string tableRowName;
        string tableSummaryName;

        string markLayerName;

        List<string> bendingNames;

        public Tabler()
        {
            boxName = "Drawing_Area";

            tableHeadName = "Painutustabel_pais";
            tableRowName = "Painutustabel_rida";
            tableSummaryName = "PainutusKokkuvõte";

            markLayerName = "K023TL";

            bendingNames = new List<string>();

            bendingNames.Add("Raud_A");
            bendingNames.Add("Raud_B");
            bendingNames.Add("Raud_C");
            bendingNames.Add("Raud_D");
            bendingNames.Add("Raud_E");
            bendingNames.Add("Raud_F");
            bendingNames.Add("Raud_G");
            bendingNames.Add("Raud_H");
            bendingNames.Add("Raud_J");
            bendingNames.Add("Raud_K");
            bendingNames.Add("Raud_L");
            bendingNames.Add("Raud_M");
            bendingNames.Add("Raud_N");
            bendingNames.Add("Raud_R");
            bendingNames.Add("Raud_S");
            bendingNames.Add("Raud_T");
            bendingNames.Add("Raud_U");
            bendingNames.Add("Raud_V");
            bendingNames.Add("Raud_W");
            bendingNames.Add("Raud_X");
            bendingNames.Add("Raud_Y");
            bendingNames.Add("Raud_Z1");
            bendingNames.Add("Raud_Z2");
            bendingNames.Add("Raud_Z3");
            bendingNames.Add("Raud_Z4");
            bendingNames.Add("Raud_Z5");
            bendingNames.Add("Raud_Z6");
            bendingNames.Add("Raud_Z7");
            bendingNames.Add("Raud_Z8");
            bendingNames.Add("Raud_Z9");
        }


        public void main2()
        {
            List<G.Area> areas = Tabler_Inputs.getAllAreas(boxName);
            if (areas.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + boxName + " not found");
            }

            List<T.TableHead> heads = Tabler_Inputs.getAllTableHeads(tableHeadName);
            if (heads.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + tableHeadName + " not found");
            }

            List<T.ReinforcementMark> marks = Tabler_Inputs.getAllMarks(markLayerName);
            if (marks.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + "Reinforcement marks" + " not found");
            }

            List<T.Bending> bendings = Tabler_Inputs.getAllBendings(bendingNames);
            List<T.TableRow> rows = Tabler_Inputs.getAllTableRows(tableRowName);

            List<T.DrawingArea> data = T.TablerHandler.main(areas, heads, marks, bendings, rows);

            Tabler_Outputs.main(data);

            Universal.writeCadMessage("PROGRAM FINISED");

        }


        public void main3()
        {
            List<G.Area> areas = Tabler_Inputs.getAllAreas(boxName);
            if (areas.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + boxName + " not found");
            }

            List<T.TableHead> heads = Tabler_Inputs.getAllTableHeads(tableHeadName);
            if (heads.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + tableHeadName + " not found");
            }

            List<T.TableRow> rows = Tabler_Inputs.getAllTableRows(tableRowName);
            List<T.TableSummary> summarys = Tabler_Inputs.getAllTableSummarys(tableSummaryName);

            List<T.DrawingArea> data = T.SummarHandler.main(areas, heads, rows, summarys);

            Summar_Outputs.main(data);

            Universal.writeCadMessage("PROGRAM FINISED");
        }


        public void main4()
        {
            List<G.Area> areas = Tabler_Inputs.getAllAreas(boxName);
            if (areas.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + boxName + " not found");
            }

            List<T.TableHead> heads = Tabler_Inputs.getAllTableHeads(tableHeadName);
            if (heads.Count < 1)
            {
                Universal.writeCadMessage("ERROR - " + tableHeadName + " not found");
            }

            List<T.ReinforcementMark> marks = Tabler_Inputs.getAllMarks(markLayerName);
            List<T.Bending> bendings = Tabler_Inputs.getAllBendings(bendingNames);
            List<T.TableRow> rows = Tabler_Inputs.getAllTableRows(tableRowName);
            List<T.TableSummary> summarys = Tabler_Inputs.getAllTableSummarys(tableSummaryName);

            List<T.ErrorPoint> errors = T.CheckerHandler.main(areas, heads, marks, bendings, rows, summarys);

            Checker_Outputs.main(errors);

            Universal.writeCadMessage("PROGRAM FINISED");
        }

    }
}