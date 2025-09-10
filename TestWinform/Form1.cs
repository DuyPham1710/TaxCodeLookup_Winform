using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TestWinform.Constants;
using TestWinform.dto;
using TestWinform.models;
using TestWinform.utils;

namespace TestWinform
{
    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient;
        private string _sessionId;
        private CompanyInfo _companyInfo;
        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();

        }
        private async void Form1_Load(object sender, EventArgs e)
        {
            ConfigureGridView();
            await getCaptcha();
            lblCaptcha.Visible = false;
        }

        private async Task getCaptcha()
        {
            //try
            //{
            //    HttpResponseMessage response = await _httpClient.GetAsync($"{GlConstants.BASE_URL}/captcha");
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseJson = await response.Content.ReadAsStringAsync();
            //        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);
            //        if (result != null && result.ContainsKey("captcha"))
            //        {
            //            lblCaptcha.Text = result["captcha"];

            //        }
            //        else
            //        {
            //            MessageBox.Show("Không lấy được mã captcha");
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show($"Lỗi: {response.StatusCode}");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Exception: {ex.Message}");
            //}

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{GlConstants.BASE_URL}/captcha");
                if (response.IsSuccessStatusCode)
                {
                   // lấy sessionId từ header
                    if (response.Headers.Contains("X-Session-Id"))
                    {
                        _sessionId = response.Headers.GetValues("X-Session-Id").FirstOrDefault();
                    }
                    Console.WriteLine("SessionId: " + _sessionId);
                    //   đọc bytes của ảnh captcha
                    var imgBytes = await response.Content.ReadAsByteArrayAsync();

                    using (var ms = new MemoryStream(imgBytes))
                    {
                        Image img = Image.FromStream(ms);
                        pictureBoxCaptcha.Image = img;

                        string captchaText = Helper.ReadCaptchaFromImage(img);
                        txtCaptcha.Text = captchaText;
                    }
                }
                else
                {
                    MessageBox.Show($"Lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                MessageBox.Show($"Exception: {ex.Message}");
            }
        }

        private void ConfigureGridView()
        {
            listBoxCompanies.AutoGenerateColumns = false;
            listBoxCompanies.AllowUserToAddRows = false;
            listBoxCompanies.AllowUserToDeleteRows = false;
            listBoxCompanies.ReadOnly = true;
            listBoxCompanies.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            listBoxCompanies.MultiSelect = false;
            listBoxCompanies.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            listBoxCompanies.RowHeadersVisible = false;

            // Cột tên công ty
            listBoxCompanies.Columns.Clear();
            var companyNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "CompanyName",
                HeaderText = "Tên công ty",
                DataPropertyName = "Name",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            listBoxCompanies.Columns.Add(companyNameColumn);

            listBoxCompanies.SelectionChanged += ListBoxCompanies_SelectionChanged;

            // Cấu hình GridView bên phải 
            detailGrid.AutoGenerateColumns = false;
            detailGrid.AllowUserToAddRows = false;
            detailGrid.AllowUserToDeleteRows = false;
            detailGrid.ReadOnly = true;
            detailGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            detailGrid.MultiSelect = false;
            detailGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            detailGrid.RowHeadersVisible = false;

            // Cột cho thông tin chi tiết
            detailGrid.Columns.Clear();
            var infoColumn = new DataGridViewTextBoxColumn
            {
                Name = "Info",
                HeaderText = "Thông tin",
                DataPropertyName = "Info",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var dataColumn = new DataGridViewTextBoxColumn
            {
                Name = "Data",
                HeaderText = "Dữ liệu",
                DataPropertyName = "Data",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            detailGrid.Columns.Add(infoColumn);
            detailGrid.Columns.Add(dataColumn);
        }

        private void ListBoxCompanies_SelectionChanged(object sender, EventArgs e)
        {
            if (listBoxCompanies.SelectedRows.Count > 0)
            {
                var selectedRow = listBoxCompanies.SelectedRows[0];
                var selectedCompany = selectedRow.DataBoundItem as CompanyInfo;

                if (selectedCompany != null)
                {
                    ShowCompanyDetails(selectedCompany);
                }
            }
        }

        private void ShowCompanyDetails(CompanyInfo company)
        {
            var details = new List<CompanyDetail>
            {
                new CompanyDetail { Info = company.Name, Data =  "" },
                new CompanyDetail { Info = "Mã số thuế", Data = company.TaxID },
                new CompanyDetail { Info = "Địa chỉ Thuế", Data = company.TaxAuthority },
                new CompanyDetail { Info = "Địa chỉ", Data = company.Address },
                new CompanyDetail { Info = "Tình trạng", Data = company.Status },
                new CompanyDetail { Info = "Tên quốc tế", Data = company.InternationalName },
                new CompanyDetail { Info = "Tên viết tắt", Data = company.ShortName },
                new CompanyDetail { Info = "Người đại diện", Data = company.Representative },
                new CompanyDetail { Info = "Điện thoại", Data = company.Telephone },
                new CompanyDetail { Info = "Ngày hoạt động", Data = company.FoundingDate },
                new CompanyDetail { Info = "Quản lý bởi", Data = company.ManagingBy },
                new CompanyDetail { Info = "Loại hình DN", Data = company.CompanyType },
                new CompanyDetail { Info = "Ngành nghề chính", Data = company.MainIndustry },
            };

            // Chỉ giữ lại những dòng có Data khác null/empty
            var filteredDetails = details
                  .Where(d => d.Info == company.Name || !string.IsNullOrWhiteSpace(d.Data))
                 .ToList();


            detailGrid.DataSource = filteredDetails;
            detailGrid.Refresh();
        }

        private async void btnFind_Click(object sender, EventArgs e)
        {
            string taxCode = txtInput.Text.Trim();
            string CaptchaCode = txtCaptcha.Text.Trim();

            if (string.IsNullOrEmpty(taxCode) && string.IsNullOrEmpty(CaptchaCode))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ mã số thuế và captcha");
                return;
            }
            await LoadDataAsync(taxCode, CaptchaCode);
            getCaptcha();
            txtCaptcha.Text = string.Empty;
        }

        private async Task LoadDataAsync(string taxCode, string captchaCode)
        {
            var requestObj = new
            {
                maSoThue = taxCode,
                captcha = captchaCode
            };

            //if (captchaCode != txtCaptcha.Text.Trim())
            //{
            //    MessageBox.Show("Mã captcha không đúng, vui lòng thử lại.");
            //    await getCaptcha();
            //    return;
            //}

            string json = JsonConvert.SerializeObject(requestObj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                // gán sessionId vào header trước khi gọi API
                _httpClient.DefaultRequestHeaders.Remove("X-Session-Id");
                _httpClient.DefaultRequestHeaders.Add("X-Session-Id", _sessionId);

                HttpResponseMessage response = await _httpClient.PostAsync($"{GlConstants.BASE_URL}/lookup-company", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CompanyInfoResponse>(responseJson);

                    if (result != null && result.Data != null)
                    {
                        var dataList = new List<CompanyInfo> { result.Data };

                        _companyInfo = result.Data;
                     
                        listBoxCompanies.DataSource = dataList;
                        listBoxCompanies.Refresh();

                        // Tự động chọn dòng đầu tiên và hiển thị chi tiết
                        if (listBoxCompanies.Rows.Count > 0)
                        {
                            listBoxCompanies.Rows[0].Selected = true;
                            ShowCompanyDetails(result.Data);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu");
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu");
                    //MessageBox.Show($"Lỗi: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");

            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {

            var json = JsonConvert.SerializeObject(_companyInfo, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync($"{GlConstants.BASE_URL}/save-enterprise", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<EnterpriseResponse>(responseJson);
                if (result != null && result.Success)
                {
                    MessageBox.Show("Lưu thành công");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Lưu không thành công");
                }
            }
            else
            {
                // Lấy nội dung lỗi từ API
                var errorContent = await response.Content.ReadAsStringAsync();

                var apiError = JsonConvert.DeserializeObject<ApiError>(errorContent);

                if (apiError != null)
                {
                    MessageBox.Show(apiError.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }   
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {
            if (txtInput.Text.Length >= 10)
            {
                btnFind_Click(sender, e);
            }
           
        }
    }
}
