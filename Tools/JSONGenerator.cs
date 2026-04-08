namespace JSONVomitorium {
    using System.IO;
    using System.Text.Json;

    public static class Utility {
        public static readonly Random rnd = new Random();
    }

    public class Data {
        public int id { get; set; }
        public int value { get; set; }
        public DateTime timestamp { get; set; }

        public Data() {}

        public Data(int i, int v, DateTime t) {
            id = i;
            value = v;
            timestamp = t;
        }
    }

    public class Writer {
        public void WriteData() {
            WriteOneHour();
            WriteSixHours();
            WriteOneDay();
            WriteThreeDays();
            WriteOneWeek();
            WriteOneMonth();
        }

        private void WriteOneHour() {
            const string fileName = "dataOneHour.json";
            const int length = 240;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            for (int i = 0; i < length; i++) {
                data[i] = new Data(index++, Utility.rnd.Next(0, 256), date);
                date = date.AddSeconds(15);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }

        private void WriteSixHours() {
            const string fileName = "dataSixHours.json";
            const int length = 360;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            for (int i = 0; i < length; i++) {
                data[i] = new Data(index++, Utility.rnd.Next(0, 256), date);
                date = date.AddMinutes(1);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }

        private void WriteOneDay() {
            const string fileName = "dataOneDay.json";
            const int length = 288;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            for (int i = 0; i < length; i++) {
                data[i] = new Data(index++, Utility.rnd.Next(0, 256), date);
                date = date.AddMinutes(5);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }

        private void WriteThreeDays() {
            const string fileName = "dataThreeDays.json";
            const int length = 288;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            for (int i = 0; i < length; i++) {
                data[i] = new Data(index++, Utility.rnd.Next(0, 256), date);
                date = date.AddMinutes(15);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }

        private void WriteOneWeek() {
            const string fileName = "dataOneWeek.json";
            const int length = 336;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            for (int i = 0; i < length; i++) {
                data[i] = new Data(index++, Utility.rnd.Next(0, 256), date);
                date = date.AddMinutes(30);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }

        private void WriteOneMonth() {
            const string fileName = "dataOneMonth.json";
            const int length = 720;

            string strStart = "07/04/2026 04.00.00Z";

            DateTime startOfDay = DateTime.Parse(strStart);

            Data[] data = new Data[length];
            int index = 0;
            DateTime date = startOfDay;

            const int peak = 128;
            const int peakHour = 15;

            for (int i = 0; i < length; i++) {
                int value;
                int diff;

                if (date.Hour > 6 && date.Hour < 22) {
                    if (date.Hour < peakHour) {
                        diff = peakHour - date.Hour;
                    }
                    else {
                        diff = date.Hour - peakHour;
                    }

                    value = peak - (diff * Utility.rnd.Next(6, 8));
                    value += Utility.rnd.Next(-4, 5);
                }
                else {
                    value = 24;
                    value += Utility.rnd.Next(-2, 4);
                }

                data[i] = new Data(index++, value, date);
                date = date.AddHours(1);
            }

            string json = JsonSerializer.Serialize<Data[]>(data);
            File.WriteAllText(fileName, json);
        }
    }

    internal class JSONGenerator {
        static void Main(string[] args) {
            Writer writer = new Writer();

            writer.WriteData();
        }
    }
}
