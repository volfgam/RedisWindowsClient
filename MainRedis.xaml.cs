using RedisWindowsClient.Extensions;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace RedisWindowsClient
{
    public partial class MainRedis : Window
    {
        //private void txtEditor_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    var row = txtEditor.GetLineIndexFromCharacterIndex(txtEditor.CaretIndex);
        //    var col = txtEditor.CaretIndex - txtEditor.GetCharacterIndexFromLineIndex(row);
        //    lblCursorPosition.Text = "Line " + (row + 1) + ", Char " + (col + 1);
        //}

        private void btnJsonPrettify_Click(object sender, RoutedEventArgs e)
        {
            var textRange = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);

            textRange.Text = textRange.Text.PrettyPrint();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text.Contains('*'))
            {
                await GetSchemas(txtSearch.Text);
            }
            else
            {
                await GetKey(txtSearch.Text);
            }
        }

        private async void trvDbZero_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var currentKey = trvDbZero.SelectedItem.ToString();
            await GetKey(currentKey);
        }

        private readonly ConnectionMultiplexer Redis;
        private readonly IDatabase RedisDB;
        private readonly TextRange _richTextBox;

        public async Task GetSchemas(string filter)
        {
            trvDbZero.Items.Clear();
            var schemas = new List<string>();
            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            var nextCursor = 0;
            do
            {
                schemas.Clear();
                var redisResult = await RedisDB.ExecuteAsync("SCAN", new object[] { nextCursor.ToString(), "MATCH", filter, "COUNT", "1000" });
                var innerResult = (RedisResult[])redisResult;

                nextCursor = int.Parse((string)innerResult[0]);

                var resultLines = ((string[])innerResult[1]).ToList();
                schemas.AddRange(resultLines);

                if (schemas.Count > 0)
                {
                    foreach (var result in schemas)
                    {
                        trvDbZero.Items.Add(result.ToString());
                    }
                }
                lblStatus1.Content = $"Buscando {(decimal)sw.Elapsed.TotalSeconds}s";
            }
            while (nextCursor != 0);
            sw.Stop();
        }

        private async Task GetKey(string key)
        {
            var keyValue = await RedisDB.StringGetAsync(key);
            _richTextBox.Text = string.Empty;

            if (!keyValue.IsNullOrEmpty)
            {
                _richTextBox.Text = keyValue;
                _richTextBox.Text = _richTextBox.Text.PrettyPrint();
            }
        }

        public MainRedis()
        {
            InitializeComponent();

            var options = ConfigurationOptions.Parse("<urlServer>");
            options.AsyncTimeout = 20 * 1000;
            options.SyncTimeout = 20 * 1000;
            options.Password = "<Password>";
            options.Ssl = true;
            options.SetDefaultPorts();

            Redis = ConnectionMultiplexer.Connect(options);
            RedisDB = Redis.GetDatabase(0);
            _richTextBox = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);
        }

        private void btnClearList_Click(object sender, RoutedEventArgs e)
        {
            trvDbZero.Items.Clear();
        }
    }
}