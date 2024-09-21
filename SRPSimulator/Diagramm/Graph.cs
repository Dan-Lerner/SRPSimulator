using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace SRPSimulator
{
    // Control for drawing ciclogramms
    public partial class Graph : UserControl
    {
        private const int grayFadeStart = 150;
        private const int grayFadeEnd = 240;
        private const byte VisibleStrokesN = 5;

        private Color colorUpstroke = Color.LightGreen;
        private Color colorDownstroke = Color.Blue;

        public string Title { 
            get => zedGraph.GraphPane.Title.Text; 
            set { 
                zedGraph.GraphPane.Title.Text = value; 
            } 
        }

        public Graph()
        {
            InitializeComponent();

            // ZedGraph creation and initialization

            zedGraph = new ZedGraphControl();

            zedGraph.Name = "";
            zedGraph.AutoSize = true;
            zedGraph.Dock = DockStyle.Fill;
            zedGraph.BorderStyle = BorderStyle.None;

            this.Controls.Add(zedGraph);

            GraphPane pane = zedGraph.GraphPane;

            pane.XAxis.Title.Text = "X Axis";
            pane.YAxis.Title.Text = "Y Axis";

            pane.Border.Color = Color.Transparent;

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.DashOn = 100;
            pane.XAxis.MajorGrid.DashOff = 0;
            pane.XAxis.MajorGrid.Color = Color.LightGray;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.DashOn = 100;
            pane.YAxis.MajorGrid.DashOff = 0;
            pane.YAxis.MajorGrid.Color = Color.LightGray;

            pane.IsFontsScaled = false;
            pane.XAxis.Scale.FontSpec.Size = 12;
            pane.YAxis.Scale.FontSpec.Size = 12;
            pane.XAxis.Title.FontSpec.Size = 12;
            pane.YAxis.Title.FontSpec.Size = 12;
            pane.Legend.FontSpec.Size = 12;
            pane.Title.FontSpec.Size = 12;
//            pane.Title.FontSpec.IsUnderline = true;

            pane.XAxis.MajorTic.IsOpposite = false;
            pane.XAxis.MinorTic.IsOpposite = false;
            pane.YAxis.MajorTic.IsOpposite = false;
            pane.YAxis.MinorTic.IsOpposite = false;

            pane.XAxis.MajorTic.Size = 0;
            pane.XAxis.MinorTic.Size = 0;
            pane.YAxis.MajorTic.Size = 0;
            pane.YAxis.MinorTic.Size = 0;

            pane.XAxis.Scale.Mag = 0;
            pane.YAxis.Scale.Mag = 0;

            pane.XAxis.Title.Gap = 0;
            pane.YAxis.Title.Gap = 0;

            pane.XAxis.Title.IsVisible = false;
            pane.YAxis.Title.IsVisible = false;

            pane.Legend.IsVisible = false;
//            pane.Title.IsVisible = false;

            pane.Chart.Border.IsVisible = false;

            zedGraph.AxisChange();
            zedGraph.Refresh();
        }

        public void Clear()
        {
            GraphPane pane = zedGraph.GraphPane;
            pane.CurveList.Clear();
            zedGraph.AxisChange();
            zedGraph.Refresh();
        }

        public void BeginUpstroke()
        {
            // Shift data for past strokes
            ShiftStrokes();

            // New curve for Upstroke
            curveCurrent = AddCurveToHead(colorUpstroke);

            // New curve for Downstroke
            AddCurveToHead(colorDownstroke);
        }

        private LineItem AddCurveToHead(Color color)
        {
            GraphPane pane = zedGraph.GraphPane;
            LineItem curve = pane.AddCurve(null, new PointPairList(), color, SymbolType.None);
            curve.Line.IsSmooth = true;
            int index = pane.CurveList.Count - 1;
            pane.CurveList.Move(index, -index);

            return curve;
        }

        public void BeginDownstroke()
        {
            // Switch to the next curve
            curveCurrent = zedGraph.GraphPane.CurveList[0] as LineItem;

            // Initialisation the first point to avoid a gap
            AddData(this.X, this.Y);
        }

        public void AddData(double X, double Y)
        {
            this.X = X;
            this.Y = Y;

            curveCurrent.AddPoint(X, Y);
            zedGraph.AxisChange();
            zedGraph.Refresh();
        }

        private void ShiftStrokes()
        {
            GraphPane pane = zedGraph.GraphPane;

            // Remove the oldest stroke
            while (pane.CurveList.Count > 2 * (VisibleStrokesN - 1))
                pane.CurveList.RemoveAt(pane.CurveList.Count - 1);

            // Set gray-fading colors for past strokes
            int grayValue;
            for (byte ii = 0; ii < pane.CurveList.Count / 2 && ii < VisibleStrokesN; ii++) {
                grayValue = grayFadeStart + ii * (grayFadeEnd - grayFadeStart) / (VisibleStrokesN - 2);
                pane.CurveList[2 * ii].Color = 
                pane.CurveList[2 * ii + 1].Color = Color.FromArgb(grayValue, grayValue, grayValue);
            }
        }

        private LineItem curveCurrent;

        // For memorize
        private double X;
        private double Y;
    }
}
