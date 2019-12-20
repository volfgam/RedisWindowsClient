using RedisWindowsClient.Converters;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Windows;

namespace RedisWindowsClient
{
    public partial class MainWindow : Window
    {
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await GetEmissorToken();
        }

        private async Task GetEmissorToken()
        {
            var arr = (RedisResult[])RedisDB.Execute("SCAN", new object[] { 0, "MATCH", txtSearch.Text, "COUNT", "100" });
            var nextCursor = (int)arr[0];
            var keys = (RedisKey[])arr[1];

            if (keys.Length > 0)
            {
                foreach (var key in keys)
                {
                    trvDbZero.Items.Add(key.ToString());
                }
            }
            //https://github.com/StackExchange/StackExchange.Redis/issues/763
        }

        private async void trvDbZero_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var currentKey = trvDbZero.SelectedItem.ToString();
            var keyValue = await RedisDB.StringGetAsync(currentKey);

            var jsonHtml = JsonHtml.GetHtml(keyValue.ToString());
            //Gera um html do json em formato de table
            webBrowser1.NavigateToString(jsonHtml);

            //NavigateToString
            //txtCacheValue.se = keyValue.ToString();
            //txtCacheValue.AppendText(keyValue.ToString());
        }

        private readonly ConnectionMultiplexer Redis;

        private readonly IDatabase RedisDB;

        public MainWindow()
        {
            InitializeComponent();

            var options = ConfigurationOptions.Parse("server");
            options.AsyncTimeout = 20 * 1000;
            options.SyncTimeout = 20 * 1000;
            options.Password = "password";
            options.Ssl = true;
            options.SetDefaultPorts();

            Redis = ConnectionMultiplexer.Connect(options);
            RedisDB = Redis.GetDatabase(0);

            webBrowser1.NavigateToString(
              "<html><body>Please enter your name:<br/>" +
              "<input type='text' name='userName'/><br/>" +
              "<a href='http://www.microsoft.com'>continue</a>" +
              "</body></html>");
        }
    }
}