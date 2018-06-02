/**************************************************************************
 * 类名：G_Point.cs
 * 描述：GMap地图图元
 * 作者：Allen
 * 日期：July 1,2016
 * 
 * ************************************************************************/
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using MapFrame.Core.Interface;
using System;
using System.Drawing;

namespace MapFrame.GMap
{
    /// <summary>
    /// GMap地图图元
    /// </summary>
    class G_Point : IElement
    {
      
        //private LngLat _lngLat;
        //public LngLat LngLat
        //{
        //    get { return _lngLat; }
        //    set { _lngLat = value; }
        //}

        //private int _index;
        //public int Index
        //{
        //    get { return _index; }
        //    set { _index = value; }
        //}

        //private string _name;
        //public string Name
        //{
        //    get { return _name; }
        //    set { _name = value; }
        //}

        //private Color _elementColor;
        //public Color ElementColor
        //{
        //    get { return _elementColor; }
        //    set { _elementColor = value; }
        //}

        //private ElementTypeEnum _elementType;
        //public ElementTypeEnum ElementType
        //{
        //    get { return _elementType; }
        //    set { _elementType = value; }
        //}

        //private GMarkerGoogle marker = null;
        //public GMarkerGoogle Marker
        //{
        //    get { return marker; }
        //    set { marker = value; }
        //}

        //private bool _visible;
        //public bool IsVisible
        //{
        //    get { return _visible; }
        //    set { _visible = value; }
        //}


        //public G_Point(string name, LngLat lngLat, int index)
        //{
        //    _name = name;
        //    _index = index;
        //    _lngLat = lngLat;

        //    PointLatLng p = new PointLatLng() { Lng = _lngLat.Lng, Lat = _lngLat.Lat };
        //    marker = new GMarkerGoogle(p, GMarkerGoogleType.green);
        //    marker.IsVisible = true;
        //}

        //public void HightLight(bool isHightLight)
        //{
        //    throw new NotImplementedException();
        //}

        //public void Show(bool isVisible)
        //{
        //    marker.IsVisible = isVisible;
        //}

        //public void Draw()
        //{

        //}


        #region MyRegion
        
        //private float ang;
        //public float Ang
        //{
        //    get { return ang; }
        //    set { ang = value; }
        //}

        //private Image image;
        //public Image Image
        //{
        //    get
        //    {
        //        return image;
        //    }
        //    set
        //    {
        //        image = value;
        //        if (image != null)
        //        {
        //            this.Size = new Size(image.Width, image.Height);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 是否高亮
        ///// </summary>
        //public bool IsHighlight = true;
        ///// <summary>
        ///// 高亮颜色
        ///// </summary>
        //public Pen HighlightPen { set; get; }
        //public Pen FlashPen { set; get; }
        //private Timer flashTimer = new Timer();
        //private int radius;
        //private int flashRadius;
        //private bool bIsFlash = false;

        //public G_Point(PointLatLng p, Image image, float angle)
        //    : base(p)
        //{
        //    ang = angle;
        //    Image = image;
        //    Size = new System.Drawing.Size(image.Width, image.Height);
        //    Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);

        //    HighlightPen = new System.Drawing.Pen(Brushes.Red, 2);
        //    radius = Size.Width >= Size.Height ? Size.Width : Size.Height;
        //    flashTimer.Interval = 100;
        //    flashTimer.Tick += new EventHandler(flashTimer_Tick);
        //}

        ///// <summary>
        ///// 开始闪烁
        ///// </summary>
        //public void StartFlash()
        //{
        //    if (bIsFlash == false)
        //    {
        //        flashTimer.Start();
        //        bIsFlash = true;
        //    }
        //}

        ///// <summary>
        ///// 停止闪烁
        ///// </summary>
        //public void StopFlash()
        //{
        //    if (bIsFlash == true)
        //    {
        //        flashTimer.Stop();
        //        if (FlashPen != null)
        //        {
        //            FlashPen.Dispose();
        //            FlashPen = null;
        //        }

        //        if (this.Overlay.Control.InvokeRequired)
        //        {
        //            this.Overlay.Control.Invoke(new Action(delegate
        //            {
        //                this.Overlay.Control.Refresh();
        //            }));
        //        }
        //        else
        //            this.Overlay.Control.Refresh();

