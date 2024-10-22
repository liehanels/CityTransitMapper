using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CityTransitMapper
{
    public partial class MainForm : Form
    {
        private Panel cyPanel;
        private Panel controlsPanel;
        private Button btnAddStation;
        private Button btnAddConnection;
        private Button btnFindRoute;
        private Label pathInfo;
        private Dictionary<string, Node> nodes;
        private List<Edge> edges;
        private Random random;
        private int stationCount;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "City Transit Mapper";
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            nodes = new Dictionary<string, Node>();
            edges = new List<Edge>();
            random = new Random();
            stationCount = 0;

            // Setup cy panel
            cyPanel = new Panel
            {
                Size = new Size(this.ClientSize.Width, (int)(this.ClientSize.Height * 0.8)),
                Location = new Point(0, 0),
                BackColor = Color.White
            };
            this.Controls.Add(cyPanel);
            cyPanel.Paint += CyPanel_Paint;

            // Setup controls panel
            controlsPanel = new Panel
            {
                Size = new Size(this.ClientSize.Width, (int)(this.ClientSize.Height * 0.2)),
                Location = new Point(0, cyPanel.Bottom),
                BackColor = Color.LightGray
            };
            this.Controls.Add(controlsPanel);

            // Continue to Segment 2...


            btnAddStation = new Button { Text = "Add Station", Location = new Point(10, 10), Width = 150 };
            btnAddConnection = new Button { Text = "Add Connection", Location = new Point(170, 10), Width = 150 };
            btnFindRoute = new Button { Text = "Find Shortest Route", Location = new Point(330, 10), Width = 150 };

            pathInfo = new Label { Location = new Point(10, 50), AutoSize = true, Text = "Shortest path: " };

            controlsPanel.Controls.Add(btnAddStation);
            controlsPanel.Controls.Add(btnAddConnection);
            controlsPanel.Controls.Add(btnFindRoute);
            controlsPanel.Controls.Add(pathInfo);

            btnAddStation.Click += BtnAddStation_Click;
            btnAddConnection.Click += BtnAddConnection_Click;
            btnFindRoute.Click += BtnFindRoute_Click;
        }
        private void BtnAddStation_Click(object sender, EventArgs e)
        {
            stationCount++;
            string stationId = $"Station{stationCount}";
            Node newNode = new Node(stationId, new Point(random.Next(300), random.Next(300)));
            nodes.Add(stationId, newNode);
            cyPanel.Invalidate();
        }

        private void BtnAddConnection_Click(object sender, EventArgs e)
        {
            string source = Prompt.ShowDialog("Enter source station ID:", "Add Connection");
            string target = Prompt.ShowDialog("Enter target station ID:", "Add Connection");
            if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(target))
            {
                Edge newEdge = new Edge(source, target);
                edges.Add(newEdge);
                cyPanel.Invalidate();
            }
        }

        private void BtnFindRoute_Click(object sender, EventArgs e)
        {
            string start = Prompt.ShowDialog("Enter start station ID:", "Find Shortest Route");
            string end = Prompt.ShowDialog("Enter end station ID:", "Find Shortest Route");
            if (!string.IsNullOrWhiteSpace(start) && !string.IsNullOrWhiteSpace(end))
            {
                // Implement Dijkstra's algorithm to find the shortest path (simplified here)
                var path = new List<string> { start, end }; // This is a placeholder. You should implement the actual pathfinding algorithm.
                pathInfo.Text = $"Shortest path: {string.Join(" → ", path)}";
            }
        }
        private void CyPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (var edge in edges)
            {
                if (nodes.ContainsKey(edge.Source) && nodes.ContainsKey(edge.Target))
                {
                    Node sourceNode = nodes[edge.Source];
                    Node targetNode = nodes[edge.Target];
                    g.DrawLine(Pens.Gray, sourceNode.Position, targetNode.Position);
                }
            }

            foreach (var node in nodes.Values)
            {
                g.FillEllipse(Brushes.Gray, node.Position.X - 10, node.Position.Y - 10, 20, 20);
                g.DrawEllipse(Pens.Black, node.Position.X - 10, node.Position.Y - 10, 20, 20);
                g.DrawString(node.Id, this.Font, Brushes.Black, node.Position.X - 10, node.Position.Y - 20);
            }
        }
    }
}
public class Node
{
    public string Id { get; set; }
    public Point Position { get; set; }

    public Node(string id, Point position)
    {
        Id = id;
        Position = position;
    }
}

public class Edge
{
    public string Source { get; set; }
    public string Target { get; set; }

    public Edge(string source, string target)
    {
        Source = source;
        Target = target;
    }
}
public static class Prompt
{
    public static string ShowDialog(string text, string caption)
    {
        Form prompt = new Form()
        {
            Width = 500,
            Height = 150,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = caption,
            StartPosition = FormStartPosition.CenterScreen
        };

        Label textLabel = new Label() { Left = 50, Top = 20, Text = text, AutoSize = true };
        TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
        Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
        confirmation.Click += (sender, e) => { prompt.Close(); };

        prompt.Controls.Add(textLabel);
        prompt.Controls.Add(textBox);
        prompt.Controls.Add(confirmation);
        prompt.AcceptButton = confirmation;

        return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
    }
}
