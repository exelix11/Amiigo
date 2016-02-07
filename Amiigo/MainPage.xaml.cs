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
using Windows.UI.Core;
using System.Threading;
using System.ComponentModel;

namespace Amiigo
{
    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            //DataContext = this;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            base.OnNavigatedTo(e);
        }

        private async void button_Click(object sender, RoutedEventArgs e)
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
                button2.IsEnabled = true;
                button.IsEnabled = false;
                button.Content = "Amiigo started, Tap your amiibo ";
                m_cardReader = await SmartCardReader.FromIdAsync(deviceInfo.Id);
                m_cardReader.CardAdded += cardReader_CardAdded;
                m_cardReader.CardRemoved += cardReader_CardRemoved;
            }
        }

        SmartCardReader m_cardReader;

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

        private void cardReader_CardRemoved(SmartCardReader sender, CardRemovedEventArgs args)
        {
            LogMessage("Card removed");
        }

        private async void cardReader_CardAdded(SmartCardReader sender, CardAddedEventArgs args)
        {
            await HandleCard(args.SmartCard);
        }

        byte[] Dump;
        SmartCardConnection connection;
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
                    List<byte> dumpTmp = new List<byte>();

                    byte[] responseUid = await mifareULAccess.GetUidAsync();
                    dumpTmp.AddRange(new byte[] { 0x41, 0x6D, 0x69, 0x69, 0x67, 0x6F, 0x44, 0x75, 0x6D, 0x70, 0x00, 0x00, 0x00, 0x00 }); //MAGIC
                    LogMessage("UID:  " + BitConverter.ToString(responseUid));
                    dumpTmp.AddRange(responseUid);
                    dumpTmp.AddRange(new byte[4]);
                    for (byte i = 0; i < 33; i++)
                    {
                        byte[] response = await mifareULAccess.ReadAsync((byte)(4 * i));
                        dumpTmp.AddRange(response);
                        LogMessage("Block " + (4 * i).ToString() + " to Block " + (4 * i + 3).ToString() + " " + BitConverter.ToString(response));
                    }
                    LogMessage("Dump ended :) !");
                    dumpTmp.AddRange(new byte[4] { 0x5F, 0x00, 0x00, 0x00 });
                    Dump = dumpTmp.ToArray();
                    MessageDialog msg = new MessageDialog("Dump completed !");
                    await msg.ShowAsync();
                }
                else
                {
                    LogMessage("This tag is not an Amiibo and can't be dumped!");
                    MessageDialog msg = new MessageDialog("This tag is not an Amiibo and can't be dumped !");
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
        string Log = "";

        async void LogMessage(string data)
        {
            Log = Log + data + Environment.NewLine;
            Debug.WriteLine(data);
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textBox.Text = Log;
            }); //This void is called by HandleCard from another thread, with this it's possible to update the textbox that is in the UI thrad
        }

        private async void button2_Click(object sender, RoutedEventArgs e)
        {
            if (Dump == null || Dump.Length < 0x10)
            {
                MessageDialog msg = new MessageDialog("Can't save the dump !");
                await msg.ShowAsync();
                return;
            }
            MessageDialog msg1 = new MessageDialog("The amiigo dump can be used in the amiigo PC app, automatically generates the write key and checks the uid before writing, use the binary dump only if you know what to do with it, you can also extract the binary dump from an amiigo dump with the PC app", "Save binary dump or amiigo dump ?");
            msg1.Commands.Add(new UICommand("Amiigo dump", async (command) =>
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("binary dump", new List<string>() { ".bin" });
                savePicker.SuggestedFileName = "New dump";
                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    await Windows.Storage.FileIO.WriteBytesAsync(file, Dump);
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        textBox.Text = "Dump saved !";
                    }
                    else
                    {
                        textBox.Text = ("File " + file.Name + " couldn't be saved.");
                    }
                }
            }));

            msg1.Commands.Add(new UICommand("Binary dump", async (command) =>
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation =
                    Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("binary dump", new List<string>() { ".bin" });
                savePicker.SuggestedFileName = "New dump";
                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    Windows.Storage.CachedFileManager.DeferUpdates(file);
                    byte[] BinDump = new byte[540];
                    Buffer.BlockCopy(Dump, 17, BinDump, 0, 540);
                    await Windows.Storage.FileIO.WriteBytesAsync(file, BinDump);
                    Windows.Storage.Provider.FileUpdateStatus status =
                        await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                    if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                    {
                        textBox.Text = "Dump saved !";
                    }
                    else
                    {
                        textBox.Text = ("File " + file.Name + " couldn't be saved.");
                    }
                }
            }));
            await msg1.ShowAsync();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DumpRestorer));
        }
    }
}
