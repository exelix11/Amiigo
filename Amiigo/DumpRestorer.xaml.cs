using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Pcsc;
using System.Threading.Tasks;
using Pcsc.Common;
using Windows.UI.Popups;
using Windows.Storage.Streams;
using Windows.Foundation.Metadata;
using Windows.UI.Core;


namespace Amiigo
{
    public sealed partial class DumpRestorer : Page
    {
        public DumpRestorer()
        {
            this.InitializeComponent();
        }
        byte[] AmiiboDump = new byte[0x210];
        byte[] AmiiboUID = new byte[0x7];
        byte[] AmiiboPass;
        SmartCardReader m_cardReader;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };
            if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (s, a) =>
                {
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                        a.Handled = true;
                    }
                };
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (m_cardReader != null)
            {
                m_cardReader.CardAdded -= cardReader_CardAdded;
                m_cardReader.CardRemoved -= cardReader_CardRemoved;
                m_cardReader = null;
            }

            base.OnNavigatingFrom(e);
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            var deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Nfc);
            if (deviceInfo == null)
            {
                deviceInfo = await SmartCardReaderUtils.GetFirstSmartCardReaderInfo(SmartCardReaderKind.Any);
            }

            if (deviceInfo == null)
            {
                MessageDialog msg = new MessageDialog("NFC card reader mode not supported on this device");
                await msg.ShowAsync();
                return;
            }
            if (m_cardReader == null)
            {
                button1.IsEnabled = false;
                button1.Content = "Tap your amiibo to continue !";
                m_cardReader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                m_cardReader.CardAdded += cardReader_CardAdded;
                m_cardReader.CardRemoved += cardReader_CardRemoved;
            }
        }

        private void cardReader_CardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
        {
            LogMessage("Card removed");
        }

        private async void cardReader_CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            await HandleCard(args.SmartCard);
        }
        SmartCardConnection connection;
        bool BinaryDump = false;
        private async Task HandleCard(SmartCard card)
        {
            try
            {
                connection = await card.ConnectAsync();
                IccDetection cardIdentification = new IccDetection(card, connection);
                await cardIdentification.DetectCardTypeAync();
                LogMessage("Connected to card\r\nPC/SC device class: " + cardIdentification.PcscDeviceClass.ToString());
                LogMessage("Card name: " + cardIdentification.PcscCardName.ToString());
                LogMessage("ATR: " + BitConverter.ToString(cardIdentification.Atr));

                if ((cardIdentification.PcscDeviceClass == Pcsc.Common.DeviceClass.StorageClass) &&
                    (cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightC
                    || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralight
                    || cardIdentification.PcscCardName == Pcsc.CardName.MifareUltralightEV1))
                {
                    MifareUltralight.AccessHandler mifareULAccess = new MifareUltralight.AccessHandler(connection);
                    
                    byte[] responseUid = await mifareULAccess.GetUidAsync();
                    LogMessage("UID:  " + BitConverter.ToString(responseUid));
                    if (!BinaryDump && !ByteArrayEqual(responseUid, AmiiboUID)) //I used this because for some reasons responseUid == AmiiboUID was always false
                    {
                        MessageDialog dialog = new MessageDialog("The dump UID and the amiibo UID don't match");
                        LogMessage("Amiibo UID: " + BitConverter.ToString(responseUid) + " != Dump UID: " + BitConverter.ToString(AmiiboUID));
                        await dialog.ShowAsync();
                        return;
                    }
                    LogMessage("Using key: " + BitConverter.ToString(AmiiboPass));
                    byte[] Authresponse = await mifareULAccess.TransparentExchangeAsync(new byte[] { 0x1B, AmiiboPass[0], AmiiboPass[1], AmiiboPass[2], AmiiboPass[3] }); //PWD_AUTH
                    LogMessage("Auth sent !");
                    LogMessage("Auth Response length: " + Authresponse.Length.ToString());
                    if (Authresponse.Length == 0)
                    {
                        LogMessage("No response from Amiibo, can't restore dump !");
                        MessageDialog dlg = new MessageDialog("No response from Amiibo, wrong password ?");
                        await dlg.ShowAsync();
                        return;
                    }
                    LogMessage("Auth response: " + BitConverter.ToString(Authresponse));
                    //Using page layout from: https://www.3dbrew.org/wiki/Amiibo#Page_layout
                    #region WritePages
                    mifareULAccess.WriteAsync(0x4, getNBytes(AmiiboDump,0x10));
                    LogMessage("Page 0x4 wrote !");
                    mifareULAccess.WriteAsync(0x5, getNBytes(AmiiboDump, 0x14));
                    LogMessage("Page 0x5 wrote !");
                    mifareULAccess.WriteAsync(0x6, getNBytes(AmiiboDump, 0x18));
                    LogMessage("Page 0x6 wrote !");
                    mifareULAccess.WriteAsync(0x7, getNBytes(AmiiboDump, 0x1C));
                    LogMessage("Page 0x7 wrote !");
                    mifareULAccess.WriteAsync(0x8, getNBytes(AmiiboDump, 0x20));
                    LogMessage("Page 0x8 wrote !");
                    mifareULAccess.WriteAsync(0x9, getNBytes(AmiiboDump, 0x24));
                    LogMessage("Page 0x9 wrote !");
                    mifareULAccess.WriteAsync(0xA, getNBytes(AmiiboDump, 0x28));
                    LogMessage("Page 0xA wrote !");
                    mifareULAccess.WriteAsync(0xB, getNBytes(AmiiboDump, 0x2C));
                    LogMessage("Page 0xB wrote !");
                    mifareULAccess.WriteAsync(0xC, getNBytes(AmiiboDump, 0x30));
                    LogMessage("Page 0xC wrote !");
                    mifareULAccess.WriteAsync(0x20, getNBytes(AmiiboDump, 0x80));
                    LogMessage("Page 0x20 wrote !");
                    mifareULAccess.WriteAsync(0x21, getNBytes(AmiiboDump, 0x84));
                    LogMessage("Page 0x21 wrote !");
                    mifareULAccess.WriteAsync(0x22, getNBytes(AmiiboDump, 0x88));
                    LogMessage("Page 0x22 wrote !");
                    mifareULAccess.WriteAsync(0x23, getNBytes(AmiiboDump, 0x8C));
                    LogMessage("Page 0x23 wrote !");
                    mifareULAccess.WriteAsync(0x24, getNBytes(AmiiboDump, 0x90));
                    LogMessage("Page 0x24 wrote !");
                    mifareULAccess.WriteAsync(0x25, getNBytes(AmiiboDump, 0x94));
                    LogMessage("Page 0x25 wrote !");
                    mifareULAccess.WriteAsync(0x26, getNBytes(AmiiboDump, 0x98));
                    LogMessage("Page 0x26 wrote !");
                    mifareULAccess.WriteAsync(0x27, getNBytes(AmiiboDump, 0x9c));
                    LogMessage("Page 0x27 wrote !");//Until here i manually wrote the write addreses to test if the writing works, but i'm too lazy to replace those arleady working functions with a cycle
                    for (int i = 0; i < 0x5A; i++) mifareULAccess.WriteAsync((byte)(0x28/*Page*/ + i), getNBytes(AmiiboDump, 0xA0 /*ADDR of page 0x28*/ + (i*4)));
                    LogMessage("Page 0x28 to 0x81 wrote !");
                    #endregion
                    LogMessage("Dump restored :) !");
                    MessageDialog msg = new MessageDialog("Dump restored !");
                    await msg.ShowAsync();
                }
                else
                {
                    LogMessage("This tag is not an Amiibo !");
                    MessageDialog msg = new MessageDialog("This tag is not an Amiibo !");
                    await msg.ShowAsync();
                    var apduRes = await connection.TransceiveAsync(new Pcsc.GetUid());
                    if (!apduRes.Succeeded)
                    {
                        LogMessage("Failure getting UID of card, " + apduRes.ToString());
                    }
                    else
                    {
                        LogMessage("UID:  " + BitConverter.ToString(apduRes.ResponseData));
                    }
                }
                connection.Dispose();
            }
            catch (Exception ex)
            {
                connection.Dispose();
                LogMessage("Exception handling card: " + ex.ToString());
                MessageDialog msg = new MessageDialog("Exception handling card: " + ex.ToString());
                await msg.ShowAsync();
            }
        }

        string FULLLOG = "";

        async void LogMessage(string data)
        {
            FULLLOG = FULLLOG + data + Environment.NewLine;
            Debug.WriteLine(data);
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textBox1.Text = FULLLOG;
            });
        }


        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked == false)
            {
                MessageDialog msg = new MessageDialog("Please check the checkbox");
                await msg.ShowAsync();
                return;
            }
            BinaryDump = false;
            var Picker = new Windows.Storage.Pickers.FileOpenPicker();
            Picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            Picker.FileTypeFilter.Add(".bin");
            Windows.Storage.StorageFile file = await Picker.PickSingleFileAsync();
            if (file != null)
            {
                IBuffer fulldump = await Windows.Storage.FileIO.ReadBufferAsync(file);
                if (fulldump.Length != 0x22D)
                {
                    MessageDialog msg = new MessageDialog("The selected file is not an amiigo dump, wrong size");
                    await msg.ShowAsync();
                    return;
                }
                byte[] magic = new byte[0xA];
                System.Buffer.BlockCopy(fulldump.ToArray(), 0x00, magic, 0, 0xA);
                if (!ByteArrayEqual(magic, new byte[] { 0x41, 0x6D, 0x69, 0x69, 0x67, 0x6F, 0x44, 0x75, 0x6D, 0x70 }))
                {
                    MessageDialog msg = new MessageDialog("The selected file is not an amiigo dump, missing header");
                    await msg.ShowAsync();
                    return;
                }
                AmiiboUID = new byte[0x7];
                System.Buffer.BlockCopy(fulldump.ToArray(), 0x0E, AmiiboUID, 0, 0x7);
                if (textBox.Text.Trim().Length == 0)
                {
                    textBox.Text = GetPWD(AmiiboUID);
                }
                else if (textBox.Text.Trim().Length != 8)
                {
                    MessageDialog msg = new MessageDialog("The write key must be of 8 characters, leave the field empty to generate it from the dump");
                    await msg.ShowAsync();
                    return;
                }
                AmiiboDump = new byte[0x214];
                System.Buffer.BlockCopy(fulldump.ToArray(), 0x19, AmiiboDump, 0, 0x214);
                AmiiboPass = StringToByteArray(textBox.Text);
                button1.IsEnabled = true;
                button.IsEnabled = false;
                textBox.IsEnabled = false;
                button_Copy.IsEnabled = false;
            }
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        bool ByteArrayEqual(byte[] input1, byte[] input2)
        {
            try
            {
                if (input1.Length != input2.Length) return false;
                for (int i = 0; i < input1.Length; i++)
                {
                    if (input1[i] != input2[i]) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        byte[] getNBytes(byte[] dump, int offset, int lenght = 4)  //get bytes from the dump
        {
            int off = offset;
            if (BinaryDump) off = off + 8;
            List<byte> lst = new List<byte>();
            for (int i = 0; i < lenght; i++) lst.Add(dump[i + off]);
            return lst.ToArray();
        }

        private string GetPWD(byte[] Uid)
        {
                List<byte> Res = new List<byte>();
                int XorVal = 0xaa;
                Res.Add((byte)((Uid[1] ^ Uid[3]) ^ XorVal));
                Res.Add((byte)((Uid[2] ^ Uid[4]) ^ XorVal / 2));
                Res.Add((byte)((Uid[3] ^ Uid[5]) ^ XorVal));
                Res.Add((byte)((Uid[4] ^ Uid[6]) ^ XorVal / 2));
                return BitConverter.ToString(Res.ToArray()).Replace("-", "");           
        }

        private async void buttonBIN_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox.IsChecked == false)
            {
                MessageDialog msg = new MessageDialog("Please check the checkbox");
                await msg.ShowAsync();
                return;
            }
            BinaryDump = true;
            var Picker = new Windows.Storage.Pickers.FileOpenPicker();
            Picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            Picker.FileTypeFilter.Add(".bin");
            Windows.Storage.StorageFile file = await Picker.PickSingleFileAsync();
            if (file != null)
            {
                IBuffer fulldump = await Windows.Storage.FileIO.ReadBufferAsync(file);
                if (fulldump.Length != 540)
                {
                    MessageDialog msg = new MessageDialog("The selected file is not an amiibo binary dump, wrong size");
                    await msg.ShowAsync();
                    return;
                }
                if (textBox.Text.Trim().Length != 8)
                {
                    MessageDialog msg = new MessageDialog("The write key must be of 8 characters");
                    await msg.ShowAsync();
                    return;
                }
                AmiiboDump = new byte[0x214];
                System.Buffer.BlockCopy(fulldump.ToArray(), 0, AmiiboDump, 0, 0x214);
                AmiiboPass = StringToByteArray(textBox.Text);
                button1.IsEnabled = true;
                button.IsEnabled = false;
                textBox.IsEnabled = false;
                button_Copy.IsEnabled = false;
            }
        }
    }
}
