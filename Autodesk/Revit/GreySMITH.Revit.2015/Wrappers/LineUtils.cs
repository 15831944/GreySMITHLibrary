using System;
using Autodesk.Revit.DB;

namespace GreySMITH.Revit.Wrappers
{
    public partial class LineUtils
    {
        /* questions
         * 
         *  is this line based families or just lines?
         *  which lines? symbolic, detail, model?
         * /

        // push information about line from Revit to Assemble

        /* would probably have to create Line Class in Assemble
         * or have to match up with some of their pre-existing information?
         */

        // would have to serialize the information to send it from one point to the next

        // probably have to utilize threading (at the very least producer consumer functions) to usefully
        // translate info without crashing Revit

        // what info is important in a line?
            // length?
                // global start and end?
            // placement level
            // height in space
            // phase created
            // phase demolished
            // line subcategory
            
            // color?
            // view? (if symbolic line[which is in a family] or detail line in a view)
            // workset?
            // 
//
//        /// <summary>
//        /// method should transpose line information to new object
//        /// </summary>
//        /// <param name="lineinmodel">abstracted parent class of line objects</param>
//        public static void Export(this CurveElement lineinmodel)
//        {
//            CurveElementType cet = lineinmodel.CurveElementType;
//
//            switch(cet)
//            {
//                case CurveElementType.Invalid:
//                    throw new Exception("{0}: \tis not a valid element. ");
//
//                case CurveElementType.DetailCurve:
//                    // do work
//                    break;
//                
//                default:
//                    break;
//            }
//            
    }
}
