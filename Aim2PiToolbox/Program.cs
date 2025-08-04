using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Aim2PiToolbox
{
    [StructLayout(LayoutKind.Sequential)]
    public struct tm
    {
        public int tm_sec;   // seconds after the minute [0-60]
        public int tm_min;   // minutes after the hour [0-59]
        public int tm_hour;  // hours since midnight [0-23]
        public int tm_mday;  // day of the month [1-31]
        public int tm_mon;   // months since January [0-11]
        public int tm_year;  // years since 1900
        public int tm_wday;  // days since Sunday [0-6]
        public int tm_yday;  // days since January 1 [0-365]
        public int tm_isdst; // daylight savings time flag
    }

    
    class Program
    {

        // Import the native functions from the Aim DLL
        private const string dllPath = "MatLabXRK-2022-64-ReleaseU.dll";
        
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int open_file(string full_path_name);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int close_file_n(string full_path_name);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_vehicle_name(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_track_name(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_racer_name(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_championship_name(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_session_type_name(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_date_and_time(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_laps_count(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_lap_info(int idxf, int idxl, ref double pstart, ref double pduration);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_channels_count(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_channel_name(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_channel_units(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_channel_samples_count(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_channel_samples(int idxf, int idxl, ref double ptime, ref double pvalues, int cnt);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_channels_count(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_GPS_channel_name(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_GPS_channel_units(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_channel_samples_count(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_channel_samples(int idxf, int idxl, ref double ptime, ref double pvalues, int cnt);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_raw_channels_count(int idx);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_GPS_raw_channel_name(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr get_GPS_raw_channel_units(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_raw_channel_samples_count(int idxf, int idxc);
        [DllImport(dllPath, CallingConvention = CallingConvention.Cdecl)] public static extern int get_GPS_raw_channel_samples(int idxf, int idxl, ref double ptime, ref double pvalues, int cnt);

        static string FormatTm(tm dateTime)
        {
            // tm_year is years since 1900, tm_mon is months since January (0-11)
            int year = dateTime.tm_year + 1900;
            int month = dateTime.tm_mon + 1;
            int day = dateTime.tm_mday;
            int hour = dateTime.tm_hour;
            int min = dateTime.tm_min;
            //int sec = dateTime.tm_sec;

            return $"{year:D4}-{month:D2}-{day:D2} {hour:D2}:{min:D2}";//:{sec:D2}";
        }

        static void Main(string[] args)
        {
            int m_idx = 0;

            // Try to open data file from command line argument
            m_idx = Program.open_file(args[0]);
            if (m_idx == 0)
            {
                Console.WriteLine("Failed to open source file: " + args[0]);
                return;
            }

            string outfile = args[0]+".txt";

            //Get outing information
            IntPtr ptrVehicle = get_vehicle_name(m_idx);
            string VehicleName = Marshal.PtrToStringAnsi(ptrVehicle);

            IntPtr ptrTrack = get_track_name(m_idx);
            string TrackName = Marshal.PtrToStringAnsi(ptrTrack);

            IntPtr ptrRacer = get_racer_name(m_idx);
            string RacerName = Marshal.PtrToStringAnsi(ptrRacer);

            IntPtr ptrChampionship = get_championship_name(m_idx);
            string ChampionshipName = Marshal.PtrToStringAnsi(ptrChampionship);

            IntPtr ptrSessionType = get_session_type_name(m_idx);
            string SessionTypeName = Marshal.PtrToStringAnsi(ptrSessionType);

            IntPtr ptrDateAndTime = get_date_and_time(m_idx);
            tm dateTime = Marshal.PtrToStructure<tm>(ptrDateAndTime);
            string dateTimeStr = FormatTm(dateTime);

            using (var writer = new StreamWriter(outfile,false,Encoding.GetEncoding(1252)))      //Important to use the 1252 encoding to match Pi Toolbox ASCII format      
            {
                // File header
                
                writer.WriteLine("PiToolboxVersionedASCIIDataSet");
                writer.WriteLine("Version\t2");
                writer.WriteLine();
                writer.WriteLine("{OutingInformation}");
                writer.WriteLine($"CarName\t{VehicleName}");
                writer.WriteLine($"TrackName\t{TrackName}");
                writer.WriteLine($"RacerName\t{RacerName}");
                writer.WriteLine($"ChampionshipName\t{ChampionshipName}");
                writer.WriteLine($"SessionType\t{SessionTypeName}");
                writer.WriteLine($"DateAndTime\t{dateTimeStr}");
                writer.WriteLine("FirstLapNumber\t0");

                // Logger channel data
                Console.WriteLine("\nProcessing logger channels...\n");
                int count = get_channels_count(m_idx);
                for (int idx = 0; idx < count; idx++)
                {
                    //Flag to reverse the direction of signed channels if desired
                    int direction = 1; // 1 for normal, -1 for reversed

                    int samples = get_channel_samples_count(m_idx, idx);
                    if (samples == 0) continue; // Skip channel with no samples

                    IntPtr namePtr = get_channel_name(m_idx, idx);
                    string name = Marshal.PtrToStringAnsi(namePtr);
                    IntPtr unitsPtr = get_channel_units(m_idx, idx);
                    string units = Marshal.PtrToStringAnsi(unitsPtr);
                    Console.WriteLine($"{name}[{units}]");

                    // Replace units
                    units = units.Replace("deg", "\xB0"); // Degree symbol
                    if (units == "g") units = "G";
                    if (units == "C") units = "\xB0\x43"; // degrees Celsius symbol

                    //Name cleanup
                    if (name == "LateralAcc") name = "Lateral Acceleration";
                    if (name == "LongAcc") name = "Longitudinal Acceleration";
                    if (name == "VerticalAcc") name = "Vertical Acceleration";
                    if (name == "ACCEL POS") name = "Accelerator";
                    if (name == "COOLANT TEMP") name = "Coolant Temp";
                    if (name == "External Voltage") name = "Battery Voltage";
                    if (name == "STEER ANGLE") name = "Steering Angle";
                    if (name == "YawRate") name = "Yaw Rate";

                    //reverse direction for specific channels
                    if (name == "Yaw Rate") // || name == "Steering Angle")
                    {
                        direction = -1; // Reverse direction for these channels
                    }

                    double[] times = new double[samples];
                    double[] values = new double[samples];
                    int result = get_channel_samples(m_idx, idx, ref times[0], ref values[0], samples);
                    if (result == 0)
                    {
                        Console.WriteLine($"Error retrieving samples for channel {name}: {result}");
                        continue;
                    }

                    writer.WriteLine();
                    writer.WriteLine("{ChannelBlock}");
                    writer.WriteLine($"Time\t{name}[{units}]");
                    for (int i = 0; i < samples; i++)
                    {
                        values[i] *= direction; // Apply direction
                        writer.WriteLine($"{times[i]}\t{values[i]}");
                    }
                }

                // GPS Channel Data
                Console.WriteLine("\nProcessing GPS channels...\n");
                count = get_GPS_channels_count(m_idx);
                for (int idx = 0; idx < count; idx++)
                {
                    int samples = get_GPS_channel_samples_count(m_idx, idx);
                    if (samples == 0) continue; // Skip channel with no samples

                    IntPtr namePtr = get_GPS_channel_name(m_idx, idx);
                    string name = Marshal.PtrToStringAnsi(namePtr);
                    IntPtr unitsPtr = get_GPS_channel_units(m_idx, idx);
                    string units = Marshal.PtrToStringAnsi(unitsPtr);
                    Console.WriteLine($"{name}[{units}]");

                    units = units.Replace("deg", "\xB0"); // Degree symbol

                    if (units == "g") units = "G";

                    //Name cleanup
                    if (name == "GPS Speed") name = "Speed";
                    if (name == "GPS Heading") name = "Heading";
                    if (name == "GPS Latitude") name = "Latitude";
                    if (name == "GPS Longitude") name = "Longitude";
                    if (name == "GPS Altitude") name = "Altitude";


                    double[] times = new double[samples];
                    double[] values = new double[samples];

                    int result = get_GPS_channel_samples(m_idx, idx, ref times[0], ref values[0], samples);
                    if (result == 0)
                    {
                        Console.WriteLine($"Error retrieving samples for channel {name}: {result}");
                        continue;
                    }

                    writer.WriteLine();
                    writer.WriteLine("{ChannelBlock}");
                    writer.WriteLine($"Time\t{name}[{units}]");
                    for (int i = 0; i < samples; i++)
                    {
                        writer.WriteLine($"{times[i] / 1000.0}\t{values[i]}");
                    }
                }

                // GPS Raw Channel Data
                Console.WriteLine("\nProcessing GPS raw channels...\n");
                count = get_GPS_raw_channels_count(m_idx);
                for (int idx = 0; idx < count; idx++)
                {
                    int samples = get_GPS_raw_channel_samples_count(m_idx, idx);
                    if (samples == 0) continue; // Skip channel with no samples
                    // Retrieve channel name and units
                    IntPtr namePtr = get_GPS_raw_channel_name(m_idx, idx);
                    string name = Marshal.PtrToStringAnsi(namePtr);
                    IntPtr unitsPtr = get_GPS_raw_channel_units(m_idx, idx);
                    string units = Marshal.PtrToStringAnsi(unitsPtr);
                    Console.WriteLine($"{name}[{units}]");

                    units = units.Replace("deg", "\xB0"); // Degree symbol
                    //if (units == "deg") units = "\xB0"; // Degree symbol
                    if (units == "g") units = "G";

                    double[] times = new double[samples];
                    double[] values = new double[samples];

                    int result = get_GPS_raw_channel_samples(m_idx, idx, ref times[0], ref values[0], samples);
                    if (result == 0)
                    {
                        Console.WriteLine($"Error retrieving samples for channel {name}: {result}");
                        continue;
                    }

                    writer.WriteLine();
                    writer.WriteLine("{ChannelBlock}");
                    writer.WriteLine($"Time\t{name}[{units}]");
                    for (int i = 0; i < samples; i++)
                    {
                        writer.WriteLine($"{times[i]}\t{values[i]}");
                    }
                }


                // Event block for lap breakpoints
                writer.WriteLine();
                writer.WriteLine("{EventBlock}");
                writer.WriteLine("Time\tName\tCategory\tSource\tMessage");

                int lapcount = get_laps_count(m_idx);
                for (int idx = 0; idx < lapcount; ++idx)
                {
                    double start = 0;
                    double duration = 0;
                    int result = Program.get_lap_info(m_idx, idx, ref start, ref duration);
                    writer.WriteLine($"{start}\tEnd of lap\tToolbox Added\tDRV\tEnd of lap");
                }
            }

            Console.WriteLine($"Output file created: {outfile}");
        }
    }
}

