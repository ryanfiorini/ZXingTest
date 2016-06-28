using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using ZXing.Mobile;
using Android.Graphics;
using ZXing;
using System.IO;
using ZXing.Common;
using System.Collections.Generic;
using ZXing.Datamatrix;
using Java.IO;

namespace ZXingTest.Droid
{
	[Activity (Label = "ZXingTest.Droid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;
        ImageView _imageView;
        
        // This is a test QR!

        MobileBarcodeScanner scanner;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Somewhere in your app, call the initialization code:
            MobileBarcodeScanner.Initialize(Application);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Create a new instance of our Scanner
            scanner = new MobileBarcodeScanner();

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.myButton);

            button.Click += delegate
            {
                _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
                //BitmapFactory.Options options = new BitmapFactory.Options
                //{
                //    InJustDecodeBounds = false
                //};

                //// The result will be null because InJustDecodeBounds == true.
                //Bitmap bitmapToDisplay = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.download1);
                //_imageView.SetImageBitmap(bitmapToDisplay);
                _imageView.SetImageBitmap(
                    decodeSampledBitmapFromResource(Resources, Resource.Drawable.qrcode, 375, 375));
            };


            Button buttonScan = FindViewById<Button>(Resource.Id.buttonScan);

            //buttonScan.Click += async delegate {
            //var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            //var result = await scanner.Scan();

            //if (result != null)
            //    Console.WriteLine("Scanned Barcode: " + result.Text);


            //};

            buttonScan.Click += delegate
            {
                //var fullName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "download.png");

                String path = System.Environment.CurrentDirectory;

                BitmapFactory.Options options = new BitmapFactory.Options
                {
                    InPreferredConfig = Bitmap.Config.Argb8888
                };

                //BitmapFactory.Options options = new BitmapFactory.Options
                //{
                //    InJustDecodeBounds = true
                //};



                // The result will be null because InJustDecodeBounds == true.
                //Bitmap bitmap = await BitmapFactory.DecodeResourceAsync(Resources, Resource.Drawable.download1, options);
                Bitmap bitmap = decodeSampledBitmapFromResource(Resources, Resource.Drawable.qrcode, 375, 375);


               // var imageHeight = options.OutHeight; 
                //var imageWidth = options.OutWidth;  
                var imageHeight = (int)bitmap.GetBitmapInfo().Height;
                var imageWidth = (int)bitmap.GetBitmapInfo().Width;

                //using (var stream = new MemoryStream())
                //{
                //    bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                //    bitmapData = stream.ToArray();
                //}

                int size = (int)bitmap.RowBytes * (int)bitmap.GetBitmapInfo().Height;
                Java.Nio.ByteBuffer byteBuffer = Java.Nio.ByteBuffer.Allocate(size);
                bitmap.CopyPixelsToBuffer(byteBuffer);
                byte[] byteArray = new byte[byteBuffer.Remaining()];
                var imageBytes = byteBuffer.Get(byteArray);

                LuminanceSource source = new ZXing.RGBLuminanceSource(byteArray, (int)imageWidth, (int)imageHeight);

                var barcodeReader = new BarcodeReader();
                barcodeReader.Options.TryHarder = true;
                //barcodeReader.Options.ReturnCodabarStartEnd = true;
                //barcodeReader.Options.PureBarcode = true;
                //barcodeReader.Options.PossibleFormats = new List<BarcodeFormat>();
                //barcodeReader.Options.PossibleFormats.Add(BarcodeFormat.QR_CODE);
                ZXing.Result result = barcodeReader.Decode(source);
                var result2 = barcodeReader.DecodeMultiple(source);

                ZXing.Result result3 = barcodeReader.Decode(byteArray, (int)imageWidth, (int)imageHeight, RGBLuminanceSource.BitmapFormat.RGB24);
                ZXing.Result result4 = barcodeReader.Decode(source);

                HybridBinarizer hb = new HybridBinarizer(source);
                var a = hb.createBinarizer(source);
                BinaryBitmap bBitmap = new BinaryBitmap(a);
                MultiFormatReader reader1 = new MultiFormatReader();
                var r = reader1.decodeWithState(bBitmap);


                int[] intarray = new int[((int)imageHeight * (int)imageWidth)];
                bitmap.GetPixels(intarray, 0, (int)bitmap.GetBitmapInfo().Width, 0, 0, (int)imageWidth, (int)imageHeight);
                LuminanceSource source5 = new RGBLuminanceSource(byteArray, (int)imageWidth, (int)imageHeight);

                BinaryBitmap bitmap3 = new BinaryBitmap(new HybridBinarizer(source));
                ZXing.Reader reader = new DataMatrixReader();
                //....doing the actually reading
                ZXing.Result result10 = reader.decode(bitmap3);



                //InputStream is = this.Resources.OpenRawResource(Resource.Id.imageView1);
                //Bitmap originalBitmap = BitmapFactory.decodeStream(is);

                //UIImage

                //var barcodeBitmap = (Bitmap)Bitmap.FromFile(@"C:\Users\jeremy\Desktop\qrimage.bmp");
            };


            Button buttonScan2 = FindViewById<Button>(Resource.Id.buttonScan2);
            buttonScan2.Click += async delegate
            {
                //Tell our scanner to activiyt use the default overlay
                scanner.UseCustomOverlay = false;

                //We can customize the top and bottom text of the default overlay
                scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
                scanner.BottomText = "Wait for the barcode to automatically scan!";

                //Start scanning
                var result = await scanner.Scan();

                // Handler for the result returned by the scanner.
                HandleScanResult(result);
            };
        }

        void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                msg = "Found Barcode: " + result.Text;
            else
                msg = "Scanning Canceled!";

        }

        public Bitmap decodeSampledBitmapFromResource(Android.Content.Res.Resources res, int resId,
        int reqWidth, int reqHeight)
        {

            // First decode with inJustDecodeBounds=true to check dimensions
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeResource(res, resId, options);

            // Calculate inSampleSize
            options.InSampleSize = calculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeResource(res, resId, options);
        }

        public static int calculateInSampleSize(
            BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > reqHeight
                        && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        //public static String readQRImage(Bitmap bMap)
        //{
        //    String contents = null;

        //    int[] intArray = new int[bMap.GetBitmapInfo().Width * bMap.GetBitmapInfo().Height];
        //    //copy pixel data from the Bitmap into the 'intArray' array  
        //    bMap.getPixels(intArray, 0, bMap.GetBitmapInfo().Width, 0, 0, bMap.GetBitmapInfo().Width, bMap.GetBitmapInfo().Height);

        //    LuminanceSource source = new RGBLuminanceSource(bMap.GetBitmapInfo().Width, bMap.GetBitmapInfo().Height, intArray);
        //    BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));

        //    Reader reader = new MultiFormatReader();// use this otherwise ChecksumException
        //    try
        //    {
        //        Result result = reader.decode(bitmap);
        //        contents = result.getText();
        //        //byte[] rawBytes = result.getRawBytes(); 
        //        //BarcodeFormat format = result.getBarcodeFormat(); 
        //        //ResultPoint[] points = result.getResultPoints();
        //    }
        //    catch (NotFoundException e) { e.printStackTrace(); }
        //    catch (ChecksumException e) { e.printStackTrace(); }
        //    catch (FormatException e) { e.printStackTrace(); }
        //    return contents;
        //}
    }
}


