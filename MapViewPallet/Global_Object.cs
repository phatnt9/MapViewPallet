using MapViewPallet.Shape;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace MapViewPallet
{
    public static class Global_Object
    {
        public enum RequestMethod
        {
            GET,
            POST,
            DELETE
        }

        public static readonly object syncLock = new object();

        //#######################################
        public static StationShape bufferToMove;

        //public static string url = @"http://"+ Properties.Settings.Default.serverIp + ":"+ Properties.Settings.Default.serverPort + @"/robot/rest/";
        //public static string url = @"http://"+ "192.168.1.32" + ":"+ Properties.Settings.Default.serverPort + @"/robot/rest/";
        public static string hostUrl = "localhost";

        public static string hostPort = "8081";

        public static int userLogin = -2;
        public static string userName = "";
        public static int userAuthor = -2;

        //public static double StaticPalletWidth = 13;
        //public static double StaticPalletHeight = 12;
        //public static double StaticPalletMargin = 0;
        //public static double StaticPalletPadding = 0;

        public static string messageDuplicated = "{0} is duplicated.";
        public static string messageSaveSucced = "Save operation succeeded.";
        public static string messageSaveFail = "Failed to save. Please try again.";
        public static string messageValidate = "{0} is mandatory. Please enter {1}.";
        public static string messageNothingSelected = "Nothing selected.";
        public static string messageDeleteConfirm = "Do you want to delete the selected {0}?";
        public static string messageDeleteSucced = "Delete operation succeeded.";
        public static string messageDeleteFail = "Failed to delete. Please try again.";
        public static string messageDeleteUse = "Can\'t delete {0} because it has been using on {1}.";
        public static string messageValidateNumber = "{0} must be {1} than {2}.";
        public static string messageNoDataSave = "There is no updated data to save.";

        public static string messageTitileInformation = "Information";
        public static string messageTitileError = "Error";
        public static string messageTitileWarning = "Warning";

        //#######################################
        public static string RequestDataAPI(string jsonData, string apiUrl, RequestMethod method)
        {
            string resultData = "";
            try
            {
                switch (method)
                {
                    case RequestMethod.GET:
                    {
                        HttpWebRequest request =
                            (HttpWebRequest)WebRequest.Create(@"http://" +
                            MapViewPallet.Properties.Settings.Default.serverIp + ":" +
                            MapViewPallet.Properties.Settings.Default.serverPort +
                            @"/robot/rest/" + apiUrl);
                        request.Method = method.ToString();
                        request.ContentType = @"application/json";
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            resultData = reader.ReadToEnd();
                        }
                        break;
                    }
                    case RequestMethod.POST:
                    {
                        goto default;
                    }
                    case RequestMethod.DELETE:
                    {
                        goto default;
                    }
                    default:
                    {
                        HttpWebRequest request =
                            (HttpWebRequest)WebRequest.Create(@"http://" +
                            MapViewPallet.Properties.Settings.Default.serverIp + ":" +
                            MapViewPallet.Properties.Settings.Default.serverPort +
                            @"/robot/rest/" + apiUrl);
                        request.Method = method.ToString();
                        request.ContentType = @"application/json";

                        System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                        Byte[] byteArray = encoding.GetBytes(jsonData);
                        request.ContentLength = byteArray.Length;
                        using (Stream dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            dataStream.Flush();
                        }
                        HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                            resultData = reader.ReadToEnd();
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resultData;
        }

        //#######################################
        //public static MusicPlayerOld musicPlayerOld = new MusicPlayerOld("ALARM.mp3");

        //public static void PlayWarning(bool isLoop)
        //{
        //    if (musicPlayerOld == null)
        //    {
        //        musicPlayerOld = new MusicPlayerOld("ALARM.mp3");
        //        musicPlayerOld.Play(true);
        //    }
        //    else
        //    {
        //        if (musicPlayerOld.IsBeingPlayed)
        //        {
        //            return;
        //        }
        //        //if (Global_Object.musicPlayerOld.IsBeingPlayed)
        //        //{
        //        //    Global_Object.musicPlayerOld.StopPlaying();
        //        //}
        //        else
        //        {
        //            musicPlayerOld.Play(true);
        //        }
        //    }
        //}

        //public static void StopWarning()
        //{
        //    if (musicPlayerOld != null)
        //    {
        //        if (musicPlayerOld.IsBeingPlayed)
        //        {
        //            musicPlayerOld.StopPlaying();
        //        }
        //    }
        //}

        //#######################################
        //public static bool ServerAlive()
        //{
        //    var url = "http://localhost:8081/robot/rest/timeWork/getListTimeWork";
        //    try
        //    {
        //        var myRequest = (HttpWebRequest)WebRequest.Create(url);

        //        var response = (HttpWebResponse)myRequest.GetResponse();

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            //  it's at least in some way responsive
        //            //  but may be internally broken
        //            //  as you could find out if you called one of the methods for real
        //            Console.WriteLine(string.Format("{0} Available", url));
        //            return true;
        //        }
        //        else
        //        {
        //            //  well, at least it returned...
        //            Console.WriteLine(string.Format("{0} Returned, but with status: {1}", url, response.StatusDescription));
        //            return true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //  not available at all, for some reason
        //        Console.WriteLine(string.Format("{0} unavailable: {1}", url, ex.Message));
        //        return false;
        //    }
        //}
        public static bool ServerAlive()
        {
            //=====================================================
            //var client = new TcpClient();
            //try
            //{
            //    var result = client.BeginConnect(Properties.Settings.Default.serverIp, int.Parse(Properties.Settings.Default.serverPort), null, null);

            //    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            //    if (!success)
            //    {
            //        return false;
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            //=====================================================
            //var client = new TcpClient();
            //if (!client.ConnectAsync(Properties.Settings.Default.serverIp, int.Parse(Properties.Settings.Default.serverPort)).Wait(1000))
            //{
            //    return false;
            //}
            //return true;
            //=====================================================
            //TcpClient tcpClient = new TcpClient();
            //try
            //{
            //    tcpClient.Connect("localhost", 8081);
            //    return true;
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(Properties.Settings.Default.serverIp, int.Parse(Properties.Settings.Default.serverPort));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            //=====================================================
            //try
            //{
            //    using (var client = new TcpClient(hostUri, portNumber))
            //    {
            //        return true;
            //    }
            //}
            //catch (SocketException ex)
            //{
            //    return false;
            //}
            //=====================================================
        }

        public static Point LaserOriginalCoor = new Point(648, 378);
        public static Point OriginPoint = new Point(0, 0);

        public static Point CoorLaser(Point canvas)
        {
            Point laser = new Point();
            laser.X = (Math.Cos(0) * (canvas.X - OriginPoint.X)) * resolution;
            laser.Y = (Math.Cos(Math.PI) * (canvas.Y - OriginPoint.Y)) * resolution;
            return laser;
        }

        public static Point CoorCanvas(Point laser)
        {
            Point canvas = new Point();
            canvas.X = (laser.X / (resolution * Math.Cos(0))) + OriginPoint.X;
            canvas.Y = (laser.Y / (resolution * Math.Cos(Math.PI))) + OriginPoint.Y;
            return canvas;
        }

        public static DataTable LoadExcelFile()
        {
            DataTable data = new DataTable();
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel files (*.xls)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 4;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    Console.WriteLine(filePath);
                    //Read the contents of the file into a stream
                    //var fileStream = openFileDialog.OpenFile();
                    //using (StreamReader reader = new StreamReader(fileStream))
                    //{
                    //    string fileContent = reader.ReadToEnd();
                    //    Console.WriteLine(fileContent);
                    //}
                    string name = "Sheet1";
                    string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                                    filePath +
                                    ";Extended Properties='Excel 12.0 XML;HDR=YES;';";

                    OleDbConnection con = new OleDbConnection(constr);
                    OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
                    con.Open();
                    OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
                    sda.Fill(data);
                }
                return data;
            }
        }

        public static DataTable LoadExcelFile(string path)
        {
            DataTable data = new DataTable();
            string name = "Sheet1";
            string constr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            path +
                            ";Extended Properties='Excel 12.0 XmL;HDR=YES;';";

            OleDbConnection con = new OleDbConnection(constr);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + name + "$]", con);
            con.Open();
            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            sda.Fill(data);
            return data;
        }

        public static double resolution = 0.1; // Square meters per pixel

        public static string Foo<T>(T parameter)
        {
            return typeof(T).Name;
        }

        public static double LengthBetweenPoints(Point pt1, Point pt2)
        {
            //Calculate the distance between the both points
            //for both axes separately.
            double dblDistX = Math.Abs(pt1.X - pt2.X);
            double dblDistY = Math.Abs(pt1.Y - pt2.Y);

            //Calculate the length of a line traveling from pt1 to pt2
            //(according to Pythagoras).
            double dblHypotenuseLength = Math.Sqrt(
               dblDistX * dblDistX
               +
               dblDistY * dblDistY
             );

            return dblHypotenuseLength;
        }
    }
}