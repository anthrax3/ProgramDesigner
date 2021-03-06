﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ExpressionCompiler;
using ProgramDesigner.Controls;
using ProgramDesigner.Converters;
using System.Collections.Generic;
using System.Globalization;

namespace ProgramDesigner
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private bool _isNameDialogVisible;
        public bool IsNameDialogVisible
        {
            get { return _isNameDialogVisible; }
            set
            {
                _isNameDialogVisible = value;
                NotifyPropertyChanged("IsNameDialogVisible");
            }
        }
        public DragGrip RenameDragGrip { get; set; }

        private List<DragGrip> ControlList { get;set; }
        private List<DragGrip> VarList { get;set; }
        private List<DragGrip> TokenList { get;set; }
    
        private bool _isControlsVisible=true;
        public bool IsControlsVisible
        {
            get { return _isControlsVisible; }
            set { _isControlsVisible = value; }
        }

        private bool _isVarVisible = true;
        public bool IsVarVisible
        {
            get { return _isVarVisible; }
            set { _isVarVisible = value; }
        }

        private bool _isTokenVisible = true;
        public bool IsTokenVisible
        {
            get { return _isTokenVisible; }
            set { _isTokenVisible = value; }
        }


        public static object LocalClipboard { get; set; }
        public Utils Utils { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ControlList = new List<DragGrip>();
            VarList = new List<DragGrip>();
            TokenList = new List<DragGrip>();

            Loaded+= (a,b) => { };

            AddInLists(ControlList, GetItem("if", "if", new TranslateTransform(50, 80), 50, Brushes.Orange, new Thickness(-7, 0, 0, 0), "ContextMenu1"));
            AddInLists(ControlList, GetItem("else", "else", new TranslateTransform(50, 110), 50, Brushes.Orange, new Thickness(-7, 0, 0, 0), "ContextMenu1"));
            AddInLists(VarList, GetItem("var", "var", new TranslateTransform(50, 140), 50, Brushes.DeepSkyBlue, new Thickness(-7, 0, 0, 0), "ContextMenu1"));
            AddInLists(VarList, GetItem("variable", "variable", new TranslateTransform(50, 170), 65, Brushes.DeepSkyBlue, new Thickness(-4, 0, 0, 0), "ContextMenu2"));
            AddInLists(VarList, GetItem("number", "number", new TranslateTransform(50, 200), 65, Brushes.DeepSkyBlue, new Thickness(-7, 0, 0, 0), "ContextMenu2"));
            AddInLists(TokenList, GetItem("OpenRoundBracket", "(", new TranslateTransform(50, 230), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("CloseRoundBracket", ")", new TranslateTransform(50, 260), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("OpenBlockBracket", "{", new TranslateTransform(50, 290), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("CloseBlockBracket", "}", new TranslateTransform(50, 320), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Plus", "+", new TranslateTransform(50, 350), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("LessThan", ">", new TranslateTransform(50, 380), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("MoreThan", "<", new TranslateTransform(50, 410), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Minus", "-", new TranslateTransform(50, 440), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Mult", "*", new TranslateTransform(50, 470), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Div", "/", new TranslateTransform(50, 500), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Equal", "=", new TranslateTransform(50, 530), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));
            AddInLists(TokenList, GetItem("Semi", ";", new TranslateTransform(50, 560), 30, Brushes.DarkViolet, new Thickness(-4, -5, 0, 0), "ContextMenu1"));

            Utils = new Utils();
            DataContext = this;

            #region ToolBar Items Visibility
            Control.MouseDown += (a, b) =>
            {
                if (b.LeftButton != MouseButtonState.Pressed)
                    return;

                _isControlsVisible = !_isControlsVisible;

                var x = Canvas.GetLeft(VerticalBarrier);

                MainCanvas.Children.OfType<DragGrip>()
                    .Where( w =>
                            ((TranslateTransform) w.RenderTransform).X <= x &&
                            ((Border) w.Child).Background == Brushes.Orange)
                    .ToList()
                    .ForEach(f =>
                    {
                        f.Visibility = _isControlsVisible ? Visibility.Visible : Visibility.Collapsed;
                    });

                NotifyPropertyChanged("");
            };
            Var.MouseDown += (a, b) =>
            {
                if (b.LeftButton != MouseButtonState.Pressed)
                    return;

                _isVarVisible = !_isVarVisible;

                var x = Canvas.GetLeft(VerticalBarrier);

                MainCanvas.Children.OfType<DragGrip>()
                    .Where(w =>
                            ((TranslateTransform)w.RenderTransform).X <= x &&
                            ((Border)w.Child).Background == Brushes.DeepSkyBlue)
                    .ToList()
                    .ForEach(f =>
                    {
                        f.Visibility = _isVarVisible ? Visibility.Visible : Visibility.Collapsed;
                    });

                NotifyPropertyChanged("");
            };
            Token.MouseDown += (a, b) =>
            {
                if (b.LeftButton != MouseButtonState.Pressed)
                    return;

                _isTokenVisible = !_isTokenVisible;

                var x = Canvas.GetLeft(VerticalBarrier);

                MainCanvas.Children.OfType<DragGrip>()
                    .Where(w =>
                            ((TranslateTransform)w.RenderTransform).X <= x &&
                            ((Border)w.Child).Background == Brushes.DarkViolet)
                    .ToList()
                    .ForEach(f =>
                    {
                        f.Visibility = _isTokenVisible ? Visibility.Visible : Visibility.Collapsed;
                    });

                NotifyPropertyChanged("");
            };
            #endregion
        }

        private void AddInLists(List<DragGrip> list,DragGrip item)
        {
            list.Add(item);
            MainCanvas.Children.Add(item);
        }

        public DragGrip GetItem(string name ,string text,TranslateTransform translateTransform,
            double width,Brush backgound,Thickness margin,string contextMenuName)
        {
            var newItem = new DragGrip
            {
                Name = name,
                RenderTransform = translateTransform,
                IsDragable = false,
                IsToolBarItem = true,
                IsSelected = false,
                ContextMenuName = contextMenuName
            };

            var binding = new Binding
            {
                RelativeSource = new RelativeSource
                {
                    Mode = RelativeSourceMode.FindAncestor,
                    AncestorType = typeof(DragGrip)
                },
                Path = new PropertyPath("IsSelected"),
                Converter = new BoolToIntConverter(),
                ConverterParameter = "2.5",
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.TwoWay
            };
            var border = new Border
            {
                Background = backgound,
                Width = width,
                Height = 22,
                BorderBrush = Brushes.DeepSkyBlue,
                Child =
                    new TextBlock
                    {
                        Text = text,
                        FontSize = 16,
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = margin
                    }
            };
            border.SetBinding(Border.BorderThicknessProperty, binding);
            newItem.Child = border;
            newItem.ContextMenu = MainCanvas.FindResource(contextMenuName) as ContextMenu;

            return newItem;
        }

        private int _offset = 5;//pixel
        private Point? _point = null;
        private Point? _startingPoint = null;
        private FrameworkElement _element;
        private bool isDragging;
        private bool isCanvasDragging;
        private Point clickPosition;
        private Point initialPoint;
        private Point _multiInitialPoint;
        private void CanvasOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var draggableControl = e.OriginalSource as FrameworkElement;
            draggableControl = Utils.GetParent<DragGrip>(draggableControl) as DragGrip;
            
            if (draggableControl != null)
            {
                isDragging = true;
                clickPosition = e.GetPosition(this);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform != null)
                {
                    initialPoint = new Point(transform.X, transform.Y);
                    _multiInitialPoint = new Point(clickPosition.X , clickPosition.Y );
                    foreach (var child in MainCanvas.Children.OfType<DragGrip>().Where(w => w.IsSelected))
                    {
                        var childtransform = child.RenderTransform as TranslateTransform;
                        child.InitialPoint = new Point(childtransform.X.ZeroBased(), childtransform.Y.ZeroBased());
                    }
                }

                draggableControl.CaptureMouse();

                if (((DragGrip)draggableControl).IsToolBarItem)
                {
                    MainCanvas.Children.Add(((DragGrip)draggableControl).Clone());
                    ((DragGrip) draggableControl).IsToolBarItem = false;
                    ((DragGrip) draggableControl).IsDragable = true;
                }

            }

            var canvas = e.OriginalSource as Canvas;
            if (canvas != null)
            {
                clickPosition = e.GetPosition(this);
                isCanvasDragging = true;
                _rectangleDrawn = false;
                canvas.CaptureMouse();
            }
        }

        private Point _snapOffset = new Point {X = 5, Y = 15}; 
        private Point _canvasDragOffset = new Point {X = 3, Y = 3};
        private bool _rectangleDrawn;

        private void CanvasOnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && e.OriginalSource is DragGrip)
            {
                var draggableControl = Utils.GetParent<DragGrip>(e.OriginalSource as FrameworkElement) as DragGrip;
                if (draggableControl == null || !draggableControl.IsDragable)
                { return; }

                var currentPosition = e.GetPosition(Parent as FrameworkElement);
                var currentPosition2 = e.GetPosition(MainCanvas);
                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                //if(current selected && multi DragGrip selected)
                if (draggableControl.IsSelected && MainCanvas.Children.OfType<DragGrip>().Count(c => c.IsSelected) > 1)
                {
                    //foreach(selected items) { move them }
                    foreach (var child in MainCanvas.Children.OfType<DragGrip>().Where(w=>w.IsSelected))
                    {
                        var childtransform = child.RenderTransform as TranslateTransform;
                        childtransform.X = child.InitialPoint.X.ZeroBased() + currentPosition2.X.ZeroBased() - _multiInitialPoint.X.ZeroBased();
                        childtransform.Y = child.InitialPoint.Y.ZeroBased() + currentPosition2.Y.ZeroBased() - _multiInitialPoint.Y.ZeroBased();
                    }
                }
                else
                {
                    transform.X = (currentPosition.X - clickPosition.X) + initialPoint.X.ZeroBased();
                    transform.Y = (currentPosition.Y - clickPosition.Y) + initialPoint.Y.ZeroBased();
                }

                //if( current item moving is selected && many items selected )
                if (draggableControl.IsSelected && MainCanvas.Children.OfType<DragGrip>().Count(c => c.IsSelected) > 1)
                {
                    #region Snap Group
                    //i should know the graggable elements full rectangle see Fig.1
                    double x1, y1, x2, y2 = 0;
                    var seletedItems = MainCanvas.Children.OfType<DragGrip>().Where(w => w.IsSelected).ToList();
                    x1 = seletedItems.Min(s => ((TranslateTransform) s.RenderTransform).X);
                    y1 = seletedItems.Min(s => ((TranslateTransform) s.RenderTransform).Y);

                    x2 = seletedItems.Max(m => ((TranslateTransform) m.RenderTransform).X + m.ActualWidth);
                    y2 = seletedItems.Max(m => ((TranslateTransform) m.RenderTransform).Y + m.ActualHeight);

                    var notSelectedItems = MainCanvas.Children.OfType<DragGrip>().Where(w => !w.IsSelected && w.IsDragable).ToList();

                    //calculate (x1`,y1`) and (x2`,y2`)
                    double x1_ = x1, y1_ = y1, x2_ = x2, y2_ = y2;
                    foreach (var child in notSelectedItems)
                    {
                        //skip the dragable element
                        if (child.Equals(draggableControl))
                        {
                            continue;
                        }

                        if (!(child.RenderTransform is TranslateTransform))
                        {
                            continue;
                        }

                        var childX = ((TranslateTransform)child.RenderTransform).X;
                        var childY = ((TranslateTransform)child.RenderTransform).Y;
                        var childXBottom = childX + child.ActualWidth.ZeroBased();
                        var childYBottom = childY + child.ActualHeight.ZeroBased();

                        //Reversed Snapping
                        //1 is the moving part 
                        //2 is the stable part
                        //T2 <--> B1
                        if (y1.Between(Math.Abs(childYBottom - _snapOffset.Y), Math.Abs(childYBottom + _snapOffset.Y)))
                        {
                            y1_ = childYBottom;
                        }
                        //B2 <--> T1
                        if (y2.Between(Math.Abs(childY - _snapOffset.Y), Math.Abs(childY + _snapOffset.Y)))
                        {
                            y2_ = childY;
                        }
                        //L2 <--> R1
                        if (x1.Between(Math.Abs(childXBottom - _snapOffset.X), Math.Abs(childXBottom + _snapOffset.X)))
                        {
                            x1_ = childXBottom;
                        }
                        //R2 <--> L1
                        if (x2.Between(Math.Abs(childX - _snapOffset.X), Math.Abs(childX + _snapOffset.X)))
                        {
                            x2_ = childX;
                        }

                        //Similar Snapping
                        //L2 <--> L1
                        if (x1.Between(Math.Abs(childX - _snapOffset.X), Math.Abs(childX + _snapOffset.X)))
                        {
                            x1_ = childX;
                        }
                        ////B2 <--> B1
                        if (y1.Between(Math.Abs(childY - _snapOffset.Y), Math.Abs(childY + _snapOffset.Y)))
                        {
                            y1_ = childY;
                        }
                    }
                    
                    //change the selected items accourding to the diffrenece between (x1`-x1,y1`-y1) and (x2`-x2,y2`-y2)
                    foreach (var seletedItem in seletedItems)
                    {
                        var dy1 = y1_ - y1 ;
                        var dy2 = y2_ - y2;
                        var dx1 = x1_ - x1 ;
                        var dx2 = x2_ - x2;

                        var seletedItemTransform = seletedItem.RenderTransform as TranslateTransform;
                        if (seletedItemTransform != null && (dy1 != 0 || dy2 != 0) )
                        {
                            seletedItemTransform.Y += (dy1 == 0) ? dy2 : dy1;
                        }
                        if (seletedItemTransform != null && (dx1 != 0 || dx2 != 0))
                        {
                            seletedItemTransform.X += (dx1 == 0) ? dx2 : dx1;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Snapping

                    var x = transform.X;
                    var y = transform.Y;

                    var xbottom = transform.X + draggableControl.ActualWidth.ZeroBased();
                    var ybottom = transform.Y + draggableControl.ActualHeight.ZeroBased();

                    foreach (DragGrip child in MainCanvas.Children.OfType<DragGrip>().Where(w=>w.IsDragable))
                    {
                        //skip the dragable element
                        if (child.Equals(draggableControl))
                        {
                            continue;
                        }

                        if (!(child.RenderTransform is TranslateTransform))
                        {
                            continue;
                        }

                        var childX = ((TranslateTransform) child.RenderTransform).X;
                        var childY = ((TranslateTransform) child.RenderTransform).Y;
                        var childXBottom = childX + child.ActualWidth.ZeroBased();
                        var childYBottom = childY + child.ActualHeight.ZeroBased();

                        //Reversed Snapping
                        //T2 <--> B1
                        if (y.Between(Math.Abs(childYBottom - _snapOffset.Y), Math.Abs(childYBottom + _snapOffset.Y)))
                        {
                            transform.Y = childYBottom;
                        }
                        //B2 <--> T1
                        if (ybottom.Between(Math.Abs(childY - _snapOffset.Y), Math.Abs(childY + _snapOffset.Y)))
                        {
                            transform.Y = childY - draggableControl.ActualHeight.ZeroBased();
                        }
                        //L2 <--> R1
                        if (x.Between(Math.Abs(childXBottom - _snapOffset.X), Math.Abs(childXBottom + _snapOffset.X)))
                        {
                            transform.X = childXBottom;
                        }
                        //R2 <--> L1
                        if (xbottom.Between(Math.Abs(childX - _snapOffset.X), Math.Abs(childX + _snapOffset.X)))
                        {
                            transform.X = childX - draggableControl.ActualWidth.ZeroBased();
                        }

                        //Similar Snapping
                        //L2 <--> L1
                        if (x.Between(Math.Abs(childX - _snapOffset.X), Math.Abs(childX + _snapOffset.X)))
                        {
                            transform.X = childX;
                        }
                        //B2 <--> B1
                        if (y.Between(Math.Abs(childY - _snapOffset.Y), Math.Abs(childY + _snapOffset.Y)))
                        {
                            transform.Y = childY;
                        }
                    }

                    #endregion
                }
            }

            //if(dragging in empty space in the canvas) then {draw rectagle}
            if (isCanvasDragging && e.OriginalSource is Canvas)
            {
                //Multi select 
                var currentPosition = e.GetPosition(MainCanvas);
                var x1 = currentPosition.X;
                var y1 = currentPosition.Y;
                var x2 = clickPosition.X.ZeroBased();
                var y2 = clickPosition.Y.ZeroBased();
                Extentions.Order(ref x1, ref x2);
                Extentions.Order(ref y1, ref y2);

                if (x2 - x1 > _canvasDragOffset.X && y2 - y1 > _canvasDragOffset.Y)
                {
                    _rectangleDrawn = true;
                    SelectionRectangle.Visibility = Visibility.Visible;
                    Canvas.SetLeft(SelectionRectangle, (x1 < x2) ? x1 : x2);
                    Canvas.SetTop(SelectionRectangle, (y1 < y2) ? y1 : y2);
                    SelectionRectangle.Width = Math.Abs(x1 - x2);
                    SelectionRectangle.Height = Math.Abs(y1 - y2);

                    foreach (var child in MainCanvas.Children.OfType<DragGrip>())
                    {
                        var translate = ((TranslateTransform) child.RenderTransform);
                        var cx1 = translate.X;
                        var cy1 = translate.Y;
                        var cx2 = cx1 + child.ActualWidth;
                        var cy2 = cy1 + child.ActualHeight;

                        if (x1 < cx1 && x2 > cx2 && y1 < cy1 && y2 > cy2)
                        {
                            child.IsSelected = true;
                            Debug.WriteLine("####"+child.Name);
                        }
                    }
                }
            }
        }

        private void CanvasOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var draggable = e.OriginalSource as DragGrip;
            if (draggable != null)
            {
                isDragging = false;
                draggable.ReleaseMouseCapture();
            }

            if (isCanvasDragging && !_rectangleDrawn)
            {
                DeselectAll();
            }
            var canvas = e.OriginalSource as Canvas;
            if (canvas != null)
            {
                isCanvasDragging = false;
                canvas.ReleaseMouseCapture();
                SelectionRectangle.Visibility = Visibility.Collapsed;
            }
        }

        private void DeselectAll()
        {
            //deselect all
            foreach (var element in MainCanvas.Children.OfType<DragGrip>())
            {
                element.IsSelected = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }

        public void MenuItemOnDelete(object sender, RoutedEventArgs e)
        {
            var selected = MainCanvas.Children.OfType<DragGrip>().Where(w => w.IsSelected).ToList();
            if (selected.Count >= 1)
            {
                selected.ForEach(f => MainCanvas.Children.Remove(f));
            }
            else
            {
                MainCanvas.Children.Remove(((ContextMenu)((MenuItem)sender).Parent).PlacementTarget);
            }
        }

        public void MenuItemOnRename(object sender, RoutedEventArgs e)
        {
            //RenameDragGrip = (DragGrip)((ContextMenu)((MenuItem)sender).CommandParameter).PlacementTarget;
            RenameDragGrip = (DragGrip)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            TextBoxRename.Text = ((TextBlock) ((Border) RenameDragGrip.Child).Child).Text ;
            IsNameDialogVisible = true;
        }

        private void RenameOnClick(object sender, RoutedEventArgs e)
        {
            IsNameDialogVisible = false;
            ((TextBlock) ((Border) RenameDragGrip.Child).Child).Text = TextBoxRename.Text;
            if (TextBoxRename.Text.Count() >= "variable".Count())
            {
                ((Border) RenameDragGrip.Child).Width = MeasureString(TextBoxRename.Text).Width*1.5;
            }
        }

        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(TextBoxRename.FontFamily, TextBoxRename.FontStyle, TextBoxRename.FontWeight, TextBoxRename.FontStretch),
                TextBoxRename.FontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        private void TextBoxRenameOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RenameOnClick(null,null);
            }

            if (e.Key == Key.Escape)
            {
                IsNameDialogVisible = false;
            }
        }


        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var xx = Canvas.GetLeft(VerticalBarrier);

            var Ys = MainCanvas.Children.OfType<DragGrip>()
                .Where(w => ((TranslateTransform)w.RenderTransform).X > xx )
                .Select(s => ((TranslateTransform) s.RenderTransform).Y)
                .Distinct()
                .OrderBy(o=>o)
                .ToList();

            var Xs = MainCanvas.Children.OfType<DragGrip>()
                .Where(w => ((TranslateTransform)w.RenderTransform).X > xx)
                .Select(s => ((TranslateTransform)s.RenderTransform).X)
                .Distinct()
                .OrderBy(o => o)
                .ToList();

            var temp = MainCanvas.Children.OfType<DragGrip>()
                .Where(w => ((TranslateTransform)w.RenderTransform).X > xx)
                .Select(o => new
                {
                    o,
                    ((TranslateTransform) o.RenderTransform).X,
                    ((TranslateTransform) o.RenderTransform).Y
                })
                .ToList();

            var order = new List<DragGrip>();
            foreach (var y in Ys)
            {
                foreach (var x in Xs)
                {
                    var first = temp.FirstOrDefault(f => f.Y == y && f.X == x);
                    if (first != null)
                    {
                        order.Add(first.o);
                    }
                    else
                    {
                        Debug.Write("Some combination will not exsists and this is normal");
                    }
                }
            }

            var code = order.Select(s => ((TextBlock)((Border)s.Child).Child).Text).ToList().GetString();
            try
            {
                Result.Text = Compiler.Compile(code).ToString();
            }
            catch (Exception ex)
            {
                Result.Text = ex.Message;
            }
        }
    }

    public class Utils
    {
        public Rectangle Clone(Rectangle rectangle)
        {
            return new Rectangle
            {
                Width = rectangle.Width,
                Height = rectangle.Height,
                Fill = rectangle.Fill
            };
        }

        public FrameworkElement GetParent<T>(FrameworkElement current)
        {
            if (current == null)
                return null;
            if (current is T)
                return current;
            return GetParent<T>((FrameworkElement)current.Parent);
        }
    }

    public static class Extentions
    {
        public static bool Between(this Point point, Point topLeft, Point bottomRight)
        {
            if (topLeft.X.ZeroBased() <= point.X.ZeroBased() && bottomRight.X.ZeroBased() >= point.X.ZeroBased() &&
                topLeft.Y.ZeroBased() <= point.Y.ZeroBased() && bottomRight.Y.ZeroBased() >= point.Y.ZeroBased())
                return true;
            return false;
        }
        public static double ZeroBased(this double d)
        {
            return (double.IsNaN(d) ? 0 : d);
        }

        public static bool Between(this double current, double a, double b)
        {
            if ((current >= a && current <= b) || (current <= a && current >= b))
            {
                return true;
            }
            return false;
        }

        public static void Order(ref double x, ref double y)
        {
            if (x < y)
                return;

            var temp = x;
            x = y;
            y = temp;
        }

        public static string GetString(this List<string> list)
        {
            var str = "";
            list.ForEach(f=> str+=f +" ");
            return str;
        }
    }
}