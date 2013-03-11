using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Threading;

namespace Goodfornothing.AesCrypter.ComponentModel
{
    public enum TargetType
    {
        File, Directory, String
    }

    public enum InpuMode
    {
        Key, Password
    }

    public enum EncodeType
    {
        Base64, UTF8, Binary
    }

    public class MainWindowViewModel : ViewModelBase
    {
        #region Private fileds

        private Thread cryptoThread = null;

        private bool canUserOperation = true;

        private string password = string.Empty;

        private string passwordSalt = string.Empty;

        private string keyValue = string.Empty;

        private string iv = string.Empty;

        private string decrypt = string.Empty;

        private string encrypt = string.Empty;

        private TargetType targetType;

        private InpuMode inpuMode = InpuMode.Password;

        private EncodeType keyEncodeType = EncodeType.Base64;

        private EncodeType ivEncodeType = EncodeType.Base64;

        private EncodeType outputEncodeType = EncodeType.Base64;

        private CipherMode cipherMode = CipherMode.CBC;

        private PaddingMode paddingMode = PaddingMode.PKCS7;

        private ReadOnlyObservableCollection<PaddingMode> paddingModes;

        private ReadOnlyObservableCollection<EncodeType> encodeTypes;

        private ReadOnlyObservableCollection<CipherMode> cipherModes;

        private int keySize = 128;

        private int blockSize = 128;

        #endregion

        #region Public properties

        public bool CanUserOperation
        {
            get { return this.canUserOperation; }
            set
            {
                if (this.canUserOperation != value)
                {
                    this.canUserOperation = value;
                    base.OnPropertyChanged(() => this.CanUserOperation);
                }
            }
        }

