using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Project_Setup;
using ExtensionMethods;

namespace ExtensionMethods
{
    public static class Extensions_Layout
    {
        public static double GetHeight(this Layout curlay)
        {
            double height = 0.0;
            //// Does the math and rounds values over
            height = Math.Round((curlay.PlotPaperSize.Y) / 25.4);

            return height;
        }

        public static double GetWidth(this Layout curlay)
        {
            double width = 0.0;
            //// Does the math and rounds values over
            width = Math.Round((curlay.PlotPaperSize.X) / 25.4);

            return width;
        }

        public static string PaperSize(this Layout curlay)
        {
            double x_min = curlay.Limits.MinPoint.X;
            double x_max = curlay.Limits.MaxPoint.X;

            double y_min = curlay.Limits.MinPoint.Y;
            double y_max = curlay.Limits.MaxPoint.Y;

            double[] coords = { x_max, x_min, y_max, y_min };

            // Does the math and rounds values over
            double x_total = Math.Round(coords[0] - coords[1]);
            double y_total = Math.Round(coords[2] - coords[3]);

            double[] tempsize = {x_total, y_total};
            double[] papersize = new double[2];

            //try and round it to one of the known values
            double[] paper_values = { 11, 17, 24, 30, 36, 42, 48 };

            if (!paper_values.Contains(tempsize[0]) || !paper_values.Contains(tempsize[1]))
            {
                for (int x = 0; x < paper_values.Length; x++)
                {
                    if ((tempsize[0] <= (paper_values[x] - 1)) && (tempsize[0] >= (paper_values[x] - 1)) && (tempsize[0] != paper_values[x]))
                    {
                        tempsize[0] = paper_values[x];
                    }

                    if ((tempsize[1] <= (paper_values[x] - 1)) && (tempsize[1] >= (paper_values[x] - 1)) && (tempsize[1] != paper_values[x]))
                    {
                        tempsize[1] = paper_values[x];
                    }
                }
            }

            if (tempsize[0] > tempsize[1])
            {
                papersize = tempsize.Reverse().ToArray();
            }

            else
            {
                papersize = tempsize.ToArray();
            }

            string tblksize = string.Format("{0},{1}", papersize[0], papersize[1]);

            return tblksize;
                
        }

        public static void Viewport_Create(this Layout layout, Project_Setup.Viewport vpinfo, Document doc)
        {
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (DocumentLock doclock = doc.LockDocument())
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    // Get the BlockTableRecord so you can add to it
                    BlockTable blt = tr.GetObject(db.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    BlockTableRecord btr_paper = tr.GetObject(blt[BlockTableRecord.PaperSpace],
                        OpenMode.ForWrite) as BlockTableRecord;

                    //change the current space to Paper
                    ed.SwitchToPaperSpace();

                    //switch current layer to viewport layer
                    Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "x-vport");
                    doc.LayerManagement("Lock", "x-vport", false);

                    // create a new viewport
                    Autodesk.AutoCAD.DatabaseServices.Viewport vp = new Autodesk.AutoCAD.DatabaseServices.Viewport();
                    
                    vp.Width = vpinfo.Width;
                    vp.Height = vpinfo.Height;
                    vp.CenterPoint = vpinfo.CenterPoint;

                    // Add the viewport to the PaperSpaceRecord
                    btr_paper.AppendEntity(vp);
                    tr.AddNewlyCreatedDBObject(vp, true);

                    //add all the corresponding viewport properties
                    vp.ViewCenter = vpinfo.ViewCenter;
                    vp.ViewHeight = vpinfo.ViewHeight;
                    vp.ViewTarget = vpinfo.ViewTarget;
                    vp.FreezeLayersInViewport(vpinfo.FrznLayers.GetEnumerator());

                    try
                    {
                        vp.StandardScale = vpinfo.StandardScale;
                    }

                    catch (Autodesk.AutoCAD.Runtime.Exception aex)
                    {
                        if (aex.ErrorStatus == ErrorStatus.InvalidInput)
                        {
                            vp.CustomScale = vpinfo.CustomScale;
                        }
                    }

                    //Enables the viewport
                    vp.On = true;

                    Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CLAYER", "0");

                    tr.Commit();
                }
            }
        }

        public enum Size
        {
            
        }
    }
}
