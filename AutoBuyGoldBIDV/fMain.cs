using Newtonsoft.Json;
using RestSharp;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoBuyGoldBIDV
{
    public partial class fMain : Form
    {
        public string ck;
        public fMain()
        {
            InitializeComponent();
        }


        async Task<string> GetCookie()
        {
            var options = new RestClientOptions()
            {
                // MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://gold.bidv.com.vn/DKNHDT/muavang.htm", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            if (response != null)
            {
                var cookies = response.Cookies;

                foreach (var cookie in cookies)
                {
                    ck += cookie + ";";
                }
            }
            return ck;
        }

        public static string ToBase64(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            string base64Encoded = Convert.ToBase64String(plainTextBytes);
            return base64Encoded;
        }

        public string ProcessCookie(string cookie)
        {
            string[] list_ck = cookie.Split(';');
            string ck0 = list_ck[0];

            string[] parts = ck0.Split('=');
            string value = parts[1];

            string[] jsession_part = value.Split(':');
            string jsession = jsession_part[0];
            return jsession.Substring(4);
        }

        async void GetCaptcha(string cookie)
        {

            var client = new RestClient();
            var request = new RestRequest("https://gold.bidv.com.vn/DKNHDT/CaptchaServlet?1720666972456", Method.Get);
            request.AddHeader("Cookie", cookie);
            RestResponse response = await client.ExecuteAsync(request);
            if (response.IsSuccessful && response.RawBytes != null)
            {
                try
                {
                    using (var ms = new MemoryStream(response.RawBytes))
                    {
                        var image = Image.FromStream(ms);
                        picCaptcha.Image = image;
                    }


                }
                catch
                {

                }
            }
            else
            {
                GetCaptcha(cookie);
            }
        }

        async void Request(string cookie, string cs, string captcha, string name, string ns, string cccd, string nc, string phone, string mcn, string stk, string dc1, string dc2)
        {
            var options = new RestClientOptions()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36",
            };
            var client = new RestClient(options);
            var request = new RestRequest("https://gold.bidv.com.vn/DKNHDT/RegGGSev", Method.Post);
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9,vi;q=0.8,ar;q=0.7,de;q=0.6");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.AddHeader("Cookie", cookie);
            request.AddHeader("Origin", "https://gold.bidv.com.vn");
            request.AddHeader("Referer", "https://gold.bidv.com.vn/DKNHDT/muavang.htm");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("X-Requested-With", "XMLHttpRequest");
            request.AddHeader("sec-ch-ua", "\"Not/A)Brand\";v=\"8\", \"Chromium\";v=\"126\", \"Google Chrome\";v=\"126\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            string body = string.Format("<xml><step>1</step><checksum>{0}</checksum><captcha>{1}</captcha><data><fullname>{2}</fullname><birthday>{3}</birthday><idnumber>{4}</idnumber><issuedate>{5}</issuedate><issueplace>CCSQLHCVTTXH</issueplace><cellphone>{6}</cellphone><amount>1</amount><branch>{7}</branch><acctnum>{8}</acctnum><address1>{9}</address1><address2>{10}</address2><purpose>2</purpose><capital>1</capital><capitaldesc></capitaldesc></data></xml>", cs, captcha, name, ns, cccd, nc, phone, mcn, stk, dc1, dc2);
            //var body = @"<xml><step>1</step><checksum>UldhMWJ0RjJmb0FCYnRDanBOeUF4QXE=</checksum><captcha>SrrpFy</captcha><data><fullname>Nguyen Van Anh</fullname><birthday>26/09/2000</birthday><idnumber>025104000490</idnumber><issuedate>18/03/2021</issuedate><issueplace>CCSQLHCVTTXH</issueplace><cellphone>0396084000</cellphone><amount>1</amount><branch>120000</branch><acctnum>0396084000</acctnum><address1>hà Nội</address1><address2>Hà Nội</address2><purpose>2</purpose><capital>1</capital><capitaldesc></capitaldesc></data></xml>";
            request.AddParameter("application/x-www-form-urlencoded", body, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            Result json = JsonConvert.DeserializeObject<Result>(response.Content);
            tsStatus.Text = json.Message.ToString();
            GetCaptcha(cookie);

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "TXT files|*.txt";
            theDialog.InitialDirectory = Environment.CurrentDirectory;
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    txtPath.Text = theDialog.FileName;
                    btnStart.Enabled = true;
                    string data = File.ReadAllText(txtPath.Text);
                    string[] fields = data.Split('|');
                    txtName.Text = fields[0]; // Name
                    txtNgaySinh.Text = fields[1]; // Date of birth
                    txtCCCD.Text = fields[2]; // CCCD
                    txtNgayCap.Text = fields[3]; // Issue date
                    txtPhone.Text = fields[4]; // Secondary phone number
                    switch (fields[5])
                    {
                        case "120000":
                            cbxChiNhanh.SelectedIndex = 0;
                            break;
                        case "122000":
                            cbxChiNhanh.SelectedIndex = 1;
                            break;
                        case "310000":
                            cbxChiNhanh.SelectedIndex = 2;
                            break;
                    }
                    txtSTK.Text = fields[6];
                    txtDiaChi1.Text = fields[7]; // Address1
                    txtDiaChi2.Text = fields[8]; // Address2



                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtCCCD.Text == "" || txtNgaySinh.Text == "" || txtNgayCap.Text == "" || txtDiaChi1.Text == "" || txtDiaChi2.Text == "" || txtPhone.Text == "" || txtSTK.Text == "")
            {
                MessageBox.Show("Thông tin không được trống!");
            }
            else
            {
                string macn = string.Empty;
                switch (cbxChiNhanh.SelectedIndex)
                {
                    case 0:
                        macn = "120000";
                        break;
                    case 1:
                        macn = "122000";
                        break;
                    case 2:
                        macn = "310000";
                        break;
                }

                string jsid = ProcessCookie(ck);
                string checksum = ToBase64(jsid);
                Request(ck, checksum, txtCaptcha.Text, txtName.Text, txtNgaySinh.Text, txtCCCD.Text, txtNgayCap.Text, txtPhone.Text, macn, txtSTK.Text, txtDiaChi1.Text, txtDiaChi2.Text);
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await GetCookie();
            GetCaptcha(ck);
        }
    }
}
