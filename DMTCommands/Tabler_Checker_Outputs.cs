﻿#define BRX_APP
//#define ARX_APP

using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using _SWF = System.Windows.Forms;


#if BRX_APP
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
#elif ARX_APP
    using _Ap = Autodesk.AutoCAD.ApplicationServices;
    //using _Br = Autodesk.AutoCAD.BoundaryRepresentation;
    using _Cm = Autodesk.AutoCAD.Colors;
    using _Db = Autodesk.AutoCAD.DatabaseServices;
    using _Ed = Autodesk.AutoCAD.EditorInput;
    using _Ge = Autodesk.AutoCAD.Geometry;
    using _Gi = Autodesk.AutoCAD.GraphicsInterface;
    using _Gs = Autodesk.AutoCAD.GraphicsSystem;
    using _Pl = Autodesk.AutoCAD.PlottingServices;
    using _Brx = Autodesk.AutoCAD.Runtime;
    using _Trx = Autodesk.AutoCAD.Runtime;
    using _Wnd = Autodesk.AutoCAD.Windows;
#endif

using R = Reinforcement;
using G = Geometry;
using L = Logic_Reinf;
using T = Logic_Tabler;


namespace DMTCommands
{
    partial class TABLE_command
    {

        public void checker_output(_SETUP init, List<T.DrawingArea> fields)
        {
            write(" ");
            write("----- VIGADE LOETELU ALGUS -----");

            int i = 0;

            foreach (T.DrawingArea f in fields)
            {
                if (f.Valid)
                {
                    showErrors(init, f);
                    i += f._errors.Count;
                }
                else
                {
                    write(f.Reason);
                }
            }

            write("----- VIGADE LOETELU LÕPP -----");
            write(" ");
            write("VIGADE ARV - " + i.ToString());
        }
        

        private int showErrors(_SETUP init, T.DrawingArea f)
        {
            if (f._errors.Count > 0)
            {
                List<string> layerNames = new List<string>() { kontrollLayer };
                init.initLayers(layerNames);
            }

            foreach (T.ErrorPoint e in f._errors)
            {
                double scale = e.Scale;
                _Ge.Point3d insertPoint = new _Ge.Point3d(e.IP.X, e.IP.Y, 0);
                createCircle(5 * scale, insertPoint);
                createCircle(40 * scale, insertPoint);

                write(e.ErrorMessage);
            }

            return f._errors.Count;
        }


        private void createCircle(double radius, _Ge.Point3d ip)
        {
            using (_Db.Circle circle = new _Db.Circle())
            {
                circle.Center = ip;
                circle.Radius = radius;
                circle.ColorIndex = 1;
                circle.Layer = kontrollLayer;
                _c.modelSpace.AppendEntity(circle);
                _c.trans.AddNewlyCreatedDBObject(circle, true);
            }
        }

    }
}