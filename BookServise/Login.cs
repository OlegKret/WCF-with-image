using BookServise.ServiceReference2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using WcfServiceBooks.DAL;

namespace BookServise
{
    public partial class Login : Form
    {
        
        OpenFileDialog ofd = new OpenFileDialog();
        string imgLocation = "";
        
        public Login()
        {
            InitializeComponent();
            //Database.SetInitializer(new DataInitializer());
            //new BookLotEntities().Database.Initialize(true);
        }

        private void bntLogin_Click(object sender, EventArgs e)
        {

            try
            {
                if (txtLogin.Text == "")
                {
                    throw new EmptyLoginException("Введіть логін!");
                }
                if (txtPassword.Text == "")
                {
                    throw new EmptyPasswordException("Введіть пароль!");
                }

                AccountClient accountClient = new AccountClient();
                User input = accountClient.Login(txtLogin.Text, txtPassword.Text);
                //SqlConnection con = new SqlConnection();
                //SqlDataAdapter sda = new SqlDataAdapter("Select Role FROM User Where Login='" + txtLogin.Text + "' and Password='" + txtPassword.Text + "'", con);
                //DataTable dt = new System.Data.DataTable();
                //sda.Fill(dt);
                //this.Hide();
                //foreach (User c in context.Users.SqlQuery("Select role FROM Users Where Login='" + txtLogin.Text + "' and Password='" + txtPassword.Text + "'")) ;
                SqlConnection con = new SqlConnection(@"data source=WCFServise.mssql.somee.com;initial catalog=WCFServise;user id=OlegKret_SQLLogin_1;password=tqyupwho8j");
                SqlDataAdapter sda = new SqlDataAdapter("Select Role FROM Users Where Login='" + txtLogin.Text + "' and Password='" + txtPassword.Text + "'", con);
                DataTable dt = new System.Data.DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    this.Hide();
                    Parent ss = new Parent(dt.Rows[0][0].ToString());
                    ss.Show();
                    //((Form)this.MdiParent).Controls["label1"].Text = dt.Rows[0][0].ToString();
                }

                //Parent ss = new Parent(input.ToString());
                //    ss.Show();
                if (input != null)
                {
                    MessageBox.Show("Вхід успішний!");

                    //if (input.PhotoName != null)
                    //{
                    //    Form2 form2 = new Form2(accountClient.GetImage(input.PhotoName));
                    //    form2.ShowDialog();
                    //}
                    //else
                    //{
                    //    Form2 form2 = new Form2(null);
                    //    form2.ShowDialog();
                    //}

                }
                if (input == null)
                {
                    MessageBox.Show("Ви НЕ ввійшли в особистий кабінет! \n Перевірте логін і пароль!");
                }

                accountClient.Close();
            }
            catch (EmptyLoginException emptyLoginException)
            {
                MessageBox.Show(emptyLoginException.Message);
            }
            catch (EmptyPasswordException emptyPasswordException)
            {
                MessageBox.Show(emptyPasswordException.Message);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }
        private void btnUsers_Click(object sender, EventArgs e)
        {

            if (checkLogin() == true || checkPhone() == true || checkEmail() == true)
            {
                label8.Text = "Your Login Already Registered";
                txt_Login.BackColor = System.Drawing.Color.CornflowerBlue;
                label8.Visible = true;

                label10.Text = "Your Phone Already Registered";
                txtPhoneNumber.BackColor = System.Drawing.Color.CornflowerBlue;
                label10.Visible = true;

                label9.Text = "Your Email Already Registered";
                txtEmail.BackColor = System.Drawing.Color.CornflowerBlue;
                label9.Visible = true;
            }
           

            //else if ()
            //{

            //}
            //else if ()
            //{

            //}

            else
            {
                if (pictureBox1.Image != null && checkLogin() != true && checkPhone() != true && checkEmail() != true)
                {
                   
                    //AccountClient accountClient = new AccountClient();
                    string fname = txtId.Text + ".jpg";
                    string folder = "C:\\Images";
                    string pathstring = System.IO.Path.Combine(folder, fname);
                    Image a = pictureBox1.Image;
                    a.Save(pathstring);


                    Image img = Image.FromFile(ofd.FileName);
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, img.RawFormat);
                    string remoteUri = "ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text;
                    WebClient webClient = new WebClient();

                    webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
                    MessageBox.Show("Downloading " + remoteUri);
                    byte[] myDataBuffer = webClient.DownloadData(remoteUri);

                    User user = new User();
                    //accountClient.ConvertFiltoBite(this.pictureBox1.ImageLocation);
                    user.Id = Int32.Parse(txtId.Text);
                    user.Login = txt_Login.Text;
                    user.Password = txt_Password.Text;
                    user.Email = txtEmail.Text;
                    user.PhoneNumber = txtPhoneNumber.Text;
                    user.role = txtRole.Text;
                    user.Pic = txtPic.Text;
                    user.Image = myDataBuffer;
                    //user.Image = ConvertFiltoBite(pathstring);
                    //user.Image = ImageToByte(pictureBox1.Image);

                    AccountClient accountClient = new AccountClient();
                    bool registered = accountClient.Register(user);
                   //accountClient.UploadFile(txtBrowzer.Text, txtPic.Text);
                    if (registered == true)
                    {
                        MessageBox.Show("Користувача зареєстровано!");
                    }
                }
                else
                {
                    MessageBox.Show("Choose picture...");
                }
            }
        }

