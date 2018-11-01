﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapViewPallet.Shape
{
    class PalletViewControlService
    {
        //=================VARIABLE==================
        private MainWindow mainWindow;
        private Canvas map;
        private ScaleTransform scaleTransform;
        private TranslateTransform translateTransform;
        private Point startPoint;
        private Point originalPoint;
        private double zoomInLitmit = 7;
        private double zoomOutLimit;
        private double slidingScale;
        double yDistanceBottom, xDistanceLeft, yDistanceTop, xDistanceRight;
        
        public PalletViewControlService(MainWindow mainWinDowIn)
        {
            mainWindow = mainWinDowIn;
            map = mainWindow.map;
            scaleTransform = mainWindow.canvasScaleTransform;
            translateTransform = mainWindow.canvasTranslateTransform;
            //==========EVENT==========
            map.MouseWheel += Map_Zoom;
            map.MouseMove += Map_MouseMove;
            map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
            map.MouseLeftButtonUp += Map_MouseLeftButtonUp;
            mainWindow.clipBorder.SizeChanged += ClipBorder_SizeChanged;
            zoomOutLimit = mainWindow.clipBorder.ActualHeight / map.Height;

        }

        private void ClipBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            zoomOutLimit = mainWindow.clipBorder.ActualHeight / map.Height;
            scaleTransform.ScaleX = scaleTransform.ScaleY = zoomOutLimit;
            if (map.Width > (mainWindow.clipBorder.ActualWidth * scaleTransform.ScaleX))
            {
                translateTransform.X = 0;
                translateTransform.Y = 0;
            }
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            //Point mousePos = e.GetPosition(map);
            //Console.WriteLine(mousePos.X.ToString("0.") + "   " + mousePos.Y.ToString("0."));
            yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));

            //Console.WriteLine("===========" + yDistanceTop.ToString("0.") + "===========");
            //Console.WriteLine("===========================");
            //Console.WriteLine("" + xDistanceLeft.ToString("0.") + "=================" + xDistanceRight.ToString("0.") + "");
            //Console.WriteLine("===========================");
            //Console.WriteLine("===========" + yDistanceBottom.ToString("0.") + "===========");
            if ((mainWindow.drag))
            {
                if (!map.IsMouseCaptured) return;
                Vector moveVector = startPoint - e.GetPosition(mainWindow.clipBorder);
                double xCoor = originalPoint.X - moveVector.X;
                double yCoor = originalPoint.Y - moveVector.Y;
                translateTransform.X = xCoor;
                translateTransform.Y = yCoor;
                yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
                xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
                yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
                xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
                //YCoor
                if (yDistanceTop > 0)
                {
                    translateTransform.Y = translateTransform.Y - yDistanceTop;
                }
                if (yDistanceBottom > 0)
                {
                    translateTransform.Y = translateTransform.Y + yDistanceBottom;
                }
                //XCoor
                if ((map.Width * scaleTransform.ScaleX) < mainWindow.clipBorder.ActualWidth)
                {
                    translateTransform.X = 0;
                }
                else
                { 
                    if (xDistanceLeft > 0)
                    {
                        translateTransform.X = translateTransform.X - xDistanceLeft;
                    }

                    if (xDistanceRight > 0)
                    {
                        translateTransform.X = translateTransform.X + xDistanceRight;
                    }
                }
            }
            if (!mainWindow.drag)
            {
                Statectrl_MouseMove(e);
            }

            //Console.WriteLine(startPoint);
            //Console.WriteLine(originalPoint);
            //Console.WriteLine("//////////////");
        }

        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            map.ReleaseMouseCapture();
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine(translateTransform.X+"  "+ translateTransform.Y);
            //Console.WriteLine(mainWindow.clipBorder.ActualWidth + "  "+ mainWindow.clipBorder.ActualHeight);
            string elementName = (e.OriginalSource as FrameworkElement).Name;
            //Console.WriteLine(elementName);
            if (e.Source.ToString() == "System.Windows.Controls.Canvas")
            {
                map.CaptureMouse();
                startPoint = e.GetPosition(mainWindow.clipBorder);
                originalPoint = new Point(translateTransform.X, translateTransform.Y);
            }
            Statectrl_MouseDown(e);
        }

        private void Map_Zoom(object sender, MouseWheelEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            double zoomDirection = e.Delta > 0 ? 1 : -1;
            
            slidingScale = 0.1 * zoomDirection;
            if (((scaleTransform.ScaleY + slidingScale) >= zoomOutLimit) && 
                ((scaleTransform.ScaleY + slidingScale) <= zoomInLitmit))
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY += slidingScale;
            }

            yDistanceBottom = (((mainWindow.clipBorder.ActualHeight / 2) - (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceRight = ((mainWindow.clipBorder.ActualWidth / 2 - (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            yDistanceTop = (((mainWindow.clipBorder.ActualHeight / 2) + (translateTransform.Y)) - ((map.Height * scaleTransform.ScaleY) / 2));
            xDistanceLeft = ((mainWindow.clipBorder.ActualWidth / 2 + (translateTransform.X)) - ((map.Width * scaleTransform.ScaleX) / 2));
            //YCoor
            if (yDistanceTop > 0)
            {
                translateTransform.Y = translateTransform.Y - yDistanceTop;
            }
            if (yDistanceBottom > 0)
            {
                translateTransform.Y = translateTransform.Y + yDistanceBottom;
            }
            //XCoor
            if ((map.Width * scaleTransform.ScaleX) < mainWindow.clipBorder.ActualWidth)
            {
                translateTransform.X = 0;
            }
            else
            {
                if (xDistanceLeft > 0)
                {
                    translateTransform.X = translateTransform.X - xDistanceLeft;
                }

                if (xDistanceRight > 0)
                {
                    translateTransform.X = translateTransform.X + xDistanceRight;
                }
            }

        }

        //=================PROCESS===============
        void Statectrl_MouseDown(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                //EditObject();
            }
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = e.Source as FrameworkElement;
            switch (Global_Mouse.ctrl_MouseDown)
            {
                case Global_Mouse.STATE_MOUSEDOWN._ADD_STATION:
                    {
                        if (Global_Mouse.ctrl_MouseDown == Global_Mouse.STATE_MOUSEDOWN._ADD_STATION)
                        {
                            StationShape stationTemp = null;
                            stationTemp = new StationShape("MIX0", 2, 7, "Pallet2");
                            stationTemp.Move(mousePos.X, mousePos.Y);
                            map.Children.Add(stationTemp);
                        }
                        break;
                    }
                case Global_Mouse.STATE_MOUSEDOWN._KEEP_IN_OBJECT:
                    if (mouseWasDownOn != null)
                    {
                        string elementName = mouseWasDownOn.Name;
                        if (elementName != "")
                        {
                            Global_Mouse.ctrl_MouseMove = Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION;
                            Global_Mouse.ctrl_MouseDown = Global_Mouse.STATE_MOUSEDOWN._GET_OUT_OBJECT;
                        }
                    }
                    break;
                default:
                    {
                        break;
                    }
            }
        }
        void Statectrl_MouseMove(MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(map);
            var mouseWasDownOn = e.Source as FrameworkElement;
            switch (Global_Mouse.ctrl_MouseMove)
            {
                case Global_Mouse.STATE_MOUSEMOVE._NORMAL:
                    {
                        break;
                    }
                case Global_Mouse.STATE_MOUSEMOVE._MOVE_STATION:
                    {
                        //StationShape x = new StationShape();
                        //x = (StationShape)map.Children[0];
                        //x.Move(pp.X, pp.Y);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
