using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace traffic_light
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InputData ipd = new InputData()
            {
                iterationCount = 10,
                lambda1 = 10,
                lambda2 = 1,
                intensity1 = 1.0,
                intensity2 = 1.0,
                timeInterval1 = new TimeInterval() { min = 0.5, max = 2.0 },
                timeInterval2 = new TimeInterval() { min = 0.5, max = 2.0 },
                sTime = 0.1
            };
            TrafficLight tl = new TrafficLight(ipd);
            tl.run();
            
        }
    }
}