        //        bIsFlash = false;
        //    }
        //}

        //// 定时器
        //void flashTimer_Tick(object sender, EventArgs e)
        //{
        //    if (FlashPen == null)
        //    {
        //        FlashPen = new Pen(Brushes.Red, 3);
        //        flashRadius = radius;
        //    }
        //    else
        //    {
        //        flashRadius += radius / 4;
        //        if (flashRadius >= 2 * radius)
        //        {
        //            flashRadius = radius;
        //            FlashPen.Color = Color.FromArgb(255, Color.Red);
        //        }
        //        else
        //        {
        //            Random rand = new Random();
        //            int alpha = rand.Next(255);
        //            FlashPen.Color = Color.FromArgb(alpha, Color.Red);
        //        }
        //    }

        //    if (this.Overlay.Control.InvokeRequired)
        //    {
        //        this.Overlay.Control.Invoke(new Action(delegate {
        //            this.Overlay.Control.Refresh();
        //        }));
        //    }
        //    else
        //        this.Overlay.Control.Refresh();
        //}

        ///// <summary>
        ///// 重绘
        ///// </summary>
        ///// <param name="g"></param>
        //public override void OnRender(Graphics g)
        //{
        //    if (image == null)
        //        return;

        //    g.DrawImageUnscaled(RotateImage(Image, ang), LocalPosition.X, LocalPosition.Y);

        //    Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
        //    //g.DrawImage(image, rect);

        //    //if (IsMouseOver && IsHighlight)   // 红框目标
        //    //{
        //    //    g.DrawRectangle(HighlightPen, rect);
        //    //}

        //    if (FlashPen != null)
        //    {
        //        g.DrawEllipse(FlashPen,
        //            new Rectangle(LocalPosition.X - flashRadius / 2 + Size.Width / 2, LocalPosition.Y - flashRadius / 2 + Size.Height / 2, flashRadius, flashRadius));
        //    }
        //}
     
        ///// <summary>
        ///// 旋转图像
        ///// </summary>
        ///// <param name="image">图像</param>
        ///// <param name="angle">角度</param>
        ///// <returns></returns>
        //private static Bitmap RotateImage(Image image, float angle)
        //{
        //    if (image == null)
        //        throw new ArgumentNullException("image");

        //    const double pi2 = Math.PI / 2.0;

        //    // Why can't C# allow these to be const, or at least readonly
        //    // *sigh*  I'm starting to talk like Christian Graus :omg:
        //    double oldWidth = (double)image.Width;
        //    double oldHeight = (double)image.Height;

        //    // Convert degrees to radians
        //    double theta = ((double)angle) * Math.PI / 180.0;
        //    double locked_theta = theta;

        //    // Ensure theta is now [0, 2pi)
        //    while (locked_theta < 0.0)
        //        locked_theta += 2 * Math.PI;

        //    double newWidth, newHeight;
        //    int nWidth, nHeight; // The newWidth/newHeight expressed as ints

        //    #region Explaination of the calculations
        //    /*
        //     * The trig involved in calculating the new width and height
        //     * is fairly simple; the hard part was remembering that when 
        //     * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
        //     * height are switched.
        //     * 
        //     * When you rotate a rectangle, r, the bounding box surrounding r
        //     * contains for right-triangles of empty space.  Each of the 
        //     * triangles hypotenuse's are a known length, either the width or
        //     * the height of r.  Because we know the length of the hypotenuse
        //     * and we have a known angle of rotation, we can use the trig
        //     * function identities to find the length of the other two sides.
        //     * 
        //     * sine = opposite/hypotenuse
        //     * cosine = adjacent/hypotenuse
        //     * 
        //     * solving for the unknown we get
        //     * 
        //     * opposite = sine * hypotenuse
        //     * adjacent = cosine * hypotenuse
        //     * 
        //     * Another interesting point about these triangles is that there
        //     * are only two different triangles. The proof for which is easy
        //     * to see, but its been too long since I've written a proof that
        //     * I can't explain it well enough to want to publish it.  
        //     * 
        //     * Just trust me when I say the triangles formed by the lengths 
        //     * width are always the same (for a given theta) and the same 
        //     * goes for the height of r.
        //     * 
        //     * Rather than associate the opposite/adjacent sides with the
        //     * width and height of the original bitmap, I'll associate them
        //     * based on their position.
        //     * 
        //     * adjacent/oppositeTop will refer to the triangles making up the 
        //     * upper right and lower left corners
        //     * 
        //     * adjacent/oppositeBottom will refer to the triangles making up 
        //     * the upper left and lower right corners
        //     * 
        //     * The names are based on the right side corners, because thats 
        //     * where I did my work on paper (the right side).
        //     * 
        //     * Now if you draw this out, you will see that the width of the 
        //     * bounding box is calculated by adding together adjacentTop and 
        //     * oppositeBottom while the height is calculate by adding 
        //     * together adjacentBottom and oppositeTop.
        //     */
        //    #endregion

