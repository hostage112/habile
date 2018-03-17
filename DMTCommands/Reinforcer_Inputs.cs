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
    static class Reinforcer_Inputs
    {
        public static bool getSettingsVariables()
        {
            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;
            _Ed.Editor ed = doc.Editor;

            _Db.TypedValue[] filterlist = new _Db.TypedValue[2];
            filterlist[0] = new _Db.TypedValue(0, "INSERT");
            filterlist[1] = new _Db.TypedValue(2, "Reinf_program_settings");

            _Ed.SelectionFilter filter = new _Ed.SelectionFilter(filterlist);

            _Ed.PromptSelectionOptions opts = new _Ed.PromptSelectionOptions();
            opts.MessageForAdding = "\nSelect SETTINGS BLOCK: ";

            _Ed.PromptSelectionResult selection = ed.GetSelection(opts, filter);

            if (selection.Status != _Ed.PromptStatus.OK)
            {
                ed.WriteMessage("\nERROR - SETTINGS BLOCK not found");
                return false;
            }
            if (selection.Value.Count != 1)
            {
                ed.WriteMessage("\nERROR - Too many SETTINGS BLOCKs in selection");
                return false;
            }
            else
            {
                using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
                {
                    _Db.ObjectId selectionId = selection.Value.GetObjectIds()[0];
                    _Db.BlockReference selectionBR = trans.GetObject(selectionId, _Db.OpenMode.ForWrite) as _Db.BlockReference;

                    L._V_.Z_DRAWING_SCALE = selectionBR.ScaleFactors.X;

                    foreach (_Db.ObjectId arId in selectionBR.AttributeCollection)
                    {
                        _Db.DBObject obj = trans.GetObject(arId, _Db.OpenMode.ForWrite);
                        _Db.AttributeReference ar = obj as _Db.AttributeReference;
                        if (ar != null)
                        {
                            bool success = setProgramVariables(ar, ed);
                            if (!success) return false;
                        }
                    }
                }
            }

            return true;
        }


        private static bool setProgramVariables(_Db.AttributeReference ar, _Ed.Editor ed)
        {
            if (ar.Tag == "ARMATUURI_MARK")
            {
                L._V_.X_REINFORCEMENT_MARK = ar.TextString;
            }
            else
            {
                double number;
                string txt = ar.TextString;
                txt = txt.Replace('.', ',');
                bool parser = Double.TryParse(txt, out number);

                //A
                if (ar.Tag == "ELEMENDI_LAIUS")
                {
                    if (parser)
                    {
                        L._V_.X_ELEMENT_WIDTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - ELEMENDI_LAIUS");
                        return false;
                    }
                }


                //B
                else if (ar.Tag == "KAITSEKIHT")
                {
                    if (parser)
                    {
                        L._V_.X_CONCRETE_COVER_1 = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - KAITSEKIHT");
                        return false;
                    }
                }
                else if (ar.Tag == "KIHTIDE_ARV")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_NUMBER = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - KIHTIDE_ARV");
                        return false;
                    }
                }


                //C
                else if (ar.Tag == "PÕHIARMATUURI_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_MAIN_DIAMETER = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_DIAM");
                        return false;
                    }
                }
                else if (ar.Tag == "PÕHIARMATUURI_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_MAIN_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_ANKURDUS");
                        return false;
                    }
                }
                else if (ar.Tag == "PÕHIARMATUURI_ABISUURUS")
                {
                    if (parser)
                    {
                        L._V_.X_FIRST_PASS_CONSTRAINT = number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - PÕHIARMATUURI_ABISUURUS");
                        return false;
                    }
                }


                //D
                else if (ar.Tag == "DIAGONAALI_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_DIAGONAL_DIAMETER = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - DIAGONAALI_DIAM");
                        return false;
                    }
                }
                else if (ar.Tag == "DIAGONAALI_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_DIAGONAL_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - DIAGONAALI_ANKURDUS");
                        return false;
                    }
                }


                //E
                else if (ar.Tag == "RANGIDE_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_DIAMETER = (int)number;

                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - RANGIDE_DIAM");
                        return false;
                    }
                }
                else if (ar.Tag == "RANGIDE_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_SPACING = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - RANGIDE_SAMM");
                        return false;
                    }
                }
                else if (ar.Tag == "RANGIDE_ABISUURUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_STIRRUP_CONSTRAINT = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - RANGIDE_ABISUURUS");
                        return false;
                    }
                }


                //F
                else if (ar.Tag == "HORISONTAAL_D_IO")
                {
                    if (parser)
                    {
                        if ((int)number == 0 || (int)number == 1)
                        {
                            L._V_.X_REINFORCEMENT_SIDE_D_CREATE = (int)number;
                        }
                        else
                        {
                            ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_IO");
                            return false;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_IO");
                        return false;
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_DIAMETER = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_DIAM");
                        return false;
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_ANKURDUS");
                        return false;
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_SPACING = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_SAMM");
                        return false;
                    }
                }
                else if (ar.Tag == "HORISONTAAL_D_PARAND")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_SIDE_D_FIX = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - HORISONTAAL_D_PARAND");
                        return false;
                    }
                }


                //E
                else if (ar.Tag == "VERTIKAAL_D_IO")
                {
                    if (parser)
                    {
                        if ((int)number == 0 || (int)number == 1)
                        {
                            L._V_.X_REINFORCEMENT_TOP_D_CREATE = (int)number;
                        }
                        else
                        {
                            ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_IO");
                            return false;
                        }
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_IO");
                        return false;
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_DIAM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_DIAMETER = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_DIAM");
                        return false;
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_ANKURDUS")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_ANCHOR_LENGTH = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_ANKURDUS");
                        return false;
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_SAMM")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_SPACING = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_SAMM");
                        return false;
                    }
                }
                else if (ar.Tag == "VERTIKAAL_D_PARAND")
                {
                    if (parser)
                    {
                        L._V_.X_REINFORCEMENT_TOP_D_FIX = (int)number;
                    }
                    else
                    {
                        ed.WriteMessage("\nSetting invalid - VERTIKAAL_D_PARAND");
                        return false;
                    }
                }
            }

            return true;
        }


        public static List<G.Line> getSelectedPolyLines()
        {
            List<G.Line> polys = new List<G.Line>();

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                _Ed.PromptSelectionResult userSelection = doc.Editor.GetSelection();

                if (userSelection.Status == _Ed.PromptStatus.OK)
                {
                    _Ed.SelectionSet selectionSet = userSelection.Value;

                    foreach (_Ed.SelectedObject currentObject in selectionSet)
                    {
                        if (currentObject != null)
                        {
                            _Db.Entity currentEntity = trans.GetObject(currentObject.ObjectId, _Db.OpenMode.ForRead) as _Db.Entity;

                            if (currentEntity != null)
                            {
                                if (currentEntity is _Db.Polyline)
                                {
                                    _Db.Polyline poly = trans.GetObject(currentEntity.ObjectId, _Db.OpenMode.ForRead) as _Db.Polyline;
                                    int points = poly.NumberOfVertices;

                                    for (int i = 1; i < points; i++)
                                    {
                                        _Ge.Point2d p1 = poly.GetPoint2dAt(i - 1);
                                        _Ge.Point2d p2 = poly.GetPoint2dAt(i);

                                        G.Point new_p1 = new G.Point(p1.X, p1.Y);
                                        G.Point new_p2 = new G.Point(p2.X, p2.Y);

                                        if (new_p1 == new_p2) continue;

                                        G.Line line = new G.Line(new_p1, new_p2);
                                        polys.Add(line);
                                    }

                                    if (poly.Closed)
                                    {
                                        _Ge.Point2d p1 = poly.GetPoint2dAt(points - 1);
                                        _Ge.Point2d p2 = poly.GetPoint2dAt(0);
                                        G.Point new_p1 = new G.Point(p1.X, p1.Y);
                                        G.Point new_p2 = new G.Point(p2.X, p2.Y);

                                        if (new_p1 == new_p2) continue;

                                        G.Line line = new G.Line(new_p1, new_p2);
                                        polys.Add(line);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return polys;
        }


        public static G.Point getBendingInsertionPoint()
        {
            G.Point picked;

            _Ap.Document doc = _Ap.Application.DocumentManager.MdiActiveDocument;
            _Db.Database db = doc.Database;

            using (_Db.Transaction trans = db.TransactionManager.StartTransaction())
            {
                _Ed.PromptPointResult pickedPoint;
                _Ed.PromptPointOptions pickedPointOptions = new _Ed.PromptPointOptions("\nSelect bending INSERTION POINT");

                pickedPoint = doc.Editor.GetPoint(pickedPointOptions);
                _Ge.Point3d pt = pickedPoint.Value;

                picked = new G.Point(pt.X, pt.Y);
            }

            return picked;
        }

    }
}