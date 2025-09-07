using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestWinform.models;

namespace TestWinform
{
    public partial class Home : Form
    {
        private readonly HttpClient _httpClient;
        public Home()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        private async void Home_Load(object sender, EventArgs e)
        {
            await getAllCompany();
        }

        private async Task getAllCompany()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("https://localhost:7111/api/Tax/getAll");
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<List<CompanyInfo>>(responseJson);

                    if (result != null && result.Any())
                    {
                        dataGridView1.DataSource = result;
                        dataGridView1.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Không có dữ liệu công ty nào");
                    }
                }
                else
                {
                    MessageBox.Show($"Lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();
            Home_Load(sender, e);
        }
    }
}
