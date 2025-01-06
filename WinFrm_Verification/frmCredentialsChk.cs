using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace WinFrm_Verification
{
    public partial class frmCredentialsChk : Form
    {

        private string _licenceVerificationEndPoint = ConfigurationManager.AppSettings["LicenceVerificationApiEndPoint"]?.ToString() ?? string.Empty;
        //private string _statusFilePath = Path.Combine(Application.StartupPath, "Status.dat");

        private bool _licenceVerified = false;

        public frmCredentialsChk()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                SystemActivityLogic.SendSystemActivity();

                return;

                lblMessage.Visible = false;
                //ExecuteCommand("sc", "delete CompanyLicenceVrifier");

                #region FORM VALIDATIONS

                if (string.IsNullOrWhiteSpace(txtCompanyCode.Text))
                {
                    lblMessage.Text = "Company code cannot be blanked";
                    lblMessage.Visible = true;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(txtEmployeeCode.Text))
                {
                    lblMessage.Text = "Employee code cannot be blanked";
                    lblMessage.Visible = true;
                    return;
                }
                else if (string.IsNullOrWhiteSpace(txtLicenceKey.Text))
                {
                    lblMessage.Text = "Licence key cannot be blanked";
                    lblMessage.Visible = true;
                    return;
                }

                #endregion

                this.Cursor = Cursors.WaitCursor;
                btnSubmit.Enabled = false;

                //show loading message
                lblMessage.Text = "Loading...";
                lblMessage.ForeColor = Color.Black;
                lblMessage.Visible = true;

                // Request payload
                var requestData = new
                {
                    companyCode = txtCompanyCode.Text,
                    employeeCode = txtEmployeeCode.Text,
                    licenseKey = txtLicenceKey.Text
                };

                string jsonData = JsonSerializer.Serialize(requestData);

                var response = await PostApiAsync(_licenceVerificationEndPoint, jsonData);

                if (response.IsSuccessStatusCode)
                {
                    _licenceVerified = true;

                    string servicePath = Path.Combine(Application.StartupPath, "WS", "WindowsService.exe");

                    // Register and start the service
                    ExecuteCommand("sc", $"create CompanyLicenceVrifier binPath= \"{servicePath}\" start= auto");
                    ExecuteCommand("sc", "description CompanyLicenceVrifier \"Company Licence Vrifier\"");
                    ExecuteCommand("sc", "start CompanyLicenceVrifier");

                    // Write to status.dat
                    //File.WriteAllText(_statusFilePath, "verified");

                    MessageBox.Show("Service installed and started successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    //MessageBox.Show(apiResponse?.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    lblMessage.Text = apiResponse.Message;
                    lblMessage.Visible = true;
                    lblMessage.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error: {ex.Message} {ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblMessage.Text = "Sometthing went wrong. Please try again later.";
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
                btnSubmit.Enabled = true;
            }
        }

        private async Task<HttpResponseMessage> PostApiAsync(string url, string jsonData)
        {
            using (HttpClient client = new HttpClient())
            {
                // Set headers
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                return response;
            }
        }

        private void ExecuteCommand(string fileName, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    ExecuteCommand("sc", "delete CompanyLicenceVrifier");
                    throw new Exception($"Command failed. Error: {process.ExitCode} {error}");
                }
            }
        }

        private void frmCredentialsChk_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_licenceVerified)
                e.Cancel = true;
        }
    }
}
