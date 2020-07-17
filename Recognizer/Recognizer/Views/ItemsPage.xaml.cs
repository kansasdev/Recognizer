using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Recognizer.Models;
using Recognizer.Views;
using Recognizer.ViewModels;
using SkiaSharp;
using TouchTracking;
using SkiaSharp.Views.Forms;
using Recognizer.Services;
using System.Threading;
using Windows.UI.Xaml.Navigation;
using Recognizer.OCR;
using Newtonsoft.Json.Converters;

namespace Recognizer.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        List<SKPoint> completedPoints = new List<SKPoint>();

        private SKBitmap saveBitmap;

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };


        void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        UpdateBitmap();
                    }
                    break;
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Create bitmap the size of the display surface
            if (saveBitmap == null)
            {
                saveBitmap = new SKBitmap(info.Width, info.Height);
            }
            // Or create new bitmap for a new size of display surface
            else if (saveBitmap.Width < info.Width || saveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(Math.Max(saveBitmap.Width, info.Width),
                                                  Math.Max(saveBitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                saveBitmap = newBitmap;
            }

            

            // Render the bitmap
            canvas.Clear();
            canvas.DrawBitmap(saveBitmap, 0, 0);
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        void UpdateBitmap()
        {
            using (SKCanvas saveBitmapCanvas = new SKCanvas(saveBitmap))
            {
                saveBitmapCanvas.Clear();

                foreach (SKPath path in completedPaths)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                    
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }
            }
            
            canvasView.InvalidateSurface();
        }

        private void btnClear_Clicked(object sender, EventArgs e)
        {
            inProgressPaths.Clear();
            completedPaths.Clear();
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        private async void btnSend_Clicked(object sender, EventArgs e)
        {

            try
            {
                await SetIndicator(true);
                
                using (SKImage image = SKImage.FromBitmap(saveBitmap))
                {
                    SKData data = image.Encode();
                    DateTime dt = DateTime.Now;
                    int year = dt.Year;
                    int month = dt.Month;
                    int day = dt.Day;
                    int hour = dt.Hour;
                    int minute = dt.Minute;
                    int second = dt.Second;
                    int milisecond = dt.Millisecond;
                    string filename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                    year, month, day, hour, minute, second, milisecond);

                    string jsonfilename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.json",
                                                    year, month, day, hour, minute, second, milisecond);


                    int counter = 1;
                    string jsonStrokes = string.Empty;
                    jsonStrokes = "{" +
                        "\"version\": 1, " +
                        "\"language\": \"en-US\", " +
                        "\"unit\": \"mm\", " +
                        "\"strokes\": [";
                    //MOJE
                    foreach (SKPath p in completedPaths)
                    {
                        string points = string.Empty;
                        foreach (SKPoint pnt in p.Points)
                        {
                            points += pnt.X + "," + pnt.Y + ",";
                        }
                        points = points.Remove(points.Length - 1);
                        jsonStrokes += "{" +
                                        "\"id\": " + counter + "," +
                                        "\"points\": \"" +
                                        points +
                                        "\"},";
                        counter++;
                    }
                    jsonStrokes = jsonStrokes.Remove(jsonStrokes.Length - 1);
                    jsonStrokes += "]}";

                    IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                    if (photoLibrary == null)
                    {
                        await DisplayActionSheet("Couldn't save your image", "Cancel", "OK", new string[] { });
                    }
                    else
                    {


                        bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "FingerPaint", filename);

                        if (!result)
                        {
                            await DisplayActionSheet("Artwork could not be saved. Sorry!", "Cancel", "OK", new string[] { });
                        }

                        bool jsonResult = await photoLibrary.SaveJsonAsync(jsonStrokes, "FingerPaint", jsonfilename);
                        if (!jsonResult)
                        {
                            await DisplayActionSheet("Couldn't save strokes to file. Sorry!", "Cancel", "OK", new string[] { });
                        }

                        //create request
                        IAzureRecognition recognition = DependencyService.Get<IAzureRecognition>();
                        if (recognition != null)
                        {
                            string[] resultArr = recognition.GetOCR(jsonStrokes);

                            if (resultArr != null && resultArr.Length >= 1)
                            {

                                try
                                {
                                    string promptResult = await DisplayActionSheet("Text recognized, choose best one", "Finish", "Say it", resultArr);
                                    if (!string.IsNullOrEmpty(promptResult) && (promptResult != "Finish" || promptResult != "Say it"))
                                    {
                                        if (promptResult == "Say it")
                                        {
                                            string txtToSay = string.Empty;
                                            foreach(string t in resultArr)
                                            {
                                                txtToSay = txtToSay + t;
                                            }
                                            await recognition.SayIT(txtToSay);
                                        }
                                        else
                                        {
                                            inProgressPaths.Clear();
                                            completedPaths.Clear();
                                            UpdateBitmap();
                                            canvasView.InvalidateSurface();
                                        }
                                    }
                                    else
                                    {
                                        inProgressPaths.Clear();
                                        completedPaths.Clear();
                                        UpdateBitmap();
                                        canvasView.InvalidateSurface();
                                    }


                                }
                                catch (Exception exx)
                                {
                                    await DisplayActionSheet("Error parsing response from OCR/speech engine: " + exx.Message, "Cancel", "OK", new string[] { });
                                    inProgressPaths.Clear();
                                    completedPaths.Clear();
                                    UpdateBitmap();
                                    canvasView.InvalidateSurface();
                                }

                            }
                            else
                            {
                                await DisplayActionSheet("Paint doesn't contain any text", "Cancel", "OK", new string[] { });
                                inProgressPaths.Clear();
                                completedPaths.Clear();
                                UpdateBitmap();
                                canvasView.InvalidateSurface();

                            }
                        }
                        else
                        {
                            await DisplayActionSheet("Can't get IAzureRecognition implementation","Cancel", "OK",new string[] { });
                        }
                    }
                }
                await SetIndicator(false);
            }
            catch(Exception ex)
            {
                await SetIndicator(false);
                await DisplayActionSheet("Exception during execution: " + ex.Message, "Cancel", "OK", new string[] { });
            }
        }
        
        private async Task SetIndicator(bool enabled)
        {
            waitIndicator.IsRunning = enabled;
            canvasView.IsEnabled = !enabled;
            btnClear.IsEnabled = !enabled;
            btnSend.IsEnabled = !enabled;

            await Task.CompletedTask;
        }
    }
}