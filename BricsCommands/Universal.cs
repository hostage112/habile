﻿using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

//ODA
using Teigha.Runtime;
using Teigha.DatabaseServices;
using Teigha.Geometry;

//Bricsys
using Bricscad.ApplicationServices;
using Bricscad.Runtime;
using Bricscad.EditorInput;

using L = Logic_Reinf;
using R = Reinforcement;
using G = Geometry;
using T = Logic_Tabler;

namespace commands
{
    public static class Universal
    {
        public static void writeCadMessage(string errorMessage)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            ed.WriteMessage("\n" + errorMessage);
        }

        public static void programInit(List<string> blockNames, List<string> layerNames, Transaction trans)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            BlockTable blockTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            LayerTable layerTable = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;

            foreach (string blockName in blockNames)
            {
                if (!blockTable.Has(blockName))
                {
                    getBlockFromMaster(blockName, trans);
                }
            }

            foreach (string layerName in layerNames)
            {
                if (!layerTable.Has(layerName))
                {
                    createLayer(layerName, trans);
                }
            }
        }

        public static void createLayer(string layerName, Transaction trans)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;

            LayerTable lt = trans.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
            LayerTableRecord newLayer = new LayerTableRecord();

            newLayer.Name = layerName;
            if (layerName == "Armatuur")
            {
                newLayer.Color = Teigha.Colors.Color.FromColorIndex(Teigha.Colors.ColorMethod.None, 4);
            }
            else
            {
                newLayer.Color = Teigha.Colors.Color.FromColorIndex(Teigha.Colors.ColorMethod.None, 6);
            }

            ObjectId layerId = lt.Add(newLayer);
            trans.AddNewlyCreatedDBObject(newLayer, true);
            db.Clayer = layerId;
        }

        public static void getBlockFromMaster(string blockName, Transaction trans)
        {
            try
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Database destDb = doc.Database;

                DocumentCollection dm = Application.DocumentManager;
                Editor ed = dm.MdiActiveDocument.Editor;
                Database sourceDb = new Database(false, true);

                string sourceFileName = @"W:\Alex\Brics_pealeehitus\master.dwg";
                if (!File.Exists(sourceFileName))
                {
                    sourceFileName = @"C:\Brics_pealeehitus\master.dwg";
                    if (!File.Exists(sourceFileName))
                    {
                        sourceFileName = @"C:\Users\Alex\Dropbox\DMT\Brics_testimine\master.dwg";
                        if (!File.Exists(sourceFileName))
                        {
                            sourceFileName = @"C:\Users\aleksandr.ess\Dropbox\DMT\Brics_testimine\master.dwg";
                        }
                    }
                }

                sourceDb.ReadDwgFile(sourceFileName, System.IO.FileShare.Read, true, "");
                ObjectIdCollection blockIds = new ObjectIdCollection();

                Teigha.DatabaseServices.TransactionManager tm = sourceDb.TransactionManager;
                using (Transaction myT = tm.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tm.GetObject(sourceDb.BlockTableId, OpenMode.ForRead, false);

                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)tm.GetObject(btrId, OpenMode.ForRead, false);

                        if (!btr.IsAnonymous && !btr.IsLayout) blockIds.Add(btrId);
                        btr.Dispose();
                    }
                }

                IdMapping mapping = new IdMapping();
                sourceDb.WblockCloneObjects(blockIds, destDb.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
                sourceDb.Dispose();
            }
            catch
            {
                throw new System.Exception();
            }
        }
    }
}