        public string Password
        {
            get { return this.password; }
            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    base.OnPropertyChanged(() => this.Password);
                }
            }
        }

        public string PasswordSalt
        {
            get { return this.passwordSalt; }
            set
            {
                if (this.passwordSalt != value)
                {
                    this.passwordSalt = value;
                    base.OnPropertyChanged(() => this.PasswordSalt);
                }
            }
        }

        public string KeyValue
        {
            get { return this.keyValue; }
            set
            {
                if (this.keyValue != value)
                {
                    this.keyValue = value;
                    base.OnPropertyChanged(() => this.KeyValue);
                }
            }
        }

        public string IV
        {
            get { return this.iv; }
            set
            {
                if (this.iv != value)
                {
                    this.iv = value;
                    base.OnPropertyChanged(() => this.IV);
                }
            }
        }

        public string Decrypt
        {
            get { return this.decrypt; }
            set
            {
                if (this.decrypt != value)
                {
                    this.decrypt = value;
                    base.OnPropertyChanged(() => this.Decrypt);
                }
            }
        }

        public string Encrypt
        {
            get { return this.encrypt; }
            set
            {
                if (this.encrypt != value)
                {
                    this.encrypt = value;
                    base.OnPropertyChanged(() => this.Encrypt);
                }
            }
        }

        public TargetType TargetType
        {
            get { return this.targetType; }
            set
            {
                if (this.targetType != value)
                {
                    this.targetType = value;
                    base.OnPropertyChanged(() => this.TargetType);
                }
            }
        }

        public InpuMode InpuMode
        {
            get { return this.inpuMode; }
            set
            {
                if (this.inpuMode != value)
                {
                    this.inpuMode = value;
                    base.OnPropertyChanged(() => this.InpuMode);

                    switch (this.InpuMode)
                    {
                        case InpuMode.Key:
                            this.Password = this.PasswordSalt = string.Empty;
                            break;
                        case InpuMode.Password:
                            this.KeyValue = this.IV = string.Empty;
                            break;
                        default: break;
                    }
                }
            }
        }

        public EncodeType KeyEncodeType
        {
            get { return this.keyEncodeType; }
            set
            {
                if (this.keyEncodeType != value)
                {
                    this.keyEncodeType = value;
                    base.OnPropertyChanged(() => this.KeyEncodeType);
                }
            }
        }

        public EncodeType IvEncodeType
        {
            get { return this.ivEncodeType; }
            set
            {
                if (this.ivEncodeType != value)
                {
                    this.ivEncodeType = value;
                    base.OnPropertyChanged(() => this.IvEncodeType);
                }
            }
        }

        public EncodeType OutputEncodeType
        {
            get { return this.outputEncodeType; }
            set
            {
                if (this.outputEncodeType != value)
                {
                    this.outputEncodeType = value;
                    base.OnPropertyChanged(() => this.OutputEncodeType);
                }
            }
        }

        public CipherMode CipherMode
        {
            get { return this.cipherMode; }
            set
            {
                if (this.cipherMode != value)
                {
                    this.cipherMode = value;
                    base.OnPropertyChanged(() => this.CipherMode);
                }
            }
        }

        public PaddingMode PaddingMode
        {
            get { return this.paddingMode; }
            set
            {
                if (this.paddingMode != value)
                {
                    this.paddingMode = value;
                    base.OnPropertyChanged(() => this.PaddingMode);
                }
            }
        }

        public int KeySize
        {
            get { return this.keySize; }
            set
            {
                if (this.keySize != value)
                {
                    this.keySize = value;
                    base.OnPropertyChanged(() => this.KeySize);
                }
            }
        }

        public int BlockSize
        {
            get { return this.blockSize; }
            set
            {
                if (this.blockSize != value)
                {
                    this.blockSize = value;
                    base.OnPropertyChanged(() => this.BlockSize);
                }
            }
        }

        public ReadOnlyObservableCollection<EncodeType> EncodeTypes
        {
            get
            {
                return this.encodeTypes = this.encodeTypes ??
                    new ReadOnlyObservableCollection<EncodeType>(
                        new ObservableCollection<EncodeType>()
                        {
                            EncodeType.Base64, EncodeType.UTF8
                        });
            }
        }

        public ReadOnlyObservableCollection<CipherMode> CipherModes
        {
            get
            {
                return this.cipherModes = this.cipherModes ??
                    new ReadOnlyObservableCollection<CipherMode>(
                        new ObservableCollection<CipherMode>()
                        {
                            CipherMode.CBC, CipherMode.CFB, CipherMode.CTS, CipherMode.ECB, 
                            CipherMode.OFB
                        });
            }
        }

        public ReadOnlyObservableCollection<PaddingMode> PaddingModes
        {
            get
            {
                return this.paddingModes = this.paddingModes ??
                    new ReadOnlyObservableCollection<PaddingMode>(
                        new ObservableCollection<PaddingMode>()
                        {
                            PaddingMode.ANSIX923, PaddingMode.ISO10126, PaddingMode.None,
                            PaddingMode.PKCS7, PaddingMode.Zeros
                        });
            }
        }
        
        #endregion

        #region Command

        public ICommand EncryptCommand { get; private set; }

        public ICommand DecryptCommand { get; private set; }

        public ICommand DecryptReferenceCommand { get; private set; }

        public ICommand EncryptReferenceCommand { get; private set; }

        #endregion

        public MainWindowViewModel(Dispatcher dispatcher)
            : base(dispatcher)
        {
            this.EncryptReferenceCommand =
                new DelegateCommand(
                    this.EncryptReferenceCommandExcecute,
                    ()=>
                        this.TargetType == ComponentModel.TargetType.Directory ||
                        this.TargetType == ComponentModel.TargetType.File);
            this.DecryptReferenceCommand =
                new DelegateCommand(
                    this.DecryptReferenceCommandExcecute,
                    ()=>
                        this.TargetType == ComponentModel.TargetType.Directory ||
                        this.TargetType == ComponentModel.TargetType.File);

            this.DecryptCommand = new DelegateCommand(this.DecryptCommandExcecute);
            this.EncryptCommand = new DelegateCommand(this.EncryptCommandExcecute);
        }

        #region Command execute methods

        private void EncryptReferenceCommandExcecute()
        {
            if (this.TargetType == ComponentModel.TargetType.File)
            {
                var dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    this.Encrypt = dialog.FileName;
                    if (System.IO.Path.GetExtension(dialog.FileName) == ".aes")
                    {
                        this.Decrypt =
                            System.IO.Path.ChangeExtension(dialog.FileName, "plane");
                    }
                }
            }
            else if (this.TargetType == ComponentModel.TargetType.Directory)
            {
                var dialog = new CommonOpenFileDialog();

                dialog.EnsureReadOnly = true;
                dialog.IsFolderPicker = true;
                dialog.AllowNonFileSystemItems = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.Decrypt = this.Encrypt = dialog.FileName;
                }
            }
        }

        private void DecryptReferenceCommandExcecute()
        {
            if (this.TargetType == ComponentModel.TargetType.File)
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    this.Decrypt = dialog.FileName;
                    this.Encrypt = dialog.FileName + ".aes";
                }
            }
            else if (this.TargetType == ComponentModel.TargetType.Directory)
            {
                var dialog = new CommonOpenFileDialog();

                dialog.EnsureReadOnly = true;
                dialog.IsFolderPicker = true;
                dialog.AllowNonFileSystemItems = false;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    this.Decrypt = this.Encrypt = dialog.FileName;
                }
            }
        }

        private void EncryptCommandExcecute()
        {
            AesManaged aes = null;
            try
            {
                aes = this.CreateAesManaged();
            }
            catch (Exception e)
            {
                MessageBox.Show("おやぁ？" + Environment.NewLine + e.Message);
                return;
            }

            using (aes)
            {
                this.CanUserOperation = false;

                Dictionary<string, string> fileSets = new Dictionary<string, string>();

                if (this.TargetType == ComponentModel.TargetType.File)
                {
                    fileSets[this.Decrypt] = this.Encrypt;
                }
                else if (this.TargetType == ComponentModel.TargetType.Directory)
                {
                    if (!Directory.Exists(this.Encrypt))
                    {
                        Directory.CreateDirectory(this.Encrypt);
                    }

                    string inputDirectory = this.Decrypt;
                    string outputDirectory = this.Encrypt;

                    foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
                    {
                        fileSets[filePath] =
                            string.Format("{0}/{1}.aes", outputDirectory, System.IO.Path.GetFileName(filePath));
                    }
                }
                else if (this.TargetType == ComponentModel.TargetType.String)
                {
                    string message = string.Empty;
                    try
                    {
                        string decryptParam = string.Empty;
                        using (var readStream = new MemoryStream(Encoding.UTF8.GetBytes(this.Decrypt)))
                        using (var writeStream = new MemoryStream())
                        using (var cryptStream = new CryptoStream(readStream, aes.CreateEncryptor(), CryptoStreamMode.Read))
                        {
                            var readBuffer = new byte[256];
                            int readLength = 0;
                            while ((readLength = cryptStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                writeStream.Write(readBuffer, 0, readLength);
                            }

                            if (this.OutputEncodeType == EncodeType.Binary)
                            {
                                this.Encrypt = BitConverter.ToString(writeStream.ToArray()).Replace("-", "");
                            }
                            else if (this.OutputEncodeType == EncodeType.UTF8)
                            {
                                this.Encrypt = Encoding.UTF8.GetString(writeStream.ToArray());
                            }
                            else if (this.OutputEncodeType == EncodeType.Base64)
                            {
                                this.Encrypt = Convert.ToBase64String(writeStream.ToArray());
                            }
                        }

                        message = "暗号化が終了しました。";
                    }
                    catch (Exception ex)
                    {
                        message = "暗号化に失敗しました" + System.Environment.NewLine + ex.Message;
                    }

                    this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show(message); }), null);

                    this.CanUserOperation = true;
                    return;
                }

                this.cryptoThread = new Thread((o) =>
                {
                    string message = string.Empty;
                    try
                    {
                        // フォルダ内のファイル全てを暗号化
                        foreach (var fileSet in fileSets)
                        {
                            MainWindowViewModel.Crypto(o as ICryptoTransform, fileSet.Key, fileSet.Value);
                        }

                        message = "暗号化が終了しました。";
                    }
                    catch (Exception ex)
                    {
                        message = "暗号化に失敗しました" + System.Environment.NewLine + ex.Message;
                    }

                    // TODO : UIスレッドで実行
                    // メッセージを飛ばすべき
                    this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show(message); }), null);

                    this.CanUserOperation = true;
                });

                this.cryptoThread.Start(aes.CreateEncryptor());
            }
        }

        private void DecryptCommandExcecute()
        {
            AesManaged aes = null;
            try
            {
                aes = this.CreateAesManaged();
            }
            catch (Exception e)
            {
                MessageBox.Show("おやぁ？" + Environment.NewLine + e.Message);
                return;
            }

            using (aes)
            {
                this.CanUserOperation = false;

                Dictionary<string, string> fileSets = new Dictionary<string, string>();

                if (this.TargetType == ComponentModel.TargetType.File)
                {
                    fileSets[this.Encrypt] = this.Decrypt;
                }
                else if (this.TargetType == ComponentModel.TargetType.Directory)
                {
                    if (!Directory.Exists(this.Decrypt))
                    {
                        Directory.CreateDirectory(this.Encrypt);
                    }

                    string inputDirectory = this.Encrypt;
                    string outputDirectory = this.Decrypt;

                    foreach (var filePath in Directory.EnumerateFiles(inputDirectory))
                    {
                        fileSets[filePath] =
                            string.Format("{0}/{1}.plane", outputDirectory, System.IO.Path.GetFileNameWithoutExtension(filePath));
                    }
                }
                else if (this.TargetType == ComponentModel.TargetType.String)
                {
                    string message = string.Empty;
                    try
                    {
                        byte[] paramBytes = null;
                        if (this.OutputEncodeType == EncodeType.Binary)
                        {
                            paramBytes =
                                Enumerable
                                    .Range(0, (this.Encrypt.Length + ((this.Encrypt.Length % 2 == 0) ? 0 : 1)) / 2)
                                    .Select(o => this.Encrypt.Substring(o * 2, 2))
                                    .Select((o, i) => Convert.ToByte(o, 16))
                                    .ToArray();
                        }
                        else if (this.OutputEncodeType == EncodeType.UTF8)
                        {
                            paramBytes = Encoding.UTF8.GetBytes(this.Encrypt);
                        }
                        else if (this.OutputEncodeType == EncodeType.Base64)
                        {
                            paramBytes = Convert.FromBase64String(this.Encrypt);
                        }

                        using (var readStream = new MemoryStream(paramBytes))
                        using (var writeStream = new MemoryStream())
                        using (var cryptStream = new CryptoStream(readStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            var readBuffer = new byte[256];
                            int readLength = 0;
                            while ((readLength = cryptStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                writeStream.Write(readBuffer, 0, readLength);
                            }

                            this.Decrypt = Encoding.UTF8.GetString(writeStream.ToArray(), 0, (int)writeStream.Length);
                        }

                        message = "復号が終了しました。";
                    }
                    catch (Exception ex)
                    {
                        message = "復号に失敗しました" + System.Environment.NewLine + ex.Message;
                    }

                    this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show(message); }), null);

                    this.CanUserOperation = true;
                    return;
                }

                this.cryptoThread = new Thread((o) =>
                {
                    string message = string.Empty;
                    try
                    {
                        // フォルダ内のファイル全てを暗号化
                        foreach (var fileSet in fileSets)
                        {
                            MainWindowViewModel.Crypto(o as ICryptoTransform, fileSet.Key, fileSet.Value);
                        }

                        message = "復号が終了しました。";
                    }
                    catch (Exception ex)
                    {
                        message = "復号に失敗しました" + System.Environment.NewLine + ex.Message;
                    }


                    // TODO : UIスレッドで実行
                    this.Dispatcher.BeginInvoke((Action)(() => { MessageBox.Show(message); }), null);

                    this.CanUserOperation = true;
                });

                this.cryptoThread.Start(aes.CreateDecryptor());
            }
        }
        
        #endregion

        private static void Crypto(ICryptoTransform cryptoTransform, string inputFilePath, string outputFilePath)
        {
            using (var readStream = new FileStream(inputFilePath, FileMode.Open))
            using (var writeStream = new FileStream(outputFilePath, FileMode.Create))
            using (var cryptoStream = new CryptoStream(readStream, cryptoTransform, CryptoStreamMode.Read))
            {
                MainWindowViewModel.Copy(cryptoStream, writeStream);
            }
        }

        /// <summary>
        /// ストリームの内容を読んで書きます
        /// </summary>
        /// <param name="readStream"></param>
        /// <param name="writeStream"></param>
        private static void Copy(Stream readStream, Stream writeStream)
        {
            var readBuffer = new byte[1024];
            int readSize = 0;
            while ((readSize = readStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                writeStream.Write(readBuffer, 0, readSize);
            }
        }

        /// <summary>
        /// <see cref="System.Security.Cryptography.AesManaged"/> のインスタンスを初期化します。
        /// </summary>
        /// <returns></returns>
        private AesManaged CreateAesManaged()
        {
            var aes = new AesManaged();

            aes.Mode = this.CipherMode;
            aes.Padding = this.PaddingMode;

            if (this.InpuMode == ComponentModel.InpuMode.Password)
            {
                // キー生成
                Rfc2898DeriveBytes rfc2898 =
                    new Rfc2898DeriveBytes(this.Password, Encoding.UTF8.GetBytes(this.PasswordSalt));

                aes.KeySize = this.KeySize;
                aes.BlockSize = this.BlockSize;

                aes.IV = rfc2898.GetBytes(aes.BlockSize / 8);
                aes.Key = rfc2898.GetBytes(aes.KeySize / 8);

                this.IV =
                    this.IvEncodeType == EncodeType.Base64 ?
                        Convert.ToBase64String(aes.IV) :
                    this.IvEncodeType == EncodeType.UTF8 ?
                        Encoding.UTF8.GetString(aes.IV) : null;

                this.KeyValue =
                    this.KeyEncodeType == EncodeType.Base64 ?
                        Convert.ToBase64String(aes.Key) :
                    this.KeyEncodeType == EncodeType.UTF8 ?
                        Encoding.UTF8.GetString(aes.Key) : null;
            }
            else if (this.InpuMode == ComponentModel.InpuMode.Key)
            {
                aes.IV =
                    this.IvEncodeType == EncodeType.Base64 ?
                        Convert.FromBase64String(this.IV) :
                    this.IvEncodeType == EncodeType.UTF8 ?
                        Encoding.UTF8.GetBytes(this.IV) : null;

                aes.Key =
                    this.KeyEncodeType == EncodeType.Base64 ?
                        Convert.FromBase64String(this.KeyValue) :
                    this.KeyEncodeType == EncodeType.UTF8 ?
                        Encoding.UTF8.GetBytes(this.KeyValue) : null;

                this.KeySize = aes.KeySize;
                this.BlockSize = aes.BlockSize;
            }

            return aes;
        }
    }
}
