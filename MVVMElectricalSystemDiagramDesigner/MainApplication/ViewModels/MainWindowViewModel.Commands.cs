using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MainApplication
{
    public partial class MainWindowViewModel
    {

        #region Routed Command

        public static RoutedCommand Group = new();
        public static RoutedCommand BringForward = new();
        public static RoutedCommand BringToFront = new();
        public static RoutedCommand SendBackward = new();
        public static RoutedCommand SendToBack = new();
        public static RoutedCommand AlignTop = new();
        public static RoutedCommand AlignVerticalCenters = new();
        public static RoutedCommand AlignBottom = new();
        public static RoutedCommand AlignLeft = new();
        public static RoutedCommand AlignHorizontalCenters = new();
        public static RoutedCommand AlignRight = new();
        public static RoutedCommand DistributeHorizontal = new();
        public static RoutedCommand DistributeVertical = new();
        public static RoutedCommand SelectAll = new();
        public static RoutedCommand Import = new();

        #endregion


    }
}
