using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Linq;
using System.Net;
using System.Net.Sockets;
//using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//something needed for mouse controll
using System.Drawing;//something needed for mouse controll
using System.Windows.Forms;//something needed for mouse controll

namespace MouseControllerTCPServer
{
    class CursorServer
    {
        //import SetCursorPos method to use for later
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int x, int y);
        //mouse click
        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwextraInfo);
        private const int MOUSEEVENT_LEFTDOWN = 0x02;
        private const int MOUSEEVENT_LEFTUP = 0x04;
        //
        /*public int currentx;
        private int currenty;*/
        //ServerConnection
        public int Port { get; set; }
        
        public IPAddress LocalAddress { get; private set; }

        public CursorServer(int port)
        {
            Port = port;
            LocalAddress = IPAddress.Any;//IPAddress.Loopback //Any
        }

        public void Start()
        {
            /*
            Console.WriteLine("Who is connecting?");
            int androidIp = int.Parse(Console.ReadLine());
            */
            TcpListener server = new TcpListener(LocalAddress, Port);//TcpListener server = new TcpListener(LocalAddress, Port);
            server.Start();
            Trace.WriteLine("Mouse Controller server started on " + LocalAddress + "port " + Port);
            while (true)
            {
                Trace.WriteLine("Waiting for connection ...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                Task.Run(() => OnConnection(client));
            }
        }
       

        private static void OnConnection(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                String request = reader.ReadLine();
                // stuff that should happen here (which makes this different from eccho server
                // it should do more with the request than just returning it)

                // makes the variable response can be used later on

                string response = "";
                int currentx;
                int currenty;

//this is the old code slightly modified, but perhaps it is not needed to be that complicated for prototype purpose
                // splits a string into pieces when a space character is encountered,
                // [0] should be the opperator fx, use mouse or use keyboard
                // [1] should be either a digit (X-value), or a key that has to be pressed
                // [2] if 0 is mouse then this is shoud b a digit for the Y-value

                string[] part = request.Split(' ');
                //try
                //{
                    switch (part[0])
                    {
                        case "mouseGetPos":
                            //Gets the position of the mouse
                            currentx = Cursor.Position.X;
                            currenty = Cursor.Position.Y;
                            response = currentx + "," + currenty;
                            Trace.WriteLine("MousePosition: " + response);
                            break;

                        case "mouseMove":
                            //This should move the cursor to a new position relatively to it's current position
                            if (part.Length != 3)
                            {
                                response = "Requests on the operation '" + part[0] + "' must look like this: " + part[0] + " <int> <int>";
                            }
                            else
                            {
                                try
                                {
                                    
                                    int setX = int.Parse(part[1]);
                                    int setY = int.Parse(part[2]);
                                    //move mouse to new position
                                    Cursor.Position = new Point(Cursor.Position.X - setX, Cursor.Position.Y - setY);
                                    Trace.WriteLine("Mouse Moved to new position: " + Cursor.Position.X + "," + Cursor.Position.Y);
                                }
                                catch (Exception)
                                {
                                    response = "Invalid request: '" + request + "'";
                                    Trace.WriteLine(response);
                                }
                            }
                            break;

                        case "mouseSetPos":
                            //This function should Set the cursor to a new position
                            if (part.Length != 3)
                            {
                                response = "Requests on the operation '" + part[0] + "' must look like this: " + part[0] + " <int> <int>";
                            }
                            else
                            {
                                try
                                {
                                    int setX = int.Parse(part[1]);
                                    int setY = int.Parse(part[2]);
                                    //set mouse position
                                    SetCursorPos(setX, setY);
                                    String strX = setX.ToString();
                                    String strY = setY.ToString();
                                    response = strX + " " + strY;
                                    //response = "mouse pos set to X = " + strX + " and Y = " + strY;
                                    Trace.WriteLine("mouse position is set to X = " + strX + " and Y = " + strY);
                                }
                                catch (Exception)
                                {
                                    response = "Invalid request: '" + request + "' Only numbers are allowed";
                                    Trace.WriteLine(response);
                                }
                            }
                            break;
                        case "mouse1press":
                            //press and hold left mouse button, no coordinates are needed
                            currentx = Cursor.Position.X;
                            currenty = Cursor.Position.Y;
                            mouse_event(MOUSEEVENT_LEFTDOWN, currentx, currenty, 0, 0);
                            response = "Left mouse button pressed down on cordinates" + currentx + "," + currenty;
                            break;
                        case "mouse1release":
                            //release left mouse button, no coordinates are needed
                            currentx = Cursor.Position.X;
                            currenty = Cursor.Position.Y;
                            mouse_event(MOUSEEVENT_LEFTUP, currentx, currenty, 0, 0);
                            response = "Left mouse button released on cordinates" + currentx + "," + currenty;
                            break;
                        case "mouse1Click":
                            //release left mouse button, no coordinates are needed
                            currentx = Cursor.Position.X;
                            currenty = Cursor.Position.Y;
                            mouse_event(MOUSEEVENT_LEFTDOWN, currentx, currenty, 0, 0);
                            mouse_event(MOUSEEVENT_LEFTUP, currentx, currenty, 0, 0);
                            response = "Left mouse button pressed and released on cordinates" + currentx + "," + currenty;
                            break;
                        case "mouseClickCordinate":
                            /*This funtion emulates a click with the mouse on specified cordinates, 
                             * meaning that the left mouse button is pressed and released imidiately */
                                if (part.Length != 3)
                            {
                                response = "Requests on the operation '" + part[0] + "' must look like this: " + part[0] + " <int> <int>";
                            }
                            else
                            {
                                try
                                {
                                    int intX = int.Parse(part[1]);
                                    int intY = int.Parse(part[2]);
                                    SetCursorPos(intX, intY);
                                    //this.Refresh();
                                    //Application.DoEvents();
                                    mouse_event(MOUSEEVENT_LEFTDOWN, intX, intY, 0, 0);
                                    mouse_event(MOUSEEVENT_LEFTUP, intX, intY, 0, 0);
                                    String strX = intX.ToString();
                                    String strY = intY.ToString();
                                    response = strX + " " + strY;
                                    //response = "mouse pos set to X = " + strX + " and Y = " + strY;
                                    Trace.WriteLine("mouse pos set to X = " + strX + " and Y = " + strY + "and clicked");
                                }
                                catch (Exception)
                                {
                                    response = "Invalid request: '" + request + "'";
                                    Trace.WriteLine(response);
                                }
                            }
                            break;

                        case "keyboard":
                        if (part.Length != 2)
                        {
                            response = "try 'keyboard a'";
                        }
                        else
                        {
                            try
                            {
                                //kb = keyboard , This funtion should emulate a keyboard stroke
                                //http://stackoverflow.com/questions/10057608/how-do-i-generate-keystrokes-in-a-non-form-application
                                switch (part[2])
                                {
                                case "a":
                                        SendKeys.SendWait("{a}");
                                        response = part[2] + " pressed";
                                    break;
                                case "s":
                                        SendKeys.SendWait("{s}");
                                        response = part[2] + " pressed";
                                        break;
                                case "d":
                                        SendKeys.SendWait("{s}");
                                        response = part[2] + " pressed";
                                        break;
                                case "w":
                                        SendKeys.SendWait("{w}");
                                        response = part[2] + " pressed";
                                        break;
                                    default:
                                    response = "Only 'a' 's' 'd' 'w' is allowed at this time";
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                response = "Invalid request: '" + request + "' only key a and key s d w are allowed at this time";
                                Trace.WriteLine(response);
                            }
                        }
                        break;
                            //

                        default:
                            response = "Bad request,"+ part[0] + " ... try another Command" +
                                       " fx: mouseGetPos, mouseMove, mouseSetPos, mouse1press, mouse1release, mouse1Click, mouseClickCordinate, keyboard";
                            //response = "Default in switchcase hit, Request must either look like this <mouse> <int> <int>, or this <kb> <KeyName>";
                            break;
                    }
                    /*
                }
                catch (FormatException)
                {
                    response = "Error";
                    Trace.WriteLine("request is invalid!", request);
                }
                */


                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(response);
                writer.Flush();
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }

        }
    }
}
