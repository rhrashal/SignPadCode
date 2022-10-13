using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;
//using FP430SAPI;

namespace test
{
    //unsafe delegate void MemCpyImpl(byte* src, byte* dest, int len);
    public partial class Form1 : Form
    {
        [DllImport("FP430S.dll", EntryPoint = "Lib_GetSignature", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]   // 
        unsafe public static extern int Lib_GetSignature(int encryptType, byte* outputData, int timeout);

        [DllImport("FP430S.dll", EntryPoint = "USB_Open", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int USB_Open();

        [DllImport("FP430S.dll", EntryPoint = "USB_Close", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int USB_Close();

        [DllImport("FP430S.dll", EntryPoint = "Lib_Alignment", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Lib_Alignment(int timeout);

        [DllImport("FP430S.dll", EntryPoint = "Lib_TransPic", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Lib_TransPic(string srcPic, string dstPic);


        Image GuestImage;
        public Form1()
        {
            InitializeComponent();
        }

        unsafe private void Sign_Click(object sender, EventArgs e)
        {
            int ret = USB_Close();
            ret = USB_Open();

            byte[] signData = new byte[1024 * 400];


            fixed (byte* p = &signData[0])
            {
                ret = Lib_GetSignature(0, p, 60000);
            }


            int picLen;
            int dataLen;

            byte[] dataBuf;
            byte[] picBuf;

            string basePah;


            dataLen = signData[0] * 256 * 256 + signData[1] * 256 + signData[2];
            picLen = signData[3] * 256 * 256 + signData[4] * 256 + signData[5];

            picBuf = new byte[picLen];
            dataBuf = new byte[dataLen];

            Array.ConstrainedCopy(signData, 6, dataBuf, 0, dataLen);
            Array.ConstrainedCopy(signData, 6 + dataLen, picBuf, 0, picLen);


            basePah = System.Environment.CurrentDirectory;
          


            pictureBox1.BackgroundImage = ByteToImage(picBuf);
            GuestImage = ByteToImage(picBuf);
           
        }

        public Image ByteToImage(byte[] byteArrayIn)
        {


            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public bool BinaryToFile(Byte[] byTmp, string path)
        {
            FileStream fs;
            BinaryWriter bwr;

            fs = new FileStream(path, FileMode.Create);
            bwr = new BinaryWriter(fs);

            bwr.Write(byTmp);
            bwr.Flush();

            bwr.Close();
            fs.Close();
            fs.Dispose();

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            byte[] img = ImageToByte(GuestImage);

        }

        public byte[] ImageToByte(Image imageIn)
        {

            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }


    }
}
