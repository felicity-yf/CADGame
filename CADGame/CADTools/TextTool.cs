using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADGame
{
    public static partial class TextTool
    {
        public struct SpecialSymbol
        {
            public static readonly string Degree = @"\U+00B0";
            public static readonly string Tolerance = @"U+00B1";
            public static readonly string Diameter = @"\U+00D8";
            public static readonly string Angle = @"\U+2220";
            public static readonly string AlmostEqual = @"\U+2248";
            public static readonly string LineBoundary = @"\U+E100";
            public static readonly string LineCenter = @"\U+2104";
            public static readonly string Delta = @"\U+0394";
            public static readonly string NotEqual = @"\U+2260";
            public static readonly string Square = @"\U+00B2";
        }
    }
}
