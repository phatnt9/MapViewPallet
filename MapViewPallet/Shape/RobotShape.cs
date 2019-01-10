using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MapViewPallet.Shape
{
    public class RobotShape : Border
    {
        Thread th;

        double angle = 0.0f;
        public Point org = new Point(600, 350);
        public double rad = 0;
        public double anglestep = 0;
        Rect area = new Rect(30, 30, 500, 500);

        Point loc = new Point(0, 0);

        public event Action<string> RemoveHandle;
        public struct Props
        {
            public string name;
            public bool isSelected;
            public bool isHovering;
            public Canvas canvas;
            public Grid mainGrid;
            public Grid statusGrid;
            public Point position; //on canvas
            public double rotate;
            public Label rbID;
            public Label rbTask;
            public Rectangle headLed;
            public Rectangle tailLed;
            public TranslateTransform rbTranslate;
            public TransformGroup rbTransformGroup;
            public RotateTransform rbRotateTransform;
            public TranslateTransform contentTranslate;
            public TransformGroup contentTransformGroup;
            public RotateTransform contentRotateTransform;
            public Border statusBorder;
            public List<Point> eightCorner;
        }

        public Properties robotProperties;
        public Props props;

        public RobotShape(Canvas pCanvas)
        {
            //th = new Thread(DrawCircle);
            //DrawCircle();
            //th.IsBackground = true;
            //th.Start();

            ToolTip = "";
            ToolTipOpening += ChangeToolTipContent;
            props.isSelected = false;
            props.isHovering = false;
            ContextMenu = new ContextMenu();
            //===================================
            MenuItem editItem = new MenuItem();
            editItem.Header = "Edit";
            editItem.Click += EditMenu;
            //===================================
            MenuItem removeItem = new MenuItem();
            removeItem.Header = "Remove";
            removeItem.Click += RemoveMenu;
            ContextMenu.Items.Add(editItem);
            ContextMenu.Items.Add(removeItem);
            //====================EVENT=====================
            //MouseLeave += MouseLeavePath;
            //MouseMove += MouseHoverPath;
            //MouseLeftButtonDown += MouseLeftButtonDownPath;
            //MouseRightButtonDown += MouseRightButtonDownPath;
            //===================CREATE=====================
            Name = "Robotx" + Global_Mouse.EncodeTransmissionTimestamp();
            props.name = Name;
            props.position = new Point();
            props.mainGrid = new Grid();
            props.statusGrid = new Grid();
            props.statusBorder = new Border();
            props.rbID = new Label();
            props.rbTask = new Label();
            props.headLed = new Rectangle();
            props.tailLed = new Rectangle();
            props.eightCorner = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                Point temp = new Point();
                props.eightCorner.Add(temp);
            }
            props.rbRotateTransform = new RotateTransform();
            props.rbTranslate = new TranslateTransform();
            props.rbTransformGroup = new TransformGroup();
            props.contentRotateTransform = new RotateTransform();
            props.contentTranslate = new TranslateTransform();
            props.contentTransformGroup = new TransformGroup();
            robotProperties = new Properties(this);
            //===================STYLE=====================
            //Robot border
            Width = 22;
            Height = 15;
            BorderThickness = new Thickness(1);
            BorderBrush = new SolidColorBrush(Colors.Linen);
            Background = new SolidColorBrush(Colors.Black);
            CornerRadius = new CornerRadius(3);
            RenderTransformOrigin = new Point(0.5, 0.5);
            //mainGrid
            props.mainGrid.Background = new SolidColorBrush(Colors.Transparent);
            for (int i=0; i<3; i++)
            {
                ColumnDefinition colTemp = new ColumnDefinition();
                colTemp.Name = Name + "xL" + i;
                if ((i == 0) || (i == 2))
                {
                    colTemp.Width = new GridLength(1);
                }
                props.mainGrid.ColumnDefinitions.Add(colTemp);
            }
            //headLed
            props.headLed.Height = 7;
            props.headLed.Fill = new SolidColorBrush(Colors.DodgerBlue);
            Grid.SetColumn(props.headLed, 2);
            //tailLed
            props.tailLed.Height = 7;
            props.tailLed.Fill = new SolidColorBrush(Colors.OrangeRed);
            Grid.SetColumn(props.tailLed, 0);
            //statusBorder
            props.statusBorder.Width = 10;
            props.statusBorder.Height = 13;
            props.statusBorder.RenderTransformOrigin = new Point(0.5,0.5);
            Grid.SetColumn(props.statusBorder, 1);
            //statusGrid
            for (int i = 0; i < 2; i++)
            {
                RowDefinition rowTemp = new RowDefinition();
                rowTemp.Name = Name + "xR" + i;
                props.statusGrid.RowDefinitions.Add(rowTemp);
            }
            //rbID
            props.rbID.Padding = new Thickness(0);
            props.rbID.Margin = new Thickness(-5, 0, -5, 0);
            props.rbID.HorizontalAlignment = HorizontalAlignment.Center;
            props.rbID.VerticalAlignment = VerticalAlignment.Bottom;
            props.rbID.Content = "27";
            props.rbID.Foreground = new SolidColorBrush(Colors.Yellow);
            props.rbID.FontFamily = new FontFamily("Calibri");
            props.rbID.FontSize = 6;
            props.rbID.FontWeight = FontWeights.Bold;
            Grid.SetRow(props.rbID, 0);

            //rbTask
            props.rbTask.Padding = new Thickness(0);
            props.rbTask.Margin = new Thickness(-5, -1, -5, -1);
            props.rbTask.HorizontalAlignment = HorizontalAlignment.Center;
            props.rbTask.VerticalAlignment = VerticalAlignment.Top;
            props.rbTask.Content = "9999";
            props.rbTask.Foreground = new SolidColorBrush(Colors.LawnGreen);
            props.rbTask.FontFamily = new FontFamily("Calibri");
            props.rbTask.FontSize = 6;
            props.rbTask.FontWeight = FontWeights.Bold;
            Grid.SetRow(props.rbTask, 1);

            //===================CHILDREN===================
            props.statusGrid.Children.Add(props.rbID);
            props.statusGrid.Children.Add(props.rbTask);
            props.statusBorder.Child = props.statusGrid;
            props.mainGrid.Children.Add(props.headLed);
            props.mainGrid.Children.Add(props.tailLed);
            props.mainGrid.Children.Add(props.statusBorder);
            props.rbTransformGroup.Children.Add(props.rbRotateTransform);
            props.rbTransformGroup.Children.Add(props.rbTranslate);
            RenderTransform = props.rbTransformGroup;
            props.contentTransformGroup.Children.Add(props.contentRotateTransform);
            props.contentTransformGroup.Children.Add(props.contentTranslate);
            props.statusBorder.RenderTransform = props.contentTransformGroup;
            props.canvas = pCanvas;
            Child = props.mainGrid;
            props.canvas.Children.Add(this);

            //====================FINAL=====================


        }

        public void ChangeTask (string taskID)
        {
            //props.rbTask.Content = taskID;
        }



        public void ReDraw(Point Position, double Rotate)
        {
            props.rotate = Rotate;
            props.position = new Point(Position.X, Position.Y);
            Draw();
        }

        public void DrawCircle()
        {

            //while (true)
            {
                loc = CirclePoint(rad, angle, org);
                props.position.X = loc.X + area.X;
                props.position.Y = loc.Y + area.Y;

                ReDraw(props.position, angle-90);

                if (angle < 360)
                {
                    angle += anglestep;
                }
                else
                {
                    angle = 0;
                }
            }
        }

        public Point CirclePoint (double radius, double angleInDegrees, Point origin)
        {
            double x = (double)(radius * Math.Cos(angleInDegrees * Math.PI / 180)) + origin.X;
            double y = (double)(radius * Math.Sin(angleInDegrees * Math.PI / 180)) + origin.Y;

            return new Point(x,y);
        }


        public void ReDraw() //random
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                //Random rotate = new Random();
                //int rotateInt = rotate.Next(0, 360); //for ints
                //int rotateRange = 360;
                //double rotateDouble = rotate.NextDouble() * rotateRange; //for doubles

                //Random posX = new Random();
                //int posIntX = posX.Next(20, 1200); //for ints
                //int posRangeX = 1180;
                //double posDoubleX = posX.NextDouble() * posRangeX; //for doubles

                //Random posY = new Random();
                //int posIntY = posX.Next(20, 700); //for ints
                //int posRangeY = 680;
                //double posDoubleY = posY.NextDouble() * posRangeY; //for doubles


                //props.rotate = roro1;
                //props.position = new Point(roro2,roro3);

                ChangeTask("22");
                Draw();
            }));
        }

        public void PrintCount(int LowLimit, int UpperLimit)
        {
            for (int i = LowLimit; i <= UpperLimit; i++)
            {
                Console.WriteLine("> {0}", i);
            }
        }

        public void Draw()
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                //Render Robot
                props.rbRotateTransform.Angle = props.rotate;
                props.rbTranslate = new TranslateTransform(props.position.X - (Width / 2), props.position.Y - (Height / 2));
                props.rbTransformGroup.Children[1] = props.rbTranslate;
                //Render Status
                props.contentRotateTransform.Angle = -(props.rotate);
                props.contentTranslate = new TranslateTransform(0, 0);
                props.contentTransformGroup.Children[1] = props.contentTranslate;
                // SPECIAL POINTS
                //props.eightCorner[0] = CoorPointAtBorder(new Point((0), (Height / 2)));          //mid-left
                //props.eightCorner[1] = CoorPointAtBorder(new Point((0), (0)));                 //top-left
                //props.eightCorner[2] = CoorPointAtBorder(new Point((Width / 2), (0)));           //top-mid
                //props.eightCorner[3] = CoorPointAtBorder(new Point((Width), (0)));             //top-right
                //props.eightCorner[4] = CoorPointAtBorder(new Point((Width), (Height / 2)));      //mid-right
                //props.eightCorner[5] = CoorPointAtBorder(new Point((Width), (Height)));        //bot-right
                //props.eightCorner[6] = CoorPointAtBorder(new Point((Width / 2), (Height)));      //bot-mid
                //props.eightCorner[7] = CoorPointAtBorder(new Point((0), (Height)));            //bot-left
            }));
        }

        public Point CoorPointAtBorder(Point pointOnBorder)
        {
            //double xDiff = (pointOnBorder.X) - (props._start.X);
            //double yDiff = (pointOnBorder.Y) - (props._start.Y);
            //double rad1 = (props.rotate * Math.PI) / 180;
            //double rad2 = (Math.Atan2(yDiff, xDiff));
            //double L = Global_Object.LengthBetweenPoints(props._start, pointOnBorder);
            //double x1 = props._oriMousePos.X + ((L * Math.Cos(rad1 + rad2)));
            //double y1 = props._oriMousePos.Y + ((L * Math.Sin(rad1 + rad2)));
            //return new Point(x1, y1);
            return new Point(0,0);
        }























        private void ChangeToolTipContent(object sender, ToolTipEventArgs e)
        {
            ToolTip = "1234567890";
        }

        private void EditMenu(object sender, RoutedEventArgs e)
        {
            robotProperties.ShowDialog();
        }

        private void RemoveMenu(object sender, RoutedEventArgs e)
        {
            Remove();
        }

        public void Remove()
        {
            props.canvas.Children.Remove(this);
            RemoveHandle(props.name);
        }

    }
}
