using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HuffmanCompression
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private String libSelected;
        private String filepath;

        private String COMPRESSED_FILENAME = "compression.txt";
        private String DECOMPRESSED_FILENAME = "decompressed.txt";
        private String INPUT_FILE_FILTER = "Text files (*.txt)|*.txt|All file (*.*)|*.*";
        private String OUTPUT_FILE_FILTER = "Text file(*.txt)|*.txt";


        public int[] flag = { 1, 1, 1, 1 };
        public int[] resultData = new int[255];

        [DllImport("AsmDll.dll")]
        private static unsafe extern uint CountCharFrequencyAsm(int* a, long* b, int* c);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = INPUT_FILE_FILTER;
            if (openFileDialog.ShowDialog() == true)
            {
                String filename = openFileDialog.FileName;
                filenameText.Text = filename.Substring(filename.LastIndexOf('\\') + 1);
                filepath = openFileDialog.FileName;
                FileInfo fileinfo = new FileInfo(filepath);
                filesizeText.Text = getFileSize(fileinfo.Length);

                validateFileInput();
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            validateRadioButtonsCheck();
            validateFileInput();


            if (libSelected != null && filepath != null)
            {
                run(libSelected, filepath);
            }
        }

        private String getFileSize(long length)
        {
            return Convert.ToString(length) + " B";
        }

        private void validateRadioButtonsCheck()
        {
            if (csharpRadioButton.IsChecked == true || assemblyRadioButton.IsChecked == true)
            {
                radioButtonError.Visibility = Visibility.Hidden;
            }
            else
            {
                radioButtonError.Visibility = Visibility.Visible;
            }
        }

        private void validateFileInput()
        {
            if (filepath == null)
            {
                fileError.Visibility = Visibility.Visible;
            }
            else
            {
                fileError.Visibility = Visibility.Hidden;
            }
        }

        private void csharpRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            validateRadioButtonsCheck();
            libSelected = "c#";
        }

        private void assemblyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            validateRadioButtonsCheck();
            libSelected = "asm";
        }

        private void btnSaveCompressedFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = OUTPUT_FILE_FILTER;
            saveFileDialog.FileName = COMPRESSED_FILENAME;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, File.ReadAllText(COMPRESSED_FILENAME));
            }
        }

        private void btnSaveDecompressedFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = OUTPUT_FILE_FILTER;
            saveFileDialog.FileName = DECOMPRESSED_FILENAME;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, File.ReadAllText(DECOMPRESSED_FILENAME));
            }
        }

        private void run(string libSelected, string filepath)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<HuffmanNode> nodeList;
            Huffman huffman = new Huffman();
            int[] readFile = huffman.getIntsArrayFromFile(filepath);

            if (libSelected == "asm")
            {
                unsafe
                {
                    fixed (int* aArg1Addr = &readFile[0], aArg2Addr = &flag[0], aResultsAddr = &resultData[0])
                    {
                        var fileLength = Convert.ToInt64(readFile.Length);
                        CountCharFrequencyAsm(aArg1Addr, &fileLength, aResultsAddr);
                    }
                }

                huffman.charFrequency = resultData;
            }
            //else
            //{
            //    Huffman.charFrequency = CountCharFrequency(readFile, resultData);
            //}

            nodeList = huffman.getListFromFile(readFile);

            huffman.getTreeFromList(nodeList);

            huffman.setCodeToTheTree("", nodeList[0]);

            huffman.saveCompressedTree(readFile, nodeList[0], COMPRESSED_FILENAME);

            byte[] compressedFile = File.ReadAllBytes(COMPRESSED_FILENAME);

            huffman.convertByteToBits(compressedFile);

            huffman.saveDecompressedTree(nodeList[0], DECOMPRESSED_FILENAME);

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            processingTimeText.Text = Convert.ToString(elapsedMs) + " ms";

            compressedStackPanel.Visibility = Visibility.Visible;
            decompressedStackPanel.Visibility = Visibility.Visible;
            processingTimeStackPanel.Visibility = Visibility.Visible;

            compressedFilenameText.Text = COMPRESSED_FILENAME;
            compressedFilesizeText.Text = (new FileInfo(COMPRESSED_FILENAME)).Length.ToString() + " B";
            decompressedFilenameText.Text = DECOMPRESSED_FILENAME;
            decompressedFilesizeText.Text = (new FileInfo(DECOMPRESSED_FILENAME)).Length.ToString() + " B";
        }
    }
}
