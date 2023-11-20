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
    /// Interaction logic for Level2Window.xaml
    /// </summary>
    public partial class Level2Window : Window
    {
        public Level2Window()
        {
            InitializeComponent();
        }
        public class Level2 : Level
        {
            Level2Window startLevel2 = new Level2Window();
            public Level2()
            {
                LevelNumber = 2;
            }

            public override void StartLevel()
            {
                startLevel2.Show();
                MessageBox.Show("Level 2 started!");
            }                   
        }
        private void NextLevelButton_Click(object sender, RoutedEventArgs e)
        {
            Level3Window.Level3 startNext = new Level3Window.Level3();
            this.Close();
            startNext.StartLevel();
        }
    }
}