        //    double adjacentTop, oppositeTop;
        //    double adjacentBottom, oppositeBottom;

        //    // We need to calculate the sides of the triangles based
        //    // on how much rotation is being done to the bitmap.
        //    //   Refer to the first paragraph in the explaination above for 
        //    //   reasons why.
        //    if ((locked_theta >= 0.0 && locked_theta < pi2) ||
        //        (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
        //    {
        //        adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
        //        oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

        //        adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
        //        oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
        //    }
        //    else
        //    {
        //        adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
        //        oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

        //        adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
        //        oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
        //    }

        //    newWidth = adjacentTop + oppositeBottom;
        //    newHeight = adjacentBottom + oppositeTop;

        //    nWidth = (int)Math.Ceiling(newWidth);
        //    nHeight = (int)Math.Ceiling(newHeight);

        //    Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

        //    using (Graphics g = Graphics.FromImage(rotatedBmp))
        //    {
        //        // This array will be used to pass in the three points that 
        //        // make up the rotated image
        //        Point[] points;

        //        /*
        //         * The values of opposite/adjacentTop/Bottom are referring to 
        //         * fixed locations instead of in relation to the
        //         * rotating image so I need to change which values are used
        //         * based on the how much the image is rotating.
        //         * 
        //         * For each point, one of the coordinates will always be 0, 
        //         * nWidth, or nHeight.  This because the Bitmap we are drawing on
        //         * is the bounding box for the rotated bitmap.  If both of the 
        //         * corrdinates for any of the given points wasn't in the set above
        //         * then the bitmap we are drawing on WOULDN'T be the bounding box
        //         * as required.
        //         */
        //        if (locked_theta >= 0.0 && locked_theta < pi2)
        //        {
        //            points = new Point[] {
        //                                     new Point( (int) oppositeBottom, 0 ),
        //                                     new Point( nWidth, (int) oppositeTop ),
        //                                     new Point( 0, (int) adjacentBottom )
        //                                 };

        //        }
        //        else if (locked_theta >= pi2 && locked_theta < Math.PI)
        //        {
        //            points = new Point[] {
        //                                     new Point( nWidth, (int) oppositeTop ),
        //                                     new Point( (int) adjacentTop, nHeight ),
        //                                     new Point( (int) oppositeBottom, 0 )
        //                                 };
        //        }
        //        else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
        //        {
        //            points = new Point[] {
        //                                     new Point( (int) adjacentTop, nHeight ),
        //                                     new Point( 0, (int) adjacentBottom ),
        //                                     new Point( nWidth, (int) oppositeTop )
        //                                 };
        //        }
        //        else
        //        {
        //            points = new Point[] {
        //                                     new Point( 0, (int) adjacentBottom ),
        //                                     new Point( (int) oppositeBottom, 0 ),
        //                                     new Point( (int) adjacentTop, nHeight )
        //                                 };
        //        }

        //        g.DrawImage(image, points);
        //    }

        //    return rotatedBmp;
        //}

        ///// <summary>
        ///// 释放资源
        ///// </summary>
        //public override void Dispose()
        //{
        //    if (HighlightPen != null)
        //    {
        //        HighlightPen.Dispose();
        //        HighlightPen = null;
        //    }

        //    if (FlashPen != null)
        //    {
        //        FlashPen.Dispose();
        //        FlashPen = null;
        //    }

        //    base.Dispose();
        //}

        #endregion

    }
}
