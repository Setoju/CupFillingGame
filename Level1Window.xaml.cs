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
    /// Interaction logic for Level1Window.xaml
    /// </summary>
    public partial class Level1Window : Window
    {
        public Level1Window()
        {
            InitializeComponent();           
        }

        public class Level1 : Level
        {
            Level1Window startLevel1 = new Level1Window();
            public Level1()
            {
                LevelNumber = 1;
            }

            public override void StartLevel()
            {
                startLevel1.Show();
                MessageBox.Show("Level 1 started!");
            }            
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Level2Window.Level2 startNext = new Level2Window.Level2();
            this.Close();
            startNext.StartLevel();
        }
    }
}
