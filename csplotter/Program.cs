using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Console;

using LumenWorks.Framework.IO.Csv;
//using NumSharp;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

public partial class Form1 : Form {
    private OxyPlot.WindowsForms.PlotView plot1;

    public Form1(PlotModel myModel) {
        //this.InitializeComponent();
        this.plot1 = new OxyPlot.WindowsForms.PlotView();
        this.SuspendLayout();
        this.plot1.Model = myModel;
        this.plot1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.plot1.Location = new System.Drawing.Point(0, 0);
        this.plot1.Margin = new System.Windows.Forms.Padding(0);
        this.plot1.Name = "Plot";
        this.plot1.Size = new System.Drawing.Size(632, 446);
        this.plot1.TabIndex = 0;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(632, 446);
        this.Controls.Add(this.plot1);
        this.Name = "Form";
        this.Text = "OxyPlot in Windows Forms";
        this.ResumeLayout(false);
    }
}

class Program {
    static void headermenu(object[] headers) {
        WriteLine("Header options: ");
        for (int i = 0; i < headers.Length; i++) WriteLine("\t{0}. {1}", i, headers[i]); 
    }
    
    static DataTable getaggregatetable(DataTable data, 
    int[] drillaggregates = null, int[] filters = null, object[] filterdata = null) {
        var filtertable = data.Copy();
        
        for (int i = 0; i < filters.Length; i++) {
            for (int j = 0; j < filtertable.Rows.Count; j++) {
                if (!filtertable.Rows[j][filters[i]].Equals(filterdata[i])) 
                    filtertable.Rows[j].Delete();
            }
        }
        filtertable.AcceptChanges();

        var aggdata = filtertable.Copy();
        for (int i = aggdata.Columns.Count; i --> 0;)
            if (!drillaggregates.Contains(i)) aggdata.Columns.RemoveAt(i); 
        return aggdata;
    } 

    static object[] getaggregateheaders(object[] headers, int[] drillaggregates = null) {
        var aggheaders = new List<object>();
        for (int i = 1; i < drillaggregates.Length; i++) {
            aggheaders.Add(headers[drillaggregates[i]]);
        }

        aggheaders.Add(headers[drillaggregates[0]]);
        return aggheaders.ToArray();
    }

    static void displayaggregatetable(DataTable data, object[] headers,
    int[] drillaggregates = null, int[] filters = null, object[] filterdata = null) {
        if (drillaggregates.Length == 1 && filters.Length == 0) {
            double colsum = 0;
            foreach (DataRow dr in data.Rows) colsum += Convert.ToDouble(dr.ItemArray[drillaggregates[0]]);
            string[] curheaders = {"Summary", $"Sum of all {headers[drillaggregates[0]]}", "Record Count"};
            string[] curtable = {"Summary", colsum.ToString(), data.Rows.Count.ToString()};
            WriteLine(string.Join(", ", curheaders));
            WriteLine(string.Join(", ", curtable));
        } else {
            var aggheaders = getaggregateheaders(headers, drillaggregates);
            var aggdata = getaggregatetable(data, drillaggregates, filters, filterdata);

            WriteLine(string.Join(", ", aggheaders));
            for (int i = 0; i < aggdata.Rows.Count; i++) WriteLine(string.Join(", ", aggdata.Rows[i].ItemArray));
        }
    }