        private byte[] ConvertFiltoBite(string spath)
        {
            byte[] data = null;
            FileInfo fInfo = new FileInfo(spath);
            long numBytes = fInfo.Length;
            FileStream fStream = new FileStream(spath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);
            data = br.ReadBytes((int)numBytes);
            return data;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (var context = new BookLotEntities())
            {
                try
                {
                    int userId = Int32.Parse(txtId.Text);
                    User userToUpdate = context.Users.Find(userId);
                    if (userToUpdate != null)
                    {
                        AccountClient accountClient = new AccountClient();
                        string remoteUri = "ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text;
                        WebClient webClient = new WebClient();

                        webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
                        MessageBox.Show("Downloading " + remoteUri);
                        byte[] myDataBuffer = webClient.DownloadData(remoteUri);
                        userToUpdate.Id = Int32.Parse(txtId.Text);
                        userToUpdate.Login = txt_Login.Text;
                        userToUpdate.Password = txt_Password.Text;
                        userToUpdate.Email = txtEmail.Text;
                        userToUpdate.PhoneNumber = txtPhoneNumber.Text;
                        userToUpdate.role = txtRole.Text;
                        userToUpdate.Pic = txtPic.Text;
                        userToUpdate.Image = myDataBuffer;
                        accountClient.Save(userToUpdate);
                        //context.SaveChanges();
                        accountClient.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Empty format, try again");
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int userId = Int32.Parse(txtId.Text);
            using (var context = new BookLotEntities())
            {
                User userToDelete = new User() { Id = userId };
                //context.Entry(userToDelete).State = EntityState.Deleted;
                //context.SaveChanges();
                AccountClient accountClient = new AccountClient();
                accountClient.Delete(userToDelete);
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {

            using (var context = new BookLotEntities())
            {
                //foreach (Books c in context.Books)
                //{
                //    cboBooks.Items.Add(c.Title);

                //}

                var source = context.Users.ToList();
                cboUsers.DataSource = source;
                //cboBooks.BindingContext=this.BindingContext();
                cboUsers.ValueMember = "Id";
                cboUsers.DisplayMember = "Login";

                //cboBooks.DisplayMember = "Genre";
                cboUsers.Invalidate();
                cboUsers.DataBindings.Clear();
                //cboBooks.SelectedValueChanged += new EventHandler(cboBooks_SelectedValueChanged);
                txtId.DataBindings.Add("Text", source, "Id", true);

                txt_Login.DataBindings.Add("Text", source, "Login", true);
                txtLogin.DataBindings.Add("Text", source, "Login", true);
                txt_Password.DataBindings.Add("Text", source, "Password", true);
                txtEmail.DataBindings.Add("Text", source, "Email", true);
                txtPassword.DataBindings.Add("Text", source, "Password");
                txtRole.DataBindings.Add("Text", source, "role");
                txtPhoneNumber.DataBindings.Add("Text", source, "PhoneNumber");
                txtPic.DataBindings.Add("Text", source, "Pic");
                //pictureBox1.DataBindings.Add(new Binding("Image", source, "Pic", true));
                //var s = from o in context.Users select o;
                //this.pictureBox1.DataBindings.Add(new System.Windows.Forms.Binding("Image", source, "Image", true));

            }
        }

        private Boolean checkLogin()
        {

            using (var context = new BookLotEntities())
            {
                Boolean loginavailable = false;

                int userid = (from x in context.Users
                              where x.Login == txt_Login.Text
                              select x.Id).SingleOrDefault();
                if (userid > 0)
                {
                    loginavailable = true;
                }
                return loginavailable;
            }

        }

        private Boolean checkPhone()
        {

            using (var context = new BookLotEntities())
            {

                Boolean phoneavailable = false;

                int userid = (from x in context.Users
                              where x.PhoneNumber == txtPhoneNumber.Text
                              select x.Id).SingleOrDefault();
                //try
                //{
                if (userid > 0)
                {
                    phoneavailable = true;
                }

                //}
                //catch(Exception ex)
                //{
                //MessageBox.Show("Try....");
                //}
                return phoneavailable;
            }

        }

        private Boolean checkEmail()
        {
            using (var context = new BookLotEntities())
            {
                Boolean emailavailable = false;

                int userid = (from x in context.Users
                              where x.Email == txtPhoneNumber.Text
                              select x.Id).SingleOrDefault();
                if (userid > 0)
                {
                    emailavailable = true;
                }
                return emailavailable;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            
            ofd.Title = "Please select a photo";
            ofd.Filter = "JPG|*.jpg|PNG|*.png|GIF|*.gif";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.pictureBox1.ImageLocation = ofd.FileName;
                //imgLocation = ofd.FileName.ToString();
                //pictureBox1.ImageLocation = imgLocation;
            }

            //OpenFileDialog open = new OpenFileDialog();
            //PictureBox p = sender as PictureBox;

            //open.Filter = "JPG|*.jpg|PNG|*.png|GIF|*.gif";
            //open.Multiselect = false;
            //if (open.ShowDialog() == DialogResult.OK)
            //{
            //    p.Image = Image.FromFile(open.FileName);
            //}

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "All files|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBrowzer.Text = ofd.FileName;
                txtPic.Text = ofd.SafeFileName;
            }
        }
        
        private void btnImage_Click(object sender, EventArgs e)
        {
         DisplayFileFromServer(new Uri("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text), "OlegKret", "26021982OlegKret");
            //AccountClient accountClient = new AccountClient();
            //var a = accountClient.DownloadFile("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text);

            //pictureBox1.Image = Image.FromFile("" + txtPic.Text);

            //DownloadImage();
            //MessageBox.Show("\nPlease enter a URI:");
            //byte [] a=GetImgByte("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text);
            //   //Image x = (Bitmap)((new ImageConverter()).ConvertFrom(a));
            //   ByteToImage(a);

            //string remoteUri = "ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text;
            //WebClient webClient = new WebClient();

            //webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
            ////    MessageBox.Show("Downloading " + remoteUri);
            //byte[] myDataBuffer = webClient.DownloadData(remoteUri);
            //MemoryStream memoryStream = new MemoryStream(myDataBuffer);
            //pictureBox1.Image = Image.FromStream(memoryStream);
            //MemoryStream ms = new MemoryStream(myDataBuffer);
            ////ms.Write(myDataBuffer, 0, myDataBuffer.Length);
            ////Image image = (Bitmap)(new ImageConverter()).ConvertFrom(myDataBuffer);
            ////ms.ToArray();
            //var bytes = ms.ToArray();
            //var imageMemoryStream = new MemoryStream(bytes);
            //Image imgFromStream = Image.FromStream(imageMemoryStream);


            //byte[] imgdata = System.IO.File.ReadAllBytes(HttpContext.Current.Server.MapPath("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text));
            //byte[] fileContents = File.ReadAllBytes(myDataBuffer);
            //MemoryStream ms = new MemoryStream(DownloadFile("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text));
            //Image img = Image.FromStream(ms);
            //pictureBox1.Image = img;

            //pictureBox1.ImageLocation = (remoteUri);
            //pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            //string download = Encoding.ASCII.GetString(myDataBuffer);
            //pictureBox1.ImageLocation = Encoding.UTF8.GetString(myDataBuffer);
            //MessageBox.Show(download);
            //DownloadFile("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text);
            //MemoryStream ms = new MemoryStream();
            //pictureBox1.Image = Image.FromStream( DownloadFile("ftp://olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text));
            //pictureBox1.Image = ByteToImage(myDataBuffer);
        }

        public bool DisplayFileFromServer(Uri serverUri, string login, string password)
        {
            // The serverUri parameter should start with the ftp:// scheme.
            if (serverUri.Scheme != Uri.UriSchemeFtp)
            {
                return false;
            }
            // Get the object used to communicate with the server. 
            WebClient request = new WebClient();

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential(login, password);
            try
            {
                byte[] newFileData = request.DownloadData(serverUri.ToString());
                //string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                //MessageBox.Show(fileString);
                byte[] imageSource = newFileData;
                Bitmap image;
                using (MemoryStream stream = new MemoryStream(imageSource))
                {
                    image = new Bitmap(stream);
                }

                pictureBox1.Image = image;
            }
            catch (WebException e)
            {
                MessageBox.Show(e.ToString());
            }
            return true;
        }

        //private static byte[]DownloadFile(string url)
        //{
        //    byte[] result = null;
        //    using (WebClient webClient = new WebClient())
        //    {
        //        webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
        //        result = webClient.DownloadData(url);
        //    }
        //    return result;
        //}

        public Image ByteToImage(byte [] blob)
        {
            
            // MemoryStream memoryStream = new MemoryStream(blob);
            // //byte[] pdata = blob;
            // //memoryStream.Write(pdata, 0, Convert.ToInt32(pdata.Length));
            //Image bm = Image.FromStream(memoryStream);
            // //memoryStream.Dispose();
            // return bm;
            //using (var ms = new MemoryStream(blob))
            //{
            //    return Image.FromStream(ms);
            //}
            
                MemoryStream ms = new MemoryStream(blob, 0, blob.Length);
                ms.Write(blob, 0, blob.Length);
                Image returnImage = Image.FromStream(ms, true);
                
          
            return returnImage;
        }

        //public static Bitmap ByteToImg(byte[] blob)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    //byte[] pdata = blob;
        //    memoryStream.Write(blob, 0, Convert.ToInt32(blob.Length));
        //    Bitmap bm = new Bitmap(blob.Length);
        //    memoryStream.Dispose();
        //    return bm;
        //}

        public byte [] GetImgByte (string ftpFilePath)
        {
            WebClient webClient = new WebClient();
            webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
            //    MessageBox.Show("Downloading " + remoteUri);
            byte[] myDataBuffer = webClient.DownloadData(ftpFilePath);
            return myDataBuffer;

        }

        AccountClient client = new AccountClient();
        private void btnNew_Click_Click(object sender, EventArgs e)
        {

            //pictureBox1.Image = Image.FromStream(client.GetStream());
            
        }

        private void btn_Image_Click(object sender, EventArgs e)
        {
            //OpenFileDialog fd = new OpenFileDialog();
            //fd.Filter = "All files|*.*";
            //if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    //txtBrowzer.Text = ofd.FileName;
            //    txtPic.Text = fd.FileName;
            //}

            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/"+txtPic.Text);
            //request.Method = WebRequestMethods.Ftp.UploadFile;

            //// This example assumes the FTP site uses anonymous logon.
            //request.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

            //// Copy the contents of the file to the request stream.
            //byte[] fileContents;
            //using (StreamReader sourceStream = new StreamReader(@"C:\Users\MegaBoss\Pictures\Viber\"+txtPic.Text))
            //{
            //    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            //}

            //request.ContentLength = fileContents.Length;

            //using (Stream requestStream = request.GetRequestStream())
            //{
            //    requestStream.Write(fileContents, 0, fileContents.Length);
            //}

            //using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            //{
            //    MessageBox.Show($"Upload File Complete, status {response.StatusDescription}");
            //}
            //AccountClient accountClient = new AccountClient();

            //accountClient.UploadFile(txtBrowzer.Text, txtPic.Text);

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.olegkret.somee.com/www.OlegKret.somee.com/BOOOOK/App_Data/Users/Photo/" + txtPic.Text);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

            // Copy the contents of the file to the request stream.
            byte[] fileContents=File.ReadAllBytes(ofd.FileName);
          
            
            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                MessageBox.Show($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
    }

    [Serializable]
    internal class EmptyPasswordException : Exception
    {
        public EmptyPasswordException()
        {
        }

        public EmptyPasswordException(string message) : base(message)
        {
        }

        public EmptyPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    internal class EmptyLoginException : Exception
    {
        public EmptyLoginException()
        {
        }

        public EmptyLoginException(string message) : base(message)
        {
        }

        public EmptyLoginException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EmptyLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        public byte[] ConvertFiltoBite(string sPath)
        {
            byte[] data = null;
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fStream);
            data = br.ReadBytes((int)numBytes);
            return data;
        }
        AccountClient client = new AccountClient();
        public void ScreenImage(string baseAddress)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.StreamedResponse;
            binding.MaxReceivedMessageSize = 1024 * 1024 * 2;
            client = new AccountClient(binding, new EndpointAddress(baseAddress));

        }
    }
}


