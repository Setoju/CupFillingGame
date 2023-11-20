using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CupFilling
{
    /// <summary>
    /// Interaction logic for Level3Window.xaml
    /// </summary>
    public partial class Level3Window : Window
    {
        public Level3Window()
        {
            InitializeComponent();
        }
        public class Level3 : Level
        {
            Level3Window startLevel3 = new Level3Window();
            public Level3()
            {
                LevelNumber = 3;
            }

            public override void StartLevel()
            {
                startLevel3.Show();
                MessageBox.Show("Level 3 started!");
            }
        }        
    }
}