    [STAThread]
    static void Main(string[] args) {
        var data = new DataTable();  
        using (var csvReader = new CsvReader(new StreamReader(System.IO.File.OpenRead(@"data.csv")), false, ',')) {  
            data.Load(csvReader);  
        }  
        
        object[] headers = (object[])data.Rows[0].ItemArray.Clone();
        //foreach(var item in headers.ItemArray) Write(item + "   "); WriteLine();

        data.Rows[0].Delete();
        data.AcceptChanges();

        bool runprog = true;

        var drillaggregates = new List<int>();
        headermenu(headers);
        Console.Write("Enter initial aggregate column # (must be numeric values only): ");
        var initagg = Convert.ToInt32(Console.ReadLine());
        drillaggregates.Add(initagg);
        var filters = new List<int>();
        var filterdata = new List<object>();

        do {
            try {
                WriteLine("Menu options:\n\t0. Display Fact Table\n\t1. Display Aggregated Table\n\t2. Reinitialize Aggregate\n\t3. Drill-down\n\t4. Filter\n\t5. View\n\t6. Quit\n\t7. Reset Drill Aggregates\n\t8. Reset Filters\n\t9. Remove Drill Aggregate\n\t10.Remove Filter");
                // also remove aggregate on website
                Write("Enter menu choice #: ");
                int menuoption = Convert.ToInt32(Console.ReadLine());
                
                if (menuoption == 0) {          // Display Fact Table
                    WriteLine(string.Join(",", headers));
                    for (int i = 0; i < data.Rows.Count; i++) WriteLine(string.Join(", ", data.Rows[i].ItemArray));
                } else if (menuoption == 1) {   // Display Aggregated Table
                    displayaggregatetable(data, headers, drillaggregates.ToArray(), filters.ToArray(), filterdata.ToArray());
                } else if (menuoption == 2) {   // Reinitialize Aggregate
                    headermenu(headers);
                    Write("Enter initial aggregate column # (must be numeric values only): ");
                    initagg = Convert.ToInt32(Console.ReadLine());

                    if (drillaggregates.Count == 0) {
                        drillaggregates.Add(initagg);
                    } else {
                        drillaggregates[0] = initagg;
                    }

                } else if (menuoption == 3) {   // Drill-down
                    headermenu(headers);
                    Write("Enter drill-down aggregate choice #: ");
                    int drillagg = Convert.ToInt32(Console.ReadLine());

                    if (!drillaggregates.Contains(drillagg)) drillaggregates.Add(drillagg);

                    displayaggregatetable(data, headers, drillaggregates.ToArray(), filters.ToArray(), filterdata.ToArray());
                } else if (menuoption == 4) {   // Filter
                    headermenu(headers);
                    Write("Enter filter header choice #: ");
                    int filterheader = Convert.ToInt32(Console.ReadLine());
                    var filteroptions = new List<object>();
                
                    for (int i = 0; i < data.Rows.Count; i++) 
                        filteroptions.Add(data.Rows[i][filterheader]);
                    filteroptions = filteroptions.Distinct().ToList();

                    WriteLine("Filter options: ");
                    for (int i = 0; i < filteroptions.Count; i++)
                        WriteLine("\t{0}. {1}", i, filteroptions[i]);
                    
                    Write("Enter filter option choice #: ");
                    int filterby = Convert.ToInt32(Console.ReadLine());
                    var filter = filteroptions[filterby];

                    if (!filters.Contains(filterheader)) {
                        filters.Add(filterheader);
                        filterdata.Add(filter);
                    }

                    if (drillaggregates.Count > 1 || filters.Count > 0) {
                        displayaggregatetable(data, headers, drillaggregates.ToArray(), filters.ToArray(), filterdata.ToArray());
                    } else {
                        if (filters.Count == 0 && drillaggregates.Count == 1) {
                            var filtertable = data.Clone();
                            for (int i = 0; i < filtertable.Rows.Count; i++) {
                                if (filtertable.Rows[i][filterheader] != filter) 
                                    filtertable.Rows[i].Delete();
                            }
                            filtertable.AcceptChanges();
                            WriteLine(string.Join(",", headers));
                            for (int i = 0; i < filtertable.Rows.Count; i++) WriteLine(string.Join(", ", filtertable.Rows[i].ItemArray));
                        }
                    }

                } else if (menuoption == 5) {   // View
                    DataTable x, y;
                    int xhead, yhead;

                    if (filters.Count == 0 && drillaggregates.Count == 1) {
                        headermenu(headers);
                        Write("Enter x header column #: ");
                        xhead = Convert.ToInt32(Console.ReadLine());
                        Write("Enter y header column #: ");
                        yhead = Convert.ToInt32(Console.ReadLine());
                        
                        x = data.Copy();
                        y = data.Copy();

                        for (int i = data.Columns.Count; i --> 0;) {
                            if (i != xhead) x.Columns.RemoveAt(i);
                            if (i != yhead) y.Columns.RemoveAt(i); 
                        }
                    } else {
                        WriteLine("Header options (remove drill aggregates to display more options):");
                        for (int i = 0; i < drillaggregates.Count; i++)
                            WriteLine("{0}. {1}", drillaggregates[i], headers[drillaggregates[i]]);

                        Write("Enter x header column #: ");
                        xhead = Convert.ToInt32(Console.ReadLine());
                        Write("Enter y header column #: ");
                        yhead = Convert.ToInt32(Console.ReadLine());

                        var sorteddrillagg = drillaggregates.ToArray();
                        Array.Sort(sorteddrillagg);
                        int xheadind = Array.IndexOf(sorteddrillagg, xhead);
                        int yheadind = Array.IndexOf(sorteddrillagg, yhead);
                        
                        x = getaggregatetable(data, drillaggregates.ToArray(), filters.ToArray(), filterdata.ToArray());
                        for (int i = x.Columns.Count; i --> 0;)
                            if (i != xheadind) x.Columns.RemoveAt(i); 
                        
                        y = getaggregatetable(data, drillaggregates.ToArray(), filters.ToArray(), filterdata.ToArray());
                        for (int i = y.Columns.Count; i --> 0;)
                            if (i != yheadind) y.Columns.RemoveAt(i);
                    }

                    WriteLine("Graph options:\n\t1. Bar\n\t2. Line\n\t3. Line with Marker\n\t4. Scatter\n\t5. Pie");
                    Write("Enter graph choice #: ");
                    int graphtype = Convert.ToInt32(Console.ReadLine());

                    var xlist = new List<object>();
                    var ylist = new List<object>();

                    for (int i = 0; i < x.Rows.Count; i ++) {
                        xlist.Add(x.Rows[i][0]);
                        ylist.Add(y.Rows[i][0]);
                    }

                    var myModel = new PlotModel { 
                        Title = $"Data from the CSV File: {headers[xhead]} and {headers[yhead]}",
                        Background = OxyColors.White
                        };
                    
                    var linearAxis1 = new LinearAxis();
                    linearAxis1.Position = AxisPosition.Bottom;
                    var linearAxis2 = new LinearAxis();
                    linearAxis2.Position = AxisPosition.Left;

                    if (graphtype != 5 || graphtype != 1) {
                        linearAxis1.Title = headers[xhead].ToString(); 
                        linearAxis2.Title = headers[yhead].ToString();
                    } // else { linearAxis1.Title = headers[xhead].ToString(); }

                    if (graphtype != 1) myModel.Axes.Add(linearAxis1);
                    if (graphtype != 1) myModel.Axes.Add(linearAxis2);
    
                    if (graphtype == 1) {           // Bar
                        var barSource = new List<BarItem>();
                        for (int i = 0; i < xlist.Count; i++)
                            barSource.Add(new BarItem{ Value = Convert.ToDouble(ylist[i]) });
                        
                        var barSeries = new BarSeries {
                                ItemsSource = barSource,
                                LabelPlacement = LabelPlacement.Inside,
                        };

                        var catSource = new List<string>();
                        for (int i = 0; i < xlist.Count; i++)
                            catSource.Add(xlist[i].ToString());

                         
                        linearAxis1.Title = headers[yhead].ToString();
                        myModel.Series.Add(barSeries);
                        myModel.Axes.Add(new CategoryAxis {
                                Position = AxisPosition.Left,
                                Key = "X_Axis",
                                Title = headers[xhead].ToString(),
                                ItemsSource = catSource.ToArray()
                        });
                        myModel.Axes.Add(linearAxis1);
                    } else if (graphtype == 2) {    // Line
                        var lineSeries = new LineSeries();
                        for (int i = 0; i < xlist.Count; i++)
                            lineSeries.Points.Add(new DataPoint(Convert.ToDouble(xlist[i]), Convert.ToDouble(ylist[i])));
                        myModel.Series.Add(lineSeries);
                    } else if (graphtype == 3) {    // Line with Marker
                        var lineSeries = new LineSeries();
                        lineSeries.MarkerType = MarkerType.Circle;
                        lineSeries.MarkerSize = 5;
                        lineSeries.MarkerStroke = OxyColors.Black;
                        lineSeries.MarkerFill = OxyColors.SkyBlue;
                        lineSeries.MarkerStrokeThickness = 1.5;
                        for (int i = 0; i < xlist.Count; i++)
                            lineSeries.Points.Add(new DataPoint(Convert.ToDouble(xlist[i]), Convert.ToDouble(ylist[i])));
                        myModel.Series.Add(lineSeries);
                    } else if (graphtype == 4) {    // Scatter
                        var scatterSeries = new ScatterSeries();
                        for (int i = 0; i < xlist.Count; i++)
                            scatterSeries.Points.Add(new ScatterPoint(Convert.ToDouble(xlist[i]), Convert.ToDouble(ylist[i])));
                        myModel.Series.Add(scatterSeries);
                    } else if (graphtype == 5) {    // Pie
                        var aggr_x = xlist.Distinct().ToList();
                        //plt.pie(aggr_x, labels=aggr_x)
                        var pieSeries = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };
                        for (int i = 0; i < xlist.Count; i++)
                            pieSeries.Slices.Add(new PieSlice(xlist[i].ToString(), Convert.ToDouble(ylist[i])) { IsExploded = false });
                        myModel.Series.Add(pieSeries);
                        //https://oxyplot.readthedocs.io/en/latest/models/series/PieSeries.html
                    }

                    var form = new Form1(myModel);
                    Application.EnableVisualStyles();
                    Application.Run(form);


                    var stream = new MemoryStream();
                    var pngExporter = new PngExporter { Width = 600, Height = 400}; //, Background = OxyColors.White };
                    pngExporter.Export(myModel, stream);

                    var bytes = new Byte[(int)stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(bytes, 0, (int)stream.Length);

                    string b64 = Convert.ToBase64String(bytes);
                    string uri = "data:image/png;base64," + b64;
                    string html = $"<img src = \"{uri}\"/>";
                } else if (menuoption == 6) {   // Quit
                    runprog = false;
                    return;
                } else if (menuoption == 7) {   // Reset Drill Aggregates
                    drillaggregates.Clear();
                } else if (menuoption == 8) {   // Reset Filters
                    filters.Clear();
                    filterdata.Clear();
                } else if (menuoption == 9) {   // Remove Drill Aggregate
                    WriteLine("Current drill aggregates:");
                    for (int i = 0; i < drillaggregates.Count; i++)
                        WriteLine("{0}. {1}", i, headers[drillaggregates[i]]);
                    
                    Write("Enter choice # to remove: ");
                    int removechoice = Convert.ToInt32(Console.ReadLine());
                    drillaggregates.RemoveAt(removechoice);
                } else if (menuoption == 10) {  // Remove Filter
                    WriteLine("Current filters:");
                    for (int i = 0; i < filters.Count; i++)
                        WriteLine("{0}. {1}", i, headers[filters[i]]);
                    
                    Write("Enter choice # to remove: ");
                    int removechoice = Convert.ToInt32(Console.ReadLine());
                    filters.RemoveAt(removechoice);
                    filterdata.RemoveAt(removechoice);
                }
            } catch (Exception ex) {
                WriteLine(ex.StackTrace);
            }
        } while (runprog);
    }
}