using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Windows.Forms;
using WcfServiceBooks.DAL;

namespace WcfServiceBooks
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service2.svc or Service2.svc.cs at the Solution Explorer and start debugging.
    public class Service2 : IAccount
    {
        
       string host = "ftp://OlegKret.somee.com/www.OlegKret.somee.com/WCFServiceBooks/App_Data/Users/Photo/";
        private FtpWebRequest ftpRequest = null;
        private FtpWebResponse ftpResponse = null;
        private Stream ftpStream = null;
       
        public BookLotEntities Context { get; } = new BookLotEntities();
        protected DbSet<User> Table;
        //string localpath = "ftp://OlegKret.somee.com/www.OlegKret.somee.com/WCFServiceBooks/App_Data/Users/Photo/";

        public User Login(string login, string password)
        {
            BookLotEntities context = new BookLotEntities();

            var user = context.Users.FirstOrDefault(item => item.Login == login);

            if (user != null && user.Password == password)
            {
                // System.IO.File.ReadAllBytes(Properties.Settings.Default.localPath + user.PhotoName);

                return user;
            }
            else
            {
                return null;
            }
        }

        public int Add(User entity)
        {
            Table.Add(entity);
            return SaveChanges();
        }

        internal int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
            catch (CommitFailedException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool Register(User buf)
        {
            BookLotEntities context = new BookLotEntities();

            var user = context.Users.FirstOrDefault(item => item.Login == buf.Login);

            if (user != null)
            {
                return false;
            }
            //else
            //{
            //    if (buf.Photo != null && buf.PhotoName != null)
            //    {
            //        FtpWebRequest requestFind = (FtpWebRequest)WebRequest.Create(localpath);
            //        requestFind.Method = WebRequestMethods.Ftp.ListDirectory;
            //        requestFind.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

            //        FtpWebResponse responseFind = (FtpWebResponse)requestFind.GetResponse();
            //        Stream streamFind = responseFind.GetResponseStream();
            //        StreamReader readerFind = new StreamReader(streamFind);

            //        string files = readerFind.ReadToEnd();

            //        readerFind.Close();
            //        streamFind.Close();
            //        responseFind.Close();

            //        if (files.Contains(buf.PhotoName))
            //        {
            //            return false;
            //        }
            //        else
            //        {
            //            FtpWebRequest requestCreate = (FtpWebRequest)WebRequest.Create(localpath + buf.PhotoName);
            //            requestCreate.Method = WebRequestMethods.Ftp.UploadFile;
            //            requestCreate.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

            //            Stream streamCreate = requestCreate.GetRequestStream();
            //            streamCreate.Write(buf.Photo, 0, buf.Photo.Length);
            //            streamCreate.Close();

            //            FtpWebResponse responseCreate = (FtpWebResponse)requestCreate.GetResponse();
            //            responseCreate.Close();
            //        }
            //    }
           
            context.Users.Add(buf);

            context.SaveChanges();

            return true;
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            try
            {
                ftpRequest = (FtpWebRequest)FtpWebRequest.Create(host + "/" + remoteFile);
                ftpRequest.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
                ftpRequest.UseBinary = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpStream = ftpRequest.GetRequestStream();
                FileStream lfs = new FileStream(localFile, FileMode.Open);
                byte[] bytebuffer = new byte[(int)lfs.Length];
                int bytesSend = lfs.Read(bytebuffer, 0, (int)lfs.Length);
                try
                {
                    while (bytesSend != -1)
                    {
                        ftpStream.Write(bytebuffer, 0, bytesSend);
                        bytesSend = lfs.Read(bytebuffer, 0, (int)lfs.Length);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                ftpResponse.Close();
                ftpStream.Close();
                lfs.Close();
                ftpRequest = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //public byte[] GetImage(string imageName)
        //{
        //    FtpWebRequest requestFind = (FtpWebRequest)WebRequest.Create(localpath);
        //    requestFind.Method = WebRequestMethods.Ftp.ListDirectory;
        //    requestFind.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

        //    FtpWebResponse responseFind = (FtpWebResponse)requestFind.GetResponse();
        //    Stream streamFind = responseFind.GetResponseStream();
        //    StreamReader readerFind = new StreamReader(streamFind);

        //    string files = readerFind.ReadToEnd();

        //    readerFind.Close();
        //    streamFind.Close();
        //    responseFind.Close();

        //    if (files.Contains(imageName))
        //    {
        //        FtpWebRequest requestGet = (FtpWebRequest)WebRequest.Create(localpath + imageName);
        //        requestGet.Method = WebRequestMethods.Ftp.DownloadFile;
        //        requestGet.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");

        //        FtpWebResponse responseGet = (FtpWebResponse)requestGet.GetResponse();
        //        Stream streamGet = responseGet.GetResponseStream();

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            streamGet.CopyTo(ms);
        //            return ms.ToArray();
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}

        //public byte[]ConvertFiltoByte(string sPath)
        //{
        //    byte[] data = null;
        //    FileInfo fInfo = new FileInfo(sPath);
        //    long numBytes = fInfo.Length;
        //    FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fStream);
        //    data = br.ReadBytes((int)numBytes);
        //    return data;
        //}

        public int AddRange(IList<User> entities)
        {
            Table.AddRange(entities);
            return SaveChanges();
        }
        public int Save(User entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public int Delete(User entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }

        public User GetOne(int? id) => Table.Find(id);

        public List<User> GetAll() => Table.ToList();

        public List<User> ExecuteQuery(string sql) => Table.SqlQuery(sql).ToList();

        public int Delete(int id)
        {
            Context.Entry(new User()
            {
                Id = id


            }).State = EntityState.Deleted;
            return SaveChanges();
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

        public Stream GetStream()
        {
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            using (Bitmap bmp = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Position = 0;
                return ms;
            }
        }

        public Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream memoryStream = new MemoryStream();

            memoryStream.Write(blob, 0, Convert.ToInt32(blob.Length));
            Bitmap bm = new Bitmap(memoryStream, false);
            memoryStream.Dispose();
            return bm;
            //MemoryStream ms = new MemoryStream(blob, 0, blob.Length);
            //ms.Write(blob, 0, blob.Length);
            //Image returnImage = Image.FromStream(ms, true);


            //return returnImage;
        }

        public byte[] DownloadFile(string url)
        {
            byte[] result = null;
            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential("OlegKret", "26021982OlegKret");
                result = webClient.DownloadData(url);
            }
            return result;
        }
    }
}


